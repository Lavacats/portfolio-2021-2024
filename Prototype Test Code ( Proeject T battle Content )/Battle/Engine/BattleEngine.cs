using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        Vector3 pointDown = (Vector3)value;


        Battle_MapPixel pixel = BattleEngine_Manager.Instance.MapDirector.GetPixel(pointDown);
        Battle_MapCell cell = BattleEngine_Manager.Instance.MapDirector.GetCell(pointDown);
        
        if (pixel == null || cell == null) return;
        
        foreach (var selectUnit in UnitDataManager.Instance.GetCameraSelectUnit())
        {
            selectUnit.Update_GoalPixel(cell, pixel);
        }
        if (UnitDataManager.Instance.GetCameraSelectUnit().Count == 1)
        {
            //UnitDataManager.Instance.GetCameraSelectUnit()[0].SetMovePos(pointDown);

            BattleEngine_Manager.Instance.Pathfinder.GetUnitPathFinder(UnitDataManager.Instance.GetCameraSelectUnit()[0].CurPixel, pixel);
        }
    }
}
