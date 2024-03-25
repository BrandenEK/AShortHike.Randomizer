using HarmonyLib;

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
            if (!value)
                return;

            Tags tags = Singleton<GlobalData>.instance.gameData.tags;

            switch (tag)
            {
                // If taking a nap at the house: nap goal
                case "WonGameNiceJob":
                    Main.Randomizer.Connection.SendGoal(GoalType.Nap);
                    Main.Randomizer.CheckForHelpGoal();
                    return;
                // If talking to fox and the climb flag is set: photo goal
                case "MetIceHiker":
                    if (tags.GetBool("FoxClimbedToTop"))
                    {
                        Main.Randomizer.Connection.SendGoal(GoalType.Photo);
                        Main.Randomizer.CheckForHelpGoal();
                    }
                    return;

            }
        }
    }

    /// <summary>
    /// Check for goal completion when number is increased
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.SetFloat))]
    class Goal_SetFloat_Patch
    {
        public static void Postfix(string tag)
        {
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;

            switch (tag)
            {
                // If increasing victories and all victories are at least 1: race goal
                case "LighthouseRace_Victories":
                case "OldBuildingRace_Victories":
                case "MountainTopRace_Victories":
                    if (tags.GetFloat("LighthouseRace_Victories") > 0 &&
                        tags.GetFloat("OldBuildingRace_Victories") > 0 &&
                        tags.GetFloat("MountainTopRace_Victories") > 0)
                        Main.Randomizer.Connection.SendGoal(GoalType.Race);
                    return;
            }
        }
    }

    /// <summary>
    /// Check for goal completion when catching a fish
    /// </summary>
    [HarmonyPatch(typeof(GlobalData.CollectionInventory), nameof(GlobalData.CollectionInventory.AddFish))]
    class Goal_AddFish_Patch
    {
        public static void Postfix()
        {
            var inventory = Singleton<GlobalData>.instance.gameData.inventory;

            bool hasUncaught = false;
            foreach (var fish in FishSpecies.LoadAll())
            {
                if (inventory.GetCatchCount(fish) == 0)
                {
                    hasUncaught = true;
                    break;
                }
            }

            if (!hasUncaught)
                Main.Randomizer.Connection.SendGoal(GoalType.Fish);
        }
    }
}
