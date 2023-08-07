using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class ItemChanger
    {
        public void ChangeItems()
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
        }

        private void ReplaceObjectWithRandomChest(GameObject obj)
        {
            string locationId = obj.transform.position.ToString();
            if (!IsRandomized(locationId))
                return;

            Transform parent = obj.transform.parent;
            Vector3 position = obj.transform.position;

            Object.Destroy(obj.gameObject);

            // Create chest at this position with same id
            GameObject chest = Object.Instantiate(Main.ItemLoader.GetChestObject(), position, Quaternion.identity, parent);
            chest.GetComponent<GameObjectID>().id = locationId;

            // Give it a random item
            GameObject randomItem = Main.ItemLoader.GetRandomItemObject();
            chest.GetComponent<Chest>().prefabsInside = new GameObject[] { randomItem };
            chest.SetActive(true);

            Main.Log($"Replaced object {locationId} with {randomItem.name}");
        }

        public bool IsRandomized(string locationId)
        {
            return _locationMapping.ContainsKey(locationId);
        }

        private readonly Dictionary<string, string> _locationMapping = new()
        {
            { "(706.2, 7.3, 344.4)", "Beside cabin (Shell)" },
            { "(579.8, 29.3, 250.0)", "Beginning shore (Stick)" },
            { "(522.8, 7.3, 120.5)", "Beginning shore (Shell)" },
            { "(482.4, 14.4, 26.8)", "Begininng islands right chest" },
            { "(387.7, 8.8, 60.4)", "Beginning islands left chest" },
            { "(402.0, 21.6, 11.9)", "Beginning islands (Shell)" },
            { "(348.9, 8.5, 40.0)", "Rock wall shell" },
            { "(407.7, 8.5, 127.1)", "Beginning umbrellas shell" },
            { "(123.2, 35.0, 168.2)", "Ranger cabin rock" },
        };
    }
}
