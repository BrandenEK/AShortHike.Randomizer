using Newtonsoft.Json;
using System;

namespace AShortHike.Randomizer.Settings
{
    [Serializable]
    public class SettingsInfo
    {
        public ConnectionInfo saveSlotOne;
        public ConnectionInfo saveSlotTwo;
        public ConnectionInfo saveSlotThree;

        public SettingsInfo(ConnectionInfo saveSlotOne, ConnectionInfo saveSlotTwo, ConnectionInfo saveSlotThree)
        {
            this.saveSlotOne = saveSlotOne ?? new ConnectionInfo();
            this.saveSlotTwo = saveSlotTwo ?? new ConnectionInfo();
            this.saveSlotThree = saveSlotThree ?? new ConnectionInfo();
        }
    }

    [Serializable]
    public class ConnectionInfo
    {
        [JsonProperty] public readonly string server;
        [JsonProperty] public readonly string player;
        [JsonProperty] public readonly string password;

        public ConnectionInfo(string server, string player, string password)
        {
            this.server = server;
            this.player = player;
            this.password = password;
        }

        public ConnectionInfo()
        {
            server = null;
            player = null;
            password = null;
        }
    }
}
