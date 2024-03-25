using AShortHike.Randomizer.Extensions;
using AShortHike.Randomizer.Items;
using AShortHike.Randomizer.Models;
using HarmonyLib;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Patches
{
    /// <summary>
    /// These 3 patches just show the location of the object to replace - will be gone soon
    /// </summary>
    [HarmonyPatch(typeof(CollectOnInteract), nameof(CollectOnInteract.Collect))]
    class Interactable_Collect_Patch
    {
        public static void Postfix(CollectOnInteract __instance)
        {
            Main.LogWarning("Collecting interactable: " + __instance.GetLocationId());
        }
    }
    [HarmonyPatch(typeof(CollectOnTouch), nameof(CollectOnTouch.Collect))]
    class Touchable_Collect_Patch
    {
        public static void Postfix(CollectOnTouch __instance)
        {
            Main.LogWarning("Collecting touchable: " + __instance.GetLocationId());
        }
    }
    [HarmonyPatch(typeof(Holdable), nameof(Holdable.Interact))]
    class Holdable_Collect_Patch
    {
        public static void Postfix(Holdable __instance)
        {
            Main.LogWarning("Collecting holdable: " + __instance.GetLocationId());
        }
    }

    /// <summary>
    /// When interacting with a chest, send the location and display the item there
    /// </summary>
    [HarmonyPatch(typeof(Chest), nameof(Chest.Interact))]
    class Chest_Collect_Patch
    {
        public static void Postfix(Chest __instance)
        {
            string locationId = __instance.GetComponent<GameObjectID>().id;
            Main.LogWarning($"Opening chest: {locationId} at location {__instance.transform.position}");

            if (Main.LocationStorage.TryGetLocation(locationId, out ItemLocation location))
                Main.Randomizer.Items.CollectLocation(location, true);

            // Chest angle testing
            //Main.Randomizer.lastChest = __instance.transform;
        }
    }

    /// <summary>
    /// When an npc gives you an item through dialog, send the location and display the item there
    /// </summary>
    [HarmonyPatch(typeof(YarnCommands), nameof(YarnCommands.GiveItem))]
    class Dialog_GiveItem_Patch
    {
        public static void Prefix(IConversation context, ref string[] args)
        {
            // Only give random item if the amount is positive
            if (args.Length < 2 || int.TryParse(args[1], out int amount) && amount > 0)
            {
                Main.LogWarning("Receiving item from conversation: " + context.originalSpeaker.name + ", " + args[0]);

                string locationId = context.GetLocationId(args[0]);

                if (Main.LocationStorage.TryGetLocation(locationId, out ItemLocation location))
                {
                    args = [locationId];
                    Main.Randomizer.Items.CollectLocation(location, false);
                }
            }
            else if (args.Length > 1 && args[0] == "FishingRod" && args[1] == "-1")
            {
                // If the fisher guy is trying to take away the fishing rod, dont let him
                Main.LogWarning("Preventing loss of fishing rod!");
                args = ["FishingRod", "0"];
            }
        }
    }

    /// <summary>
    /// When displaying a collected item, don't actually add its value anywhere
    /// </summary>
    [HarmonyPatch(typeof(GlobalData.GameData), nameof(GlobalData.GameData.AddCollected))]
    class GameData_AddItem_Patch
    {
        public static bool Prefix(CollectableItem item)
        {
            return item.name != "APL" && item.name != "APR";
        }
    }

    /// <summary>
    /// When displaying a collected item, change the text that is displayed
    /// </summary>
    [HarmonyPatch(typeof(ItemPrompt), nameof(ItemPrompt.Setup))]
    class ItemPrompt_ChangeText_Patch
    {
        public static void Postfix(ItemPrompt __instance, CollectableItem item)
        {
            if (item.name == "APL")
                __instance.beforeName.GetComponent<Text>().text = "Found";
            else if (item.name == "APR")
                __instance.beforeName.GetComponent<Text>().text = "Received";
        }
    }

    /// <summary>
    /// Calling this method with the locationId of a valid location will instead return a new ApItem
    /// Used when setting the item argument in dialog
    /// </summary>
    [HarmonyPatch(typeof(CollectableItem), nameof(CollectableItem.Load))]
    class CollectableItem_GetItem_Patch
    {
        public static bool Prefix(string name, ref CollectableItem __result)
        {
            // If this isnt a valid location, get the regular item at this name
            if (!Main.LocationStorage.TryGetLocation(name, out ItemLocation location))
                return true;

            Item item = Main.ItemMapper.GetItemAtLocation(location);
            __result = ItemCreator.CreateFoundItem(item.Name, item.Player);

            return false;
        }
    }
}
