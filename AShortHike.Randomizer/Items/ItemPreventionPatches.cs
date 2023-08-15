using HarmonyLib;
using Yarn;

namespace AShortHike.Randomizer.Items
{
    /// <summary>
    /// After the fishing tutorial is finished, stash the fishing rod
    /// </summary>
    [HarmonyPatch(typeof(FishingTutorial), "CleanUpTutorial")]
    class FishingTutorial_StashRod_Patch
    {
        public static void Postfix()
        {
            Main.Log("Stashing fishing rod after tutorial");
            Singleton<GameServiceLocator>.instance.levelController.player.StashHeldItem();
        }
    }

    /// <summary>
    /// To prevent npcs not giving you the location check if you already have the associated item,
    /// hide what items you have based on if you have the location yet
    /// </summary>
    [HarmonyPatch(typeof(YarnFunctions), nameof(YarnFunctions.HasItem))]
    class Dialog_CheckItem_Patch
    {
        public static void Postfix(ref bool __result, Value itemName, Value amount)
        {
            Main.LogWarning($"Checking if {itemName.AsString} is greater than {amount.AsNumber}");
            string requiredLocation;

            if (itemName.AsString == "CampingPermit")
            {
                // If you already have the permit before first talking to the camper guy
                requiredLocation = "CamperNPC[0]";
            }
            else if (itemName.AsString == "Compass")
            {
                // If you already have the compass before talking to the compass guy
                requiredLocation = "Fox_WalkingNPC[0]";
            }
            else if (itemName.AsString == "WalkieTalkie")
            {
                // If you already have the walkie talkie before the parkour races
                requiredLocation = "RaceOpponent[9]";
            }
            else
            {
                return;
            }

            bool hasCheckedLocation = Singleton<GlobalData>.instance.gameData.tags.GetBool("Opened_" + requiredLocation);
            __result = __result && hasCheckedLocation;
        }
    }

    /// <summary>
    /// To prevent some locations being unobtainable because of things you've already done,
    /// only show them as true once the location has been checked
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.GetBool))]
    class Tags_GetFlag_Patch
    {
        public static void Postfix(ref bool __result, string tag)
        {
            Main.LogWarning("Checking for the flag: " + tag);
            //string requiredLocation;

            //// If you have already returned the permit before catching the lake fish
            //if (tag == "$ReturnedPermit")
            //{
            //    requiredLocation = "Player[0]";
            //MissingPermit
            //}
            //else
            //{
            //    return;
            //}

            //$BunnyQuestDone
            //$BunnyGiftHeadband
            //BunnyGotHeadband


            //bool hasCheckedLocation = Singleton<GlobalData>.instance.gameData.tags.GetBool("Opened_" + requiredLocation);
            //__result = __result && hasCheckedLocation;
        }
    }
}
