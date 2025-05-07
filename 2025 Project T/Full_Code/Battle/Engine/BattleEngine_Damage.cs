using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Pool;

public class BattleEngine_Damage
{
    private DamageDataClass CalDamageClass = new DamageDataClass();


    public void HitEvent_NormalDamage(string attackerIdx, string defenderIdx,BattleBaseUnit_Status attacker, BattleBaseUnit_Status defencder)
    {
        BattleArmy attackArmy = ArmyDataManager.Instance.GetBattleArmy(attackerIdx);
        BattleArmy defenceArmy = ArmyDataManager.Instance.GetBattleArmy(defenderIdx);

        if (attackArmy == null || defenceArmy == null) return;



        switch (attacker.AttackType)
        {
            case E_AttackType.Hit_Animation:
                {
                    defenceArmy.OnDamage(CalDamageClass.Cal_Damage(attackArmy.GetArmyIdx(), defenceArmy.GetArmyIdx()));
                }
                break;
            case E_AttackType.Hit_Effect:
                {
                    defenceArmy.OnDamage(CalDamageClass.Cal_Damage(attackArmy.GetArmyIdx(), defenceArmy.GetArmyIdx()));
                }
                break;
            case E_AttackType.Hit_SpawnObject:
                {
                    // 3°³Á¤µµ
                    SpawnObject spawn;
                    if (attacker.ObjectPool.Count < 3)
                    {
                        spawn = MonoBehaviour.Instantiate(attacker.ThrowObject, attacker.GetCurUnit().transform.position + new Vector3(0, 0.7f, 0), attacker.GetCurUnit().transform.localRotation);
                        spawn.transform.SetParent(attacker.GetCurUnit().transform);
                        spawn.AttackUserInfo = attacker;
                    }
                    else
                    {
                        spawn = attacker.ObjectPool.Dequeue();
                    }
                    spawn.gameObject.SetActive(true);
                    spawn.targetObejct = attacker.TargetUnit;
                    spawn.ShootIdx = attackerIdx;
                    spawn.TargetIdx = defenderIdx;
                    spawn.transform.position = attacker.GetCurUnit().transform.position + new Vector3(0, 0.7f, 0);

                    attacker.ObjectPool.Enqueue(spawn);
                }
                break;
        }
    }
    public void OnHit_SpawnObjectDamage(SpawnObject spawn)
    {
        BattleBaseUnit_Status attacker = spawn.AttackUserInfo;
        BattleBaseUnit_Status defencder = spawn.targetObejct.GetUnit_Status();

        string attackerIdx= spawn.ShootIdx;
        string defenderIdx = spawn.TargetIdx;

        BattleArmy attackArmy = ArmyDataManager.Instance.GetBattleArmy(attackerIdx);
        BattleArmy defenceArmy = ArmyDataManager.Instance.GetBattleArmy(defenderIdx);

        defenceArmy.OnDamage(CalDamageClass.Cal_Damage(attacker.GetCurUnit().GetArmyIdx(), defencder.GetCurUnit().GetArmyIdx()));

    }
}