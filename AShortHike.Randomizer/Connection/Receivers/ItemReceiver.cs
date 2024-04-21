using Archipelago.MultiClient.Net.Helpers;
using AShortHike.Randomizer.Extensions;
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

        private bool CanReceiveItem
        {
            get
            {
                Player player = Singleton<GameServiceLocator>.instance?.levelController?.player;
                return Input.hasFocus && player != null && !player.isClimbing && !player.isGliding;
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

                Main.Randomizer.LogHandler.Info("Queueing item: " + name);
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
                if (_itemQueue.Count > 0 && CanReceiveItem)
                    ProcessItem(_itemQueue.Dequeue());
            }
        }

        /// <summary>
        /// One at a time once the scene is loaded and not in dialog etc, each item will be added and potentially displayed
        /// </summary>
        private void ProcessItem(QueuedItem item)
        {
            int itemsReceived = Singleton<GlobalData>.instance.gameData.tags.GetInt("ITEMS_RECEIVED");
            Main.Randomizer.LogHandler.Warning($"Receiving item: {item.name} at index {item.index} with {itemsReceived} items received");

            if (item.index > itemsReceived)
            {
                Singleton<GlobalData>.instance.gameData.tags.SetInt("ITEMS_RECEIVED", itemsReceived + 1);

                CollectableItem collectable = Main.ItemStorage.GetItemFromName(item.name, out int amount);
                if (collectable == null)
                {
                    Main.Randomizer.LogHandler.Error("Received invalid item: " + item.name);
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

                if (collectable.name == "FishingRod" && Singleton<GlobalData>.instance.gameData.HasFishingRod())
                {
                    // If receiving a second fishing rod remove the first and give golden
                    Singleton<GlobalData>.instance.gameData.AddCollected(collectable, -1, false);
                    collectable = Main.ItemStorage.GetItemFromName("Golden Fishing Rod", out _);
                    Main.Randomizer.LogHandler.Info("Changing fishing rod to golden version");
                }

                // Add the item to the inventory
                Singleton<GlobalData>.instance.gameData.AddCollected(collectable, amount, false);

                // If received from another player, display it in an item prompt
                if (item.player != Main.Randomizer.ClientSettings.player)
                {
                    //Main.Randomizer.Notifications.AddNotification(item.name, item.player);
                    CollectableItem display = ItemCreator.CreateReceivedItem(item.name, item.player);
                    Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(display.PickUpRoutine(1));
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
