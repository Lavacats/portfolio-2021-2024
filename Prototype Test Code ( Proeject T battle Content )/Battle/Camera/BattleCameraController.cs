using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraController : BaseCameraController
{
    private BattleCemera_UnitController CameraUnitController = new BattleCemera_UnitController();
    private BattleCamera_CameraMove CameraMove = new BattleCamera_CameraMove();
    private void Start()
    {
        CameraUnitController.Apply(mainCamera);
        Battle_MapDataManager.Instance.MapCamaera = mainCamera;
    }
    protected override void Update()
    {
       base.Update();
        CameraMove.Update(mainCamera);
    }

    public override void OnMouseLeftDown()
    {
        base.OnMouseLeftDown();

        CameraUnitController.OnMouseLeftDown(Input.mousePosition);
    }
    public override void OnMouseLeft()
    {
        base.OnMouseLeft();

        CameraUnitController.OnLeftMouse(Input.mousePosition);
    }
    public override void OnMouseLeftUp()
    {
        base.OnMouseLeftUp();

        CameraUnitController.OnMouseLeftUp(Input.mousePosition);
    }

    public override void OnMouseRightDown()
    {
        base.OnMouseRightDown();

        CameraUnitController.OnMouseRightDown(Input.mousePosition);
    }
    public override void OnMouseRight()
    {
        base.OnMouseRight();

        CameraUnitController.OnRightMouse(Input.mousePosition);
    }
    public override void OnMouseRightUp()
    {
        base.OnMouseRightUp();

        CameraUnitController.OnMouseRightUp(Input.mousePosition);
    }


}
