using System.Collections.Generic;
using UnityEngine;


namespace PANEGamepad.Scenes.Customization
{
    public static class SingleConfirmScene
    {
        private static readonly string[] ObjectsPaths =
        [
            "CoreCanvas/LoadingScreen/New_Button_Valid",
            "Canvas/UI/BattleScreen/BattleResultScreen/Background/ControlsRow/LargeAcceptButton",
            // "Canvas/UI/BattleLaunchScreen/wrapper/New_Button_Valid",
            // "Canvas/UI/BattleScreen/New_Button_Valid"
        ];

        private static GameObject GetConfirmButton()
        {
            foreach (string path in ObjectsPaths)
            {
                GameObject control = GameObject.Find(path);
                if (control != null)
                {
                    return control;
                }
            }
            return null;
        }
        public static bool Taste(IEnumerable<GameObject> _)
        {
            return GetConfirmButton() is GameObject button

                && SceneController.IsSelectable(button);
        }

        public static bool PressConfirm()
        {
            GameObject button = GetConfirmButton();
            return (button != null) && SceneController.PressButton(button);
        }
    }
}