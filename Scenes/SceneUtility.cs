using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PANEGamepad.Scenes
{
    public static class SceneUtility
    {
        public static bool SceneMatch(IEnumerable<Component> scene, IEnumerable<string> hasNames = default, IEnumerable<string> hasNoNames = default)
        {
            IEnumerable<string> names = scene.Select(s => s.name);
            return (hasNames == default || names.Intersect(hasNames).Count() == hasNames.Count())
                && (hasNoNames == default || names.Intersect(hasNoNames).Count() == 0);
        }
        public static Vector2 VisiblePoint(Component button)
        {
            // Step 1: Get the button's center in screen space
            Canvas canvas = button.GetComponentInParent<Canvas>();
            RectTransform rect = button.GetComponent<RectTransform>();

            // Get the button's center in screen space
            Vector2 buttonCenter = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera,
                rect.position
            );

            Rect buttonRect = RectTransformUtility.PixelAdjustRect(rect, canvas);
            buttonCenter += buttonRect.center;

            List<Vector2> points = new() { buttonCenter };

            Vector2 offset1 = new(buttonRect.width / 6, buttonRect.height / 6);
            Vector2 offset2 = new(-buttonRect.width / 6, buttonRect.height / 6);

            points.Add(buttonCenter + offset1);
            points.Add(buttonCenter - offset1);
            points.Add(buttonCenter + offset2);
            points.Add(buttonCenter - offset2);

            foreach (Vector2 point in points)
            {
                // Check if the point is covered by another UI element
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = point
                };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (
                    results.Count > 0 &&
                    (
                        results[0].gameObject == button.gameObject ||
                        results[0].gameObject.transform.IsChildOf(button.transform)
                    )
                )
                {
                    return point;
                }
            }
            return default;
        }

        public static bool IsSelectable(Component obj, Component current = null)
        {
            return ((obj is Behaviour beh && beh.isActiveAndEnabled) || true)
                    && IsEnabled(obj)
                    && ((IsOnScroll(obj) && IsOnScroll(current)) || VisiblePoint(obj) != default);
        }
        public static bool IsOnScroll(Component obj)
        {
            return obj != null && obj.GetComponentInParent<ScrollRect>() != null;
        }
        public static bool IsEnabled(Component obj)
        {
            if (!obj.GetComponent<Behaviour>().isActiveAndEnabled)
            {
                return false;
            }
            // Locked property
            // Check if the component has a 'Locked' field/property
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                System.Type type = component.GetType();

                FieldInfo lockedField = type.GetField("Locked", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo lockedProperty = type.GetProperty("Locked", BindingFlags.Public | BindingFlags.Instance);

                if (lockedField != null && lockedField.FieldType == typeof(bool))
                {
                    return !(bool)lockedField.GetValue(component);
                }
                else if (lockedProperty != null && lockedProperty.PropertyType == typeof(bool))
                {
                    return !(bool)lockedProperty.GetValue(component);
                }
            }
            return true;
        }
        public static string[] GetPath(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return [];
            }

            Stack<string> pathStack = new();
            Transform current = gameObject.transform;

            while (current != null)
            {
                pathStack.Push(current.name);
                current = current.parent;
            }
            return pathStack.ToArray();
        }

        public static void EnsureVisible(GameObject control)
        {
            Vector2 margin = new Vector2(30, 30);


            ScrollRect scrollRect = control.GetComponentInParent<ScrollRect>();
            if (scrollRect == null || control.GetComponentInParent<Scrollbar>() != null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            RectTransform child = control.GetComponent<RectTransform>();

            // Get min and max of the viewport and child in local space to the viewport so we can compare them.
            // NOTE: use viewport instead of the scrollRect as viewport doesn't include the scrollbars in it.
            Vector2 viewPosMin = scrollRect.viewport.rect.min;
            Vector2 viewPosMax = scrollRect.viewport.rect.max;

            Vector2 childPosMin = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.min));
            Vector2 childPosMax = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.max));

            childPosMin -= margin;
            childPosMax += margin;

            Vector2 move = Vector2.zero;

            // Check if one (or more) of the child bounding edges goes outside the viewport and
            // calculate move vector for the content rect so it can keep it visible.
            if (childPosMax.y > viewPosMax.y)
            {
                move.y = childPosMax.y - viewPosMax.y;
            }
            if (childPosMin.x < viewPosMin.x)
            {
                move.x = childPosMin.x - viewPosMin.x;
            }
            if (childPosMax.x > viewPosMax.x)
            {
                move.x = childPosMax.x - viewPosMax.x;
            }
            if (childPosMin.y < viewPosMin.y)
            {
                move.y = childPosMin.y - viewPosMin.y;
            }

            // Transform the move vector to world space, then to content local space (in case of scaling or rotation?) and apply it.
            Vector3 worldMove = scrollRect.viewport.TransformDirection(move);
            scrollRect.content.localPosition -= scrollRect.content.InverseTransformDirection(worldMove);
        }
    }
}