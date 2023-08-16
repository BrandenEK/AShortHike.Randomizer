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
    /// Whenever a conversation is started, it stores the speaker name so that item preventions can tell what flags to change
    /// </summary>
    [HarmonyPatch(typeof(DialogueController), nameof(DialogueController.StartConversation))]
    class Dialog_Start_Patch
    {
        private static IConversation _lastConversation;

        public static void Postfix(IConversation __result)
        {
            _lastConversation = __result;
        }

        public static string CurrentConversation
        {
            get
            {
                if (_lastConversation == null || !_lastConversation.isAlive)
                    return string.Empty;

                return _lastConversation.originalSpeaker.name;
            }
        }
    }

    /// <summary>
    /// To prevent npcs not giving you the location check if you already have the associated item,
    /// hide what items you have based on if you have the location yet
    /// </summary>
    [HarmonyPatch(typeof(YarnFunctions), nameof(YarnFunctions.HasItem))]
    class Dialog_CheckItem_Patch
    {
        public static void Postfix(ref bool __result, IConversation context, Value itemName)
        {
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;
            string person = context.originalSpeaker.name;
            string item = itemName.AsString;
            Main.LogWarning($"{person} is checking for item: {item}");

            if (person == "CamperNPC" && item == "CampingPermit")
            {
                // If you already have the permit before first talking to the camper guy
                __result = __result && tags.GetBool("Opened_CamperNPC[0]");
            }
            else if (person == "Fox_WalkingNPC" && item == "Compass")
            {
                // If you already have the compass before talking to the compass guy
                __result = tags.GetBool("Opened_Fox_WalkingNPC[0]");
            }
            else if (person == "RaceOpponent" && item == "WalkieTalkie")
            {
                // If you already have the walkie talkie before the parkour races
                __result = tags.GetBool("Opened_RaceOpponent[9]");
            }
            else if (person == "Turtle_WalkingNPC" && item == "Headband")
            {
                // If you already have the headband before talking to the turtle
                __result = __result && tags.GetBool("Opened_Turtle_WalkingNPC[0]");
            }
            else if (person == "LittleKidNPCVariant (1)" && item == "ShellNecklace")
            {
                // If you dont have the shell necklace after getting the item from the shell kid
                __result = !tags.GetBool("Opened_AuntMayNPC[0]");
            }
        }
    }

    /// <summary>
    /// To prevent some locations being unobtainable because of things you've already done,
    /// only show them as true once the location has been checked
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.GetBool))]
    class Tags_CheckFlag_Patch
    {
        public static void Postfix(ref bool __result, string tag)
        {
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;
            string person = Dialog_Start_Patch.CurrentConversation;
            string flag = tag;
            Main.LogWarning($"{person} is checking for flag: {flag}");

            if (person == string.Empty && tag == "MissingPermit")
            {
                // If you have already returned the permit before catching the lake fish
                __result = tags.GetBool("Opened_CamperNPC[0]") && !tags.GetBool("Opened_Player[0]");
            }
            else if (person == "Turtle_WalkingNPC" && (tag == "$BunnyQuestDone" || tag == "$BunnyGiftHeadband" || tag == "BunnyGotHeadband"))
            {
                // If you already returned the headband to the rabbit before talking to the turtle
                __result = __result && tags.GetBool("Opened_Turtle_WalkingNPC[0]");
            }
            else if (person == "Turtle_WalkingNPC" && tag == "$RabbitQuest")
            {
                // If you already returned the headband to the rabbit before talking to the turtle
                __result = __result || tags.GetBool("Opened_Bunny_WalkingNPC (1)[0]");
            }
            else if (person == "StandingNPC (1)" && flag == "$RentedBoat")
            {
                // Allow the kid to go on the boat once you have the key
                __result = Singleton<GlobalData>.instance.gameData.GetCollected(CollectableItem.Load("BoatKey")) > 0;
            }
        }
    }

    /// <summary>
    /// Always return true so that the GetBool patch can always be applied
    /// </summary>
    [HarmonyPatch(typeof(Tags), nameof(Tags.HasBool))]
    class Tags_HasFlag_Patch
    {
        public static void Postfix(ref bool __result) => __result = true;
    }


    [HarmonyPatch(typeof(Tags), nameof(Tags.SetBool))]
    class Tags_SetFlag_Patch
    {
        public static bool Prefix(string tag, bool value)
        {
            string person = Dialog_Start_Patch.CurrentConversation;

            if (person == "Dog_WalkingNPC_BlueEyed" && tag == "TMap4")
            {
                // If the outlook dog is setting the map flag, prevent that
                return false;
            }

            Main.Log($"Setting flag: {tag} ({value})");
            return true;
        }
    }
}
