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


    [HarmonyPatch(typeof(DialogueController), nameof(DialogueController.StartConversation))]
    class Dialog_Start_Patch
    {
        public static IConversation Conversation { get; private set; }

        public static void Postfix(IConversation __result)
        {
            Conversation = __result;
        }
    }

    /// <summary>
    /// To prevent npcs not giving you the location check if you already have the associated item,
    /// hide what items you have based on if you have the location yet
    /// </summary>
    [HarmonyPatch(typeof(YarnFunctions), nameof(YarnFunctions.HasItem))]
    class Dialog_CheckItem_Patch
    {
        public static void Postfix(ref bool __result, IConversation context, Value itemName, Value amount)
        {
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;
            string person = Dialog_Start_Patch.Conversation.originalSpeaker.name;
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
        }
    }

    [HarmonyPatch(typeof(YarnToGlobalDataAdapter), nameof(YarnToGlobalDataAdapter.GetValue))]
    class Dialog_GetValue_Patch
    {
        public static void Postfix(ref Value __result, string variableName)
        {
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;
            string person = Dialog_Start_Patch.Conversation.originalSpeaker.name;
            string flag = variableName;
            Main.LogWarning($"{person} is checking for flag: {flag}");

            if (flag == "MissingPermit")
            {
                // If you have already returned the permit before catching the lake fish
                __result = new Value(tags.GetBool("Opened_CamperNPC[0]") && !tags.GetBool("Opened_Player[0]"));
            }
            else if (person == "Turtle_WalkingNPC" && flag == "$BunnyQuestDone")
            {

            }
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
            Tags tags = Singleton<GlobalData>.instance.gameData.tags;
            Main.LogWarning("Checking for flag: " + tag);

            if (tag == "MissingPermit")
            {
                // If you have already returned the permit before catching the lake fish
                __result = tags.GetBool("Opened_CamperNPC[0]") && !tags.GetBool("Opened_Player[0]");
            }
            else if (tag == "$BunnyQuestDone")
            {

            }

            //$BunnyGiftHeadband

            //
            //$bunnygiftheadband
            //bunnygotheadband
        }
    }
}
