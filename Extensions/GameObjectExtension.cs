using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PANEGamepad.Extensions
{
    public static class GameObjectExtensions
    {
        public static IEnumerable<GameObject> GetChildren(this GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                yield return child.gameObject;
            }
        }
        public static IEnumerable<T> GetChildren<T>(this GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.GetComponent<T>() is T component)
                {
                    yield return component;
                }
            }
        }
        public static T GetChild<T>(this GameObject parent, string name = null)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.GetComponent<T>() is T component && (name is null || name == child.name))
                {
                    return component;
                }
            }
            return default;
        }
        public static object GetFieldValue(this GameObject gameObject, string componentName, string fieldName)
        {
            Component component = gameObject.GetComponent(componentName);
            if (component == null)
            {
                return null;
            }

            FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(component);
        }
        public static T GetFieldValue<T>(this GameObject gameObject, string componentName, string fieldName)
        {
            object value = gameObject.GetFieldValue(componentName, fieldName);
            return value is T typedValue ? typedValue : default;
        }
    }
}