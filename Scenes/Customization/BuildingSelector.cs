using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PANEGamepad.Extensions;
using UnityEngine;

namespace PANEGamepad.Scenes.Customization
{
    public static class BuildingSelector
    {
        private const string CategoryRoot = "Canvas/UI/Bars/BuildingBar/CategoryHolders";
        private const string HiddenCategory = "Canvas/UI/Bars/BuildingBar/ButtonHiddenCategory";

        private static string _currentCategoryName = "";
        private static readonly Dictionary<string, string> _currentCategoryBuildings = new();

        public static bool pickedMode = false;

        public static bool PickNextCategory() => PickBuilding(1, 0);
        public static bool PickPrevCategory() => PickBuilding(-1, 0);

        public static bool PickNextBuilding() => PickBuilding(0, 1);
        public static bool PickPrevBuilding() => PickBuilding(0, -1);
        public static bool PickCategoryCircular()
        {
            float dX = InputTracker.GamePad.GetValue(Gamepad.GamepadCode.RightStickX);
            float dY = InputTracker.GamePad.GetValue(Gamepad.GamepadCode.RightStickY);
            float length = Mathf.Sqrt((dX * dX) + (dY * dY));

            if (length < 0.7)
            {
                return true;
            }

            float num = 9;
            float step = 360f / num;
            float angle = ((Mathf.Atan2(dX, dY) * Mathf.Rad2Deg) + (step / 2)) % 360f;
            if (angle < 0)
            {
                angle += 360f;
            }

            // Determine sector (0-7)
            string category = ((int)(angle / step) % num) switch
            {
                0 => "Services",
                1 => "FoodAndFarming",
                2 => "StockAndDistribution",
                3 => "Production",
                4 => "Religion",
                5 => "Military",
                6 => "Monuments",
                7 => "Entertainment",
                8 => "Beautification",
                _ => ""
            };

            return PickCategory(category);
        }

        private static bool PickCategory(string categoryName)
        {
            if ((categoryName != _currentCategoryName || !pickedMode)
                && GetCategories().FirstOrDefault(go => go.name == categoryName) is GameObject category)
            {
                _currentCategoryName = category.name;
                pickedMode = true;
                PickBuilding(0, 0);
            }
            return true;
        }

        public static bool OnActivateBuilding(string name)
        {
            Plugin.Log.LogInfo($"Activate building {name}");
            foreach (GameObject category in GetCategories())
            {
                foreach (GameObject building in GetCategoryBuildings(category.name))
                {
                    if (building.name == name)
                    {
                        _currentCategoryName = category.name;
                        _currentCategoryBuildings[category.name] = building.name;
                        pickedMode = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool OnCancelBuilding()
        {
            Plugin.Log.LogInfo("Canceled building");
            pickedMode = false;
            return false;
        }

        public static string GetActiveCategoryName()
        {
            return pickedMode ? _currentCategoryName.Replace("And", " & ") : "";
        }
        private static List<GameObject> GetCategories()
        {
            return GameObject.Find(CategoryRoot) is GameObject gameObject
                ? gameObject.GetChildren().ToList()
                : new List<GameObject>();
        }

        private static List<GameObject> GetCategoryBuildings(string categoryName)
        {

            if (GetCategories().FirstOrDefault(go => go.name == categoryName) is GameObject category)
            {
                List<GameObject> buildings = category.GetAllChildren(Types.BuildingBarElement)
                   .Where(g => g.GetComponent(Types.BuildingBarCategory) is null && !SceneController.IsLocked(g))
                   .ToList();
                if (buildings.Count == 0 && GameObject.Find(HiddenCategory) is GameObject hidden)
                {
                    buildings = hidden.GetAllChildren(Types.BuildingBarElement)
                        .Where(g => g.GetComponent(Types.BuildingBarCategory) is null && !SceneController.IsLocked(g))
                        .ToList();
                }
                return buildings;
            }
            return new List<GameObject>();
        }
        private static bool PickBuilding(int changeCategory, int changeBuilding)
        {
            Plugin.Log.LogInfo($"current: {_currentCategoryName}: {GetActiveCategoryName()}");
            List<GameObject> categories = GetCategories();
            if (categories.Count() == 0)
            {
                return true;
            }

            int categoryIndex = categories.FindIndex(c => c.name == _currentCategoryName);

            if (categoryIndex == -1)
            {
                pickedMode = false;
                categoryIndex = 0;
            }

            if (pickedMode && changeCategory != 0)
            {
                categoryIndex = Mathf.Clamp(categoryIndex + changeCategory, 0, categories.Count - 1);
            }

            GameObject category = categories[categoryIndex];

            _currentCategoryBuildings.TryGetValue(category.name, out string currentBuildingName);
            currentBuildingName ??= "";

            List<GameObject> buildings = GetCategoryBuildings(category.name);

            if (buildings.Count() == 0)
            {
                return true;
            }
            int buildingIndex = buildings.FindIndex(c => c.name == currentBuildingName);
            if (buildingIndex == -1)
            {
                pickedMode = false;
                buildingIndex = 0;
            }
            if (pickedMode && changeBuilding != 0)
            {
                buildingIndex = Mathf.Clamp(buildingIndex + changeBuilding, 0, buildings.Count - 1);
            }

            GameObject building = buildings[buildingIndex];

            if (!pickedMode || category.name != _currentCategoryName || building.name != currentBuildingName
                || (changeCategory == 0 && changeBuilding == 0))
            {
                _currentCategoryName = category.name;
                _currentCategoryBuildings[category.name] = building.name;
                pickedMode = true;

                SceneController.PressButton(building);
                ShowCostTooltip();

                return true;
            }
            return true;
        }

        private static void ShowCostTooltip()
        {
            if (AccessTools.TypeByName("MapGameplay") is Type mapGameplayType
             && AccessTools.Property(mapGameplayType, "Instance") is PropertyInfo instanceProperty
             && instanceProperty.GetValue(null) is object mapGameplay
             && AccessTools.TypeByName("StaggeredCellCoord") is Type staggeredCellCoordType
             && AccessTools.Field(staggeredCellCoordType, "Invalid")?.GetValue(null) is object invalidCoord
            )
            {
                AccessTools.Field(mapGameplay.GetType(), "_lastMouseCoord")?.SetValue(mapGameplay, invalidCoord);
            }
        }
    }
}