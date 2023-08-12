using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using AShortHike.Randomizer.Connection.Receivers;
using AShortHike.Randomizer.Items;
using System;
using System.Collections.Generic;

namespace AShortHike.Randomizer.Connection
{
    public class ConnectionHandler
    {
        private ArchipelagoSession _session;

        private readonly ItemReceiver _itemReceiver = new();

        public bool Connected { get; private set; }

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
                result = _session.TryConnectAndLogin("A Short Hike", player, ItemsHandlingFlags.AllItems, new Version(0, 4, 2), null, null, password);
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

            Main.Randomizer.Settings.BeginGameOnceConnected(isContinue);
            Main.Randomizer.OnConnect();
            return true;
        }

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

        private void OnDisconnect(string reason)
        {
            Connected = false;
            _session = null;
            ClearReceivers();
            Main.Randomizer.OnDisconnect();
            Main.LogWarning("OnDisconnect called");
        }

        // Receivers

        public void UpdateReceivers()
        {
            _itemReceiver.Update();
        }

        public void ClearReceivers()
        {
            _itemReceiver.ClearItemQueue();
        }

        // Sending

        public void SendLocation(string locationId)
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

            Main.Log($"Sending location: {locationId} ({location.apId})");
            _session.Locations.CompleteLocationChecksAsync(81000 + location.apId);
        }

        public void SendAllLocations()
        {
            if (!Connected)
                return;

            var checkedLocations = new List<long>();
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;

            foreach (ItemLocation location in Main.Randomizer.Data.GetAllLocations())
            {
                if (tags.GetBool("Opened_" + location.gameId))
                    checkedLocations.Add(81000 + location.apId);
            }

            Main.Log($"Sending all {checkedLocations.Count} locations");
            _session.Locations.CompleteLocationChecksAsync(checkedLocations.ToArray());
        }

        // Helpers

        public string GetPlayerNameFromSlot(int slot)
        {
            return _session.Players.GetPlayerName(slot) ?? "Server";
        }
    }
}
