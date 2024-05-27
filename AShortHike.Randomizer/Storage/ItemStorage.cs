
using System.Collections.Generic;

namespace AShortHike.Randomizer.Storage;

public class ItemStorage
{
    private readonly Dictionary<string, CollectableItem> _items = new();

    public ItemStorage()
    {
        LoadItems();
    }

    public CollectableItem GetItemFromName(string itemName, out int amount)
    {
        if (_items.TryGetValue(itemName, out CollectableItem item))
        {
            if (itemName == "Bait")
                amount = 5;
            else
                amount = 1;
            return item;
        }

        if (itemName.EndsWith("Coins"))
        {
            amount = int.Parse(itemName.Substring(0, itemName.IndexOf(' ')));
            return _items.TryGetValue("Coins", out item) ? item : null;
        }

        amount = 0;
        return null;
    }

    private void LoadItems()
    {
        // Holdables
        _items.Add("Stick", CollectableItem.Load("Stick"));
        _items.Add("Bucket", CollectableItem.Load("Bucket"));
        _items.Add("Pickaxe", CollectableItem.Load("Pickaxe"));

        // Fishing
        _items.Add("Fishing Rod", CollectableItem.Load("FishingRod"));
        _items.Add("Progressive Fishing Rod", CollectableItem.Load("FishingRod"));
        _items.Add("Golden Fishing Rod", CollectableItem.Load("GoldenFishingRod"));
        _items.Add("Fishing Journal", CollectableItem.Load("FishEncyclopedia"));
        _items.Add("Bait", CollectableItem.Load("Bait"));

        // Clothing
        _items.Add("Sunhat", CollectableItem.Load("Sunhat"));
        _items.Add("Baseball Cap", CollectableItem.Load("KidHat"));
        _items.Add("Provincial Park Hat", CollectableItem.Load("ParkHat"));
        _items.Add("Headband", CollectableItem.Load("Headband"));
        _items.Add("Running Shoes", CollectableItem.Load("RunningShoes"));

        // Shovels & Shells
        _items.Add("Toy Shovel", CollectableItem.Load("ToyShovel"));
        _items.Add("Shovel", CollectableItem.Load("Shovel"));
        _items.Add("Seashell", CollectableItem.Load("Shell"));
        _items.Add("Shell Necklace", CollectableItem.Load("ShellNecklace"));

        // Boating
        _items.Add("Motorboat Key", CollectableItem.Load("BoatKey"));
        _items.Add("Boating Manual", CollectableItem.Load("BoatManual"));

        // Feathers
        _items.Add("Golden Feather", CollectableItem.Load("GoldenFeather"));
        _items.Add("Silver Feather", CollectableItem.Load("SilverFeather"));

        // Maps
        CollectableItem map = CollectableItem.Load("TreasureMap");
        _items.Add("A Stormy View Map", map);
        _items.Add("In Her Shadow Map", map);
        _items.Add("The King Map", map);
        _items.Add("The Treasure of Sid Beach Map", map);

        // Others
        _items.Add("Compass", CollectableItem.Load("Compass"));
        _items.Add("Medal", CollectableItem.Load("Medal"));
        _items.Add("Wristwatch", CollectableItem.Load("Watch"));
        _items.Add("Camping Permit", CollectableItem.Load("CampingPermit"));
        _items.Add("Walkie Talkie", CollectableItem.Load("WalkieTalkie"));
        _items.Add("Coins", CollectableItem.Load("Coin"));

        Main.Randomizer.LogHandler.Info($"Loaded {_items.Count} items");
    }
}
