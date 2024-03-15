
namespace AShortHike.Randomizer.Items
{
    public class ItemLocation(string id, string name, int chestAngle)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public int ChestAngle { get; } = chestAngle;
    }
}
