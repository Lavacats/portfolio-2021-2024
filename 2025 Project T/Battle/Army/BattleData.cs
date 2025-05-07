using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleData 
{
    /// <summary>
    /// ���� Table �����Ϳ� ��ü BattleData�� �������־�� ������,
    /// �̹� ������Ʈ������ ���̺��� ������� �ʱ� ������ �ӽð����� ��� ����ִ� ����
    /// </summary>


    public string ArmyIdx = string.Empty;
    public bool IsPlayer = false;
    public BattleArmy TargetArmy = null;
    public float DmageLine = 20.0f;
    public E_OrderType OrderType = E_OrderType.Auto;

    public E_Attack ArmyAttack = E_Attack.Attack_Range;
    public float HealthPoint = 100;
    public float HealthMaxPoint = 100;
    public float AttackPoint = 20;
    public float AttackPoint_Original = 20;
    public float DefencePoint = 5;

    //��ų ���
    public E_SkillType SkillType = E_SkillType.None;
    public E_SKill_RnageType SkillRnageType = E_SKill_RnageType.None;
    public SpawnObject SkillThrowObject = null;
    public Queue<SpawnObject> SkillObjectPool = new Queue<SpawnObject>();
    public float SkillRange = 20f;
    public float SkillDamage = 10f;
    public List<BattleData> SkillTargetArmyList = new List<BattleData>();
    public float SkillCoolTime = 6f;
    public Sprite SkillImage;

    // ���� ���
    public float BuffCoolTime = 6f;
    public float BuffValue = 5f;
    public Sprite BuffIcon;

    public void Init(string idx)
    {
        ArmyIdx = idx;
    }

    public void SetTargetArmy(BattleArmy targetArmy)
    {
        if(targetArmy==null)
        {

        }
        else
        {
            TargetArmy = targetArmy;
            BattleArmy army = ArmyDataManager.Instance.GetBattleArmy(ArmyIdx);

            army.GetBattleUnitController().Update_TargetUnitData(targetArmy.GetBattleUnitController());

        }
    }
}
