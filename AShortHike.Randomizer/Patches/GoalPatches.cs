using HarmonyLib;
using System.Linq;

namespace AShortHike.Randomizer.Patches
{
    /// <summary>
    /// Check for goal completion when flag is set
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.SetBool))]
    class Goal_SetBool_Patch
    {
        public static void Postfix(string tag, bool value)
        {
            if (value && BOOL_TAGS.Contains(tag))
                Main.Randomizer.GoalHandler.CheckGoalCompletion();
        }

        private static readonly string[] BOOL_TAGS =
        {
            "WonGameNiceJob", // Nap goal
            "MetIceHiker", // Photo goal
        };
    }

    /// <summary>
    /// Check for goal completion when number is increased
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.SetFloat))]
    class Goal_SetFloat_Patch
    {
        public static void Postfix(string tag)
        {
            if (FLOAT_TAGS.Contains(tag))
                Main.Randomizer.GoalHandler.CheckGoalCompletion();
        }

        private static readonly string[] FLOAT_TAGS =
        {
            "LighthouseRace_Victories", // Race goal
            "OldBuildingRace_Victories", // Race goal
            "MountainTopRace_Victories", // Race goal
        };
    }

    /// <summary>
    /// Check for goal completion when catching a fish
    /// </summary>
    [HarmonyPatch(typeof(GlobalData.CollectionInventory), nameof(GlobalData.CollectionInventory.AddFish))]
    class Goal_AddFish_Patch
    {
        public static void Postfix()
        {
            Main.Randomizer.GoalHandler.CheckGoalCompletion(); // Fish goal
        }
    }
}
