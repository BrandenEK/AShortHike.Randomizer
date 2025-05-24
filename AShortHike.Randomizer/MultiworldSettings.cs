using Newtonsoft.Json;

namespace AShortHike.Randomizer
{
    public class ServerSettings
    {
        public readonly GoalType goal;

        [JsonConstructor]
        public ServerSettings(GoalType goal)
        {
            this.goal = goal;
        }

        public ServerSettings()
        {
            goal = GoalType.Nap;
        }
    }

    public class ClientSettings
    {
        public readonly string server;
        public readonly string player;
        public readonly string password;

        public readonly bool skipCutscenes;
        public readonly bool fastText;
        public readonly bool goldenChests;
        public readonly bool hidePassword;

        [JsonConstructor]
        public ClientSettings(string server, string player, string password, bool skipCutscenes, bool fastText, bool goldenChests, bool hidePassword)
        {
            this.server = server;
            this.player = player;
            this.password = password;
            this.skipCutscenes = skipCutscenes;
            this.fastText = fastText;
            this.goldenChests = goldenChests;
            this.hidePassword = hidePassword;
        }

        public ClientSettings()
        {
            server = null;
            player = null;
            password = null;
            skipCutscenes = true;
            fastText = true;
            goldenChests = true;
            hidePassword = false;
        }
    }

    public enum GoalType
    {
        Nap = 0,
        Photo = 1,
        Race = 2,
        Help = 3,
        Fish = 4,
    }
}
