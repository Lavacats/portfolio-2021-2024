using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 움직임 처리, 공격 범위 판정, 공격/스킬 데미지 등을 처리하는 Engine 스크립트
/// Event 호출 또는 직접 호출로 데이터 갱신을 하고 각 스크립트에 전달
/// </summary>

public class BattleEngine 
{
    public void Init()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, OnEvent_PointDownRight);
    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, OnEvent_PointDownRight);
    }
    public void Update()
    {
    }
    private void OnEvent_PointDownRight(object value)
    {
        // 유닛 이동 처리 계산
        Vector3 pointDown = (Vector3)value;

        // [1] 마우스 클릭 위치를 맵 좌표로 변환
        Battle_MapPixel pixel = BattleEngine_Manager.Instance.MapDirector.GetPixel(pointDown);
        Battle_MapCell cell = BattleEngine_Manager.Instance.MapDirector.GetCell(pointDown);

        // [2] 유효한 위치가 아닐 경우 처리 중단
        if (pixel == null || cell == null) return;

        // [3] 현재 선택된 유닛 리스트 조회
        foreach (var selectUnit in UnitDataManager.Instance.GetCameraSelectUnit())
        {
            // [4] 이동 목표 위치 설정
            selectUnit.Update_GoalPixel(cell, pixel);

            // [5] 이동 상태로 전환
            selectUnit.SetMoveState(pointDown);

            // [6] 경로 탐색 요청
            BattleEngine_Manager.Instance.Pathfinder.GetUnitPathFinder(selectUnit.CurPixel, pixel);
        }
    }
}
