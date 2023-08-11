using HarmonyLib;

namespace AShortHike.Randomizer.Items
{
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

    [HarmonyPatch(typeof(Chest), nameof(Chest.Interact))]
    class Chest_Collect_Patch
    {
        public static void Postfix(Chest __instance)
        {
            Main.LogWarning("Collecting chest: " + __instance.transform.position);

            string locationId = __instance.transform.position.ToString();
            Singleton<GlobalData>.instance.gameData.tags.SetBool("Opened_" + locationId, true);
            Main.Randomizer.Connection.SendLocation(locationId);
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

    [HarmonyPatch(typeof(YarnCommands), nameof(YarnCommands.GiveItem))]
    class Dialog_GiveItem_Patch
    {
        public static void Prefix(IConversation context, ref string[] args)
        {
            // Only give random item if the amount is positive
            if (args.Length < 2 || int.TryParse(args[1], out int amount) && amount > 0)
            {
                Main.LogWarning("Receiving item from conversation: " + context.originalSpeaker.name + ", " + context.originalSpeaker.position.ToString() + ", " + args[0]);
                string locationId = CalculateNewLocationId(context.originalSpeaker.name, args[0]);
                if (!Main.Randomizer.Items.IsLocationRandomized(locationId))
                    return;

                args = new string[] { "Stick", "0", "false" };
                Main.Randomizer.Connection.SendLocation(locationId);
            }

            string CalculateNewLocationId(string locationId, string itemId)
            {
                float currentLevel;
                switch (locationId)
                {
                    case "CampRangerNPC": // Visitor's center shop salesman
                        {
                            currentLevel = Singleton<GlobalData>.instance.gameData.tags.GetFloat("$FeathersSold");
                            if (itemId == "GoldenFeather")
                                return locationId + $"[{currentLevel}]";
                            else if (itemId == "ParkHat")
                                return locationId + "[9]";
                            break;
                        }
                    case "ToughBirdNPC (1)": // Tough bird salesman
                        {
                            currentLevel = Singleton<GlobalData>.instance.gameData.tags.GetFloat("$ToughBirdSales");
                            if (itemId == "GoldenFeather")
                                return locationId + $"[{currentLevel}]";
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
                            //int current = 0;
                            //if (Singleton<GlobalData>.instance.gameData.tags.GetBool("$GotVolleyPrize2"))
                            //    current = 2;
                            //else if (Singleton<GlobalData>.instance.gameData.tags.GetBool("$GotVolleyPrize"))
                            //    current = 1;
                            //return locationId + $"[{current}]";
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
                            // Need to do something with (missingpermit)
                            return locationId + "[5]";
                        }
                    case "Bunny_WalkingNPC (1)": // Racing bunny
                        {
                            if (itemId == "RunningShoes")
                                return locationId + "[0]";
                            break;
                        }
                    case "RaceOpponent": // Parkour racer
                        {
                            string race = "???"; // Get current race
                            currentLevel = race == "peak" ? 2 : (race == "old build" ? 1 : 0);
                            if (itemId == "Medal")
                                return locationId + $"[{currentLevel}]";
                            else if (itemId == "WalkieTalkie")
                                return locationId + "[9]";
                            break;
                        }
                    default:
                        return locationId + "[0]";
                }

                return locationId;
            }
        }
    }

    [HarmonyPatch(typeof(GlobalData.GameData), nameof(GlobalData.GameData.AddCollected))]
    class GameData_AddItem_Patch
    {
        public static bool Prefix(CollectableItem item)
        {
            return item.name != "AP";
        }
    }

    [HarmonyPatch(typeof(Tags), nameof(Tags.SetBool))]
    class Tags_SaveBool_Patch
    {
        public static void Postfix(string tag, bool value)
        {
            Main.Log($"Saving bool: {tag} ({value})");
        }
    }

    [HarmonyPatch(typeof(Tags), nameof(Tags.SetInt))]
    class Tags_SaveInt_Patch
    {
        public static void Postfix(string tag, int num)
        {
            Main.Log($"Saving int: {tag} ({num})");
        }
    }

    [HarmonyPatch(typeof(Tags), nameof(Tags.SetString))]
    class Tags_SaveString_Patch
    {
        public static void Postfix(string tag, string value)
        {
            Main.Log($"Saving string: {tag} ({value})");
        }
    }

    [HarmonyPatch(typeof(Tags), nameof(Tags.SetFloat))]
    class Tags_SaveFloat_Patch
    {
        public static void Postfix(string tag, float number)
        {
            Main.Log($"Saving float: {tag} ({number})");
        }
    }
}
