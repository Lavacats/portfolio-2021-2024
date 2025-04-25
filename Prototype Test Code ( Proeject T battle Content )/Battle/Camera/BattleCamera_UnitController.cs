using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera_UnitController 
{
    private Camera mainCamara;                      // 유닛 검사에 활용될 메인 카메라

    // Draw Rect 
    private RectTransform dragRectangle;            // 드래그를 통해 유닛을 검사할 Rect를 보여주기위한 transform
    private Rect selectionRect;                     // 실제 드래그에 사용되는 Rect
    private Vector2 dragStartPos = Vector2.zero;    // 드래그에서 Rect 시작값
    private Vector2 dragEndPos = Vector2.zero;      // 드래그에서 Rect 끝값

    public void Apply(Camera mainCamara)
    {
        this.mainCamara = mainCamara;
        selectionRect = new Rect();
        Load_DragRectangle();
    }
    private void Load_DragRectangle()
    {
        // 드래그에서 출력될 사각형 Rect Transform을 로드 

        GameObject loadPrefab = Resources.Load<GameObject>("Prefabs/DragRect");
        GameObject instanceObject= GameObject.Instantiate(loadPrefab, Vector3.zero, Quaternion.identity);

        dragRectangle = instanceObject.GetComponent<RectTransform>();
        Canvas mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        dragRectangle.SetParent(mainCanvas.transform);
    }

    #region MouseFunction
    public void OnMouseLeftDown(Vector3 mousePos)
    {
        // 드래그 시작 좌표 설정 및 Rect 표시 활성화
        dragStartPos = mousePos;
        dragRectangle.gameObject.SetActive(true);
        OnPointLeftDown(mousePos);
    }
    public void OnLeftMouse(Vector3 mousePos)
    {
        // Rect 표시 갱신
        dragEndPos = mousePos;
        DrawDragRectangle();
    }
    public void OnMouseLeftUp(Vector3 mousePos)
    {
        // Rect 그리기를 종료하고, 내부 유닛 검사
        dragEndPos = mousePos;
        DrawDragRectangle();
        dragRectangle.gameObject.SetActive(false);
        RefreshSelectRect();
        UnitDataManager.Instance.SelectUnit_Camera(selectionRect, mainCamara);
    }
    public void OnMouseRightDown(Vector3 mousePos)
    {
        RaycastHit hit;
        Ray ray = mainCamara.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 레이 생성

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // 레이캐스트를 쏴서 충돌 체크
        {
            GameObject hitObject = hit.collider.gameObject;
            MeshCollider meshCollider = hitObject.GetComponent<MeshCollider>(); // 특정 스크립트를 검사 (예: MyScript)

            if (meshCollider != null)
            {
                BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, hit.point); // MouseMoveDown 관련 이벤트 처리

                UnitDataManager.Instance.UnitMoveOrder(hit.point);
            }
        }
    }
    public void OnMouseRightUp(Vector3 mousePos)
    {

    }
    public void OnRightMouse(Vector3 mousePos)
    {

    }
    private void OnPointLeftDown(Vector3 mousePos)
    {
        //선택 유닛 해제
        UnitDataManager.Instance.AllDeSelectUnit();
        RaycastHit hit;
        Ray ray = mainCamara.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 레이 생성

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // 레이캐스트를 쏴서 충돌 체크
        {
            GameObject hitObject = hit.collider.gameObject;
            BattleBaseUnit battleUnit = hitObject.GetComponent<BattleBaseUnit>(); // 특정 스크립트를 검사 (예: MyScript)

            if (battleUnit != null)
            {
                battleUnit.OnSelect(true);
                UnitDataManager.Instance.SelectUnit(battleUnit);
            }
            else
            {
            }
        }
    }
    #endregion

    private void DrawDragRectangle()
    {
        // 드래그 범위를 나타내는 Image UI의 위치
        dragRectangle.position = (dragStartPos + dragEndPos) * 0.5f;
        // 드래그 범위를 나타내는 Image UI의 크기
        dragRectangle.sizeDelta = new Vector2(Mathf.Abs(dragStartPos.x - dragEndPos.x), Mathf.Abs(dragStartPos.y - dragEndPos.y));
    }
    private void RefreshSelectRect()
    {
        // Select Rect 갱신
        if (Input.mousePosition.x < dragStartPos.x)
        {
            selectionRect.xMin = Input.mousePosition.x;
            selectionRect.xMax = dragStartPos.x;
        }
        else
        {
            selectionRect.xMin = dragStartPos.x;
            selectionRect.xMax = Input.mousePosition.x;
        }

        if (Input.mousePosition.y < dragStartPos.y)
        {
            selectionRect.yMin = Input.mousePosition.y;
            selectionRect.yMax = dragStartPos.y;
        }
        else
        {
            selectionRect.yMin = dragStartPos.y;
            selectionRect.yMax = Input.mousePosition.y;
        }
    }

}
