using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투에서 사용되는 카메라 스크립트들을 용도 별로 구분해 관리하는 스크립트
/// 기본적인 입력 처리를 구분하는 BaseCameraController 를 상속받아 아래 카메라에 처리를 진행한다
///   * BattleCamera_UnitController : 카메라를 통해 유닛의 움직임을 체크하는 스크립트
///   * BattleCamera_CameraMove : 마우스 드래그를 통해 카메라의 움직임을 제어하는 스크립트
/// </summary>

public class BattleCameraController : BaseCameraController
{
    private BattleCamera_UnitController CameraUnitController = new BattleCamera_UnitController();
    private BattleCamera_CameraMove CameraMove = new BattleCamera_CameraMove();

    [SerializeField] Camera CharacterCamera;

    private void Start()
    {
        CameraUnitController.Apply(mainCamera); // 유닛검사에서 쓰이기 위한 메인카메라 
        Battle_MapDataManager.Instance.MapCamaera = mainCamera;
        BattleEngine_Manager.Instance.CameraController = this;
    }
    protected override void Update()
    {
       base.Update();
        CameraMove.Update(mainCamera);  // 카메라 이동 갱신
    }
    public void Init()
    {
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, OnEvent_SkillEvent);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, OnEvent_SkillEvent_End);
    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, OnEvent_SkillEvent);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, OnEvent_SkillEvent_End);
    }
    private void OnEvent_SkillEvent(object value)
    {
        if (value == null) return;
        CharacterCamera.transform.position=mainCamera.transform.position; 
        CharacterCamera.enabled = true;

    }
    private void OnEvent_SkillEvent_End(object value)
    {
        if (value == null) return;

        CharacterCamera.enabled = false;
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

    public Camera GetMainCamera() { return mainCamera; }
}
