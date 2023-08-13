﻿using AShortHike.Randomizer.Connection;
using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public class ItemHandler
    {
        private GameObject _chestObject;
        private bool _loaded;

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

        public void LoadItemObjects()
        {
            if (_loaded) return;

            _chestObject = Object.FindObjectOfType<Chest>().gameObject.CloneInactive();
            _chestObject.name = "RandoChest";
            _chestObject.transform.SetParent(Main.TransformHolder);
            Main.LogWarning("Loaded chest object");

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

            Main.Log($"Replaced {numObjects} objects in the world with random chests");

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
