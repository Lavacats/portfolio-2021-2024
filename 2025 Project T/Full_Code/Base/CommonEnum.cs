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
    Normal_Unit,    // ��� , ���ɹ�, ����̵� �� �Ϲ� ���ֿ� ���� ó��
    Hero_Unit,      // �Ϲ����� ���� ���ֿ� ���� ó��
    Module_Unit,    // �ͷ�, õ����, ȸ�� ��� 
    Vehicle_Unit,   // �̷��� ����, ũ�缼�̴�, ��� , Ʈ�� ��
    Boss_Unit,      // �� , ���� , ����� ��
}
public enum eTacticalType
{
    None,       // ����
    Explosion,  // ����
    Piercing,   // ����
    Mystic,     // �ź�
    Normal,     // �Ϲ�
    Sonic,      // ����
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
