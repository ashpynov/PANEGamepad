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

        public static Component GetComponentInParent(this GameObject child, string typeName)
        => child.GetComponentInParent<Component>(typeName);


        public static T GetComponentInParent<T>(this GameObject child, string typeName)
        {
            if (child == null)
            {
                return default;
            }
            Transform current = child.transform.parent;

            while (current != null)
            {
                Component[] components = current.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component.GetType().Name == typeName ||
                        component.GetType().FullName == typeName)
                    {
                        return component is T typedValue ? typedValue : default;
                    }
                }
                current = current.parent;
            }
            return default;
        }

        public static Component GetComponentByTypeName(this GameObject gameObject, string typeName)
        => gameObject.GetComponentByTypeName<Component>(typeName);


        public static T GetComponentByTypeName<T>(this GameObject gameObject, string typeName)
        {
            if (gameObject == null)
            {
                return default;
            }
            Component[] components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component.GetType().Name == typeName ||
                    component.GetType().FullName == typeName)
                {
                    return component is T typedValue ? typedValue : default;
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

        public static GameObject[] GetParents(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return [];
            }

            List<GameObject> parents = new();
            Transform current = gameObject.transform;

            while (current != null)
            {
                current = current.parent;
                if (current != null)
                {
                    parents.Add(current.gameObject);
                }

            }
            return parents.ToArray();
        }

        public static string[] GetPath(this GameObject gameObject)
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
        public static string GetPathString(this GameObject gameObject)
        {
            return string.Join("/", GetPath(gameObject));
        }
    }
}