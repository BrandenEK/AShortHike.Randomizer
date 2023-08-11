using HarmonyLib;

namespace AShortHike.Randomizer.Settings
{
    [HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.StartNewGame))]
    class TitleScreen_NewGame_Patch
    {
        public static bool SkipSaveExists { get; private set; }

        public static bool Prefix()
        {
            if (Main.Randomizer.Connection.Connected)
            {
                SkipSaveExists = true;
                return true;
            }
            else
            {
                Main.Randomizer.Settings.ClearSettings();
                Main.Randomizer.Settings.OpenSettingsMenu();
                return false;
            }
        }

        public static void Postfix()
        {
            SkipSaveExists = false;
        }
    }

    [HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.ContinueGame))]
    class TitleScreen_ContinueGame_Patch
    {
        public static bool Prefix()
        {
            if (Main.Randomizer.Connection.Connected)
            {
                return true;
            }
            else
            {
                // Load saved connection info from config
                string server = "localhost";
                string player = "Player";
                string password = null;

                Main.Randomizer.Connection.Connect(server, player, password, true);
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(CrossPlatform), nameof(CrossPlatform.DoesSaveExist))]
    class TitleScreen_SaveBypass_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (TitleScreen_NewGame_Patch.SkipSaveExists)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(LevelController), nameof(LevelController.SaveAndQuit))]
    class LevelController_SaveQuit_Patch
    {
        public static void Prefix()
        {
            Main.Randomizer.Connection.Disconnect();
        }
    }
}
