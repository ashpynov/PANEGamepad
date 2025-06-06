using System;
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
            Harmony.CreateAndPatchAll(typeof(TooltipUser_HideTooltip_Patch));
            Log = new ManualLogSource(PluginInfo.Name);
            BepInEx.Logging.Logger.Sources.Add(Log);

            // Create a GameObject to host our Update()
            GameObject obj = new("InputTracker");
            obj.AddComponent<InputTracker>();
            UnityEngine.Object.DontDestroyOnLoad(obj); // Persist across scenes
        }
    }

    [HarmonyPatch]
    public class TooltipUser_HideTooltip_Patch
    {
        private static FieldInfo _tooltipField;
        private static MethodBase TargetMethod()
        {
            Type tooltipUserType = AccessTools.TypeByName("TooltipUser");
            if (tooltipUserType == null)
            {
                return null;
            }
            _tooltipField = AccessTools.Field(tooltipUserType, "_tooltip");
            return AccessTools.Method(tooltipUserType, "HideTooltip");
        }

        private static bool Prefix(object __instance)
        {
            return _tooltipField is null || _tooltipField.GetValue(__instance) != null;
        }
    }
}