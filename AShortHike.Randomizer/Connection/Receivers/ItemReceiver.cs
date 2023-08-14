using Archipelago.MultiClient.Net.Helpers;
using QuickUnityTools.Input;
using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer.Connection.Receivers
{
    public class ItemReceiver
    {
        private readonly List<QueuedItem> _itemQueue = new();

        private static readonly object itemLock = new();

        private GameUserInput _input;
        private GameUserInput Input
        {
            get
            {
                if (_input == null)
                    _input = Object.FindObjectOfType<GameUserInput>();
                return _input;
            }
        }

        public void OnReceiveItem(ReceivedItemsHelper helper)
        {
            lock (itemLock)
            {
                string name = helper.PeekItemName();
                string player = Main.Randomizer.Connection.GetPlayerNameFromSlot(helper.PeekItem().Player);
                int index = helper.Index;
                helper.DequeueItem();

                Main.Log("Queueing item: " + name);
                _itemQueue.Add(new QueuedItem(name, player, index));
            }
        }

        public void Update()
        {
            lock (itemLock)
            {
                if (_itemQueue.Count == 0 || !Input.hasFocus)
                    return;

                // Make sure in game and grounded and not in item display
                Main.Log("Processing item queue");

                foreach (QueuedItem item in _itemQueue)
                {
                    int itemsReceived = Singleton<GlobalData>.instance.gameData.tags.GetInt("ITEMS_RECEIVED");
                    Main.LogWarning($"Receiving item: {item.name} at index {item.index} with {itemsReceived} items received");

                    if (item.index > itemsReceived)
                    {
                        Singleton<GlobalData>.instance.gameData.tags.SetInt("ITEMS_RECEIVED", itemsReceived + 1);

                        CollectableItem collectable = Main.Randomizer.Data.GetItemFromName(item.name, out int amount);
                        if (collectable == null)
                        {
                            Main.LogError("Received invalid item: " + item.name);
                            continue;
                        }

                        if (collectable.name == "TreasureMap")
                        {
                            // Maps also need to set a specific flag
                            int num = item.name switch
                            {
                                "A Stormy View Map" => 1,
                                "In Her Shadow Map" => 2,
                                "The King Map" => 3,
                                "The Treasure of Sid Beach Map" => 4,
                                _ => 0
                            };
                            Singleton<GlobalData>.instance.gameData.tags.SetBool("TMap" + num);
                        }

                        Singleton<GlobalData>.instance.gameData.AddCollected(collectable, amount, false);
                        if (item.player != Main.Randomizer.Settings.SettingsForCurrentSave.player)
                        {
                            Main.Randomizer.Items.DisplayReceivedItem(item.name, item.player);
                        }
                    }
                }

                _itemQueue.Clear();
            }
        }

        public void ClearItemQueue()
        {
            lock (itemLock)
            {
                _itemQueue.Clear();
            }
        }
    }
}
