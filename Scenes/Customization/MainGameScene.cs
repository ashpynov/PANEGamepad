using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PANEGamepad.Scenes.Customization
{
    public static class MainGameScene
    {
        public const SceneCode Code = SceneCode.MainGame;
        public static bool Taste(IEnumerable<GameObject> scene)
        {
            return SceneController.SceneMatch(scene, ["MenuButton", "PlayerButton", "Remove", "Pause"]);
        }


        private static string GetGameObjectPanel(GameObject component)
        {
            if (component == null)
            {
                return null;
            }
            string[] path = SceneController.GetPath(component.gameObject);
            return path.Length >= 4 && path[2] == "Bars" ? path[3] : null;
        }

        private static string GetDirectionPanel(Vector3 direction)
        {
            return direction == Vector3.left ? "LeftBar" :
                   direction == Vector3.right ? "BuildingBar" :
                   direction == Vector3.up ? "TopBar" :
                   direction == Vector3.down ? "MiniMap" :
                   null;
        }

        private static IEnumerable<GameObject> FilterByPanelName(IEnumerable<GameObject> scene, string panel)
        {
            return panel != null
            ? scene.Where(component => !(GetGameObjectPanel(component) is string componentPanel && componentPanel != panel))
            : scene;
        }

        public static IEnumerable<GameObject> Filter(IEnumerable<GameObject> scene, object filter, object filter2 = null)
        {
            if (filter is GameObject current && GetGameObjectPanel(current) is string panel)
            {
                return FilterByPanelName(scene, panel);
            }

            if (filter is Vector3 || filter2 is Vector3)
            {
                Vector3 direction = (Vector3)(filter2 ?? filter);
                return FilterByPanelName(scene, GetDirectionPanel(direction));
            }

            return scene;
        }
    }
}