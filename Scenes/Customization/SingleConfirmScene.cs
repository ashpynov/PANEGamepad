using System.Collections.Generic;
using UnityEngine;


namespace PANEGamepad.Scenes.Customization
{
    public static class SingleConfirmScene
    {
        private static readonly string[] ObjectsPaths =
        [
            "CoreCanvas/LoadingScreen/New_Button_Valid"
        ];

        private static GameObject GetConfirmButton()
        {
            foreach (string path in ObjectsPaths)
            {
                Plugin.Log.LogInfo($"Search at path {path}");
                GameObject control = GameObject.Find(path);
                if (control != null)
                {
                    Plugin.Log.LogInfo($"Found at path {path}");
                    return control;
                }
            }
            return null;
        }
        public static bool Taste(IEnumerable<Component> _)
        {
            Plugin.Log.LogInfo($"SingleConfirmScene");
            return GetConfirmButton() is GameObject button
            && button.GetComponent<Component>() is Component component
            && SceneUtility.IsSelectable(component);
        }

        public static bool PressConfirm()
        {
            GameObject button = GetConfirmButton();
            return (button != null) && SceneController.PressButton(button);
        }
    }
}