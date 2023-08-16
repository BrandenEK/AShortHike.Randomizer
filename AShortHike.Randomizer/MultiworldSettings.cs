
namespace AShortHike.Randomizer
{
    public class MultiworldSettings
    {
        public readonly GoalType goal;

        public readonly bool showGoldenChests;
        public readonly bool skipCutscenes;

        public MultiworldSettings(GoalType goal, bool showGoldenChests, bool skipCutscenes)
        {
            this.goal = goal;
            this.showGoldenChests = showGoldenChests;
            this.skipCutscenes = skipCutscenes;
        }

        public MultiworldSettings()
        {
            goal = GoalType.Nap;
            showGoldenChests = true;
            skipCutscenes = true;
        }

        public enum GoalType
        {
            Nap = 0,
            Photo = 1,
        }
    }
}
