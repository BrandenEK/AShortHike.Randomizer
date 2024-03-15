using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AShortHike.Randomizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer { get; private set; }

        public static Transform TransformHolder { get; private set; }
        private static ManualLogSource MessageLogger { get; set; }

        // New
        public static LocationHandler LocationHandler { get; private set; }

        private void Awake()
        {
            TransformHolder = transform;
            MessageLogger = Logger;

            // New
            LocationHandler = new();

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadMissingAssemblies);
            Randomizer = new Randomizer();
            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
        }

        public static void Log(object message) => MessageLogger.LogMessage(message);

        public static void LogWarning(object message) => MessageLogger.LogWarning(message);

        public static void LogError(object message) => MessageLogger.LogError(message);

        private Assembly LoadMissingAssemblies(object send, ResolveEventArgs args)
        {
            string assemblyPath = Path.GetFullPath($"Modding/data/{args.Name.Substring(0, args.Name.IndexOf(","))}.dll");
            LogWarning("Loading missing assembly from " + assemblyPath);
            return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }
    }
}
