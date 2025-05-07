using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamageDataClass 
{
    // 연산 보조 클래스
    // 원본은 좀더 길고 복잡한 함수를 사용하기 때문에 분리해서 제작
    public float Cal_SkillDamage(string attackerIdx, string defenderIdx)
    {
        float damage = 0;
        BattleArmy attackArmy = ArmyDataManager.Instance.GetBattleArmy(attackerIdx);
        BattleArmy defenceArmy = ArmyDataManager.Instance.GetBattleArmy(defenderIdx);

        if (attackArmy == null || defenceArmy == null) return damage;
        // 공식 예시 간략화
        // 피해랑 = 공격력- 방어력 
        damage = attackArmy.GetBattleArmyBattleData().SkillDamage - defenceArmy.GetBattleArmyBattleData().DefencePoint;

        return damage;
    }



}
