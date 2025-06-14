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

        private static readonly string[] CustomTypes = ["ButtonWithHover", "GameModeWidget", "MapEntry", "WorldMapCity"];

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
                    InputTracker.SetFocused(null);
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
                    InputTracker.SetFocused(null);
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
            _filtered ??= GetSceneCode() switch
            {
                MainGameScene.Code => MainGameScene.Filter(GetScene(), filter, filter2),
                DropdownScene.Code => DropdownScene.Filter(GetScene()),
                WorldMapScene.Code => WorldMapScene.Filter(GetScene()),
                _ => GetScene(),
            };
            return _filtered;
        }

        private SceneCode DetectScene(IEnumerable<GameObject> scene)
        {
            return
                WorldMapScene.Taste(scene) ? WorldMapScene.Code :
                DropdownScene.Taste(scene) ? DropdownScene.Code :
                OverseersScene.Taste(scene) ? OverseersScene.Code :
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
                if (component != null)
                {
                    return PressButton(component.gameObject);
                }
                else
                {
                    GameObject go = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(c => c.name == buttonName);
                    if (go != null)
                    {
                        PressButton(go);
                        return true;
                    }
                }
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
                    pointerEnter = enter,
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

            components.AddRange(GameObject.FindObjectsOfType<MonoBehaviour>()
                .Where(c => CustomTypes.Contains(c.GetType().Name))
                .Select(c => c.gameObject));

            return components;
        }

    }
}