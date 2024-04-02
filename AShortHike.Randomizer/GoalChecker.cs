
using AShortHike.Randomizer.Extensions;
using UnityEngine;

namespace AShortHike.Randomizer;

public class GoalChecker
{
    public void OpenGoalCheckbox()
    {
        UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

        LinearMenu menu = ui.CreateSimpleMenu();

        GameObject[] items =
        [
            ui.CreateTextMenuItem("Help bird salesman"),
            ui.CreateTextMenuItem("Trade toy shovel"),
            ui.CreateTextMenuItem("Return camping permit"),
        ];

        foreach (GameObject obj in items)
        {
            obj.transform.SetParent(menu.transform, false);
        }

        menu.AddTitle("Help Everyone Checklist");
        menu.Finalize(0);
    }
}
