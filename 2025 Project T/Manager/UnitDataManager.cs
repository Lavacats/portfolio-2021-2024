using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̱��� ó���� ���� �Ŵ����� �ʿ� ������ ���ֵ��� ȣ���� 
/// ���� ������ ����, ���� ������ �����մϴ�.
/// </summary>

public class UnitDataManager : SingleTon<UnitDataManager>
{
    // �ʵ忡 �����ϴ� ��� ���� ����
    private Dictionary<int, BattleBaseUnit> dicUnit = new Dictionary<int, BattleBaseUnit>();

    // ī�޶� ���� ������ ���ֵ� ����
    private List<BattleBaseUnit> CameraSelectUnit = new List<BattleBaseUnit>();


    // �巡�׸� ���� ��ü ���� ���� �������� �Լ�
    public void SelectUnit_Camera(Rect selectRect,Camera mainCamera)
    {
        foreach (var unit in UnitDataManager.Instance.GetDicUnit())
        {
            // ������ ���� ��ǥ�� ȭ�� ��ǥ�� ��ȯ�� �巡�� ���� ���� �ִ��� �˻�
            if (selectRect.Contains(mainCamera.WorldToScreenPoint(unit.Value.transform.position)))
            {
                unit.Value.OnSelect(true);

                if (CameraSelectUnit.Contains(unit.Value) == false)
                {
                    CameraSelectUnit.Add(unit.Value);
                }
            }
        }
    }
    
    // Ŭ���� ���� ���� ���� ����
    public void SelectUnit(BattleBaseUnit unit)
    {
        CameraSelectUnit.Clear();
        unit.OnSelect(true);
        CameraSelectUnit.Add(unit);
    }

    // ���� ���� �̵� ����
    public void UnitMoveOrder(Vector3 pos)
    {
        foreach (var unit in CameraSelectUnit)
        {
            unit.SetMoveState(pos);
        }
    }

    // ���� ���� ���� �Լ�
    public void AllDeSelectUnit()
    {
        CameraSelectUnit.Clear();
        foreach (var unit in dicUnit)
        {
            unit.Value.OnSelect(false);
        }
    }

    // Get , Add ó�� �Լ�
    public Dictionary<int, BattleBaseUnit> GetDicUnit() { return dicUnit; }
    public List<BattleBaseUnit> GetCameraSelectUnit() { return CameraSelectUnit; }
    public void AddBattleUnit(BattleBaseUnit unit) { dicUnit[dicUnit.Count] = unit; }
}
