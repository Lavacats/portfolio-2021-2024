using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent :PBMonoBehaviour
{
    protected bool isSelected = false;
    private RectTransform cachedRectTransform = null;

    public RectTransform CachedRectTransform
    {
        get
        {
            if (ReferenceEquals(cachedRectTransform, null))
            {
               // cachedRectTransform = CachedGameObject.SafeGetComponent<RectTransform>();
            }

            return cachedRectTransform;
        }
    }

    protected Vector3 oriSize = Vector3.one;

    protected virtual void Awake()
    {
        oriSize = CachedRectTransform.localScale;
    }

    protected virtual void OnAction(GameObject go) { }
    protected virtual void OnCancel(GameObject go) { }

    protected virtual void OnSelect(bool isSelect)
    {
        isSelected = isSelect;
    }

    protected virtual void OnPress(bool isPress)
    {
        Vector3 size = isPress ? oriSize * 0.97f : oriSize;
        CachedRectTransform.localScale = size;
    }

    protected virtual void OnHover(bool isHover)
    {
        Vector3 size = isHover ? oriSize * 1.03f : oriSize;
        CachedRectTransform.localScale = size;
    }

    protected virtual void OnClick()
    {
    }

    /// <summary>
    /// UIHandler를 통해서 지워질때 자동으로 호출
    /// </summary>
    public virtual void OnClose()
    {
        StopAllCoroutines();
        CachedGameObject.SetActive(false);
    }

    /// <summary>
    /// UIHandler를 통해서 만들어 질때 자동으로 호출 
    /// </summary>
    public virtual void OnCreate()
    {
        CachedGameObject.SetActive(true);
    }
}
