
namespace AShortHike.Randomizer.Models;

public class Item(string name, string player, bool progression)
{
    public string Name { get; } = name;
    public string Player { get; } = player;
    public bool IsProgression { get; } = progression;

    public static Item Unknown => new("Unknown item", "Unknown player", false);
}
