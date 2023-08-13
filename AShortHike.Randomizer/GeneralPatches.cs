using HarmonyLib;

namespace AShortHike.Randomizer
{
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
