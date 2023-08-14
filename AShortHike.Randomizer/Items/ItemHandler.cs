using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Items
{
    public class ItemHandler
    {
        // Item mapping

        public void CollectLocation(string locationId, bool showDisplay)
        {
            Singleton<GlobalData>.instance.gameData.tags.SetBool("Opened_" + locationId, true);
            Main.Randomizer.Connection.SendLocation(locationId);

            ItemLocation location = Main.Randomizer.Data.GetLocationFromId(locationId);
            if (showDisplay && location != null)
            {
                CollectableItem item = CollectableItem.Load(locationId);
                Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(item.PickUpRoutine(1));
            }
        }

        public void DisplayReceivedItem(string itemName, string playerName)
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            //TextBoxContent text = ui.CreateTextBoxContent($"Received {itemName} from {playerName}");
            //text.SkipTextAnimation();

            //// Set position
            //RectTransform rect = text.transform as RectTransform;
            //rect.CenterWithinParent();
            //rect.sizeDelta = new Vector2((rect.parent as RectTransform).rect.size.x, rect.sizeDelta.y);

            //GameObject obj = new GameObject("Text", typeof(RectTransform), typeof(Text));

            //RectTransform rect = obj.GetComponent<RectTransform>();
            //rect.SetParent(ui.transform, false);
            //rect.CenterWithinParent();

            //Text text = obj.GetComponent<Text>();
            //text.fontSize = 22;
            //text.font = (Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font);
            //text.text = $"Received {itemName} from {playerName}";
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

            Main.Log("Loaded chest objects");
        }

        // Item changing

        public void ReplaceWorldObjectsWithChests()
        {
            // Change all items for buried chests
            foreach (BuriedChest buriedChest in Object.FindObjectsOfType<BuriedChest>())
            {
                GameObject chestObj = ReplaceObjectWithRandomChest(buriedChest.chest.gameObject);
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

                ReplaceObjectWithRandomChest(chest.gameObject);
            }

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

            // Change all items for holdables
            foreach (Holdable holdable in Object.FindObjectsOfType<Holdable>())
            {
                ReplaceObjectWithRandomChest(holdable.gameObject);
            }

            Main.Log($"Replaced objects in the world with random chests");
        }

        private GameObject ReplaceObjectWithRandomChest(GameObject obj, bool ignoreIsRandomized = false)
        {
            // Determine whether to randomize this location or not
            string locationId = obj.transform.position.ToString();
            ItemLocation location = Main.Randomizer.Data.GetLocationFromId(locationId);
            if (location == null)
                return null;

            Transform parent = obj.transform.parent;
            Vector3 position = obj.transform.position;

            Object.Destroy(obj.gameObject);

            // Create chest at this position with same id
            GameObject chest = Object.Instantiate(location.ShouldBeGolden ? _goldenChest : _regularChest, position, Quaternion.identity, parent);
            chest.GetComponent<GameObjectID>().id = locationId;
            chest.SetActive(true);
            return chest;
        }
    }
}
