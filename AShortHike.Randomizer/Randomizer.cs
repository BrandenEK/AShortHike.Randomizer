using AShortHike.ModdingAPI;
using AShortHike.Randomizer.Connection;
using AShortHike.Randomizer.Goal;
using AShortHike.Randomizer.Items;
using AShortHike.Randomizer.Notifications;
using AShortHike.Randomizer.Settings;
using UnityEngine;

namespace AShortHike.Randomizer
{
    public class Randomizer : ShortHikeMod
    {
        public Randomizer() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

        private readonly ConnectionHandler _connection = new();
        private readonly ItemHandler _items = new();
        private readonly NotificationHandler _notifications = new();
        private readonly SettingsHandler _settings = new();

        public ConnectionHandler Connection => _connection;
        public ItemHandler Items => _items;
        public NotificationHandler Notifications => _notifications;
        public SettingsHandler Settings => _settings;
        public GoalHandler GoalHandler { get; } = new();

        // Both are set right after connecting to server before loading game scene
        public ServerSettings ServerSettings { get; set; } = new();
        public ClientSettings ClientSettings { get; set; } = new();

        protected override void OnLevelLoaded(string level)
        {
            if (level == "GameScene")
            {
                _items.LoadChestObjects();
                _items.ReplaceWorldObjectsWithChests();
                _connection.SendLocations(Main.LocationStorage.GetAllCheckedLocations());
                _settings.SaveFileSettings = ClientSettings;
            }
            else
            {
                _connection.Disconnect();
            }
        }

        protected override void OnUpdate()
        {
            _connection.UpdateReceivers();
            _notifications.UpdateNotifications();

            if (Input.GetKeyDown(KeyCode.G))
            {
                GoalHandler.ToggleGoalDisplay();
            }

            // temp debug !!!!
            if (Input.GetKeyDown(KeyCode.H))
            {
                ServerSettings = new(GoalType.Photo);
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
            //        Main.Randomizer.LogHandler.Error("New rotation angle: " + currAngle);
            //    }
            //    else if (Input.GetKeyDown(KeyCode.Minus))
            //    {
            //        currAngle = Mathf.Round((currAngle - factor) / factor) * factor;
            //        lastChest.rotation = Quaternion.Euler(0, currAngle, 0);
            //        Main.Randomizer.LogHandler.Error("New rotation angle: " + currAngle);
            //    }
            //}
        }

        // Chest angle testing
        public Transform lastChest;
    }
}
