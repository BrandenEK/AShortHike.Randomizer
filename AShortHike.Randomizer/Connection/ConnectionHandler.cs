using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using AShortHike.Randomizer.Connection.Receivers;
using AShortHike.Randomizer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

                Main.Randomizer.LogHandler.Error("Failed to connect");
                return false;
            }

            // Connection successful
            Connected = true;
            Main.Randomizer.LogHandler.Warning("Multiworld connection successful");
            LoginSuccessful login = result as LoginSuccessful;

            ProcessSlotData(login);

            return true;
        }

        /// <summary>
        /// Manually disconnect from the server whenever quitting a save file
        /// </summary>
        public void Disconnect()
        {
            if (Connected)
            {
                Main.Randomizer.LogHandler.Warning("Disconnected from multiworld");
                _session.Socket.DisconnectAsync();
                OnDisconnect(null); // In case the actual event handler isnt called
            }
        }

        /// <summary>
        /// Should be called when the socket is closed, but doesnt happen
        /// </summary>
        private void OnDisconnect(string reason)
        {
            Connected = false;
            _session = null;
            ClearReceivers();
            Main.Randomizer.LogHandler.Warning("OnDisconnect called");
            Main.ItemMapper.OnDisconnect();
        }

        /// <summary>
        /// Receives the server settings from the slot data
        /// </summary>
        private void ProcessSlotData(LoginSuccessful login)
        {
            var settings = ((JObject)login.SlotData["settings"]).ToObject<ServerSettings>() ?? new ServerSettings();

            Main.Randomizer.ServerSettings = settings;
            Main.Randomizer.LogHandler.Info($"Received server settings from slot data");
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

        public async Task<Item> ScoutLocation(ItemLocation location)
        {
            if (!Connected)
            {
                //Main.Randomizer.LogHandler.Warning($"Can't scout location {location.Id}: Not connected to a server!");
                throw new Exception($"Can't scout location {location.Id}: Not connected to a server!");
            }

            long apId = _session.Locations.GetLocationIdFromName(GAME_NAME, location.Name);

            var packet = await _session.Locations.ScoutLocationsAsync(false, apId);
            if (packet.Locations.Length == 0)
                throw new Exception($"Failed to scout location for {location.Id}");

            NetworkItem item = packet.Locations[0];
            string playerName = GetPlayerNameFromSlot(item.Player);
            string itemName = GetItemNameFromId(item.Item);

            return new Item(itemName, playerName, item.Flags == ItemFlags.Advancement || item.Flags == ItemFlags.Trap);
        }

        public async Task<Dictionary<ItemLocation, Item>> ScoutLocations(IEnumerable<ItemLocation> locations)
        {
            if (!Connected)
                throw new Exception($"Can't scout locations: Not connected to a server!");

            long[] apIds = locations.Select(x => _session.Locations.GetLocationIdFromName(GAME_NAME, x.Name)).ToArray();

            var packet = await _session.Locations.ScoutLocationsAsync(false, apIds);
            if (packet.Locations.Length == 0)
                throw new Exception($"Failed to scout locations group");

            return packet.Locations.ToDictionary(
                x => Main.LocationStorage.GetAllLocations().First(l => l.Name == GetLocationNameFromId(x.Location)),
                x => new Item(
                    player: GetPlayerNameFromSlot(x.Player),
                    name: GetItemNameFromId(x.Item),
                    progression: x.Flags == ItemFlags.Advancement || x.Flags == ItemFlags.Trap));
        }

        /// <summary>
        /// When collecting a location in the game, send its id to the server
        /// </summary>
        public async void SendLocation(ItemLocation location)
        {
            if (!Connected)
            {
                Main.Randomizer.LogHandler.Warning($"Can't send location {location.Id}: Not connected to a server!");
                return;
            }

            long apId = _session.Locations.GetLocationIdFromName(GAME_NAME, location.Name);

            Main.Randomizer.LogHandler.Info($"Sending location: {location.Id} ({apId})");
            await _session.Locations.CompleteLocationChecksAsync(apId);
        }

        /// <summary>
        /// When first loading the game after connecting, send all collected locations
        /// </summary>
        public async void SendLocations(IEnumerable<ItemLocation> locations)
        {
            if (!Connected)
            {
                Main.Randomizer.LogHandler.Warning($"Can't send all locations: Not connected to a server!");
                return;
            }

            long[] apIds = locations.Select(x => _session.Locations.GetLocationIdFromName(GAME_NAME, x.Name)).ToArray();

            Main.Randomizer.LogHandler.Info($"Sending {apIds.Length} locations");
            await _session.Locations.CompleteLocationChecksAsync(apIds);
        }

        /// <summary>
        /// Sends the goal completion packet and performs no validation on the goal
        /// </summary>
        public void SendGoal()
        {
            Main.Randomizer.LogHandler.Warning("Sending goal completion");

            _session.Socket.SendPacket(new StatusUpdatePacket()
            {
                Status = ArchipelagoClientState.ClientGoal
            });
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
