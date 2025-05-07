using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : Button, IPointerDownHandler
{

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (IsActive() && IsInteractable())
            {
                UISystemProfilerApi.AddMarker("Button.onClick", this);
                base.onClick.Invoke();
       
            }
        }
    }
}
