using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public class ItemHandler
    {
        // Item mapping

        public bool IsLocationRandomized(string locationId)
        {
            return Main.Randomizer.Data.GetLocationFromId(locationId) != null;
        }

        public void CollectLocation(string locationId)
        {
            Singleton<GlobalData>.instance.gameData.tags.SetBool("Opened_" + locationId, true);
            Main.Randomizer.Connection.SendLocation(locationId);

            ArchipelagoLocation apLocation = Main.Randomizer.Data.GetApDataAtLocation(locationId);
            if (apLocation != null)
                DisplayItem(apLocation);
        }

        public void DisplayItem(ArchipelagoLocation apLocation)
        {
            Main.Log($"Displaying {apLocation.itemName} for {apLocation.playerName}");

            CollectableItem item;
            if (apLocation.playerName == Main.Randomizer.Settings.SettingsForCurrentSave.player)
            {
                // The item belongs to this world
                CollectableItem localItem = Main.Randomizer.Data.GetItemFromName(apLocation.itemName, out _);
                if (localItem == null)
                {
                    Main.LogError(apLocation.itemName + " doesn't exist in this world");
                    return;
                }
                item = CreateLocalItem(apLocation.itemName, localItem.icon);
            }
            else
            {
                // The item goes to another player's world
                item = CreateExternalItem(apLocation.itemName, apLocation.playerName);
            }
            Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(item.PickUpRoutine(1));
        }

        private CollectableItem CreateExternalItem(string itemName, string playerName)
        {
            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "AP";
            item.readableName = $"{itemName} for {playerName}";
            item.icon = Main.Randomizer.Data.ApImage;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        private CollectableItem CreateLocalItem(string itemName, Sprite icon)
        {
            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "AP";
            item.readableName = $"{itemName}";
            item.icon = icon;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        // Item loading

        private GameObject _regularChest = null;
        private GameObject _goldenChest = null;

        public void LoadChestObjects()
        {
            if (_regularChest != null && _goldenChest != null)
                return;

            foreach (Chest chest in Object.FindObjectsOfType<Chest>())
            {
                if (chest.transform.position.ToString() == "(594.7, 143.3, 345.6)")
                {
                    if (_goldenChest != null)
                        continue;

                    _goldenChest = chest.gameObject.CloneInactive();
                    _goldenChest.name = "GoldenChest";
                    _goldenChest.transform.SetParent(Main.TransformHolder);
                    _goldenChest.GetComponent<Chest>().prefabsInside = System.Array.Empty<GameObject>();

                    if (_regularChest != null)
                        return;
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
                        return;
                }
            }

            Main.Log("Loaded chest objects");
        }

        // Item changing

        public void ReplaceWorldObjectsWithChests()
        {
            // Change all items for interactable pickups
            foreach (CollectOnInteract interact in Object.FindObjectsOfType<CollectOnInteract>())
            {
                ReplaceObjectWithRandomChest(interact.gameObject);
            }

            // Change all items for touchable pickups
            foreach (CollectOnTouch touch in Object.FindObjectsOfType<CollectOnTouch>())
            {
                ReplaceObjectWithRandomChest(touch.gameObject);
            }

            // Change all items for chests
            foreach (Chest chest in Object.FindObjectsOfType<Chest>())
            {
                ReplaceObjectWithRandomChest(chest.gameObject);
            }

            // Change all items for holdables
            foreach (Holdable holdable in Object.FindObjectsOfType<Holdable>())
            {
                ReplaceObjectWithRandomChest(holdable.gameObject);
            }

            Main.Log($"Replaced objects in the world with random chests");
        }

        private void ReplaceObjectWithRandomChest(GameObject obj)
        {
            string locationId = obj.transform.position.ToString();
            if (!IsLocationRandomized(locationId))
                return;

            Transform parent = obj.transform.parent;
            Vector3 position = obj.transform.position;

            Object.Destroy(obj.gameObject);

            // Determine what type of chest to spawn
            ArchipelagoLocation apLocation = Main.Randomizer.Data.GetApDataAtLocation(locationId);
            bool useGoldenChest = apLocation != null && apLocation.ShoudlBeGolden;

            // Create chest at this position with same id
            GameObject chest = Object.Instantiate(useGoldenChest ? _goldenChest : _regularChest, position, Quaternion.identity, parent);
            chest.GetComponent<GameObjectID>().id = locationId;
            chest.SetActive(true);
        }

    }
}
