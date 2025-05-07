using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BattleCamera_UnitController 
{
    private Camera mainCamara;                      // ���� �˻翡 Ȱ��� ���� ī�޶�

    // Draw Rect 
    private RectTransform dragRectangle;            // �巡�׸� ���� ������ �˻��� Rect�� �����ֱ����� transform
    private Rect selectionRect;                     // ���� �巡�׿� ���Ǵ� Rect
    private Vector2 dragStartPos = Vector2.zero;    // �巡�׿��� Rect ���۰�
    private Vector2 dragEndPos = Vector2.zero;      // �巡�׿��� Rect ����

    public void Apply(Camera mainCamara)
    {
        this.mainCamara = mainCamara;
        selectionRect = new Rect();
        Load_DragRectangle();
    }
    private void Load_DragRectangle()
    {
        // �巡�׿��� ��µ� �簢�� Rect Transform�� �ε� 

        GameObject loadPrefab = Resources.Load<GameObject>("Prefabs/DragRect");
        GameObject instanceObject= GameObject.Instantiate(loadPrefab, Vector3.zero, Quaternion.identity);

        dragRectangle = instanceObject.GetComponent<RectTransform>();
        Canvas mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        dragRectangle.SetParent(mainCanvas.transform);
    }

    #region MouseFunction
    public void OnMouseLeftDown(Vector3 mousePos)
    {
        // �巡�� ���� ��ǥ ���� �� Rect ǥ�� Ȱ��ȭ
        dragStartPos = mousePos;
        dragRectangle.gameObject.SetActive(true);
        OnPointLeftDown(mousePos);
    }
    public void OnLeftMouse(Vector3 mousePos)
    {
        // Rect ǥ�� ����
        dragEndPos = mousePos;
        DrawDragRectangle();
    }
    public void OnMouseLeftUp(Vector3 mousePos)
    {
        // Rect �׸��⸦ �����ϰ�, ���� ���� �˻�
        dragEndPos = mousePos;
        DrawDragRectangle();
        dragRectangle.gameObject.SetActive(false);
        RefreshSelectRect();
    }
    public void OnMouseRightDown(Vector3 mousePos)
    {
        //RaycastHit hit;
        Ray ray = mainCamara.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ���� ���� ����

        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.MOUSE_DOWN_RIGHT, ray); // MouseMoveDown ���� �̺�Ʈ ó��
    }
    public void OnMouseRightUp(Vector3 mousePos)
    {

    }
    public void OnRightMouse(Vector3 mousePos)
    {

    }
    private void OnPointLeftDown(Vector3 mousePos)
    {
        //���� ���� ����
        RaycastHit hit;
        Ray ray = mainCamara.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ���� ���� ����

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // ����ĳ��Ʈ�� ���� �浹 üũ
        {
            GameObject hitObject = hit.collider.gameObject;
            BattleArmyCell battleArmyCell = hitObject.GetComponent<BattleArmyCell>(); // Ư�� ��ũ��Ʈ�� �˻� (��: MyScript)

            BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.MOUSE_DESELECT_ARMY, null);

            if (battleArmyCell != null)
            {
                BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.ONSELECT_ARMY, battleArmyCell.GetArmyIdx());
            }
            else
            {
            }
        }
    }
    #endregion

    private void DrawDragRectangle()
    {
        // �巡�� ������ ��Ÿ���� Image UI�� ��ġ
        dragRectangle.position = (dragStartPos + dragEndPos) * 0.5f;
        // �巡�� ������ ��Ÿ���� Image UI�� ũ��
        dragRectangle.sizeDelta = new Vector2(Mathf.Abs(dragStartPos.x - dragEndPos.x), Mathf.Abs(dragStartPos.y - dragEndPos.y));
    }
    private void RefreshSelectRect()
    {
        // Select Rect ����
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
