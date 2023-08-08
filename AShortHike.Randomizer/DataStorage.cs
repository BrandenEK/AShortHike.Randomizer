using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class DataStorage
    {
        private readonly string dataPath = Environment.CurrentDirectory + "\\Modding\\data\\Randomizer\\";

        public readonly Dictionary<string, ItemLocation> allLocations = new();

        public readonly string[] tempAllItems = new string[]
        {
            "Bait",
            "BoatKey",
            "Bucket",
            "CampingPermit",
            "Cellphone",
            "Coin",
            "Compass",
            "FishingRod",
            "GoldenFeather",
            "Headband",
            "Pickaxe",
            "RunningShoes",
            "Shell",
            "ShellNecklace",
            "Shovel",
            "SilverFeather",
            "Stick",
            "ToyShovel",
            "Trash",
            "TreasureMap",
            "WalkieTalkie",
            "Watch",
        };

        public void LoadData()
        {
            string locationsPath = dataPath + "item-locations.json";
            if (!File.Exists(locationsPath))
            {
                Main.LogError("Data file was not present!");
                return;
            }

            string json = File.ReadAllText(locationsPath);
            string[] jsonObjects = json.Substring(1, json.Length - 2).Replace("},", "}~").Split('~');

            foreach (string str in jsonObjects)
            {
                ItemLocation location = JsonUtility.FromJson<ItemLocation>(str);
                allLocations.Add(location.id, location);
            }

            Main.Log($"Loaded {allLocations.Count} item locations!");
        }
    }
}
