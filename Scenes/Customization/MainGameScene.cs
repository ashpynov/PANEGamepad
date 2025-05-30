using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PANEGamepad.Scenes.Customization
{
    public static class MainGameScene
    {
        public static bool Taste(IEnumerable<Component> scene)
        {
            return SceneUtility.SceneMatch(scene, ["MenuButton", "PlayerButton", "Remove", "Pause"]);
        }


        private static string GetComponentPanel(Component component)
        {
            if (component == null)
            {
                return null;
            }
            string[] path = SceneUtility.GetPath(component.gameObject);
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

        private static IEnumerable<Component> FilterByPanelName(IEnumerable<Component> scene, string panel)
        {
            return panel != null
            ? scene.Where(component => !(GetComponentPanel(component) is string componentPanel && componentPanel != panel))
            : scene;
        }

        public static IEnumerable<Component> Filter(IEnumerable<Component> scene, object filter, object filter2 = null)
        {
            if (filter is Component current && GetComponentPanel(current) is string panel)
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