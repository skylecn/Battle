using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtension
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    public static bool TryRemoveComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            UnityEngine.Object.Destroy(component);
            return true;
        }

        return false;
    }

    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void ResetTransformRS(this Transform transform){
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public static void SetParentAndResetTransform(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void SetParentAndResetTransformRS(this Transform transform, Transform parent){
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public static void SetActive(this Component component, bool active)
    {
        if (component == null)
        {
            return;
        }

        if (component.gameObject.activeSelf != active)
        {
            component.gameObject.SetActive(active);
        }
    }

    public static Transform DeepFindChild(this Transform root, string childName)
    {
        Transform result = null;
        result = root.Find(childName);
        if (result == null)
        {
            for (int index = 0; index < root.childCount; index++)
            {
                var childTrans = root.GetChild(index);
                result = DeepFindChild(childTrans, childName);
                if (result != null)
                {
                    return result;
                }
            }
        }

        return result;
    }
}