using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BattleArmy 클래스는 전투에 참여하는 군단(1명의 영웅 + 복수 유닛)의 상태, 위치, 연출, 전투 데이터를 종합적으로 관리한다.
/// 각 군단은 Cell 단위 위치, 유닛 컨트롤러, UI 요소, 전투 데이터로 구성되어 있으며, 개별 전투 흐름과 명령을 처리한다.
/// </summary>
public class BattleArmy : MonoBehaviour
{
    [SerializeField] BattleArmyCell ArmyCell;                   // 각 군단의 위치 및 진형 처리 컴포넌트
    [SerializeField] BattleUnitController UnitController;       // 유닛(영웅, 병사 포함)의 이동/상태/전투 애니메이션 제어 컴포넌트
    [SerializeField] BattleArmy_FXandUI ArmyFx;                 // UI 및 이펙트 처리 (ex: 선택 마커, 피격 이펙트 등)
    [SerializeField] GameObject SelectMarker;                    // 유닛 선택 시 표시되는 마커 오브젝트

    [SerializeField] BattleData ArmyBattleData = new BattleData();    // 이 군단의 전투용 데이터 구조 (HP, 상태, 공격 대상 등)



    public string ArmyIdx = string.Empty;                       // 군단 고유 인덱스 (ID)  
    public E_ARMY_STATE ArmyState = E_ARMY_STATE.None;          // 현재 군단 상태 (대기, 이동, 공격, 스킬 등)
    void Start()
    {
        
    }
    void Update()
    {
        // 영웅 유닛이 존재하면 마커 아이콘 위치를 따라 UI 업데이트
        if (UnitController.GetHeroUnit() != null)
        {
            ArmyFx.Update_Icon(UnitController.GetHeroUnit().GetBattleUnitAnimation().GetBaseModel().transform);
        }
        if (ArmyState != E_ARMY_STATE.SKILL)
        {
            ArmyCell.Cell_Update(ArmyState);
        }
    }

    /// <summary>
    /// 군단 초기화 함수: 위치, 데이터, 유닛, UI, 전역 등록
    /// </summary>
    public void Init()
    {
        ArmyCell.Init(ArmyIdx, Update_ArmyState);
        ArmyBattleData.Init(ArmyIdx);
        UnitController.Init(ArmyCell, ArmyBattleData);
        ArmyFx.Init(ArmyIdx);
 
        ArmyDataManager.Instance.AddBattleArmy(ArmyIdx, this);
        ArmyState = E_ARMY_STATE.Idle;
    }

    /// <summary>
    /// 외부 입력(유저 명령 등)에 따라 군단의 다음 이동 위치 갱신
    /// </summary>
    public void Update_NextPos(Vector3 pos)
    {
        if (ArmyState != E_ARMY_STATE.SKILL)
        {
            Update_ArmyState(E_ARMY_STATE.Move);
            UnitController.Update_UnitState(E_ARMY_STATE.Move, E_OrderType.User);
            ArmyCell.Update_NextPos(pos);
        }
    }
    public void Update_ArmyState(E_ARMY_STATE state)
    {
        switch (state)
        {
            case E_ARMY_STATE.Idle:
                {
                    ArmyState = state;
                    ArmyBattleData.OrderType = E_OrderType.Auto;
                }
                break;
            case E_ARMY_STATE.Move:
                {
                    ArmyState = state;
                }
                break;
            case E_ARMY_STATE.Attack:
                {
                    if(ArmyState!=E_ARMY_STATE.Attack && ArmyBattleData.ArmyAttack==E_Attack.Attack_Infight)
                    {
                        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.COMBAT_START, Tuple.Create(this,ArmyBattleData.TargetArmy));
                    }
                    if(state!=E_ARMY_STATE.Idle)
                    {
                        ArmyState = state;
                    }
                    UnitController.Update_UnitState(E_ARMY_STATE.Attack,E_OrderType.Auto);
                }
                break;
            case E_ARMY_STATE.SKILL:
                {
                    ArmyState = state;
                    UnitController.Update_UnitState(E_ARMY_STATE.SKILL, E_OrderType.User);
                }
                break;
        }
    }

    public void OnDamage(float damage)
    {
        ArmyBattleData.HealthPoint -= damage;
    }
    public void OnSkillDamage(float damage)
    {
        ArmyBattleData.HealthPoint -= damage;
    }
    public BattleArmy_FXandUI GetArmyUI() { return ArmyFx; }
    public string GetArmyIdx() { return ArmyIdx; }
    public void OnSelectArmy(bool select) { SelectMarker.SetActive(select); }
    public BattleUnitController GetBattleUnitController() { return UnitController; }
    public BattleArmyCell GetBattleArmyCell() { return ArmyCell; }
    public BattleData GetBattleArmyBattleData() { return ArmyBattleData; }
}
