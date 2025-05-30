using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PANEGamepad
{
    [BepInPlugin($"com.ashpynov.{PluginInfo.Name}", PluginInfo.Name, PluginInfo.Version)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        private static Harmony _hi;
        public static ManualLogSource Log;

        public void Awake()
        {
            _hi = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Log = new ManualLogSource(PluginInfo.Name);
            BepInEx.Logging.Logger.Sources.Add(Log);

            // Create a GameObject to host our Update()
            GameObject obj = new("InputTracker");
            obj.AddComponent<InputTracker>();
            UnityEngine.Object.DontDestroyOnLoad(obj); // Persist across scenes
        }
    }
}