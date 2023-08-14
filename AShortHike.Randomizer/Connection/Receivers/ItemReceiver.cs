using Archipelago.MultiClient.Net.Helpers;
using AShortHike.Randomizer.Items;
using QuickUnityTools.Input;
using System.Collections.Generic;
using UnityEngine;

namespace AShortHike.Randomizer.Connection.Receivers
{
    public class ItemReceiver
    {
        private readonly Queue<QueuedItem> _itemQueue = new();

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

        /// <summary>
        /// When an item is received from the server, queue its data to be processed when in game
        /// </summary>
        public void OnReceiveItem(ReceivedItemsHelper helper)
        {
            lock (itemLock)
            {
                string name = helper.PeekItemName();
                string player = Main.Randomizer.Connection.GetPlayerNameFromSlot(helper.PeekItem().Player);
                int index = helper.Index;
                helper.DequeueItem();

                Main.Log("Queueing item: " + name);
                _itemQueue.Enqueue(new QueuedItem(name, player, index));
            }
        }

        /// <summary>
        /// Every frame, check if an item can be processed, and if so, dequeue it one at a time
        /// </summary>
        public void Update()
        {
            lock (itemLock)
            {
                if (_itemQueue.Count == 0 || !Input.hasFocus)
                    return;

                ProcessItem(_itemQueue.Dequeue());
            }
        }

        /// <summary>
        /// One at a time once the scene is loaded and not in dialog etc, each item will be added and potentially displayed
        /// </summary>
        private void ProcessItem(QueuedItem item)
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
                    return;
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

                // Add the item to the inventory
                Singleton<GlobalData>.instance.gameData.AddCollected(collectable, amount, false);

                // If received from another player, display it in an item prompt
                if (item.player != Main.Randomizer.Settings.SettingsForCurrentSave.player)
                {
                    CollectableItem displayCollectable = ItemCreator.CreateReceivedItem(item.name, item.player);
                    Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(displayCollectable.PickUpRoutine(1));
                }
            }
        }

        /// <summary>
        /// When disconnecting from the server, clear the item queue
        /// </summary>
        public void ClearItemQueue()
        {
            lock (itemLock)
            {
                _itemQueue.Clear();
            }
        }
    }
}
