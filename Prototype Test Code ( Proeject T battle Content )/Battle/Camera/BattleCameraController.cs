using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ���Ǵ� ī�޶� ��ũ��Ʈ���� �뵵 ���� ������ �����ϴ� ��ũ��Ʈ
/// �⺻���� �Է� ó���� �����ϴ� BaseCameraController �� ��ӹ޾� �Ʒ� ī�޶� ó���� �����Ѵ�
///   * BattleCamera_UnitController : ī�޶� ���� ������ �������� üũ�ϴ� ��ũ��Ʈ
///   * BattleCamera_CameraMove : ���콺 �巡�׸� ���� ī�޶��� �������� �����ϴ� ��ũ��Ʈ
/// </summary>

public class BattleCameraController : BaseCameraController
{
    private BattleCamera_UnitController CameraUnitController = new BattleCamera_UnitController();
    private BattleCamera_CameraMove CameraMove = new BattleCamera_CameraMove();
    private void Start()
    {
        CameraUnitController.Apply(mainCamera); // ���ְ˻翡�� ���̱� ���� ����ī�޶� 
        Battle_MapDataManager.Instance.MapCamaera = mainCamera;
    }
    protected override void Update()
    {
       base.Update();
        CameraMove.Update(mainCamera);  // ī�޶� �̵� ����
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
