using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using AShortHike.Randomizer.Connection.Receivers;
using AShortHike.Randomizer.Items;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AShortHike.Randomizer.Connection
{
    public class ConnectionHandler
    {
        private ArchipelagoSession _session;

        private readonly ItemReceiver _itemReceiver = new();
        private const string GAME_NAME = "A Short Hike";

        /// <summary>
        /// Set whenever the server is connected to or when manually disconnecting
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Attempts to connect to the specified server, and on success, will process and store the slot data and begin the game
        /// </summary>
        public bool Connect(string server, string player, string password, bool isContinue)
        {
            // Create login
            LoginResult result;
            string resultMessage;

            // Try connection
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(server);
                _session.Items.ItemReceived += _itemReceiver.OnReceiveItem;
                //_session.Socket.PacketReceived += messageReceiver.OnReceiveMessage;
                _session.Socket.SocketClosed += OnDisconnect;
                result = _session.TryConnectAndLogin(GAME_NAME, player, ItemsHandlingFlags.AllItems, new Version(0, 4, 4), null, null, password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            // Connection failed
            if (!result.Successful)
            {
                Connected = false;
                LoginFailure failure = result as LoginFailure;
                resultMessage = "Multiworld connection failed: ";
                if (failure.Errors.Length > 0)
                    resultMessage += failure.Errors[0];
                else
                    resultMessage += "Reason unknown.";

                Main.LogError(resultMessage);
                Main.Randomizer.Settings.DisplayFailure(resultMessage);
                return false;
            }

            // Connection successful
            Connected = true;
            Main.LogWarning("Multiworld connection successful");
            LoginSuccessful login = result as LoginSuccessful;

            OnConnect(); // remove

            ProcessSlotData(login);
            Main.Randomizer.Settings.BeginGameOnceConnected(isContinue);
            Main.Randomizer.OnConnect();
            return true;
        }

        /// <summary>
        /// Manually disconnect from the server whenever quitting a save file
        /// </summary>
        public void Disconnect()
        {
            if (Connected)
            {
                Main.LogWarning("Disconnected from multiworld");
                _session.Socket.DisconnectAsync();
                OnDisconnect(null); // In case the actual event handler isnt called
            }
        }

        private void OnConnect()
        {

        }

        /// <summary>
        /// Should be called when the socket is closed, but doesnt happen
        /// </summary>
        private void OnDisconnect(string reason)
        {
            Connected = false;
            _session = null;
            ClearReceivers();
            Main.Randomizer.OnDisconnect();
            Main.LogWarning("OnDisconnect called");
        }

        /// <summary>
        /// Receives the list of item locations and settings from the server, and stores the necessary data in the data storage
        /// </summary>
        private void ProcessSlotData(LoginSuccessful login)
        {
            //var apLocations = ((JObject)login.SlotData["locations"]).ToObject<Dictionary<string, ItemLocation>>();
            var settings = ((JObject)login.SlotData["settings"]).ToObject<ServerSettings>() ?? new ServerSettings();

            //Main.Randomizer.Data.StoreItemLocations(apLocations);
            Main.Randomizer.ServerSettings = settings;
            Main.Log($"Received server settings from slot data");

            //List<NewItemLocation> locs = new();
            //foreach (var kvp in apLocations)
            //{
            //    Main.LogError("Storing " + kvp.Key);
            //    locs.Add(new NewItemLocation(kvp.Key, GetLocationNameFromId(kvp.Value.ap_id), kvp.Value.chest_angle));
            //}

            //var sortedLocs = locs.OrderBy(x => x.Id.Contains('['))
            //    .ThenBy(x => x.Id).ToArray();

            //File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Modding", "data", "locations.json"), JsonConvert.SerializeObject(sortedLocs, Formatting.Indented));
        }

        // Receivers

        /// <summary>
        /// Every frame, update all receivers
        /// </summary>
        public void UpdateReceivers()
        {
            _itemReceiver.Update();
        }

        /// <summary>
        /// When disconnecting from the server, clear all receivers
        /// </summary>
        public void ClearReceivers()
        {
            _itemReceiver.ClearItemQueue();
        }

        // Sending

        /// <summary>
        /// When collecting a location in the game, send its id to the server
        /// </summary>
        public async void SendLocation(string locationId)
        {
            if (!Connected)
            {
                Main.LogWarning($"Can't send location {locationId}: Not connected to a server!");
                return;
            }

            ItemLocation location = Main.Randomizer.Data.GetLocationFromId(locationId);
            if (location == null)
            {
                Main.LogWarning($"Can't send location {locationId}: Location doesn't exist!");
                return;
            }

            long apId = _session.Locations.GetLocationIdFromName(GAME_NAME, location.Name);

            Main.Log($"Sending location: {locationId} ({apId})");
            await _session.Locations.CompleteLocationChecksAsync(apId);
        }

        /// <summary>
        /// When first loading the game after connecting, send all collected locations by checking the "Opened_" flag
        /// </summary>
        public async void SendAllLocations()
        {
            if (!Connected)
                return;

            Tags tags = Singleton<GlobalData>.instance.gameData.tags;

            var checkedLocations = Main.Randomizer.Data.GetAllLocations()
                .Where(x => tags.GetBool($"Opened_{x.Key}"))
                .Select(x => _session.Locations.GetLocationIdFromName(GAME_NAME, x.Value.Name))
                .ToArray();

            Main.Log($"Sending all {checkedLocations.Length} locations");
            await _session.Locations.CompleteLocationChecksAsync(checkedLocations);
        }

        /// <summary>
        /// When something happens that would trigger a goal send, check if it exceeds the player's goal and send it to server
        /// </summary>
        public void SendGoal(GoalType goal)
        {
            Main.LogWarning("Obtained goal completion?: " + goal);
            if (goal != Main.Randomizer.ServerSettings.goal)
                return;

            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            _session.Socket.SendPacket(statusUpdatePacket);
        }

        // Helpers

        public string GetPlayerNameFromSlot(int slot)
        {
            return _session.Players.GetPlayerName(slot) ?? "Server";
        }

        public string GetItemNameFromId(long id)
        {
            return _session.Items.GetItemName(id) ?? $"Item[{id}]";
        }

        public string GetLocationNameFromId(long id)
        {
            return _session.Locations.GetLocationNameFromId(id) ?? $"Location[{id}]";
        }
    }
}
