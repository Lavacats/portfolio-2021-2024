using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBaseUnit : MonoBehaviour
{
    [SerializeField] private BaseModel unitModel;       // ���� ������ ��
    [SerializeField] private GameObject unitMarker;     // ���� ǥ�ÿ� ��Ŀ

    // ���� ��ġ �̵� / ��ã�� �� Pxiel 
    public Battle_MapPixel PrePixel = null;
    public Battle_MapPixel CurPixel = null;
    public Battle_MapPixel GoalPixel = null;

    // ���� State ����      
    private E_UNIT_STATE UnitState = E_UNIT_STATE.None;     // Unit�� ���� ���� ����
    private Vector3 nextPos = Vector3.zero;                 // Unit�� �̵��� ��ġ
    private Vector3 prevPos = Vector3.zero;                 // Unit�� ���� �̵� ��ġ
    private float unitSpeed = 1.0f;

  
    private void Start()
    {
        UnitDataManager.Instance.AddBattleUnit(this);   // UnitManager�� ���
        nextPos = transform.position;                   // �̵���ġ�� �� ��ġ�� ����
    }

    private void Update()
    {
        // ���� ���¿� ���� ó�� ����
        // ���� ���� �ӽ��� ���� ������ �ڵ� �и��� ��.

        if (UnitState == E_UNIT_STATE.Move)            
        {
            MoveAndLookAtTarget();
        
            // �������� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, new Vector3(nextPos.x, 0, nextPos.z)) < 0.1f)
            {
                // �������� �����ϸ� �� �̻� �̵����� ����
                UnitState = E_UNIT_STATE.Idle;
                SetAnimation(UnitState);
                transform.position = new Vector3(nextPos.x, 0, nextPos.z);
            }
        }
    }

    public void Update_GoalPixel(Battle_MapCell cell,Battle_MapPixel goal)
    {
        // Test Show Object���� Goal Pixel ���� ���� ���� ( ����ϴ� ��츸 )
        if (GoalPixel != null) cell.ShowPixelController.ShowPixel(GoalPixel, true, Color.green);

        GoalPixel = goal;

        cell.ShowPixelController.Refresh_ShowPixel(GoalPixel, true, Color.yellow);
    }


    void MoveAndLookAtTarget()
    {
        // ��ǥ ��ġ�� ���� ���� ���� ���
        Vector3 directionToTarget = nextPos - transform.position;

        // �������θ� ȸ���ϵ��� Y���� 0���� ����
        directionToTarget.y = 0;
        
        // ��ǥ ȸ�� ���
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        // �ε巴�� ȸ���ϵ��� ȸ�� �ӵ��� Time.deltaTime ����
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unitSpeed*1000 * Time.deltaTime);

        // ������ �ӵ��� �̵� (MoveTowards�� �̵�)
        transform.position = Vector3.MoveTowards(transform.position, nextPos, unitSpeed * Time.deltaTime);
    }


    public void OnSelect(bool select)
    {
        // ���� ���� ��Ŀ ó��
        unitMarker.SetActive(select);
    }


    public void SetMoveState(Vector3 pos) 
    {
        // ���� �̵� ���°� ���� ( ���� ���� �ӽ����� ��ũ��Ʈ �и� )

        prevPos = transform.position;       // ���� �̵� ��ġ ����
        nextPos = pos;                      // ���� �̵� ��ġ ����  
        UnitState = E_UNIT_STATE.Move;      // ���� ���� ����
   
        SetAnimation(UnitState);            // �ִϸ��̼� ����
    }
    private void SetAnimation(E_UNIT_STATE state)
    {
        // �ִϸ��̼� ���� �ӽ����� ��ü�ϵ� ���� ���� ���� �ӽ����� �߰��Ұ�
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
