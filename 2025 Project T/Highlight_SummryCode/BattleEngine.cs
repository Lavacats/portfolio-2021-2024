using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 움직임 처리, 공격 범위 판정, 공격/스킬 데미지 등을 처리하는 Engine 스크립트
/// Event 호출 또는 직접 호출로 데이터 갱신을 하고 각 스크립트에 전달
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
