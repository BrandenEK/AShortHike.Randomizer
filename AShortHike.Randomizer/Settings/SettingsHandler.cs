using AShortHike.Randomizer.Extensions;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AShortHike.Randomizer.Settings
{
    public class SettingsHandler
    {
        private readonly StatusDisplay _statusDisplay = new();

        private LinearMenu _settingsMenu;
        private LinearMenu _textMenu;
        private LinearMenu _qualityMenu;

        // These settings only persist while menuing and should be reset before that
        private SettingType _currentSetting;
        private bool _currentIsContinue;
        private string _currentServer;
        private string _currentPlayer;
        private string _currentPassword;
        private bool _goldenChests;
        private bool _skipCutscenes;
        private bool _fastText;
        private bool _hidePassword;

        /// <summary>
        /// Before opening a begin/continue, always need to reset the settings from last time
        /// </summary>
        public void RestoreMenuSettings(ClientSettings settings, bool isContinue)
        {
            _currentServer = settings.server;
            _currentPlayer = settings.player;
            _currentPassword = settings.password;
            _skipCutscenes = settings.skipCutscenes;
            _goldenChests = settings.goldenChests;
            _fastText = settings.fastText;
            _hidePassword = settings.hidePassword;
            _currentIsContinue = isContinue;
        }

        /// <summary>
        /// Saves or loads the client settings to/from the actual save file
        /// </summary>
        public ClientSettings SaveFileSettings
        {
            get
            {
                Main.Randomizer.LogHandler.Warning("Loading client settings from save file");
                Tags tags = Singleton<GlobalData>.instance.gameData.tags;

                return new ClientSettings(
                    tags.GetString("AP.server"),
                    tags.GetString("AP.player"),
                    tags.GetString("AP.password"),
                    tags.GetBool("AP.cutscenes"),
                    tags.GetBool("AP.text"),
                    tags.GetBool("AP.chests"),
                    tags.GetBool("AP.hide"));
            }
            set
            {
                Main.Randomizer.LogHandler.Warning("Saving client settings to save file");
                Tags tags = Singleton<GlobalData>.instance.gameData.tags;

                tags.SetString("AP.server", value.server);
                tags.SetString("AP.player", value.player);
                tags.SetString("AP.password", value.password);
                tags.SetBool("AP.cutscenes", value.skipCutscenes);
                tags.SetBool("AP.text", value.fastText);
                tags.SetBool("AP.chests", value.goldenChests);
                tags.SetBool("AP.hide", value.hidePassword);
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
            Main.Randomizer.ClientSettings = new ClientSettings(
                _currentServer, _currentPlayer, _currentPassword, _skipCutscenes, _fastText, _goldenChests, _hidePassword);

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

        private async void TryConnectAndScout()
        {
            _statusDisplay.DisplayStatus("Connecting...");
            await Task.Delay(100);

            if (!Main.Randomizer.Connection.Connect(_currentServer, _currentPlayer, _currentPassword, _currentIsContinue))
            {
                _statusDisplay.DisplayFailure("Multiworld connection failed");
                return;
            }

            //_statusDisplay.DisplayStatus("Scouting...");

            if (!await Main.ItemMapper.OnConnect())
            {
                _statusDisplay.DisplayFailure("Multiworld connection failed");
                //_statusDisplay.DisplayFailure("Location scouting failed");
                return;
            }

            CloseSettingsMenu();
            _statusDisplay.Hide();

            BeginGameOnceConnected(_currentIsContinue);
        }

        // Settings menu

        public void OpenSettingsMenu(int index)
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

            var options = new string[]
            {
                $"Server: <color=#EE0000>{_currentServer.DisplayAsDashIfNull()}</color>",
                $"Name: <color=#EE0000>{_currentPlayer.DisplayAsDashIfNull()}</color>",
                $"Password: <color=#EE0000>{_currentPassword.DisplayAsDashIfNull()}</color>",
                $"QoL settings",
                _currentIsContinue ? "Continue game" : "Start game",
                "Back",
            };

            var events = new System.Action[]
            {
                delegate ()
                {
                    _currentSetting = SettingType.Server;
                    OpenTextMenu();
                },
                delegate ()
                {
                    _currentSetting = SettingType.Name;
                    OpenTextMenu();
                },
                delegate ()
                {
                    _currentSetting = SettingType.Password;
                    OpenTextMenu();
                },
                delegate ()
                {
                    _currentSetting = SettingType.Quality;
                    OpenQualityMenu(0);
                },
                TryConnectAndScout,
                CloseSettingsMenu,
            };

            _settingsMenu = ui.CreateSimpleMenu(options, events)
                .AddTitle($"{(_currentIsContinue ? "Confirm" : "Enter")} multiworld connection details")
                .Finalize(index);
        }

        private void CloseSettingsMenu()
        {
            _settingsMenu?.Kill();
        }

        private void RefreshSettingsMenu()
        {
            _settingsMenu?.Kill();
            OpenSettingsMenu((int)_currentSetting);
        }

        // Text menu

        private void OpenTextMenu()
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

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
            _textMenu = ui.CreateUndismissableSimpleMenu(new string[0], new System.Action[0])
                .AddTextInput(value)
                .AddTitle(title)
                .Finalize(0);
        }

        public void CloseTextMenu()
        {
            string input = _textMenu.GetComponentInChildren<TextInputItem>().FinalInput;
            switch (_currentSetting)
            {
                case SettingType.Server: _currentServer = input; break;
                case SettingType.Name: _currentPlayer = input; break;
                case SettingType.Password: _currentPassword = input; break;
            }

            _textMenu?.Kill();
            RefreshSettingsMenu();
        }

        // Quality Menu

        private void OpenQualityMenu(int index)
        {
            UI ui = Singleton<ServiceLocator>.instance.Locate<UI>(false);

            var options = new string[]
            {
                $"Golden chests: <color=#EE0000>{_goldenChests.DisplayONOFF()}</color>",
                $"Skip cutscenes: <color=#EE0000>{_skipCutscenes.DisplayONOFF()}</color>",
                $"Fast text: <color=#EE0000>{_fastText.DisplayONOFF()}</color>",
                $"Hide password: <color=#EE0000>{_hidePassword.DisplayONOFF()}</color>",
                "Back",
            };

            var events = new System.Action[]
            {
                delegate ()
                {
                    _goldenChests = !_goldenChests;
                    RefreshQualityMenu(0);
                },
                delegate ()
                {
                    _skipCutscenes = !_skipCutscenes;
                    RefreshQualityMenu(1);
                },
                delegate ()
                {
                    _fastText = !_fastText;
                    RefreshQualityMenu(2);
                },
                delegate ()
                {
                    _hidePassword = !_hidePassword;
                    RefreshQualityMenu(3);
                },
                CloseQualityMenu,
            };

            _qualityMenu = ui.CreateUndismissableSimpleMenu(options, events)
                .AddTitle("Quality of life settings")
                .Finalize(index);
        }

        private void CloseQualityMenu()
        {
            _qualityMenu?.Kill();
            RefreshSettingsMenu();
        }

        private void RefreshQualityMenu(int index)
        {
            _qualityMenu?.Kill();
            OpenQualityMenu(index);
        }
    }
}
