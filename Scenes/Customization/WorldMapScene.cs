using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PANEGamepad.Scenes.Customization
{
    public static class WorldMapScene
    {
        public const SceneCode Code = SceneCode.WorldMap;
        public static bool Taste(IEnumerable<GameObject> _)
        {
            return GameObject.Find("Canvas/UI/Generic_WorldMap")?.activeSelf == true;
        }

        public static IEnumerable<GameObject> Filter(IEnumerable<GameObject> scene)
        {
            PropertyInfo stateProperty = AccessTools.Property(Types.WorldMapCity, "State");
            PropertyInfo canTradeProperty = AccessTools.Property(Types.WorldMapCityState, "CanTrade");

            return scene.Where(go => go.GetComponent(Types.WorldMapCity) is not Component city || (bool)canTradeProperty.GetValue(stateProperty.GetValue(city)));
        }
    }
}