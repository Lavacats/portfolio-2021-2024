using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDataClass
{
   // ���� ���� Ŭ����
   // ������ ���� ��� ������ �Լ��� ����ϱ� ������ �и��ؼ� ����

    public float Cal_Damage(string attackerIdx, string defenderIdx)
    {
        float damage = 0;
        BattleArmy attackArmy =  ArmyDataManager.Instance.GetBattleArmy(attackerIdx);
        BattleArmy defenceArmy = ArmyDataManager.Instance.GetBattleArmy(defenderIdx);

        if (attackArmy == null || defenceArmy == null) return damage;
        // ���� ���� ����ȭ
        // ���ض� = ���ݷ�- ���� 
        damage = attackArmy.GetBattleArmyBattleData().AttackPoint - defenceArmy.GetBattleArmyBattleData().DefencePoint;

        return damage;
    }


}
