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
            GameObject randomItem = Main.ItemLoader.GetItemObject(GetItemAtLocation(locationId));
            chest.GetComponent<Chest>().prefabsInside = new GameObject[] { randomItem };
            chest.SetActive(true);

            Main.Log($"Replaced object {locationId} with {randomItem.name}");
        }

        public bool IsRandomized(string locationId)
        {
            return Main.DataStorage.allLocations.ContainsKey(locationId);
        }

        public string GetItemAtLocation(string locationId)
        {
            return Main.DataStorage.tempAllItems[Random.RandomRangeInt(0, Main.DataStorage.tempAllItems.Length - 1)];
        }
    }
}
