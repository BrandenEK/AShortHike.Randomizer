
namespace AShortHike.Randomizer.Models;

public class Item(string name, string player)
{
    public string Name { get; } = name;
    public string Player { get; } = player;
}
