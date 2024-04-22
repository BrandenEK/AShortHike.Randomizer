using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Goal;

public class GoalHandler
{
    public void ToggleGoalDisplay()
    {
        Main.Randomizer.LogHandler.Error("Toggling goal display");
    }

    public void CheckGoalCompletion()
    {
        Main.Randomizer.LogHandler.Error("Checking for goal completion");
    }

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
