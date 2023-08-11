using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;

namespace AShortHike.Randomizer.Settings
{
    public class SettingsHandler
    {
        private string server = "localhost";
        private string name = "Player";
        private string password = null;

        private LinearMenu _settingsMenu;

        public void SetupInputUI()
        {
            TextInput text = Object.FindObjectOfType<TextInput>();
            if (text != null)
            {
                Main.LogWarning(text.name);
            }
            else
            {
                Main.LogError("Couldnt find text");
            }
        }

        public void OpenTextMenu(SettingType type)
        {
            // Create menu
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            LinearMenu submenu = null;
            submenu = ui.CreateSimpleMenu(
                new string[]
                {
                    "Confirm",
                },
                new System.Action[]
                {
                    delegate ()
                    {
                        switch (type)
                        {
                            case SettingType.Server:
                                server = "archipelago.gg:24242"; break;
                            case SettingType.Name:
                                name = "Damocles"; break;
                            case SettingType.Password:
                                password = "12345678"; break;
                        }
                        submenu.Kill();
                        Main.Randomizer.Settings.RefreshSettingsMenu();
                    },
                });

            string title = type switch
            {
                SettingType.Server => "Server ipPort:",
                SettingType.Name => "Player name:",
                SettingType.Password => "Room password:",
                _ => "Unknown setting:"
            };

            string value = type switch
            {
                SettingType.Server => server,
                SettingType.Name => name,
                SettingType.Password => password,
                _ => "unknown"
            };

            // Create setting text
            GameObject valueObject = ui.CreateTextMenuItem(value);
            valueObject.GetComponentInChildren<TMP_Text>().color = Color.red;
            valueObject.transform.SetParent(submenu.transform, false);
            valueObject.transform.SetAsFirstSibling();

            // Create title text
            GameObject titleObject = ui.CreateTextMenuItem(title);
            titleObject.transform.SetParent(submenu.transform, false);
            titleObject.transform.SetAsFirstSibling();

            // Finish menu
            LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
            (submenu.transform as RectTransform).CenterWithinParent();
        }

        public void OpenSettingsMenu()
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            LinearMenu submenu = null;
            submenu = ui.CreateSimpleMenu(
                new string[]
                {
                    $"Server: <color=#EE0000>{server}</color>",
                    $"Name: <color=#EE0000>{name}</color>",
                    $"Password: <color=#EE0000>{password}</color>",
                    "Confirm",
                    "Back",
                },
                new System.Action[]
                {
                    delegate ()
                    {
                        //submenu.Kill();
                        Main.Randomizer.Settings.OpenTextMenu(SettingType.Server);
                    },
                    delegate ()
                    {
                        //submenu.Kill();
                        Main.Randomizer.Settings.OpenTextMenu(SettingType.Name);
                    },
                    delegate ()
                    {
                        //submenu.Kill();
                        Main.Randomizer.Settings.OpenTextMenu(SettingType.Password);
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

            //foreach (GameObject item in submenu.GetMenuObjects())
            //{
            //    Main.LogWarning("Use rich text: " + item.GetComponentInChildren<TMP_Text>().richText);
            //    item.GetComponentInChildren<TMP_Text>().richText = true; // Is this necessary
            //}

            GameObject gameObject = ui.CreateTextMenuItem("Confirm multiworld connection details");
            gameObject.transform.SetParent(submenu.transform, false);
            gameObject.transform.SetAsFirstSibling();
            LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
            (submenu.transform as RectTransform).CenterWithinParent();
            _settingsMenu = submenu;
        }

        public void RefreshSettingsMenu()
        {
            if (_settingsMenu == null)
                return;

            _settingsMenu.Kill();
            OpenSettingsMenu();
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
