using AShortHike.Randomizer.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Settings
{
    public class SettingsHandler
    {
        private LinearMenu _settingsMenu;
        private LinearMenu _textMenu;

        // These settings only persist while menuing and should be reset before that
        private SettingType _currentSetting;
        private bool _currentIsContinue;
        private string _currentServer;
        private string _currentPlayer;
        private string _currentPassword;

        public SettingsInfo SettingsConfig { get; private set; }

        public void SetupInputUI()
        {
            // Load config from file
            SettingsConfig = Main.Randomizer.Data.LoadConfig();
        }

        public void RestoreMenuSettings(ConnectionInfo settings, bool isContinue)
        {
            _currentServer = settings.server;
            _currentPlayer = settings.player;
            _currentPassword = settings.password;
            _currentIsContinue = isContinue;
        }

        public ConnectionInfo SettingsForCurrentSave
        {
            get
            {
                string save = GlobalData.currentSaveFile;
                if (save.Contains("2"))
                    return SettingsConfig.saveSlotThree;
                else if (save.Contains("1"))
                    return SettingsConfig.saveSlotTwo;
                else
                    return SettingsConfig.saveSlotOne;
            }
            set
            {
                string save = GlobalData.currentSaveFile;
                if (save.Contains("2"))
                    SettingsConfig.saveSlotThree = value;
                else if (save.Contains("1"))
                    SettingsConfig.saveSlotTwo = value;
                else
                    SettingsConfig.saveSlotOne = value;
            }
        }

        public void BeginGameOnceConnected(bool isContinue)
        {
            TitleScreen title = Object.FindObjectOfType<TitleScreen>();
            if (title == null)
            {
                Main.Randomizer.Connection.Disconnect();
                return;
            }

            // Save connection info
            SettingsForCurrentSave = new ConnectionInfo(_currentServer, _currentPlayer, _currentPassword);
            Main.Randomizer.Data.SaveConfig(SettingsConfig);

            // Load game scene
            if (isContinue)
                title.ContinueGame();
            else
                title.StartNewGame();
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
                    value = _currentServer;
                    break;
                case SettingType.Name:
                    title = "Enter player name:";
                    value = _currentPlayer;
                    break;
                case SettingType.Password:
                    title = "Enter room password:";
                    value = _currentPassword;
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
                case SettingType.Server: _currentServer = input; break;
                case SettingType.Name: _currentPlayer = input; break;
                case SettingType.Password: _currentPassword = input; break;
            }
            _textMenu.Kill();
            CloseSettingsMenu();
        }

        public void OpenSettingsMenu(int index)
        {
            // Get title and option
            string title = $"{(_currentIsContinue ? "Confirm" : "Enter")} multiworld connection details";
            string option = _currentIsContinue ? "Continue game" : "Start game";

            // Create menu
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);
            LinearMenu submenu = null;
            submenu = ui.CreateSimpleMenu(
                new string[]
                {
                    $"Server: <color=#EE0000>{_currentServer.DisplayAsDashIfNull()}</color>",
                    $"Name: <color=#EE0000>{_currentPlayer.DisplayAsDashIfNull()}</color>",
                    $"Password: <color=#EE0000>{_currentPassword.DisplayAsDashIfNull()}</color>",
                    option,
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
                        Main.Randomizer.Connection.Connect(_currentServer, _currentPlayer, _currentPassword, _currentIsContinue);
                    },
                    delegate ()
                    {
                        submenu.Kill();
                    },
                });

            // Create title text
            GameObject titleObject = ui.CreateTextMenuItem(title);
            titleObject.transform.SetParent(submenu.transform, false);
            titleObject.transform.SetAsFirstSibling();

            // Finish menu
            LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as RectTransform);
            (submenu.transform as RectTransform).CenterWithinParent();
            submenu.selectedIndex = index;
            _settingsMenu = submenu;
        }

        public void CloseSettingsMenu()
        {
            if (_settingsMenu == null)
                return;

            _settingsMenu.Kill();
            OpenSettingsMenu((int)_currentSetting);
        }
    }
}
