
using System.Threading.Tasks;
using UnityEngine;

namespace AShortHike.Randomizer.Settings;

public class StatusDisplay
{
    private GameObject _displayText;

    public void DisplayStatus(string message)
    {
        if (_displayText == null)
            CreateDisplayText();

        _displayText.SetActive(true);
        _displayText.transform.SetAsLastSibling();
        UI.SetGenericText(_displayText, message);
    }

    public async void DisplayFailure(string message)
    {
        DisplayStatus(message);

        await Task.Delay(TIME_FOR_FAILURE);
        Hide();
    }

    public void Hide()
    {
        _displayText?.SetActive(false);
    }

    private void CreateDisplayText()
    {
        UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

        _displayText = ui.CreateSimpleDialogue(string.Empty);
        Object.Destroy(_displayText.GetComponent<KillOnBackButton>());
    }

    private const int TIME_FOR_FAILURE = 2000;
}
