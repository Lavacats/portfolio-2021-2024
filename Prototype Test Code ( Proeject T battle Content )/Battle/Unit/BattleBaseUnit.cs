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
        
            // �������� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, new Vector3(unitPos.x, 0, unitPos.z)) < 0.1f)
            {
                // �������� �����ϸ� �� �̻� �̵����� ����
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
        // ��ǥ ��ġ�� ���� ���� ���� ���
        Vector3 directionToTarget = unitPos - transform.position;

        // �������θ� ȸ���ϵ��� Y���� 0���� ����
        directionToTarget.y = 0;

        // ��ǥ �������� ȸ��
        //if (directionToTarget.magnitude > 0.1f)  // ���������� �Ÿ��� ����� �ָ�
        {
            // ��ǥ ȸ�� ���
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            // �ε巴�� ȸ���ϵ��� ȸ�� �ӵ��� Time.deltaTime ����
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unitSpeed*1000 * Time.deltaTime);

            // ������ �ӵ��� �̵� (MoveTowards�� �̵�)
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
        // ���¸ӽ����� ��ü�ϵ� ���� ���� ���� �ӽ��� �߰��Ұ�
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
