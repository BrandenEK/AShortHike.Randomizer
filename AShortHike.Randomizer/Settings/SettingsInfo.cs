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
    }
}
