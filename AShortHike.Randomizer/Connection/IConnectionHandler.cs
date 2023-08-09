
namespace AShortHike.Randomizer.Connection
{
    public interface IConnectionHandler
    {
        public bool Connect(string server, string player, string password);

        public void Disconnect();

        public bool Connected { get; }
    }
}
