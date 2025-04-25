using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCemera_UnitController 
{
    private Camera mainCamara;
    private RectTransform dragRectangle;
    private Rect selectionRect;
    private Vector2 dragStartPos = Vector2.zero;
    private Vector2 dragEndPos = Vector2.zero;

    public void Apply(Camera mainCamara)
    {
        this.mainCamara = mainCamara;
        selectionRect = new Rect();
        Load_DragRectangle();
    }
    private void Load_DragRectangle()
    {
        GameObject loadPrefab = Resources.Load<GameObject>("Prefabs/DragRect");
        GameObject instanceObject= GameObject.Instantiate(loadPrefab, Vector3.zero, Quaternion.identity);

        dragRectangle = instanceObject.GetComponent<RectTransform>();
        Canvas mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        dragRectangle.SetParent(mainCanvas.transform);
    }
    public void OnMouseLeftDown(Vector3 mousePos)
    {
        dragStartPos = mousePos;
        dragRectangle.gameObject.SetActive(true);
        OnPointLeftDown(mousePos);
    }
    public void OnMouseLeftUp(Vector3 mousePos)
    {
        dragEndPos = mousePos;
        DrawDragRectangle();
        dragRectangle.gameObject.SetActive(false);
        CalculateDragRect();
        UnitDataManager.Instance.SelectUnit_Camera(selectionRect, mainCamara);
    }
    public void OnLeftMouse(Vector3 mousePos)
    {
        dragEndPos = mousePos;
        DrawDragRectangle();
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
                BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, hit.point);


                // 유닛 무버 개선.
                UnitDataManager.Instance.UnitMoveOrder(hit.point);
            }
        }

       // UnitDataManager.Instance.UnitMoveOrder(mousePos);
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
    private void DrawDragRectangle()
    {
        // 드래그 범위를 나타내는 Image UI의 위치
        dragRectangle.position = (dragStartPos + dragEndPos) * 0.5f;
        // 드래그 범위를 나타내는 Image UI의 크기
        dragRectangle.sizeDelta = new Vector2(Mathf.Abs(dragStartPos.x - dragEndPos.x), Mathf.Abs(dragStartPos.y - dragEndPos.y));
    }
    private void CalculateDragRect()
    {
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
