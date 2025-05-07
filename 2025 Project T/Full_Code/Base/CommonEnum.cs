using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSceneType
{
    None = -1,
    StartScene,
    IntroScene,

}


#region Unit Type
public enum eUnitType
{
    None = 0,
    Normal_Unit,    // 모브 , 스케반, 드로이드 등 일반 유닛에 대한 처리
    Hero_Unit,      // 일반적인 전투 유닛에 대한 처리
    Module_Unit,    // 터렛, 천둥이, 회복 드론 
    Vehicle_Unit,   // 이로하 전차, 크루세이더, 헬기 , 트럭 등
    Boss_Unit,      // 비나 , 고즈 , 쿠로코 등
}
public enum eTacticalType
{
    None,       // 없음
    Explosion,  // 폭발
    Piercing,   // 관통
    Mystic,     // 신비
    Normal,     // 일반
    Sonic,      // 진동
}
public enum eWeaponType
{
    None,
}

public enum eSchoolType
{
    None,
}

public enum eUnitState
{
    None = 0,
    Idle,
    Move,
    Attack,
    Die,
}
#endregion
