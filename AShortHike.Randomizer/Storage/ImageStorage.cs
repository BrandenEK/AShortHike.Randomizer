using System;
using System.IO;
using UnityEngine;

namespace AShortHike.Randomizer.Storage;

public class ImageStorage
{
    private readonly Sprite _apImage;
    public Sprite ApImage => _apImage;

    public ImageStorage()
    {
        string imagePath = Path.Combine(DATA_PATH, "ap-item.png");

        if (File.Exists(imagePath))
            _apImage = LoadItemImage(imagePath);
        else
            Main.LogError("Failed to load ap image from " + imagePath);
    }

    private Sprite LoadItemImage(string path)
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

        return Sprite.Create(tex, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.Tight, border);
    }

    private readonly string DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Modding", "data", "Randomizer");

}
