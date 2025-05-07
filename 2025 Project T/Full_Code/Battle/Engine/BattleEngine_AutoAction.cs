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
                // 1. 사거리 체크
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
                // 타겟이 없다면 자동으로 타겟 지정 ( 가까운 거리 순 )
                // 실제 게임에서는 타겟 지정 관련 스탯을 지정해, 해당 값을 이용한 공식이 있어 그걸로 대상을 지정
                var targetList = ArmyDataManager.Instance.Get_DicArmy().Values.Where(a => a.GetBattleArmyBattleData().IsPlayer != army.Value.GetBattleArmyBattleData().IsPlayer).ToList();

                BattleArmy nearestEnemy = null;
                float minDist = float.MaxValue;

                foreach (var target in targetList)
                {
                    float dist = Vector3.SqrMagnitude(army.Value.transform.position - target.transform.position); // SqrDistance로 성능 향상

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
