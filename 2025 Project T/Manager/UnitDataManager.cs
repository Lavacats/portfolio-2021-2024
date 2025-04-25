using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글톤 처리된 유닛 매니저는 맵에 존재한 유닛들을 호출해 
/// 유닛 데이터 전달, 갱신 관리를 진행합니다.
/// </summary>

public class UnitDataManager : SingleTon<UnitDataManager>
{
    // 필드에 존재하는 모든 유닛 정보
    private Dictionary<int, BattleBaseUnit> dicUnit = new Dictionary<int, BattleBaseUnit>();

    // 카메라가 현재 선택한 유닛들 정보
    private List<BattleBaseUnit> CameraSelectUnit = new List<BattleBaseUnit>();


    // 드래그를 통한 단체 유닛 선택 가져오는 함수
    public void SelectUnit_Camera(Rect selectRect,Camera mainCamera)
    {
        foreach (var unit in UnitDataManager.Instance.GetDicUnit())
        {
            // 유닛의 월드 좌표를 화면 좌표로 변환해 드래그 범위 내에 있는지 검사
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
    
    // 클릭을 통한 단일 유닛 선택
    public void SelectUnit(BattleBaseUnit unit)
    {
        CameraSelectUnit.Clear();
        unit.OnSelect(true);
        CameraSelectUnit.Add(unit);
    }

    // 유닛 상태 이동 갱신
    public void UnitMoveOrder(Vector3 pos)
    {
        foreach (var unit in CameraSelectUnit)
        {
            unit.SetMoveState(pos);
        }
    }

    // 유닛 선택 해제 함수
    public void AllDeSelectUnit()
    {
        CameraSelectUnit.Clear();
        foreach (var unit in dicUnit)
        {
            unit.Value.OnSelect(false);
        }
    }

    // Get , Add 처리 함수
    public Dictionary<int, BattleBaseUnit> GetDicUnit() { return dicUnit; }
    public List<BattleBaseUnit> GetCameraSelectUnit() { return CameraSelectUnit; }
    public void AddBattleUnit(BattleBaseUnit unit) { dicUnit[dicUnit.Count] = unit; }
}
