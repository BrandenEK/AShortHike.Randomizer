using AShortHike.Randomizer.Items;
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

        private Sprite _apImage;
        public Sprite ApImage => _apImage;

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

        public DataStorage()
        {
            string locationsPath = dataPath + "item-locations.json";
            if (File.Exists(locationsPath))
                LoadLocationsList(locationsPath);
            else
                Main.LogError("Failed to load locations list from " + locationsPath);

            string imagePath = dataPath + "ap-item.png";
            if (File.Exists(imagePath))
                LoadItemImage(imagePath);
            else
                Main.LogError("Failed to load ap image from " + imagePath);
        }

        private void LoadLocationsList(string path)
        {
            string json = File.ReadAllText(path);
            string[] jsonObjects = json.Substring(1, json.Length - 2).Replace("},", "}~").Split('~');

            foreach (string str in jsonObjects)
            {
                ItemLocation location = JsonUtility.FromJson<ItemLocation>(str);
                allLocations.Add(location.gameId, location);
            }

            Main.Log($"Loaded {allLocations.Count} item locations!");
        }

        private void LoadItemImage(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(bytes);
            tex.filterMode = FilterMode.Point;

            var size = new Vector2Int(12, 12);
            var rect = new Rect(0, 0, size.x, size.y);
            var pivot = new Vector2(0.5f, 0.5f);
            int pixelsPerUnit = 100;
            var border = Vector4.zero;
            _apImage = Sprite.Create(tex, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.Tight, border);

            Main.Log("Loaded ap item image!");
        }








        public void ExportSprite(Sprite sprite)
        {
            // Create texture
            var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Rect rect = sprite.textureRect;
            Color[] pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            output.SetPixels(pixels);
            output.Apply();
            output.name = sprite.name;

            // Save to file
            string exportPath = $"{Environment.CurrentDirectory}\\{output.name}.png";
            File.WriteAllBytes(exportPath, output.EncodeToPNG());
        }
    }
}
