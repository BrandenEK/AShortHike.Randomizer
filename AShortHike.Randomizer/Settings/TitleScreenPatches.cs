using HarmonyLib;

namespace AShortHike.Randomizer.Settings
{
    [HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.StartNewGame))]
    class TitleScreen_NewGame_Patch
    {
        public static bool Prefix()
        {
            if (Main.Randomizer.Connection.Connected)
            {
                return true;
            }
            else
            {
                Main.Randomizer.Settings.ClearSettings();
                Main.Randomizer.Settings.OpenSettingsMenu();
                return false;
            }
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

                Main.Randomizer.Connection.Connect(server, player, password);
                return false;
            }
        }
    }
}
