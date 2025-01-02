
namespace AShortHike.Randomizer.Models;

public class Item(string name, string player, int flags)
{
    public string Name { get; } = name;
    public string Player { get; } = player;
    public int Flags { get; } = flags;

    // Advancement (Without progression) or a trap
    public bool IsProgression => (Flags & 0x01) != 0 && (Flags & 0x08) == 0 || (Flags & 0x04) != 0;

    public static Item Unknown => new("Unknown item", "Unknown player", 0);
}
