using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AShortHike.Randomizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer { get; private set; }

        public static Transform TransformHolder => _instance.transform;

        private static Main _instance;

        private void Awake()
        {
            _instance ??= this;
            Randomizer = new Randomizer();

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LogWarning("Scene loaded: " + scene.name);
            if (scene.name == "GameScene")
            {
                Randomizer.OnSceneLoaded();
            }
        }

        private void Update()
        {
            Randomizer.Update();
        }

        public static void Log(object message) => _instance.Logger.LogMessage(message);

        public static void LogWarning(object message) => _instance.Logger.LogWarning(message);

        public static void LogError(object message) => _instance.Logger.LogError(message);
    }
}
