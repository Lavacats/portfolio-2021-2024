using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

/// <summary>
/// BattleBaseUnit은 전투 유닛(병사/영웅 포함)의 핵심 로직을 담당합니다.
/// 유닛의 상태(Idle, Move, Attack, Skill), 애니메이션, 위치 이동, 타겟 추적 및 피격 처리를 포함합니다.
/// </summary>
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
                // 근접 유닛의 경우, NextPixel 위치에 도달하면 공격
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
                    // 이동 중이면 계속 이동
                    UnitStatus.Update_Move(this.transform, NextPixel.ShowPixel.transform, Set_State, UnitState,E_OrderType.User);
                    return;
                }

                // 원거리 유닛: 사거리 안에 있으면 공격
                if (Vector3.Distance(this.transform.position, UnitStatus.TargetUnit.transform.position) < UnitStatus.GetAttackRage())
                {
                    UnitStatus.Update_Look(this.transform, new Vector3(UnitStatus.TargetUnit.transform.position.x, 0, UnitStatus.TargetUnit.transform.position.z));
                    Set_State(E_UNIT_STATE.Attack);
                    return;
                }
            }
            // 타겟에 도달하지 못했거나 사거리 밖인 경우 위치로 이동
            UnitStatus.Update_Move(this.transform, UnitPixel.ShowPixel.transform,Set_State,UnitState);
        }
        else if(UnitState==E_UNIT_STATE.SKILL)
        {
            // 스킬 상태일 때는 애니메이션이 끝날 때까지 대기 (처리는 별도 함수에서 수행)
        }
        else
        {
            // Idle 또는 Move 상태인 경우, 지정된 픽셀 위치로 이동
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

    /// <summary>
    /// 유닛의 상태 설정 (애니메이션 연동 포함)
    /// </summary>
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

    /// <summary>
    /// 스킬 애니메이션 종료 시 호출되는 콜백
    /// </summary>
    private void OnEndEvent_SkillAnimation()
    {
        // 스킬 종료 후 공격 상태로 복귀
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
