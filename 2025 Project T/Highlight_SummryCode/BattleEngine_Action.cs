using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

/// <summary>
/// BattleEngine_Action은 플레이어 입력(마우스 클릭 등)을 받아 유닛(군단)에게 이동 또는 타겟 지정 명령을 전달합니다.
/// UI에서 선택된 유닛과 직접 클릭한 유닛 간의 구분, 우클릭 이동/공격 명령, 선택 해제까지 담당합니다.
/// </summary>
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

    /// <summary>
    /// 마우스 우클릭으로 이동 또는 공격 명령 전달
    /// </summary>
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

    /// <summary>
    /// 마우스 클릭으로 유닛 선택 시 호출되는 이벤트 처리
    /// </summary>
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
    /// <summary>
    /// UI 버튼 클릭으로 유닛 선택 시 호출되는 이벤트 처리
    /// </summary>
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
    /// <summary>
    /// 마우스 외부 클릭 또는 해제 입력 시 선택 해제 처리
    /// </summary>
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
