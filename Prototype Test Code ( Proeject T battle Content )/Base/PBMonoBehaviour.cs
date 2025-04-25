using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBMonoBehaviour : MonoBehaviour
{
    private GameObject cachedGameObject = null;
    public GameObject CachedGameObject
    {
        get
        {
            if (ReferenceEquals(cachedGameObject, null))
            {
                cachedGameObject = gameObject;
            }
            return cachedGameObject;
        }
    }

    private Transform cachedTransform = null;
    public Transform CachedTransform
    {
        get
        {
            if (ReferenceEquals(cachedTransform, null))
            {
                cachedTransform = CachedGameObject.transform;
            }

            return cachedTransform;
        }
    }

    public virtual void SetActive(bool isActive)
    {
        if (ReferenceEquals(CachedGameObject, null)) return;
        if (CachedGameObject.activeSelf == isActive) return;
        CachedGameObject.SetActive(isActive);
    }

    public bool IsActive()
    {
        return CachedGameObject.activeSelf;
    }

    public bool IsActiveInHierarchy()
    {
        return CachedGameObject.activeInHierarchy;
    }
}
