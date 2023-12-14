using HarmonyLib;
using UnityEngine;

namespace AShortHike.Randomizer
{
    /// <summary>
    /// When starting the intro conversation, skip it if the setting is enabled
    /// </summary>
    [HarmonyPatch(typeof(DialogueController), nameof(DialogueController.StartConversation))]
    class DialogueController_SkipStart_Patch
    {
        public static bool Prefix(string startNode, ref IConversation __result)
        {
            if (startNode == "TitleScreenIntroStart" && Main.Randomizer.ClientSettings.skipCutscenes)
            {
                // If starting the intro cutscene, set conversation to null and skip start
                __result = null;
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(Cutscene), nameof(Cutscene.Start))]
    class Cutscene_Start_Patch
    {
        public static bool Prefix(Cutscene __instance)
        {
            // If starting the intro cutscene, give the phone and skip
            if (!Main.Randomizer.ClientSettings.skipCutscenes)
                return true;

            var data = Singleton<GlobalData>.instance.gameData;

            if (!data.tags.GetBool("OpeningCutscene"))
            {
                data.AddCollected(CollectableItem.Load("Cellphone"), 1, false);
                data.tags.SetBool("OpeningCutscene", true);
            }

            Object.Destroy(__instance.gameObject);
            return false;
        }
    }

    /// <summary>
    /// When in dialog where it makes you wait for a long time, dont wait
    /// </summary>
    [HarmonyPatch(typeof(YarnCommands), nameof(YarnCommands.Wait))]
    class Dialog_Wait_Patch
    {
        public static void Prefix(string[] args)
        {
            if (Main.Randomizer.ClientSettings.skipCutscenes && args.Length > 0 && float.Parse(args[0]) >= 5)
            {
                Main.Log("Shortening waiting cutscene");
                args[0] = "1";
            }
        }
    }
}
