using AShortHike.Randomizer.Extensions;
using AShortHike.Randomizer.Models;
using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public class ItemHandler
    {
        private GameObject _regularChest = null;
        private GameObject _goldenChest = null;

        /// <summary>
        /// When a location is "collected" through a chest or npc dialog, set its collected flag and send to the server
        /// Chests should display the location, while npc dialog handles it itself
        /// </summary>
        public void CollectLocation(ItemLocation location, bool showDisplay)
        {
            Singleton<GlobalData>.instance.gameData.tags.SetBool($"Opened_{location.Id}", true);
            Main.Randomizer.Connection.SendLocation(location);
            Main.Randomizer.GoalHandler.CheckGoalCompletion();

            if (showDisplay)
            {
                Item item = Main.ItemMapper.GetItemAtLocation(location);

                CollectableItem collectable = ItemCreator.CreateFoundItem(item.Name, item.Player);
                Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(collectable.PickUpRoutine(1));
            }
        }

        /// <summary>
        /// When the game scene is loaded, find both versions of the chest object and store them for use in the replace method
        /// </summary>
        public void LoadChestObjects()
        {
            if (_regularChest != null && _goldenChest != null)
                return;

            foreach (Chest chest in Object.FindObjectsOfType<Chest>())
            {
                if (chest.GetLocationId() == "Feathers.0")
                {
                    if (_goldenChest != null)
                        continue;

                    _goldenChest = chest.gameObject.CloneInactive();
                    _goldenChest.name = "GoldenChest";
                    _goldenChest.transform.SetParent(Main.TransformHolder);
                    _goldenChest.GetComponent<Chest>().prefabsInside = System.Array.Empty<GameObject>();

                    if (_regularChest != null)
                        break;
                }
                else
                {
                    if (_regularChest != null)
                        continue;

                    _regularChest = chest.gameObject.CloneInactive();
                    _regularChest.name = "RegularChest";
                    _regularChest.transform.SetParent(Main.TransformHolder);
                    _regularChest.GetComponent<Chest>().prefabsInside = System.Array.Empty<GameObject>();

                    if (_goldenChest != null)
                        break;
                }
            }

            Main.Randomizer.LogHandler.Info("Loaded chest objects");
        }

        /// <summary>
        /// When the game scene is loaded, find every object that should be randomized and replace it with a chest
        /// </summary>
        public void ReplaceWorldObjectsWithChests()
        {
            // Change all items for buried chests
            foreach (BuriedChest buriedChest in Object.FindObjectsOfType<BuriedChest>())
            {
                GameObject chestObj = ReplaceObjectWithRandomChest(buriedChest.chest.gameObject, buriedChest.GetLocationId());
                if (chestObj == null)
                    continue;

                buriedChest.chest = chestObj;
                chestObj.SetActive(buriedChest.GetComponent<GameObjectID>().GetBoolForID("Unearthed_"));
            }

            // Change all items for chests
            foreach (Chest chest in Object.FindObjectsOfType<Chest>())
            {
                if (chest.GetComponentInParent<BuriedChest>() != null)
                    continue;

                ReplaceObjectWithRandomChest(chest.gameObject, chest.GetLocationId());
            }

            // Change all items for interactable pickups
            foreach (CollectOnInteract interact in Object.FindObjectsOfType<CollectOnInteract>())
            {
                ReplaceObjectWithRandomChest(interact.gameObject, interact.GetLocationId());
            }

            // Change all items for touchable pickups
            foreach (CollectOnTouch touch in Object.FindObjectsOfType<CollectOnTouch>())
            {
                ReplaceObjectWithRandomChest(touch.gameObject, touch.GetLocationId());
            }

            // Change all items for holdables
            foreach (Holdable holdable in Object.FindObjectsOfType<Holdable>())
            {
                ReplaceObjectWithRandomChest(holdable.gameObject, holdable.GetLocationId());
            }

            Main.Randomizer.LogHandler.Info($"Replaced objects in the world with random chests");
        }

        /// <summary>
        /// For every object (Feathers, sticks, etc) in the world that is randomized, replace it with a golden or regular chest
        /// </summary>
        private GameObject ReplaceObjectWithRandomChest(GameObject obj, string locationId)
        {
            // Determine whether to randomize this location or not
            if (!Main.LocationStorage.TryGetLocation(locationId, out ItemLocation location))
                return null;

            Transform parent = obj.transform.parent;
            Vector3 position = obj.transform.position;
            Quaternion rotation = Quaternion.Euler(0, location.ChestAngle, 0);
            bool useGoldenChest = Main.ItemMapper.GetItemAtLocation(location).IsProgression;

            Object.Destroy(obj.gameObject);

            // Create chest at this position with same id
            GameObject chest = Object.Instantiate(useGoldenChest ? _goldenChest : _regularChest, position, rotation, parent);
            chest.GetComponent<GameObjectID>().id = locationId;
            chest.SetActive(true);
            return chest;
        }
    }
}
