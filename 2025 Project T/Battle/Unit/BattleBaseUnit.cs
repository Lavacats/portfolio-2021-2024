using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBaseUnit : MonoBehaviour
{
    [SerializeField] private BaseModel unitModel;       // 유닛 프리팹 모델
    [SerializeField] private GameObject unitMarker;     // 유닛 표시용 마커

    // 유닛 위치 이동 / 길찾기 용 Pxiel 
    public Battle_MapPixel PrePixel = null;
    public Battle_MapPixel CurPixel = null;
    public Battle_MapPixel GoalPixel = null;

    // 유닛 State 정보      
    private E_UNIT_STATE UnitState = E_UNIT_STATE.None;     // Unit의 현재 상태 정보
    private Vector3 nextPos = Vector3.zero;                 // Unit이 이동할 위치
    private Vector3 prevPos = Vector3.zero;                 // Unit이 이전 이동 위치
    private float unitSpeed = 1.0f;

  
    private void Start()
    {
        UnitDataManager.Instance.AddBattleUnit(this);   // UnitManager에 등록
        nextPos = transform.position;                   // 이동위치를 현 위치로 고정
    }

    private void Update()
    {
        // 유닛 상태에 따른 처리 구현
        // 이후 상태 머신을 따로 구현해 코드 분리할 것.

        if (UnitState == E_UNIT_STATE.Move)            
        {
            MoveAndLookAtTarget();
        
            // 목적지에 도달했는지 확인
            if (Vector3.Distance(transform.position, new Vector3(nextPos.x, 0, nextPos.z)) < 0.1f)
            {
                // 목적지에 도달하면 더 이상 이동하지 않음
                UnitState = E_UNIT_STATE.Idle;
                SetAnimation(UnitState);
                transform.position = new Vector3(nextPos.x, 0, nextPos.z);
            }
        }
    }

    public void Update_GoalPixel(Battle_MapCell cell,Battle_MapPixel goal)
    {
        // Test Show Object에서 Goal Pixel 관련 정보 갱신 ( 사용하는 경우만 )
        if (GoalPixel != null) cell.ShowPixelController.ShowPixel(GoalPixel, true, Color.green);

        GoalPixel = goal;

        cell.ShowPixelController.Refresh_ShowPixel(GoalPixel, true, Color.yellow);
    }


    void MoveAndLookAtTarget()
    {
        // 목표 위치를 향한 방향 벡터 계산
        Vector3 directionToTarget = nextPos - transform.position;

        // 수평으로만 회전하도록 Y값을 0으로 설정
        directionToTarget.y = 0;
        
        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        // 부드럽게 회전하도록 회전 속도와 Time.deltaTime 적용
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unitSpeed*1000 * Time.deltaTime);

        // 일정한 속도로 이동 (MoveTowards로 이동)
        transform.position = Vector3.MoveTowards(transform.position, nextPos, unitSpeed * Time.deltaTime);
    }


    public void OnSelect(bool select)
    {
        // 유닛 선택 마커 처리
        unitMarker.SetActive(select);
    }


    public void SetMoveState(Vector3 pos) 
    {
        // 유닛 이동 상태값 갱신 ( 이후 상태 머신으로 스크립트 분리 )

        prevPos = transform.position;       // 이전 이동 위치 저장
        nextPos = pos;                      // 다음 이동 위치 갱신  
        UnitState = E_UNIT_STATE.Move;      // 유닛 상태 갱신
   
        SetAnimation(UnitState);            // 애니메이션 갱신
    }
    private void SetAnimation(E_UNIT_STATE state)
    {
        // 애니메이션 상태 머신으로 교체하되 이후 서브 상태 머신으로 추가할것
        switch(state)
        {
            case E_UNIT_STATE.Idle:
                {
                    unitModel.PlayAnimation("Idle");
                }
                break;
            case E_UNIT_STATE.Move:
                {
                    unitModel.PlayAnimation("Move");
                }
                break;
        }
    }
}
