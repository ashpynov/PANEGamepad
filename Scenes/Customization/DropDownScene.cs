using System.Collections.Generic;
using System.Linq;
using PANEGamepad.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace PANEGamepad.Scenes.Customization
{
    public static class DropdownScene
    {
        public const SceneCode Code = SceneCode.Dropdown;
        public static bool Taste(IEnumerable<GameObject> _)
        {
            return GameObject.Find("Canvas/Blocker") != null;
        }

        public static bool CloseDropdown()
        {
            if (GameObject.Find("Canvas/Blocker")?.GetComponent<Button>() is Button blocker)
            {
                if (InputTracker.GetHoveredGameObject() is GameObject go
                    && go.GetComponentInParent("TMP_Dropdown") is Component dd
                )
                {
                    InputTracker.SetMoused(dd.gameObject);
                }
                blocker.onClick.Invoke();
                return true;
            }
            return false;
        }

        public static bool SelectDropdown()
        {
            if (InputTracker.GetHoveredGameObject() is GameObject go && go.GetComponentInParent("TMP_Dropdown") is Component dd)
            {
                if (SceneController.PressButton(go))
                {
                    InputTracker.SetMoused(dd.gameObject);
                    return true;
                }
            }

            return false;
        }
        public static IEnumerable<GameObject> Filter(IEnumerable<GameObject> scene)
        {
            return scene.Where(go => go.GetComponent(Types.TMPro_DropdownItem) is not null);
        }
    }
}