using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AShortHike.Randomizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public static ItemLoader ItemLoader { get; private set; }
        public static ItemChanger ItemChanger { get; private set; }
        public static DataStorage DataStorage { get; private set; }

        public static Transform TransformHolder => _instance.transform;

        private static Main _instance;

        private void Awake()
        {
            _instance ??= this;
            ItemLoader = new ItemLoader();
            ItemChanger = new ItemChanger();
            DataStorage = new DataStorage();

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
            DataStorage.LoadData();
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
                ItemLoader.LoadItems();
                ItemChanger.ChangeItems();
            }
        }

        public static void Log(object message) => _instance.Logger.LogMessage(message);

        public static void LogWarning(object message) => _instance.Logger.LogWarning(message);

        public static void LogError(object message) => _instance.Logger.LogError(message);
    }
}
