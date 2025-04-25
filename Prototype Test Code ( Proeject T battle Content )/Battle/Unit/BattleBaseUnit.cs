using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBaseUnit : MonoBehaviour
{
    [SerializeField] private BaseModel unitModel;
    [SerializeField] private GameObject unitMarker;


    public Battle_MapPixel PrePixel = null;
    public Battle_MapPixel CurPixel = null;
    public Battle_MapPixel GoalPixel = null;

    private E_UNIT_STATE UnitState = E_UNIT_STATE.None;
    private Vector3 unitPos = Vector3.zero;
    private Vector3 startPos = Vector3.zero;
    private float unitSpeed = 1.0f;

  
    private void Start()
    {
        UnitDataManager.Instance.AddBattleUnit(this);
        unitPos = transform.position;
    }

    private void Update()
    {
        if (UnitState == E_UNIT_STATE.Move)
        {
            MoveAndLookAtTarget();
        
            // 목적지에 도달했는지 확인
            if (Vector3.Distance(transform.position, new Vector3(unitPos.x, 0, unitPos.z)) < 0.1f)
            {
                // 목적지에 도달하면 더 이상 이동하지 않음
                UnitState = E_UNIT_STATE.Idle;
                SetAnimation(UnitState);
                transform.position = new Vector3(unitPos.x, 0, unitPos.z);
            }

            
        }
    }
    public void Update_GoalPixel(Battle_MapCell cell,Battle_MapPixel goal)
    {
        if (GoalPixel != null) cell.ShowPixelController.ShowPixel(GoalPixel, true, Color.green);

        GoalPixel = goal;


        cell.ShowPixelController.ShowPixelBlock(GoalPixel, true, Color.yellow);
    }
    void MoveAndLookAtTarget()
    {
        // 목표 위치를 향한 방향 벡터 계산
        Vector3 directionToTarget = unitPos - transform.position;

        // 수평으로만 회전하도록 Y값을 0으로 설정
        directionToTarget.y = 0;

        // 목표 방향으로 회전
        //if (directionToTarget.magnitude > 0.1f)  // 목적지와의 거리가 충분히 멀면
        {
            // 목표 회전 계산
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            // 부드럽게 회전하도록 회전 속도와 Time.deltaTime 적용
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unitSpeed*1000 * Time.deltaTime);

            // 일정한 속도로 이동 (MoveTowards로 이동)
            transform.position = Vector3.MoveTowards(transform.position, unitPos, unitSpeed * Time.deltaTime);
        }
    }
    public void OnSelect(bool select)
    {
        unitMarker.SetActive(select);
    }
    public void SetMovePos(Vector3 pos) 
    {
        startPos = transform.position;
        unitPos = pos;
        UnitState = E_UNIT_STATE.Move;
   
        SetAnimation(UnitState);
    }
    private void SetAnimation(E_UNIT_STATE state)
    {
        // 상태머신으로 교체하되 이후 서브 상태 머신으 추가할것
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
