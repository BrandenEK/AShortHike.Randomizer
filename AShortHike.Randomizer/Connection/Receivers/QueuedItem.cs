
namespace AShortHike.Randomizer.Connection.Receivers
{
    public struct QueuedItem
    {
        public readonly string name;
        public readonly string player;
        public readonly int index;

        public QueuedItem(string name, string player, int index)
        {
            this.name = name;
            this.player = player;
            this.index = index;
        }
    }
}
