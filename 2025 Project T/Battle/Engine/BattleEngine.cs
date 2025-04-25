using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ó��, ���� ���� ����, ����/��ų ������ ���� ó���ϴ� Engine ��ũ��Ʈ
/// Event ȣ�� �Ǵ� ���� ȣ��� ������ ������ �ϰ� �� ��ũ��Ʈ�� ����
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
        // ���� �̵� ó�� ���
        Vector3 pointDown = (Vector3)value;

        // [1] ���콺 Ŭ�� ��ġ�� �� ��ǥ�� ��ȯ
        Battle_MapPixel pixel = BattleEngine_Manager.Instance.MapDirector.GetPixel(pointDown);
        Battle_MapCell cell = BattleEngine_Manager.Instance.MapDirector.GetCell(pointDown);

        // [2] ��ȿ�� ��ġ�� �ƴ� ��� ó�� �ߴ�
        if (pixel == null || cell == null) return;

        // [3] ���� ���õ� ���� ����Ʈ ��ȸ
        foreach (var selectUnit in UnitDataManager.Instance.GetCameraSelectUnit())
        {
            // [4] �̵� ��ǥ ��ġ ����
            selectUnit.Update_GoalPixel(cell, pixel);

            // [5] �̵� ���·� ��ȯ
            selectUnit.SetMoveState(pointDown);

            // [6] ��� Ž�� ��û
            BattleEngine_Manager.Instance.Pathfinder.GetUnitPathFinder(selectUnit.CurPixel, pixel);
        }
    }
}
