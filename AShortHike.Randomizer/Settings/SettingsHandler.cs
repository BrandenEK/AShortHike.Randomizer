using AShortHike.Randomizer.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Settings
{
    public class SettingsHandler
    {
        private string server = null;
        private string name = null;
        private string password = null;

        private LinearMenu _settingsMenu;
        private LinearMenu _textMenu;
        private SettingType _currentSetting;

        public void SetupInputUI()
        {

        }

        //public string Server => server;
        //public string Name => name;
        //public string Password => password;

        public void ClearSettings()
        {
            server = null;
            name = null;
            password = null;
        }

        public void DisplayFailure(string message)
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            GameObject obj = ui.CreateTextMenuItem(message);
            ui.AddUI(obj, true);

            // Set text
            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            text.color = new Color(0.55f, 0f, 0f);
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = true;

            // Set position
            RectTransform rect = obj.transform as RectTransform;
            rect.CenterWithinParent();
            rect.sizeDelta = new Vector2((rect.parent as RectTransform).rect.size.x, rect.sizeDelta.y);
            //rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5, (rect.parent as RectTransform).rect.size.x);
            //rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 5, rect.rect.size.y);
            //rect.localScale = new Vector3(0.25f, 0.25f);
            //(text.transform as RectTransform).CenterWithinParent();
            ui.StartCoroutine(DestroyFailedText());

            IEnumerator DestroyFailedText()
            {
                yield return new WaitForSeconds(4f);
                Object.Destroy(text);
            }
        }

        public void OpenTextMenu()
        {
            // Get title and value
            string title, value;
            switch (_currentSetting)
            {
                case SettingType.Server:
                    title = "Enter server ipPort:";
                    value = server;
                    break;
                case SettingType.Name:
                    title = "Enter player name:";
                    value = name;
                    break;
                case SettingType.Password:
                    title = "Enter room password:";
                    value = password;
                    break;
                default:
                    title = "Unknown setting:";
                    value = "unknown";
                    break;
            }

            // Create menu
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            LinearMenu submenu = ui.CreateUndismissableSimpleMenu(new string[0], new System.Action[0]);

            // Create setting text
            GameObject valueObject = ui.CreateTextMenuItem(value);
            valueObject.transform.SetParent(submenu.transform, false);
            valueObject.transform.SetAsFirstSibling();
            valueObject.GetComponentInChildren<TMP_Text>().color = Color.red;
            valueObject.AddComponent<TextInputItem>();

            // Create title text
            GameObject titleObject = ui.CreateTextMenuItem(title);
            titleObject.transform.SetParent(submenu.transform, false);
            titleObject.transform.SetAsFirstSibling();

            // Finish menu
            LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
            (submenu.transform as RectTransform).CenterWithinParent();
            _textMenu = submenu;
        }

        public void CloseTextMenu()
        {
            if (_textMenu == null)
                return;

            string input = _textMenu.GetComponentInChildren<TextInputItem>().FinalInput;
            switch (_currentSetting)
            {
                case SettingType.Server: server = input; break;
                case SettingType.Name: name = input; break;
                case SettingType.Password: password = input; break;
            }
            _textMenu.Kill();
            CloseSettingsMenu();
        }

        public void OpenSettingsMenu()
        {
            // Create menu
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            LinearMenu submenu = null;
            submenu = ui.CreateSimpleMenu(
                new string[]
                {
                    $"Server: <color=#EE0000>{server.DisplayAsDashIfNull()}</color>",
                    $"Name: <color=#EE0000>{name.DisplayAsDashIfNull()}</color>",
                    $"Password: <color=#EE0000>{password.DisplayAsDashIfNull()}</color>",
                    "Confirm",
                    "Back",
                },
                new System.Action[]
                {
                    delegate ()
                    {
                        _currentSetting = SettingType.Server;
                        Main.Randomizer.Settings.OpenTextMenu();
                    },
                    delegate ()
                    {
                        _currentSetting = SettingType.Name;
                        Main.Randomizer.Settings.OpenTextMenu();
                    },
                    delegate ()
                    {
                        _currentSetting = SettingType.Password;
                        Main.Randomizer.Settings.OpenTextMenu();
                    },
                    delegate ()
                    {
                        submenu.Kill();
                        Main.Randomizer.Connection.Connect(server, name, password);
                    },
                    delegate ()
                    {
                        submenu.Kill();
                    },
                });

            // Create title text
            GameObject titleObject = ui.CreateTextMenuItem("Enter multiworld connection details");
            titleObject.transform.SetParent(submenu.transform, false);
            titleObject.transform.SetAsFirstSibling();

            // Finish menu
            LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
            (submenu.transform as RectTransform).CenterWithinParent();
            _settingsMenu = submenu;
        }

        public void CloseSettingsMenu()
        {
            if (_settingsMenu == null)
                return;

            _settingsMenu.Kill();
            OpenSettingsMenu();
            _settingsMenu.selectedIndex = (int)_currentSetting;
        }
    }
}
