using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Unit_Icon : MonoBehaviour
{
    [SerializeField] Image UnitIcon;
    private string ArmyIdx;

    public void Set_IconImage(Sprite iconSprite,string idx)
    {
        UnitIcon.sprite = iconSprite;
        ArmyIdx = idx;
    }
    public void On_Click_Icon()
    {
        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.MOUSE_DESELECT_ARMY, null);
        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.ONSELECT_ARMY_UI, ArmyIdx);
    }
}
