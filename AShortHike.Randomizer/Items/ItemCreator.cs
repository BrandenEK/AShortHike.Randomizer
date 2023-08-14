using UnityEngine;

namespace AShortHike.Randomizer.Items
{
    public static class ItemCreator
    {
        /// <summary>
        /// Creates an instance of an item to display when you find an item for another player
        /// </summary>
        public static CollectableItem CreateExternalItem(string itemName, string playerName)
        {
            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "APL";
            item.readableName = $"{TruncateItemName(itemName)} {ChangeTextWhite("for")} {playerName}";
            item.icon = Main.Randomizer.Data.ApImage;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        /// <summary>
        /// Creates an instance of an item to display when you find an item for yourself
        /// </summary>
        public static CollectableItem CreateLocalItem(string itemName)
        {
            CollectableItem localItem = Main.Randomizer.Data.GetItemFromName(itemName, out _);

            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "APL";
            item.readableName = $"{TruncateItemName(itemName)}";
            item.icon = localItem?.icon;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        /// <summary>
        /// Creates an instance of an item to display when someone else finds an item for you
        /// </summary>
        public static CollectableItem CreateReceivedItem(string itemName, string playerName)
        {
            CollectableItem localItem = Main.Randomizer.Data.GetItemFromName(itemName, out _);

            CollectableItem item = ScriptableObject.CreateInstance<CollectableItem>();
            item.name = "APR";
            item.readableName = $"{TruncateItemName(itemName)} {ChangeTextWhite("from")} {playerName}";
            item.icon = localItem?.icon;
            item.showPrompt = CollectableItem.PickUpPrompt.Always;
            return item;
        }

        /// <summary>
        /// If the string is greater than 20 characters long, it will truncate it and add a point at the end
        /// </summary>
        private static string TruncateItemName(string itemName)
        {
            return itemName.Length > 20 ? itemName.Substring(0, 19).Trim() + '.' : itemName;
        }

        /// <summary>
        /// Wraps a message in white color text to appear normal in the item display prompt
        /// </summary>
        private static string ChangeTextWhite(string text)
        {
            return $"<color=#FFFFFF>{text}</color>";
        }
    }
}
