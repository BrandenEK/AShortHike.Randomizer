using BepInEx;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AShortHike.Randomizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer { get; private set; }

        public static Transform TransformHolder { get; private set; }

        private static Main _instance;

        private void Awake()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadMissingAssemblies);
            if (_instance == null)
                _instance = this;
            TransformHolder = transform;
            Randomizer = new Randomizer();

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LogWarning("Scene loaded: " + scene.name);
            Randomizer.OnSceneLoaded(scene.name);
        }

        private void Update()
        {
            Randomizer.Update();
        }

        public static void Log(object message) => _instance.Logger.LogMessage(message);

        public static void LogWarning(object message) => _instance.Logger.LogWarning(message);

        public static void LogError(object message) => _instance.Logger.LogError(message);

        private Assembly LoadMissingAssemblies(object send, ResolveEventArgs args)
        {
            string assemblyPath = Path.GetFullPath($"Modding/data/{args.Name.Substring(0, args.Name.IndexOf(","))}.dll");
            LogWarning("Loading missing assembly from " + assemblyPath);
            return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        }
    }
}
