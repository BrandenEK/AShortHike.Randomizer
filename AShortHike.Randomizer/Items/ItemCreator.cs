using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public static class ItemCreator
    {
        public static CollectableItem CreateExternalItem(string itemName, string playerName)
        {
            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "AP";
            item.readableName = $"Found {TruncateItemName(itemName)} for {playerName}";
            item.icon = Main.Randomizer.Data.ApImage;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        public static CollectableItem CreateLocalItem(string itemName)
        {
            CollectableItem localItem = Main.Randomizer.Data.GetItemFromName(itemName, out _);

            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "AP";
            item.readableName = $"Found {TruncateItemName(itemName)}";
            item.icon = localItem?.icon;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        public static CollectableItem CreateReceivedItem(string itemName, string playerName)
        {
            CollectableItem localItem = Main.Randomizer.Data.GetItemFromName(itemName, out _);

            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "AP";
            item.readableName = $"Got {TruncateItemName(itemName)} from {playerName}";
            item.icon = localItem?.icon;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        private static string TruncateItemName(string itemName)
        {
            return itemName.Length > 20 ? itemName.Substring(0, 19).Trim() + '.' : itemName;
        }

        private static string ChangeTextWhite(string text)
        {
            return $"<color=#FFFFFF>{text}</color>";
        }
    }
}
