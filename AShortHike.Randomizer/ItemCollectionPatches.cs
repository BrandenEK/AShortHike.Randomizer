using HarmonyLib;

namespace AShortHike.Randomizer
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
        public static void Prefix(ref string[] args)
        {
            if (args.Length < 2 || int.TryParse(args[1], out int amount) && amount > 0)
            {
                Main.LogWarning("Giving item: " + args[0]);
                //args = new string[] { "RunningShoes", "1" };
            }
        }
    }

    [HarmonyPatch(typeof(Chest), nameof(Chest.SpawnRewards))]
    class Chest_SpawnTime_Patch
    {
        public static void Prefix(ref float autoCollectTime)
        {
            autoCollectTime = 0.3f;
        }
    }
}
