using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ó��, ���� ���� ����, ����/��ų ������ ���� ó���ϴ� Engine ��ũ��Ʈ
/// Event ȣ�� �Ǵ� ���� ȣ��� ������ ������ �ϰ� �� ��ũ��Ʈ�� ����
/// </summary>

public class BattleEngine 
{
    private BattleEngine_Action Engine_Action = new BattleEngine_Action();
    private BattleEngine_AutoAction Engine_Auto = new BattleEngine_AutoAction();
    private BattleEngine_Damage Engine_Damage = new BattleEngine_Damage();
    private BattleEngine_Skill Engine_Skill = new BattleEngine_Skill();
    public void Init()
    {
        Engine_Action.Init();
        Engine_Skill.Init();
        ArmyDataManager.Instance.Engine = this;
    }
    public void OnDestroy()
    {
        Engine_Action.OnDestroy();
        Engine_Skill.OnDestroy();
    }
    public void Update()
    {
        Engine_Auto.Update();
    }
    public BattleEngine_Damage GetEngine_Damage() { return Engine_Damage; }
    public BattleEngine_Skill GetEngine_SKill() { return Engine_Skill; }
}
