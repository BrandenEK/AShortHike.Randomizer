﻿using HarmonyLib;

namespace AShortHike.Randomizer.Patches
{
    /// <summary>
    /// When starting a new game, either show the connection menu, or return normally if connected
    /// </summary>
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
                var settings = new ClientSettings();

                Main.Randomizer.Settings.RestoreMenuSettings(settings, false);
                Main.Randomizer.Settings.OpenSettingsMenu(0);
                return false;
            }
        }

        public static void Postfix()
        {
            SkipSaveExists = false;
        }
    }

    /// <summary>
    /// When loading a game, either show the connection menu, or return normally if connected
    /// </summary>
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
                ClientSettings settings = Main.Randomizer.Settings.SaveFileSettings;

                Main.Randomizer.Settings.RestoreMenuSettings(settings, true);
                Main.Randomizer.Settings.OpenSettingsMenu(4);
                return false;
            }
        }
    }

    /// <summary>
    /// When starting a new game, skip the "This will overwrite save" message
    /// </summary>
    [HarmonyPatch(typeof(GlobalData), nameof(GlobalData.DoesSaveExist))]
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

    /// <summary>
    /// When exiting the game, also disconnect from the server
    /// </summary>
    [HarmonyPatch(typeof(LevelController), nameof(LevelController.SaveAndQuit))]
    class LevelController_SaveQuit_Patch
    {
        public static void Prefix()
        {
            Main.Randomizer.Connection.Disconnect();
        }
    }
}
