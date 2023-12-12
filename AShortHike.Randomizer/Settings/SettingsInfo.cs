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
        [JsonProperty] public readonly bool skipCutscenes;
        [JsonProperty] public readonly bool fastText;

        public ConnectionInfo(string server, string player, string password, bool skipCutscenes, bool fastText)
        {
            this.server = server;
            this.player = player;
            this.password = password;
            this.skipCutscenes = skipCutscenes;
            this.fastText = fastText;
        }

        public ConnectionInfo()
        {
            server = null;
            player = null;
            password = null;
            skipCutscenes = true;
            fastText = true;
        }
    }
}
