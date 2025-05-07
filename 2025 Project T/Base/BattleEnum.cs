using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_UNIT_STATE
{
    None,
    Idle,
    Move,
    Attack,
    SKILL,
    Dead,
}
public enum E_ARMY_STATE
{ 
    None,
    Idle,
    Move,
    Attack,
    SKILL,
}

public enum E_OrderType
{
    Auto,
    User
}
public enum E_AttackType
{
    Hit_Animation,
    Hit_SpawnObject,
    Hit_Effect,
}
public enum E_SkillType
{
    None,
    Skill_Throw,
    SKill_Buff,
    SKill_InFightLine,
}
public enum E_SKill_RnageType
{
    None,
    Skill_Target,
    Skill_Infight,
    Skill_MySelf,
}
public enum E_SpawnType
{
    Attack,
    Skill
}
public enum E_Attack
{
    Attack_Range,
    Attack_Infight,
}

public class BattleEnum 
{

}
