using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseParam { }

public class UIBase : PGMonoBehaviour
{
    private RectTransform cachedRectTransform = null;

    public RectTransform CachedRectTransform
    {
        get
        {
            if (cachedRectTransform == null)
            {
                //cachedRectTransform = CachedGameObject.SafeGetComponent<RectTransform>();
            }

            return cachedRectTransform;
        }
    }

    public virtual void Setup(UIBaseParam param) { }

    /// <summary>
    /// �ڵ����� ȣ��
    /// </summary>
    public virtual void OnClose()
    {
        StopAllCoroutines();
        CachedGameObject.SetActive(false);
    }

    /// <summary>
    /// �ڵ����� ȣ��
    /// </summary>
    public virtual void OnCreate()
    {
        CachedGameObject.SetActive(true);
    }
}
