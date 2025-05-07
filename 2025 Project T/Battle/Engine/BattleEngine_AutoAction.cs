using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BattleEngine_AutoAction 
{
   public void Update()
    {
        foreach(var army in ArmyDataManager.Instance.Get_DicArmy())
        {
            if(army.Value.GetBattleArmyBattleData().TargetArmy!=null)
            {
                // 1. ��Ÿ� üũ
                float armyDistance = Vector3.Distance(army.Value.GetBattleArmyCell().transform.position, army.Value.GetBattleArmyBattleData().TargetArmy.GetBattleArmyCell().transform.position);
                if (armyDistance < army.Value.GetBattleArmyBattleData().DmageLine)
                {
                    if (army.Value.ArmyState != E_ARMY_STATE.SKILL)
                    {
                        if (army.Value.GetBattleArmyBattleData().OrderType != E_OrderType.User)
                        {
                            army.Value.Update_ArmyState(E_ARMY_STATE.Attack);
                        }
                    }
                }
                else
                {
                    if (army.Value.GetBattleArmyBattleData().OrderType != E_OrderType.User)
                    {
                        army.Value.Update_NextPos(army.Value.GetBattleArmyBattleData().TargetArmy.GetBattleArmyCell().transform.position);
                    }
                }
            }
            else if (army.Value.GetBattleArmyBattleData().TargetArmy == null)
            {
                // Ÿ���� ���ٸ� �ڵ����� Ÿ�� ���� ( ����� �Ÿ� �� )
                // ���� ���ӿ����� Ÿ�� ���� ���� ������ ������, �ش� ���� �̿��� ������ �־� �װɷ� ����� ����
                var targetList = ArmyDataManager.Instance.Get_DicArmy().Values.Where(a => a.GetBattleArmyBattleData().IsPlayer != army.Value.GetBattleArmyBattleData().IsPlayer).ToList();

                BattleArmy nearestEnemy = null;
                float minDist = float.MaxValue;

                foreach (var target in targetList)
                {
                    float dist = Vector3.SqrMagnitude(army.Value.transform.position - target.transform.position); // SqrDistance�� ���� ���

                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestEnemy = target;
                    }
                }
                army.Value.GetBattleArmyBattleData().SetTargetArmy(nearestEnemy);
            }
        }
    }
}
