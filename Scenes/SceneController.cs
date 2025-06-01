using System.Collections.Generic;
using System.Linq;
using PANEGamepad.Scenes.Customization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PANEGamepad.Scenes
{
    public partial class SceneController
    {
        private IEnumerable<GameObject> _current = null;
        private IEnumerable<GameObject> _filtered = null;
        private Vector2 _filteredDirection = Vector2.zero;
        private SceneCode _sceneCode = SceneCode.None;

        private SceneCode _previousScene = SceneCode.None;
        private GameObject _focused = null;
        private int _lastSelectablesCount = 0;


        public void Invalidate(bool force = false)
        {
            int selectablesCount = Selectable.allSelectableCount;
            if (selectablesCount != _lastSelectablesCount || force)
            {
                _current = null;
                _lastSelectablesCount = selectablesCount;
            }
            _sceneCode = SceneCode.None;
            _filtered = null;
        }

        public SceneCode GetSceneCode()
        {
            if (_sceneCode == SceneCode.None)
            {
                _sceneCode = DetectScene(GetScene());
                if (_sceneCode != _previousScene)
                {
                    InputTracker.SetFocus(null);
                    _previousScene = _sceneCode;
                }
            }
            return _sceneCode;
        }

        public void SetSceneCode(SceneCode sceneCode)
        {
            _sceneCode = sceneCode;
        }

        public void CheckSceneChanged(bool force)
        {
            SceneCode[] CheckPrevScene = [SceneCode.Overseers];
            if (force || CheckPrevScene.Contains(_previousScene))
            {
                _sceneCode = DetectScene(GetScene());
                if (_sceneCode != _previousScene)
                {
                    InputTracker.SetFocus(null);
                    _previousScene = _sceneCode;
                }
            }
        }

        public IEnumerable<GameObject> GetScene()
        {
            _current ??= GetSceneGameObjects();
            return _current;
        }

        public IEnumerable<GameObject> GetScene(object filter, object filter2 = null)
        {
            if (_filtered == null)
            {
                if (GetSceneCode() == SceneCode.MainGame)
                {
                    _filtered = MainGameScene.Filter(GetScene(), filter, filter2);
                }
                else
                {
                    _filtered = GetScene();
                }
            }
            return _filtered;
        }

        private SceneCode DetectScene(IEnumerable<GameObject> scene)
        {
            return
                OverseersScene.Taste(scene) ? SceneCode.Overseers :
                SingleConfirmScene.Taste(scene) ? SceneCode.SingleConfirm :
                SceneController.SceneMatch(scene, ["Play", "Continue", "Exit"]) ? SceneCode.Title :
                MainGameScene.Taste(scene) ? SceneCode.MainGame :
                SceneCode.Undefined;
        }

        public bool PressSceneButton(SceneCode sceneCode, string buttonName)
        {
            if (GetSceneCode() == sceneCode)
            {
                GameObject component = GetScene().FirstOrDefault(c => c.name == buttonName);
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
            else if (go != null && go.GetComponent("CustomToggle") as MonoBehaviour is MonoBehaviour toggle)
            {
                toggle.Invoke("OnToggleClick", 0f);
                return true;
            }
            return false;
        }

        public void SetFocus(GameObject enter)
        {
            if (_focused != null)
            {
                if (!InputTracker.IsHovered(_focused))
                {
                    PointerEventData pointerData = new(EventSystem.current)
                    {
                        position = _focused.GetComponent<RectTransform>().position
                    };
                    ExecuteEvents.Execute(_focused, pointerData, ExecuteEvents.pointerExitHandler);
                }
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
        private IEnumerable<GameObject> GetSceneGameObjects(Vector2 direction = default)
        {


            List<GameObject> components = new();
            components.AddRange(Selectable.allSelectablesArray.Where(c => c.interactable).Select(c => c.gameObject));

            string[] CustomTypes = ["ButtonWithHover", "GameModeWidget", "MapEntry", "WorldMapCity"];
            components.AddRange(GameObject.FindObjectsOfType<MonoBehaviour>()
                .Where(c => CustomTypes.Contains(c.GetType().Name))
                .Select(c => c.gameObject));

            return components;
        }

    }
}