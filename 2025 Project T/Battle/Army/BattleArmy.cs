using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleArmy : MonoBehaviour
{
    [SerializeField] BattleArmyCell ArmyCell;
    [SerializeField] BattleUnitController UnitController;
    [SerializeField] BattleArmy_FXandUI ArmyFx;
    [SerializeField] GameObject SelectMarker;

    [SerializeField] BattleData ArmyBattleData = new BattleData();



    public string ArmyIdx = string.Empty;
    public E_ARMY_STATE ArmyState = E_ARMY_STATE.None;
    void Start()
    {
        
    }
    void Update()
    {
        if (UnitController.GetHeroUnit() != null)
        {
            ArmyFx.Update_Icon(UnitController.GetHeroUnit().GetBattleUnitAnimation().GetBaseModel().transform);
        }
        if (ArmyState != E_ARMY_STATE.SKILL)
        {
            ArmyCell.Cell_Update(ArmyState);
        }
    }
    public void Init()
    {
        ArmyCell.Init(ArmyIdx, Update_ArmyState);
        ArmyBattleData.Init(ArmyIdx);
        UnitController.Init(ArmyCell, ArmyBattleData);
        ArmyFx.Init(ArmyIdx);
 
        ArmyDataManager.Instance.AddBattleArmy(ArmyIdx, this);
        ArmyState = E_ARMY_STATE.Idle;
    }
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
