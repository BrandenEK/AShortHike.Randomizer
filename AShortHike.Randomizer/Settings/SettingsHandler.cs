using AShortHike.Randomizer.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;

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

    [HarmonyPatch(typeof(TitleScreen), "BeginLoadingNewGame")]
    class TitleScreen_NewGame_Patch
    {
        public static bool Prefix()
        {
            Main.Randomizer.Settings.OpenSettingsMenu();
            return false;
        }
    }
}
