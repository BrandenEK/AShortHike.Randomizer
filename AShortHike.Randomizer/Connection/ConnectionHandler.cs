using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using AShortHike.Randomizer.Items;
using System;

namespace AShortHike.Randomizer.Connection
{
    public class ConnectionHandler
    {
        private ArchipelagoSession _session;

        public bool Connected { get; private set; }

        //public bool ConnectFromTitleScreen()
        //{
        //    return Connect(Main.Randomizer.Settings.Server, Main.Randomizer.Settings.Name, Main.Randomizer.Settings.Password);
        //}

        public bool Connect(string server, string player, string password)
        {
            // Create login
            LoginResult result;
            string resultMessage;

            // Try connection
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(server);
                _session.Items.ItemReceived += OnReceiveItem;
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
                Main.Randomizer.Settings.DisplayFailure(resultMessage);//$"Failed to connect to '{server}' as '{player}' with password '{password}'");
                return false;
            }

            // Connection successful
            Connected = true;
            Main.LogWarning("Multiworld connection successful");
            LoginSuccessful login = result as LoginSuccessful;

            OnConnect();
            return true;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        private void OnConnect()
        {

        }

        private void OnDisconnect(string reason)
        {

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
            _session.Locations.CompleteLocationChecks(81000 + location.apId);
        }

        // Receiving (Temp - move to receiver class)

        private void OnReceiveItem(ReceivedItemsHelper helper)
        {
            int itemIndex = helper.Index;
            string itemName = helper.PeekItemName();
            helper.DequeueItem();

            int itemsReceived = Singleton<GlobalData>.instance.gameData.tags.GetInt("ITEMS_RECEIVED");
            Main.LogWarning($"Receiving item: {itemName} at index {itemIndex} with {itemsReceived} items received");

            if (itemIndex > itemsReceived)
            {
                // Queue this until in game and grounded
                Singleton<GlobalData>.instance.gameData.tags.SetInt("ITEMS_RECEIVED", itemsReceived + 1);
                CollectableItem item = Main.Randomizer.Data.GetItemFromName(itemName, out int amount);
                Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(item.PickUpRoutine(amount));
            }
        }
    }
}
