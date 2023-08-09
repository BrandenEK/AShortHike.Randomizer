using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public class ItemHandler
    {
        private readonly Dictionary<string, string> _mappedItems = new();
        private readonly Dictionary<string, GameObject> _pickupObjects = new();
        private GameObject _chestObject;
        private bool _loaded;

        // Item mapping

        public string GetItemAtLocation(string locationId)
        {
            return _mappedItems.TryGetValue(locationId, out string item) ? item : null;
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
                Main.LogWarning("Loading item: " + item.name);
                item.showPrompt = CollectableItem.PickUpPrompt.Always;
                var obj = new GameObject(item.name);
                obj.AddComponent<CollectFromChest>().item = item;
                obj.SetActive(false);
                obj.transform.SetParent(Main.TransformHolder);
                _pickupObjects.Add(item.name, obj);
            }

            _loaded = true;
        }

        public GameObject GetChestObject()
        {
            return _chestObject;
        }

        public GameObject GetItemObject(string item)
        {
            if (_pickupObjects.TryGetValue(item, out GameObject pickup))
            {
                return pickup;
            }

            throw new System.Exception($"Item '{item}' was not loaded");
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

            void ReplaceObjectWithRandomChest(GameObject obj)
            {
                string locationId = obj.transform.position.ToString();
                string item = Main.Randomizer.Items.GetItemAtLocation(locationId);
                if (item is null)
                    return;

                Transform parent = obj.transform.parent;
                Vector3 position = obj.transform.position;

                Object.Destroy(obj.gameObject);

                // Create chest at this position with same id
                GameObject chest = Object.Instantiate(GetChestObject(), position, Quaternion.identity, parent);
                chest.GetComponent<GameObjectID>().id = locationId;

                // Give it a random item
                GameObject randomItem = GetItemObject(item);
                chest.GetComponent<Chest>().prefabsInside = new GameObject[] { randomItem };
                chest.SetActive(true);

                Main.Log($"Replaced object {locationId} with {randomItem.name}");
            }
        }
    }
}
