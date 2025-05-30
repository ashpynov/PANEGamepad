using System.Collections.Generic;
using System.Linq;
using PANEGamepad.Scenes.Customization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PANEGamepad.Scenes
{
    public class SceneController
    {
        private IEnumerable<Component> _current = null;
        private IEnumerable<Component> _filtered = null;
        private Vector2 _filteredDirection = Vector2.zero;
        private SceneCode _sceneCode = SceneCode.None;

        private GameObject _focused = null;
        private int _lastSelectablesCount = 0;


        public void Invalidate(bool force = false)
        {
            int selectablesCount = Selectable.allSelectableCount;
            if (selectablesCount != _lastSelectablesCount || force)
            {
                _current = null;
                _sceneCode = SceneCode.None;
                _lastSelectablesCount = selectablesCount;
            }
            _filtered = null;
        }

        public SceneCode GetSceneCode()
        {
            if (_sceneCode == SceneCode.None)
            {
                _sceneCode = DetectScene(GetScene());
            }
            return _sceneCode;
        }
        public IEnumerable<Component> GetScene()
        {
            _current ??= GetSceneComponents();
            return _current;
        }

        public IEnumerable<Component> GetScene(object filter, object filter2 = null)
        {
            if (_filtered == null)
            {
                if (GetSceneCode() == SceneCode.MainGame)
                {
                    Debug.LogWarning("MainGame scene");
                    _filtered = MainGameScene.Filter(GetScene(), filter, filter2);
                }
                else
                {
                    Debug.LogWarning("Undefined scene");
                    _filtered = GetScene();
                }

            }
            return _filtered;
        }

        private SceneCode DetectScene(IEnumerable<Component> scene)
        {
            return
                SceneUtility.SceneMatch(scene, ["Play", "Continue", "Exit"]) ? SceneCode.Title :
                MainGameScene.Taste(scene) ? SceneCode.MainGame :
                SceneCode.Undefined;
        }

        public bool PressSceneButton(SceneCode sceneCode, string buttonName)
        {
            if (GetSceneCode() == sceneCode)
            {
                Component component = GetScene().FirstOrDefault(c => c.name == buttonName);
                return PressButton(component.gameObject);

            }
            return false;
        }

        public static bool PressButton(GameObject go)
        {
            if (go != null && go.GetComponent<Button>() is Button button)
            {
                button.onClick.Invoke();
                return true;
            }
            return false;
        }

        public void SetFocus(GameObject enter)
        {
            if (_focused != null)
            {
                PointerEventData pointerData = new(EventSystem.current)
                {
                    position = _focused.GetComponent<RectTransform>().position
                };
                ExecuteEvents.Execute(_focused, pointerData, ExecuteEvents.pointerExitHandler);

                _focused = null;
            }

            if (enter != null)
            {
                PointerEventData pointerData = new(EventSystem.current)
                {
                    position = enter.GetComponent<RectTransform>().position
                };
                ExecuteEvents.Execute(enter, pointerData, ExecuteEvents.pointerEnterHandler);

                _focused = enter;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private IEnumerable<Component> GetSceneComponents(Vector2 direction = default)
        {


            List<Component> components = new();
            components.AddRange(Selectable.allSelectablesArray.Where(c => c.interactable));

            string[] CustomTypes = ["ButtonWithHover", "GameModeWidget", "MapEntry", "WorldMapCity"];
            components.AddRange(GameObject.FindObjectsOfType<MonoBehaviour>()
                .Where(c => CustomTypes.Contains(c.GetType().Name)));

            return components;
        }

    }
}