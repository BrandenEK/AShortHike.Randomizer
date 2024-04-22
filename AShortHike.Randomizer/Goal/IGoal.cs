using System;
using System.Collections.Generic;
using System.Linq;

namespace AShortHike.Randomizer.Goal;

public interface IGoal
{
    public string Name { get; }

    public bool CheckCompletion();

    public IEnumerable<string> GetMissingRequirements();
}

public class NapGoal : IGoal
{
    public string Name => "Take a nap";

    public bool CheckCompletion()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return tags.GetBool("WonGameNiceJob");
    }

    public IEnumerable<string> GetMissingRequirements()
    {
        return CheckCompletion() ? [] : [ "Reach the top" ];
    }
}

public class PhotoGoal : IGoal
{
    public string Name => "Take a photo";

    public bool CheckCompletion()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return tags.GetBool("FoxClimbedToTop");
    }

    public IEnumerable<string> GetMissingRequirements()
    {
        return CheckCompletion() ? [] : [ "Help out the fox" ];
    }
}

public class RaceGoal : IGoal
{
    public string Name => "Complete all races";

    public bool CheckCompletion()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return tags.GetFloat("LighthouseRace_Victories") > 0
            && tags.GetFloat("OldBuildingRace_Victories") > 0
            && tags.GetFloat("MountainTopRace_Victories") > 0;
    }

    public IEnumerable<string> GetMissingRequirements()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        List<string> missing = new();
        if (tags.GetFloat("LighthouseRace_Victories") == 0)
            missing.Add("Lighthouse race");
        if (tags.GetFloat("OldBuildingRace_Victories") == 0)
            missing.Add("Old building race");
        if (tags.GetFloat("MountainTopRace_Victories") == 0)
            missing.Add("Mountain peak race");

        return missing;
    }
}

public class HelpGoal : IGoal
{
    public string Name => "Help everyone";

    public bool CheckCompletion()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return HELP_FLAGS.Keys.All(tags.GetBool);
    }

    public IEnumerable<string> GetMissingRequirements()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return HELP_FLAGS
            .Where(kvp => !tags.GetBool(kvp.Key))
            .Select(kvp => kvp.Value);
    }

    private static readonly Dictionary<string, string> HELP_FLAGS = new()
    {
        { "Opened_ToughBirdNPC (1)[9]", "Pay Tough Bird Salesman's Tuition Fee" },
        { "Opened_Frog_StandingNPC[0]", "Give Frog a Toy Shovel" },
        { "Opened_CamperNPC[1]", "Return the Camper's Camping Permit" }, 
        { "Opened_DeerKidBoat[0]", "Complete the Deer Kid's Boating Challenge" },
        { "Opened_Bunny_WalkingNPC (1)[0]", "Find Sue's Headband" },
        { "Opened_SittingNPC[0]", "Purchase the Sunhat from the Deer" },
        { "Opened_Goat_StandingNPC[0]", "Return the Camper's Wristwatch" },
        { "Opened_StandingNPC[0]", "Cheer Up the Artist" },
        { "Opened_LittleKidNPCVariant (1)[0]", "Collect 15 Shells for the Kid" },
        { "Opened_AuntMayNPC[0]", "Give the Shell Necklace to Aunt May" },
        { "FoxClimbedToTop", "Help the Fox Climb the Mountain" },
    };

}

public class FishGoal : IGoal
{
    public string Name => "Catch all fish";

    public bool CheckCompletion()
    {
        var inventory = Singleton<GlobalData>.instance.gameData.inventory;

        return FishSpecies.LoadAll().All(fish => inventory.GetCatchCount(fish) > 0);
    }

    public IEnumerable<string> GetMissingRequirements()
    {
        var inventory = Singleton<GlobalData>.instance.gameData.inventory;

        return FishSpecies.LoadAll()
            .Where(fish => inventory.GetCatchCount(fish) == 0)
            .Select(fish => fish.readableName.Substring(0, fish.readableName.IndexOf('#')));
    }
}
