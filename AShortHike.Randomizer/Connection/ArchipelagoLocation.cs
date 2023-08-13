﻿
namespace AShortHike.Randomizer.Connection
{
    public class ArchipelagoLocation
    {
        //public readonly long apId;
        public readonly string itemName;
        public readonly int itemType;
        public readonly string playerName;

        public bool ShoudlBeGolden => itemType == 0 || itemType == 3;

        public ArchipelagoLocation(string itemName, int itemType, string playerName)
        {
            //this.apId = apId;
            this.itemName = itemName;
            this.itemType = itemType;
            this.playerName = playerName;
        }
    }
}