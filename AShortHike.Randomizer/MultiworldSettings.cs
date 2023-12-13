using Newtonsoft.Json;

namespace AShortHike.Randomizer
{
    public class MultiworldSettings
    {
        public readonly GoalType goal;

        [JsonConstructor]
        public MultiworldSettings(GoalType goal)
        {
            this.goal = goal;
        }

        public MultiworldSettings()
        {
            goal = GoalType.Nap;
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
