using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
public class BattleBaseUnit : MonoBehaviour
{
    [SerializeField] private BattleBaseUnit_Animation UnitAnimation = new BattleBaseUnit_Animation();
    [SerializeField] private BattleBaseUnit_Status UnitStatus = new BattleBaseUnit_Status();

    public E_UNIT_STATE UnitState = E_UNIT_STATE.Idle;
    public E_Attack EAttackc = E_Attack.Attack_Range;
    private string ArmyIdx = string.Empty;

    private Battle_MapPixel UnitPixel;
    public Battle_MapPixel NextPixel;

    private void Update()
    {
        if (UnitState == E_UNIT_STATE.Attack)
        {
            if (UnitStatus.TargetUnit != null)
            {
                if (EAttackc == E_Attack.Attack_Infight)
                {
                    Transform nextPos = NextPixel.ShowPixel.transform;
                    if (Vector3.Distance(this.transform.position, nextPos.position) < 0.1f)
                    {
                        //this.transform.position = nextPos.position;
                        UnitStatus.Update_Look(this.transform, new Vector3(nextPos.transform.position.x,0,nextPos.transform.position.z));
                        Set_State(E_UNIT_STATE.Attack);
                        return;
                    }
                    UnitStatus.Update_Move(this.transform, NextPixel.ShowPixel.transform, Set_State, UnitState,E_OrderType.User);
                    return;
                }
             
                
                if (Vector3.Distance(this.transform.position, UnitStatus.TargetUnit.transform.position) < UnitStatus.GetAttackRage())
                {
                    UnitStatus.Update_Look(this.transform, new Vector3(UnitStatus.TargetUnit.transform.position.x, 0, UnitStatus.TargetUnit.transform.position.z));
                    Set_State(E_UNIT_STATE.Attack);
                    return;
                }
            }
            UnitStatus.Update_Move(this.transform, UnitPixel.ShowPixel.transform,Set_State,UnitState);
        }
        else if(UnitState==E_UNIT_STATE.SKILL)
        {

        }
        else
        {
            UnitStatus.Update_Move(this.transform, UnitPixel.ShowPixel.transform, Set_State, E_UNIT_STATE.Idle);
        }
        
   
    }
    public void Init(Battle_MapPixel pixel,string idx, E_Attack attack)
    {
        UnitPixel = pixel;
        UnitStatus.Init(pixel,this);
        NextPixel = pixel;
        UnitAnimation.Init(OnEvent_Animation_Attack);
        ArmyIdx = idx;
        EAttackc = attack;
    }
    public void Set_State(E_UNIT_STATE state,E_OrderType order=E_OrderType.Auto)
    {
        if (UnitState == E_UNIT_STATE.Attack)
        {
            if(order==E_OrderType.User)
            {
                UnitState = state;
                UnitAnimation.Set_StateAnimation(state);
            }
            else if(state!=E_UNIT_STATE.Move)
            {
                UnitState = state;
                UnitAnimation.Set_StateAnimation(state);
            }
        }
        else if(UnitState==E_UNIT_STATE.SKILL)
        {
            UnitState = state;
            UnitAnimation.Set_StateAnimation(state);
            UnitAnimation.GetBaseModel().Set_AnimationEnd_Event("Skill", OnEndEvent_SkillAnimation);
        }
        else
        {
            UnitState = state;
            UnitAnimation.Set_StateAnimation(state);
        }
    }
    private void OnEndEvent_SkillAnimation()
    {

        UnitAnimation.GetBaseModel().GetAnimator().updateMode = AnimatorUpdateMode.Normal;
        Set_State(E_UNIT_STATE.Attack, E_OrderType.Auto);
        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, ArmyIdx);
    }
    private void OnEvent_Animation_Attack()
    {
        // 히트 이벤트 처리
        ArmyDataManager.Instance.Engine.GetEngine_Damage().HitEvent_NormalDamage(
            ArmyIdx, UnitStatus.TargetUnit.GetArmyIdx(),
            this.UnitStatus,UnitStatus.TargetUnit.GetUnit_Status()
            );
        //UnitStatus.HitEvent();

    }
    public BattleBaseUnit_Animation GetBattleUnitAnimation() { return UnitAnimation; }
    public BattleBaseUnit_Status GetUnit_Status() { return UnitStatus; }
    public String GetArmyIdx() { return ArmyIdx; }
}
