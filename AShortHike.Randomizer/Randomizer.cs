using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Items;
using AShortHike.Randomizer.Settings;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class Randomizer
    {
        private readonly ConnectionHandler _connection = new();
        private readonly ItemHandler _items = new();
        private readonly DataStorage _data = new();
        private readonly SettingsHandler _settings = new();

        private string _currentScene;

        public ConnectionHandler Connection => _connection;
        public ItemHandler Items => _items;
        public DataStorage Data => _data;
        public SettingsHandler Settings => _settings;

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
            }
        }

        public void OnConnect()
        {
        }

        public void OnDisconnect()
        {
        }
    }
}
