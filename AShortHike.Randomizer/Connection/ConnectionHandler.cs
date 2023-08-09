using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;

namespace AShortHike.Randomizer.Connection
{
    public class ConnectionHandler
    {
        private ArchipelagoSession _session;

        public bool Connected { get; private set; }

        public bool Connect(string server, string player, string password)
        {
            // Create login
            LoginResult result;
            string resultMessage;

            // Try connection
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(server);
                //_session.Items.ItemReceived += itemReceiver.OnReceiveItem;
                //_session.Socket.PacketReceived += messageReceiver.OnReceiveMessage;
                _session.Socket.SocketClosed += OnDisconnect;
                result = _session.TryConnectAndLogin("A Short Hike", player, ItemsHandlingFlags.AllItems, new Version(0, 4, 1), null, null, password);
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
    }
}
