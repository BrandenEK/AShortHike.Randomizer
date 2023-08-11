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
            // Create menu
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            LinearMenu submenu = null;
            submenu = ui.CreateUndismissableSimpleMenu(new string[0], new System.Action[0]);
                //new string[]
                //{
                //    "Confirm",
                //},
                //new System.Action[]
                //{
                //    delegate ()
                //    {
                //        string input = submenu.GetComponentInChildren<TextInputItem>().FinalInput;
                //        switch (type)
                //        {
                //            case SettingType.Server: server = input; break;
                //            case SettingType.Name: name = input; break;
                //            case SettingType.Password: password = input; break;
                //        }
                //        submenu.Kill();
                //        Main.Randomizer.Settings.RefreshSettingsMenu();
                //    },
                //});

            string title = _currentSetting switch
            {
                SettingType.Server => "Enter server ipPort:",
                SettingType.Name => "Enter player name:",
                SettingType.Password => "Enter room password:",
                _ => "Unknown setting:"
            };

            string value = _currentSetting switch
            {
                SettingType.Server => server,
                SettingType.Name => name,
                SettingType.Password => password,
                _ => "unknown"
            };

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

            GameObject gameObject = ui.CreateTextMenuItem("Enter multiworld connection details");
            gameObject.transform.SetParent(submenu.transform, false);
            gameObject.transform.SetAsFirstSibling();
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
