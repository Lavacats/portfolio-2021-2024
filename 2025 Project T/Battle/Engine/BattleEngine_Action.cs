using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BattleEngine_Action 
{
    private BattleArmy SelectArmy = null;
    private bool UI_SelectArmy = false;

    public void Init()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, OnEvent_PointDownRight);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ONSELECT_ARMY, OnEvent_SelectArmy);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ONSELECT_ARMY_UI, OnEvent_SelectArmy_UI);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.MOUSE_DESELECT_ARMY, OnEvent_DeSelectArmy);
    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, OnEvent_PointDownRight);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.ONSELECT_ARMY, OnEvent_SelectArmy);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.ONSELECT_ARMY_UI, OnEvent_SelectArmy_UI);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.MOUSE_DESELECT_ARMY, OnEvent_DeSelectArmy);
    }

    private void OnEvent_PointDownRight(object value)
    {
        if (value == null) return;
        Ray ray = (Ray)value;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // 레이캐스트를 쏴서 충돌 체크
        {
            GameObject hitObject = hit.collider.gameObject;
            MeshCollider meshCollider = hitObject.GetComponent<MeshCollider>(); // 특정 스크립트를 검사 (예: MyScript)

            if (meshCollider != null)
            {
                if (SelectArmy != null)
                {
                    SelectArmy.GetBattleArmyBattleData().OrderType = E_OrderType.User;
                    SelectArmy.Update_NextPos(hit.point);
                }
            }

            BattleArmyCell TargetArmyCell = hitObject.GetComponent<BattleArmyCell>(); // 특정 스크립트를 검사 (예: MyScript)

            if (TargetArmyCell != null)
            {
                if (SelectArmy != null)
                {
                    BattleArmy TargetArmy = ArmyDataManager.Instance.GetBattleArmy(TargetArmyCell.GetArmyIdx());
               
                    SelectArmy.GetBattleArmyBattleData().SetTargetArmy(TargetArmy);
                }
            }
        }

       
    }
    private void OnEvent_SelectArmy(object value)
    {
        if (value == null) return;
        string armyIdx = (string)value;

        BattleArmy army = ArmyDataManager.Instance.GetBattleArmy(armyIdx);
        if (army != null)
        {
            if (army.GetBattleArmyBattleData().IsPlayer)
            {
                army.OnSelectArmy(true);
                SelectArmy = army;
                UI_SelectArmy = false;
            }
        }
    }
    private void OnEvent_SelectArmy_UI(object value)
    {
        if (value == null) return;
        string armyIdx = (string)value;

        BattleArmy army = ArmyDataManager.Instance.GetBattleArmy(armyIdx);
        if (army != null)
        {
            if (army.GetBattleArmyBattleData().IsPlayer)
            {
                army.OnSelectArmy(true);
                UI_SelectArmy = true;
                SelectArmy = army;
            }
        }
    }
    private void OnEvent_DeSelectArmy(object value)
    {
        if (UI_SelectArmy == false)
        {
            ArmyDataManager.Instance.ALL_DeselectArmy();
            SelectArmy = null;
        }
        else
        {
            UI_SelectArmy = false;
        }
    }
    
}
