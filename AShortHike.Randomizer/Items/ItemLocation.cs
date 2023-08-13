
namespace AShortHike.Randomizer.Items
{
    public class ItemLocation
    {
        public readonly string gameId;
        public readonly long apId;
        public readonly string name;

        public ItemLocation(string gameId, long apId, string name)
        {
            this.gameId = gameId;
            this.apId = apId;
            this.name = name;
        }
    }
}
