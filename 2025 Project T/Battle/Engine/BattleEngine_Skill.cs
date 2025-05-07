using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattleEngine_Skill 
{
    private SkillDamageDataClass Cal_SkillDamageClass = new SkillDamageDataClass();
    private BattleEngine_Skill_Buff SkillBuffController = new BattleEngine_Skill_Buff();

    public void Init()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ON_EVENT_SKILL, OnEvent_SkillEvent);
    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.ON_EVENT_SKILL, OnEvent_SkillEvent);
    }


    private void OnEvent_SkillEvent(object value)
    {
        if(value==null) return; 

        // Amry정보 수령
       string armyIdx= (string)value;
        BattleArmy casterArmy = ArmyDataManager.Instance.GetBattleArmy(armyIdx);
        BattleData casterBattleData = casterArmy.GetBattleArmyBattleData();


        // 스킬 대상 유닛 확인
        List<BattleData> targetArmy = new List<BattleData>();
        switch(casterBattleData.SkillRnageType)
        {
            case E_SKill_RnageType.Skill_Target:
                {
                    // 공격하던 대상에게 투사체 
                    targetArmy.Add(casterBattleData.TargetArmy.GetBattleArmyBattleData());
                }
                break;
            case E_SKill_RnageType.Skill_Infight:
                {
                    // 사거리 내부에 있는 모든 적들
                    List<BattleArmy> armyList = ArmyDataManager.Instance.GetArmyList(casterArmy, casterBattleData.SkillRange);
                    foreach (var army in armyList) targetArmy.Add(army.GetBattleArmyBattleData());
                }
                break;
            case E_SKill_RnageType.Skill_MySelf:
                {
                    // 자기자신 대상 스킬
                    targetArmy.Add(casterBattleData);
                }
                break;
        }
        casterBattleData.SkillTargetArmyList = targetArmy;
        // 유닛 애니메이션 이벤트
        // 여기서 시간을 정지할거라 한번 나눈다.

        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, armyIdx);

    }
    public void Skill_AfterEffect(string armyIdx)
    {
        BattleArmy casterArmy = ArmyDataManager.Instance.GetBattleArmy(armyIdx);
        BattleData casterBattleData = casterArmy.GetBattleArmyBattleData();
        // 스킬 타입 확인 => 처리
        switch (casterBattleData.SkillType)
        {
            case E_SkillType.Skill_Throw:
                {
                    // 투사체 생성
                    SKill_ThrowObject(casterBattleData, casterBattleData.SkillTargetArmyList[0]);
                }
                break;
            case E_SkillType.SKill_InFightLine:
                {
                    foreach (var battleData in casterBattleData.SkillTargetArmyList)
                    {
                        // 근거리 광역 데미지
                        OnHit_SkillDamage(casterBattleData.ArmyIdx, battleData.ArmyIdx);
                    }
                }
                break;
            case E_SkillType.SKill_Buff:
                {
                    SkillBuffController.OnBuff_Skill(casterBattleData, casterBattleData.SkillTargetArmyList);
                }
                break;
        }
    }
    private void SKill_ThrowObject(BattleData caster,BattleData target)
    {
        SpawnObject spawn;
        BattleBaseUnit_Status attacker = ArmyDataManager.Instance.GetBattleArmy(caster.ArmyIdx).GetBattleUnitController().GetHeroUnit().GetUnit_Status();
        BattleBaseUnit_Status defencder = ArmyDataManager.Instance.GetBattleArmy(target.ArmyIdx).GetBattleUnitController().GetHeroUnit().GetUnit_Status();


        if (caster.SkillObjectPool.Count < 3)
        {
            spawn = MonoBehaviour.Instantiate(caster.SkillThrowObject, attacker.GetCurUnit().transform.position + new Vector3(0, 0.7f, 0), attacker.GetCurUnit().transform.localRotation);
            spawn.transform.SetParent(attacker.GetCurUnit().transform);
            spawn.AttackUserInfo = attacker;
        }
        else
        {
            spawn = caster.SkillObjectPool.Dequeue();
        }
        spawn.gameObject.SetActive(true);
        spawn.targetObejct = attacker.TargetUnit;
        spawn.ShootIdx = caster.ArmyIdx;
        spawn.TargetIdx = target.ArmyIdx;
        spawn.SpawnType = E_SpawnType.Skill;
        spawn.transform.position = attacker.GetCurUnit().transform.position + new Vector3(0, 0.7f, 0);

        caster.SkillObjectPool.Enqueue(spawn);
    }
    public void OnHit_SkillDamage(string attackerIdx,     string defenderIdx )
    {
        BattleArmy attackArmy = ArmyDataManager.Instance.GetBattleArmy(attackerIdx);
        BattleArmy defenceArmy = ArmyDataManager.Instance.GetBattleArmy(defenderIdx);
        
        defenceArmy.OnSkillDamage(Cal_SkillDamageClass.Cal_SkillDamage(attackerIdx, defenderIdx));

    }
}
