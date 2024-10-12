using AShortHike.Randomizer.Storage;
using BepInEx;
using UnityEngine;

namespace AShortHike.Randomizer;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("AShortHike.ModdingAPI", "1.0.1")]
public class Main : BaseUnityPlugin
{
    public static Randomizer Randomizer { get; private set; }
    public static Transform TransformHolder { get; private set; }

    // New
    public static ImageStorage ImageStorage { get; private set; }
    public static ItemMapper ItemMapper { get; private set; }
    public static ItemStorage ItemStorage { get; private set; }
    public static LocationStorage LocationStorage { get; private set; }

    private void Awake()
    {
        TransformHolder = transform;
        Randomizer = new Randomizer();

        // New
        LocationStorage = new();
        ImageStorage = new();
        ItemStorage = new();
        ItemMapper = new(Randomizer.Connection, LocationStorage);
    }
}
