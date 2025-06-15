using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PANEGamepad.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PANEGamepad.Scenes
{
    public partial class SceneController
    {
        public static bool SceneMatch(IEnumerable<GameObject> scene, IEnumerable<string> hasNames = default, IEnumerable<string> hasNoNames = default)
        {
            IEnumerable<string> names = scene.Where(s => s != null).Select(s => s.name);
            return (hasNames == default || names.Intersect(hasNames).Count() == hasNames.Count())
                && (hasNoNames == default || names.Intersect(hasNoNames).Count() == 0);
        }
        public static Vector2 CenterPoint(GameObject gameObject)
        {
            // Step 1: Get the button's center in screen space
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            RectTransform target = gameObject.GetComponent<RectTransform>();

            // Get the button's center in screen space
            Vector2 targetCenter = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera,
                target.position
            );
            Rect targetRect = RectTransformUtility.PixelAdjustRect(target, canvas);
            targetCenter += targetRect.center;

            return targetCenter;
        }
        public static Vector2 VisiblePoint(GameObject gameObject)
        {
            // Step 1: Get the button's center in screen space
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            RectTransform target = gameObject.GetComponent<RectTransform>();

            // Get the button's center in screen space
            Vector2 targetCenter = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera,
                target.position
            );

            Rect targetRect = RectTransformUtility.PixelAdjustRect(target, canvas);
            targetCenter += targetRect.center;

            List<Vector2> points = new() { targetCenter };

            Vector2 offset1 = new(targetRect.width / 6, targetRect.height / 6);
            Vector2 offset2 = new(-targetRect.width / 6, targetRect.height / 6);

            points.Add(targetCenter + offset1);
            points.Add(targetCenter - offset1);
            points.Add(targetCenter + offset2);
            points.Add(targetCenter - offset2);

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
                        results[0].gameObject == gameObject.gameObject ||
                        results[0].gameObject.transform.IsChildOf(gameObject.transform)
                    )
                )
                {
                    return point;
                }
            }
            return default;
        }
        private static bool IsSceneObject(GameObject go)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                Type type = component.GetType();
                if (component is Selectable || CustomTypes.Contains(type.Name))
                {
                    return true;
                }
            }
            return false;
        }
        public static GameObject GetTopObject(Vector2 point)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = point
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            List<GameObject> gameObjects = new();
            foreach (RaycastResult obj in results)
            {
                gameObjects.AddRange([obj.gameObject]);
                gameObjects.AddRange(obj.gameObject.GetParents());
            }
            GameObject blocker = GameObject.Find("Canvas/Blocker");
            foreach (GameObject gameObject in gameObjects)
            {
                if (IsSceneObject(gameObject) && gameObject != blocker)
                {
                    Plugin.Log.LogInfo($"Hovered on {gameObject.name}");
                    return gameObject;
                }
            }
            Plugin.Log.LogInfo($"No Hovered");
            return null;
        }
        // public static bool CheckDropdown(GameObject obj, GameObject current)
        // {
        //     // filterout if not same dropdown list
        //     return !(current is not null
        //         && current.GetComponentInParent("TMP_Dropdown") is Component dd
        //         && dd.gameObject != obj.GetComponentInParent("TMP_Dropdown")?.gameObject);
        // }
        public static bool IsSelectable(GameObject obj, GameObject current = null)
        {
            Behaviour beh = obj.GetComponent<Behaviour>();
            return (beh == null || beh.isActiveAndEnabled)
                    && IsEnabled(obj)
                    //&& CheckDropdown(obj, current)
                    && ((IsOnScroll(obj) && IsOnScroll(current)) || VisiblePoint(obj) != default);
        }
        public static bool IsOnScroll(GameObject obj)
        {
            return obj != null && obj.GetComponentInParent<ScrollRect>() != null;
        }
        public static bool IsEnabled(GameObject obj)
        {
            if (!obj.GetComponent<Behaviour>().isActiveAndEnabled)
            {
                return false;
            }
            return !IsLocked(obj);
        }

        public static bool IsLocked(GameObject obj)
        {
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
                    return (bool)lockedField.GetValue(component);
                }
                else if (lockedProperty != null && lockedProperty.PropertyType == typeof(bool))
                {
                    return (bool)lockedProperty.GetValue(component);
                }
            }
            return false;
        }

        public static void EnsureVisible(GameObject control)
        {
            Vector2 margin = new Vector2(40, 40);
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

            Vector2 contentPosMin = scrollRect.viewport.InverseTransformPoint(scrollRect.content.TransformPoint(scrollRect.content.rect.min));
            Vector2 contentPosMax = scrollRect.viewport.InverseTransformPoint(scrollRect.content.TransformPoint(scrollRect.content.rect.max));

            Vector2 childPosMin = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.min));
            Vector2 childPosMax = scrollRect.viewport.InverseTransformPoint(child.TransformPoint(child.rect.max));

            childPosMin -= margin;
            childPosMax += margin;

            Vector2 move = Vector2.zero;

            // Check if one (or more) of the child bounding edges goes outside the viewport and
            // calculate move vector for the content rect so it can keep it visible.
            if (scrollRect.verticalScrollbar is Scrollbar verticalScrollbar
                && verticalScrollbar.gameObject.activeInHierarchy
                && verticalScrollbar.size < 1.0f)
            {
                if (childPosMax.y > viewPosMax.y)
                {
                    move.y = Mathf.Min(childPosMax.y - viewPosMax.y, contentPosMax.y - viewPosMax.y);
                }
                if (childPosMin.y < viewPosMin.y)
                {
                    move.y = Mathf.Max(childPosMin.y - viewPosMin.y, contentPosMin.y - viewPosMin.y);
                }
            }
            else
            {
                move.y = 0;
            }

            if (scrollRect.horizontalScrollbar is Scrollbar horizontalScrollbar
                && horizontalScrollbar.gameObject.activeInHierarchy
                && horizontalScrollbar.size < 1.0f)
            {
                if (childPosMin.x < viewPosMin.x)
                {
                    move.x = Mathf.Min(childPosMin.x - viewPosMin.x, contentPosMax.x - viewPosMax.x);
                }
                if (childPosMax.x > viewPosMax.x)
                {
                    move.x = Mathf.Max(childPosMax.x - viewPosMax.x, contentPosMin.x - viewPosMin.x);
                }
            }
            else
            {
                move.x = 0;
            }

            // Transform the move vector to world space, then to content local space (in case of scaling or rotation?) and apply it.
            Vector3 worldMove = scrollRect.viewport.TransformDirection(move);
            scrollRect.content.localPosition -= scrollRect.content.InverseTransformDirection(worldMove);
            Canvas.ForceUpdateCanvases();
        }
        public static bool PressButton(GameObject go)
        {
            if (go != null && go.GetComponent<Button>() is Button button)
            {
                button.onClick.Invoke();
                return true;
            }
            else if (go != null && go.GetComponent("CustomToggle") as MonoBehaviour is MonoBehaviour customToggle)
            {
                customToggle.Invoke("OnToggleClick", 0f);
                return true;
            }
            else if (go != null && go.GetComponent<Toggle>() is Toggle toggle)
            {
                toggle.isOn = !toggle.isOn;
                return true;
            }
            return false;
        }
    }
}