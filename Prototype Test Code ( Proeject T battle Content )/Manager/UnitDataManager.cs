using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataManager : SingleTon<UnitDataManager>
{
    private Dictionary<int, BattleBaseUnit> dicUnit = new Dictionary<int, BattleBaseUnit>();

    private List<BattleBaseUnit> CameraSelectUnit = new List<BattleBaseUnit>();




    public void UnitMoveOrder(Vector3 pos)
    {
       // if(CameraSelectUnit.Count==1)
       // {
       //     CameraSelectUnit[0].SetMovePos(pos);
       // }
    }


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

    public void SelectUnit(BattleBaseUnit unit)
    {
        CameraSelectUnit.Clear();
        unit.OnSelect(true);
        CameraSelectUnit.Add(unit);
    }



    public Dictionary<int, BattleBaseUnit> GetDicUnit() { return dicUnit; }
    public List<BattleBaseUnit> GetCameraSelectUnit() { return CameraSelectUnit; }
    public void AllDeSelectUnit() 
    {
        CameraSelectUnit.Clear();
        foreach (var unit in dicUnit)
        {
            unit.Value.OnSelect(false);
        }
    }
    public void AddBattleUnit(BattleBaseUnit unit) { dicUnit[dicUnit.Count] = unit; }
}
