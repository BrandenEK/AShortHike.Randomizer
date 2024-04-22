using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Goal;

public class GoalHandler
{
    private readonly Dictionary<GoalType, IGoal> _goals = new()
    {
        { GoalType.Nap, new NapGoal() },
        { GoalType.Photo, new PhotoGoal() },
        { GoalType.Race, new RaceGoal() },
        { GoalType.Help, new HelpGoal() },
        { GoalType.Fish, new FishGoal() },
    };

    private IGoal CurrentGoal => _goals.TryGetValue(Main.Randomizer.ServerSettings.goal, out IGoal goal)
        ? goal
        : throw new System.Exception($"Invalid goal type: {Main.Randomizer.ServerSettings.goal}");

    public void ToggleGoalDisplay()
    {
        Main.Randomizer.LogHandler.Info("Toggling goal display");

        GoalText.text = DisplayGoalText(CurrentGoal);
        GoalBackground.gameObject.SetActive(!GoalBackground.gameObject.activeSelf);
        GoalBackground.sizeDelta = new Vector2(GoalText.preferredWidth + 20, GoalText.preferredHeight);

        CheckGoalCompletion();
    }

    public void CheckGoalCompletion()
    {
        bool complete = CurrentGoal.CheckCompletion();

        Main.Randomizer.LogHandler.Warning($"Goal status ({Main.Randomizer.ServerSettings.goal}): {(complete ? "Complete" : "Incomplete")}");

        if (complete)
            Main.Randomizer.Connection.SendGoal();
    }

    private string DisplayGoalText(IGoal goal)
    {
        StringBuilder sb = new();

        sb.AppendLine($"Current goal: {goal.Name}");
        sb.AppendLine();

        sb.AppendLine("Missing requirements:");

        IEnumerable<string> reqs = goal.GetMissingRequirements();
        if (reqs.Any())
        {
            foreach (var req in reqs)
                sb.AppendLine(req);
        }
        else
        {
            sb.AppendLine("None");
        }

        return sb.ToString();
    }

    private Text x_goalText;
    private Text GoalText
    {
        get
        {
            if (x_goalText != null)
                return x_goalText;

            RectTransform rect = new GameObject("Goal text").AddComponent<RectTransform>();
            rect.SetParent(GoalBackground, false);
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(10, -10);

            Text text = rect.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 32;
            text.alignment = TextAnchor.UpperLeft;
            text.color = RGB(39, 101, 143);

            Main.Randomizer.LogHandler.Warning("Created new text object");
            return x_goalText = text;
        }
    }

    private RectTransform x_goalBackground;
    private RectTransform GoalBackground
    {
        get
        {
            if (x_goalBackground != null)
                return x_goalBackground;

            RectTransform rect = new GameObject("Goal image").AddComponent<RectTransform>();
            rect.SetParent(Canvas, false);
            rect.sizeDelta = new Vector2(600, 400);
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.gameObject.SetActive(false);

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = RGB(242, 238, 203);

            Main.Randomizer.LogHandler.Warning("Created new background object");
            return x_goalBackground = rect;
        }
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

    private Color RGB(float r, float g, float b) => new Color(r / 255f, g / 255f, b / 255f);
}
