using HarmonyLib;
using UnityEngine.UI;

namespace AShortHike.Randomizer.Items
{
    /// <summary>
    /// These 3 patches just show the location of the object to replace - will be gone soon
    /// </summary>
    [HarmonyPatch(typeof(CollectOnInteract), nameof(CollectOnInteract.Collect))]
    class Interactable_Collect_Patch
    {
        public static void Postfix(CollectOnInteract __instance)
        {
            Main.LogWarning("Collecting interactable: " + __instance.transform.position);
        }
    }
    [HarmonyPatch(typeof(CollectOnTouch), nameof(CollectOnTouch.Collect))]
    class Touchable_Collect_Patch
    {
        public static void Postfix(CollectOnTouch __instance)
        {
            Main.LogWarning("Collecting touchable: " + __instance.transform.position);
        }
    }
    [HarmonyPatch(typeof(Holdable), nameof(Holdable.Interact))]
    class Holdable_Collect_Patch
    {
        public static void Postfix(Holdable __instance)
        {
            Main.LogWarning("Collecting holdable: " + __instance.transform.position);
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
            Main.Randomizer.Items.CollectLocation(locationId, true);

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
                string locationId = CalculateNewLocationId(context.originalSpeaker.name, args[0]);
                ItemLocation location = Main.Randomizer.Data.GetLocationFromId(locationId);
                if (location == null)
                    return;

                args = new string[] { locationId };
                Main.Randomizer.Items.CollectLocation(locationId, false);
            }
            else if (args.Length > 1 && args[0] == "FishingRod" && args[1] == "-1")
            {
                // If the fisher guy is trying to take away the fishing rod, dont let him
                Main.LogWarning("Preventing loss of fishing rod!");
                args = new string[] { "FishingRod", "0" };
            }
        }

        private static string CalculateNewLocationId(string locationId, string itemId)
        {
            switch (locationId)
            {
                case "CampRangerNPC": // Visitor's center shop salesman
                    {
                        int visitorFeathers = (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat("$FeathersSold");
                        if (itemId == "GoldenFeather")
                            return locationId + $"[{visitorFeathers - 1}]";
                        else if (itemId == "ParkHat")
                            return locationId + "[9]";
                        break;
                    }
                case "ToughBirdNPC (1)": // Tough bird salesman
                    {
                        int toughBirdFeathers = (int)Singleton<GlobalData>.instance.gameData.tags.GetFloat("$ToughBirdSales");
                        if (itemId == "GoldenFeather")
                            return locationId + $"[{toughBirdFeathers - 1}]";
                        else if (itemId == "Watch")
                            return locationId + "[9]";
                        break;
                    }
                case "VolleyballOpponent": // Beachstickball
                    {
                        if (itemId == "GoldenFeather")
                            return locationId + "[0]";
                        else if (itemId == "Coin")
                            return locationId + "[1]";
                        else if (itemId == "KidHat")
                            return locationId + "[2]";
                        break;
                    }
                case "FishBuyer": // Fisherman
                    {
                        if (itemId == "FishEncyclopedia")
                            return locationId + "[0]";
                        else if (itemId == "GoldenFishingRod")
                            return locationId + "[1]";
                        break;
                    }
                case "CamperNPC": // Camper bribe
                    {
                        bool gotBribe = Singleton<GlobalData>.instance.gameData.tags.GetBool("Opened_CamperNPC[0]");
                        return locationId + (gotBribe ? "[1]" : "[0]");
                    }
                case "Bunny_WalkingNPC (1)": // Racing bunny
                    {
                        if (itemId == "RunningShoes")
                            return locationId + "[0]";
                        break;
                    }
                case "RaceOpponent": // Parkour racer
                    {
                        string race = Singleton<GlobalData>.instance.gameData.tags.GetString("RaceId");
                        int raceLevel = race == "MountainTopRace" ? 2 : (race == "OldBuildingRace" ? 1 : 0);
                        if (itemId == "Medal")
                            return locationId + $"[{raceLevel}]";
                        else if (itemId == "WalkieTalkie")
                            return locationId + "[9]";
                        break;
                    }
                case "LittleKidNPCVariant (1)": // Shell kid
                    {
                        if (itemId == "ShellNecklace")
                            return locationId + "[0]";
                        else if (itemId == "Shell")
                            return locationId + "[1]";
                        break;
                    }
                case "DadDeer": // Boat rental guy
                    {
                        if (itemId == "BoatKey")
                            return locationId + "[0]";
                        else if (itemId == "BoatManual")
                            return locationId + "[1]";
                        break;

                    }
                default:
                    return locationId + "[0]";
            }

            return locationId;
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
                __instance.beforeName.GetComponent<Text>().text = "Got";
        }
    }

    /// <summary>
    /// Calling this method with the locationId of a valid location will instead return a new ApItem
    /// </summary>
    [HarmonyPatch(typeof(CollectableItem), nameof(CollectableItem.Load))]
    class CollectableItem_GetItem_Patch
    {
        public static bool Prefix(string name, ref CollectableItem __result)
        {
            // If this isnt a valid location, get the regular item at this name
            ItemLocation location = Main.Randomizer.Data.GetLocationFromId(name);
            if (location == null)
                return true;

            if (location.player_name == Main.Randomizer.Settings.SettingsForCurrentSave.player)
            {
                // The item belongs to this world
                __result = ItemCreator.CreateLocalItem(location.item_name);
            }
            else
            {
                // The item goes to another player's world
                __result = ItemCreator.CreateExternalItem(location.item_name, location.player_name);
            }

            return false;
        }
    }
}
