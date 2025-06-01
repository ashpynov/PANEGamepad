using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using PANEGamepad.Scenes;

namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
        private GameObject _current;

        public class Proximity(GameObject component, float proximityFactor)
        {
            public GameObject component = component;
            public float proximityFactor = proximityFactor;
        }

        private static float GetProximity(GameObject to, Vector3 fromScreenPos, Vector3 direction, float maxSqrMagnitude)
        {
            RectTransform rectTransform = to.transform as RectTransform;
            Canvas canvas = to.GetComponentInParent<Canvas>();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, fromScreenPos, canvas.worldCamera, out Vector3 fromWorldPos);

            Vector3 centerPos = rectTransform.TransformPoint((rectTransform != null) ? ((Vector3)rectTransform.rect.center) : Vector3.zero);
            Vector3 rhs = centerPos - fromWorldPos;
            rhs.z = 0;

            float projectionNorm = Vector3.Dot(direction, rhs.normalized);
            float angleFactor = 10f;
            if (!(projectionNorm <= 0.001f) && rhs.sqrMagnitude < maxSqrMagnitude)
            {
                float angleScale = 1 / (1 + angleFactor - (angleFactor * projectionNorm * projectionNorm));
                float prox = angleScale / (rhs.sqrMagnitude + 0.00001f);
                return prox;
            }
            return float.NegativeInfinity;
        }

        public static bool IsPositionInGameObject(Vector3 position, GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            if (rectTransform == null || canvas == null)
            {
                return false;
            }

            // Check if mouse is inside the button's RectTransform
            return RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                position,
                canvas.worldCamera
            );
        }

        public static GameObject GetHoveredGameObject(IEnumerable<GameObject> GameObjects)
        {
            Vector3 position = Input.mousePosition;
            return GameObjects.FirstOrDefault(c => IsPositionInGameObject(position, c.gameObject))?.gameObject;
        }

        public static bool IsHovered(GameObject go)
        {
            Vector3 position = Input.mousePosition;
            return IsPositionInGameObject(position, go);
        }

        public static GameObject FindGameObject(IEnumerable<GameObject> GameObjects, GameObject from, Vector3 dir, float maxDistance = float.PositiveInfinity)
        {
            Vector3 dirNorm = dir.normalized;

            Vector3 fromPos = Input.mousePosition;

            if (from != null)
            {
                Canvas canvas = from.GetComponentInParent<Canvas>();
                RectTransform rect = from.GetComponent<RectTransform>();
                Vector3 localPos = GetPointOnRectEdge(rect, dirNorm);
                Vector3 worldPos = rect.TransformPoint(localPos);
                fromPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPos);
            }

            List<Proximity> candidates = new();


            float maxSqrMagnitude = float.PositiveInfinity;

            if (!float.IsPositiveInfinity(maxDistance))
            {
                float distance = maxDistance * Screen.width / 1920;
                maxSqrMagnitude = distance * distance;
            }

            foreach (GameObject component in GameObjects.Where(c => c != null))
            {
                if (from != null && component.name == from.name && component == from)
                {
                    continue;
                }

                float prox = GetProximity(component, fromPos, dirNorm, maxSqrMagnitude);
                if (prox != float.NegativeInfinity)
                {
                    candidates.Add(new Proximity(component, prox));
                }
            }
            candidates.Sort((a, b) => b.proximityFactor.CompareTo(a.proximityFactor));
            return candidates.FirstOrDefault(c => SceneController.IsSelectable(c.component, from))?.component;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
            {
                return Vector3.zero;
            }

            if (dir != Vector2.zero)
            {
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            }

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }

        private Vector2 lastCursorPos;
        private Vector2 startCursorPos;
        private void MoveCursorToElement(GameObject button)
        {
            SceneController.EnsureVisible(button.gameObject);
            lastCursorPos = SceneController.VisiblePoint(button);
            SetCursorPos(lastCursorPos);
        }

        private bool MouseWasMoved()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 rhs = lastCursorPos - mousePos;
            return rhs.sqrMagnitude > 100;
        }
        private bool Navigate(Vector3 direction)
        {
            if (direction == default)
            {
                return false;
            }

            if (MouseWasMoved())
            {
                _current = null;
            }

            if (_current == null)
            {
                startCursorPos = Input.mousePosition;
            }
            Scene.Invalidate(force: true);
            IEnumerable<GameObject> GameObjects = Scene.GetScene();

            if (GameObjects == null || GameObjects.Count() == 0)
            {
                _current = null;
                return false;
            }

            _current ??= GetHoveredGameObject(GameObjects);

            GameObject _previuos = _current;
            bool found = false;

            if (_current == null)
            {
                _current = FindGameObject(GameObjects, _current, direction, 0.5f);
                found = _current != null;
            }

            if (!found)
            {
                GameObjects = Scene.GetScene(_current, direction);

                if (GameObjects.Count() != 0)
                {
                    _current = FindGameObject(GameObjects, _current, direction);
                }

                if (_current == null && _previuos == null)
                {
                    _current = GameObjects.FirstOrDefault(c => SceneController.IsSelectable(c));
                }
            }
            else if (_current == null)
            {
                _current = _previuos;
            }
            if (_current != null)
            {
                MoveCursorToElement(_current);
            }
            else if (_previuos != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            return _current != _previuos;

        }

        public bool CustomSceneProcess(KeyCode key)
        {
            IEnumerable<GameObject> GameObjects = Scene.GetScene();
            if (GameObjects.Count() == 1 && key == KeyCode.Mouse0)
            {
                startCursorPos = Input.mousePosition;
                MoveCursorToElement(GameObjects.First());
                SimulateKey(KeyCode.Mouse0, true);
                SimulateKey(KeyCode.Mouse0, false);
                needResetPos = true;
            }
            return false;
        }
    }
}