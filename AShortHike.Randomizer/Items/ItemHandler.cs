using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public class ItemHandler
    {
        private readonly Dictionary<string, string> _mappedItems = new();
        private GameObject _chestObject;
        private bool _loaded;

        private readonly Dictionary<string, CollectableItem> _allItems = new();

        // Item mapping

        public void GiveItem(string itemName)
        {
            if (_allItems.TryGetValue(itemName, out CollectableItem item))
                Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(item.PickUpRoutine(1));
        }

        public void DisplayApItem()
        {
            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "AP";
            item.readableName = "Master Sword for Link";
            item.icon = Main.Randomizer.Data.ApImage;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(item.PickUpRoutine(1));
        }

        public bool IsLocationRandomized(string locationId)
        {
            return Main.Randomizer.Data.allLocations.ContainsKey(locationId);
        }

        public void ResetShuffledItems()
        {
            _mappedItems.Clear();
        }

        public void StoreShuffledItems(List<string> items)
        {
            ResetShuffledItems();
            // Loop over items and add key value pairs
        }

        // Item loading

        public void LoadItemObjects()
        {
            if (_loaded) return;

            _chestObject = Object.FindObjectOfType<Chest>().gameObject.CloneInactive();
            _chestObject.name = "RandoChest";
            _chestObject.transform.SetParent(Main.TransformHolder);
            Main.LogWarning("Loaded chest object");

            foreach (CollectableItem item in Resources.LoadAll<CollectableItem>("Items/"))
            {
                string itemName = item.readableName.Substring(0, item.readableName.IndexOf('#'));
                Main.LogWarning("Loading item: " + itemName);
                item.showPrompt = CollectableItem.PickUpPrompt.Always;
                _allItems[itemName] = item;
            }

            _loaded = true;
        }

        public GameObject GetChestObject()
        {
            return _chestObject;
        }

        // Item changing

        public void ReplaceWorldObjectsWithChests()
        {
            int numObjects = 0;

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

            Main.Log($"Replaced {numObjects} in the world with random chests");

            void ReplaceObjectWithRandomChest(GameObject obj)
            {
                string locationId = obj.transform.position.ToString();
                if (!IsLocationRandomized(locationId))
                    return;

                Transform parent = obj.transform.parent;
                Vector3 position = obj.transform.position;

                Object.Destroy(obj.gameObject);

                // Create chest at this position with same id and no items
                GameObject chest = Object.Instantiate(GetChestObject(), position, Quaternion.identity, parent);
                chest.GetComponent<GameObjectID>().id = locationId;
                chest.GetComponent<Chest>().prefabsInside = System.Array.Empty<GameObject>();
                chest.SetActive(true);

                numObjects++;
            }
        }
    }
}
