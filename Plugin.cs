﻿using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PANEGamepad.Configuration;
using PANEGamepad.Scenes.Customization;
using UnityEngine;
using UnityEngine.UI;

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


    [HarmonyPatch("MapGameplay, Assembly-CSharp", "BuildMode_Setup")]
    internal class MapGameplay_BuildMode_Setup_Patch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void Postfix(object type, int forcedSize = 1)
        {
            string building = type?.ToString() ?? "null";
            BuildingSelector.OnActivateBuilding(building);
        }
    }

    [HarmonyPatch("MapGameplay, Assembly-CSharp", "BuildMode_ClearAndExit")]
    internal class MapGameplay_BuildMode_ClearAndExit_Patch
    {
        private static void Postfix()
        {
            BuildingSelector.OnCancelBuilding();
        }
    }

    [HarmonyPatch("CostDisplay, Assembly-CSharp", "ShowCost")]
    internal class CostDisplay_ShowCost_Patch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void Postfix(
            object __instance,
            int costA,
            object costAType,
            int count,
            int costB,
            object costBType)
        {
            Traverse traverse = Traverse.Create(__instance);

            Traverse text = traverse
                .Field("_priceA")?
                .Property("text");

            Traverse counterText = traverse
                .Field("_count")?
                .Property("text");

            traverse
                .Field("_priceA")?
                .Property("enableWordWrapping")?.SetValue(false);


            GameObject _price = traverse
                .Field("_priceA")?
                .Property("gameObject")?
                .GetValue() as GameObject;


            object mapGameplay = Traverse.CreateWithType("MapGameplay")?
                .Property("Instance")
                .GetValue();

            string name = Traverse.Create(mapGameplay)?
                .Field("_buildContext")?
                .Field("PrefabBuilding")?
                .Property("TypeName")?
                .GetValue<string>() ?? "";

            string category = BuildingSelector.GetActiveCategoryName();

            if (name != "")
            {
                text.SetValue($"{name} - {text?.GetValue() ?? ""}");
                GameObject showCost = _price.transform.parent.parent.gameObject;
                showCost.GetComponent<LayoutElement>().preferredWidth = -1;
            }

            Traverse _priceA = traverse.Field("_priceA");
            _priceA.Property("margin")?.SetValue(new Vector4(5, 0, 0, 0));
            _priceA.Property("fontSize")?.SetValue(Settings.ToolTipFontSize);

            Traverse _priceB = traverse.Field("_priceB");
            _priceB.Property("margin")?.SetValue(new Vector4(5, 0, 0, 0));
            _priceB.Property("fontSize")?.SetValue(Settings.ToolTipFontSize);

            Traverse _count = traverse.Field("_count");
            _count?.Property("margin")?.SetValue(new Vector4(5, 0, 5, 0));
            _count?.Property("fontSize")?.SetValue(Settings.ToolTipFontSize);

            if (category != "")
            {
                counterText.SetValue(category + (count > 0 ? $"\n{counterText.GetValue() ?? ""}" : ""));
                (_count?.Property("gameObject").GetValue() as GameObject)?.SetActive(true);
                (traverse.Field("_separator")?.Property("gameObject").GetValue() as GameObject)?.SetActive(true);
            }
            else if (costB == 0 && count == 0)
            {
                (_count?.Property("gameObject").GetValue() as GameObject)?.SetActive(false);
                (traverse.Field("_separator")?.Property("gameObject").GetValue() as GameObject)?.SetActive(false);
            }
        }
    }

    public static class Types
    {
        public static readonly Type TMPro_DropdownItem = AccessTools.TypeByName("TMPro.TMP_Dropdown+DropdownItem");
        public static readonly Type WorldMapCity = AccessTools.TypeByName("WorldMapCity");
        public static readonly Type WorldMapCityState = AccessTools.TypeByName("WorldMapCityState");
        public static readonly Type BuildingBarElement = AccessTools.TypeByName("BuildingBarElement");
        public static readonly Type BuildingBarCategory = AccessTools.TypeByName("BuildingBarCategory");
    }
}