using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class ItemLoader
    {
        private readonly Dictionary<string, GameObject> _pickupObjects = new();
        private GameObject _chestObject;

        private bool _loaded;

        public void LoadItems()
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

        // This is temporary
        public GameObject GetRandomItemObject()
        {
            int num = Random.RandomRangeInt(0, 4);
            string item = num switch
            {
                0 => "RunningShoes",
                1 => "Stick",
                2 => "GoldenFeather",
                3 => "Shell",
                4 => "Bucket",
                _ => "Pickaxe",
            };
            return GetItemObject(item);
        }
    }
}
