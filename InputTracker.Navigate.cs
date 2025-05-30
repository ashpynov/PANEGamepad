using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using PANEGamepad.Scenes;

namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
        private Component _current;

        public class Proximity(Component component, float proximityFactor)
        {
            public Component component = component;
            public float proximityFactor = proximityFactor;
        }

        private static float GetProximity(Component to, Vector3 fromScreenPos, Vector3 direction, float maxSqrMagnitude)
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

        public static bool IsPositionInComponent(Vector3 position, Component component)
        {
            if (component == null)
            {
                return false;
            }

            RectTransform rectTransform = component.GetComponent<RectTransform>();
            Canvas canvas = component.GetComponentInParent<Canvas>();
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

        public static Component GetHoveredComponent(IEnumerable<Component> Components)
        {
            Vector3 position = Input.mousePosition;
            return Components.FirstOrDefault(c => IsPositionInComponent(position, c));
        }

        public static Component FindComponent(IEnumerable<Component> Components, Component from, Vector3 dir, float maxDistance = float.PositiveInfinity)
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

            foreach (Component component in Components.Where(c => c != null))
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
            return candidates.FirstOrDefault(c => SceneUtility.IsSelectable(c.component, from))?.component;
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
        private void MoveCursorToElement(Component button)
        {
            SceneUtility.EnsureVisible(button.gameObject);
            lastCursorPos = SceneUtility.VisiblePoint(button);
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
            IEnumerable<Component> Components = Scene.GetScene();

            if (Components == null || Components.Count() == 0)
            {
                _current = null;
                return false;
            }

            _current ??= GetHoveredComponent(Components);

            Component _previuos = _current;
            bool found = false;

            if (_current == null)
            {
                _current = FindComponent(Components, _current, direction, 0.5f);
                found = _current != null;
            }

            if (!found)
            {
                Components = Scene.GetScene(_current, direction);

                if (Components.Count() != 0)
                {
                    _current = FindComponent(Components, _current, direction);
                }

                if (_current == null && _previuos == null)
                {
                    _current = Components.FirstOrDefault(c => SceneUtility.IsSelectable(c));
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
            IEnumerable<Component> Components = Scene.GetScene();
            if (Components.Count() == 1 && key == KeyCode.Mouse0)
            {
                startCursorPos = Input.mousePosition;
                MoveCursorToElement(Components.First());
                SimulateKey(KeyCode.Mouse0, true);
                SimulateKey(KeyCode.Mouse0, false);
                needResetPos = true;
            }
            return false;
        }
    }
}