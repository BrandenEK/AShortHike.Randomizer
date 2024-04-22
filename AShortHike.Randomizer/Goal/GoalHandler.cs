using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Goal;

public class GoalHandler
{
    public void ToggleGoalDisplay()
    {
        Main.Randomizer.LogHandler.Error("Toggling goal display");

        CheckGoalCompletion();
    }

    public void CheckGoalCompletion()
    {
        GoalType goal = Main.Randomizer.ServerSettings.goal;

        bool complete = goal switch
        {
            GoalType.Nap => CheckNapGoal(),
            GoalType.Photo => CheckPhotoGoal(),
            GoalType.Race => CheckRaceGoal(),
            GoalType.Help => CheckHelpGoal(),
            GoalType.Fish => CheckFishGoal(),
            _ => throw new System.Exception($"Invalid goal type: {goal}")
        };

        Main.Randomizer.LogHandler.Info($"Goal status ({goal}): {(complete ? "Complete" : "Incomplete")}");

        if (complete)
            Main.Randomizer.Connection.SendGoal();
    }

    private bool CheckNapGoal()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return tags.GetBool("WonGameNiceJob");
    }

    private bool CheckPhotoGoal()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return tags.GetBool("FoxClimbedToTop");
    }

    private bool CheckRaceGoal()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return tags.GetFloat("LighthouseRace_Victories") > 0
            && tags.GetFloat("OldBuildingRace_Victories") > 0
            && tags.GetFloat("MountainTopRace_Victories") > 0;
    }

    private bool CheckHelpGoal()
    {
        var tags = Singleton<GlobalData>.instance.gameData.tags;

        return HELP_FLAGS.All(tags.GetBool);
    }

    private bool CheckFishGoal()
    {
        var inventory = Singleton<GlobalData>.instance.gameData.inventory;

        return FishSpecies.LoadAll().All(fish => inventory.GetCatchCount(fish) > 0);
    }

    private static readonly string[] HELP_FLAGS = new string[]
    {
            "Opened_ToughBirdNPC (1)[9]",       // Give coins to tough bird salesman
            "Opened_Frog_StandingNPC[0]",       // Trade toy shovel
            "Opened_CamperNPC[1]",              // Return camping permit
            "Opened_DeerKidBoat[0]",            // Complete boat challenge
            "Opened_Bunny_WalkingNPC (1)[0]",   // Return headband to rabbit
            "Opened_SittingNPC[0]",             // Purchase sunhat
            "Opened_Goat_StandingNPC[0]",       // Return watch to camper
            "Opened_StandingNPC[0]",            // Cheer up artist
            "Opened_LittleKidNPCVariant (1)[0]",// Collect 15 shells for the kid
            "Opened_AuntMayNPC[0]",             // Give shell necklace to Ranger May
            "FoxClimbedToTop",                  // Help fox up the mountain
    };

    private Transform x_canvas;
    private Transform Canvas
    {
        get
        {
            if (x_canvas != null)
                return x_canvas;

            GameObject obj = new("Better canvas");
            obj.layer = LayerMask.NameToLayer("UI");
            obj.transform.parent = Main.TransformHolder;

            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.referencePixelsPerUnit = 100;

            Main.Randomizer.LogHandler.Warning("Created new canvas object");
            return x_canvas = canvas.transform;
        }
    }
}
