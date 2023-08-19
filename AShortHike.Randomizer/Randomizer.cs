using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Items;
using AShortHike.Randomizer.Notifications;
using AShortHike.Randomizer.Settings;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class Randomizer
    {
        private readonly ConnectionHandler _connection = new();
        private readonly ItemHandler _items = new();
        private readonly NotificationHandler _notifications = new();
        private readonly SettingsHandler _settings = new();
        private readonly DataStorage _data = new();

        private string _currentScene;

        public ConnectionHandler Connection => _connection;
        public ItemHandler Items => _items;
        public NotificationHandler Notifications => _notifications;
        public SettingsHandler Settings => _settings;
        public DataStorage Data => _data;

        public MultiworldSettings MultiworldSettings { get; set; } = new();

        public void OnSceneLoaded(string scene)
        {
            if (scene == "GameScene")
            {
                _items.LoadChestObjects();
                _items.ReplaceWorldObjectsWithChests();
                _connection.SendAllLocations();
            }
            else
            {
                _connection.Disconnect();
            }
            
            if (scene == "TitleScene")
            {
                _settings.SetupInputUI();
            }

            _currentScene = scene;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                Main.Log("Giving cheat items!");
                Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("GoldenFeather"), 10, false);
                Singleton<GlobalData>.instance.gameData.AddCollected(CollectableItem.Load("SilverFeather"), 5, false);
            }

            if (_currentScene == "GameScene")
            {
                _connection.UpdateReceivers();
                _notifications.UpdateNotifications();
            }

            // Chest angle testing
            //if (lastChest != null)
            //{
            //    float currAngle = lastChest.localEulerAngles.y;
            //    float factor = 10;
            //    if (Input.GetKeyDown(KeyCode.Equals))
            //    {
            //        currAngle = Mathf.Round((currAngle + factor) / factor) * factor;
            //        lastChest.rotation = Quaternion.Euler(0, currAngle, 0);
            //        Main.LogError("New rotation angle: " + currAngle);
            //    }
            //    else if (Input.GetKeyDown(KeyCode.Minus))
            //    {
            //        currAngle = Mathf.Round((currAngle - factor) / factor) * factor;
            //        lastChest.rotation = Quaternion.Euler(0, currAngle, 0);
            //        Main.LogError("New rotation angle: " + currAngle);
            //    }
            //}
        }

        // Chest angle testing
        public Transform lastChest;

        public void OnConnect()
        {
        }

        public void OnDisconnect()
        {
        }
    }
}
