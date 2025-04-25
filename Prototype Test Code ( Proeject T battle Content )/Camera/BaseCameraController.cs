using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCameraController : MonoBehaviour
{

    [SerializeField] protected Camera mainCamera;


    private void Start()
    {

    }
    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 Down
        {
            OnMouseLeftDown();
        }
        if (Input.GetMouseButton(0))   // 
        {
            OnMouseLeft();
        }
        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼 Up
        {
            OnMouseLeftUp();
        }
        if (Input.GetMouseButtonDown(1)) // 마우스 왼쪽 버튼 Down
        {
            OnMouseRightDown();
        }
        if (Input.GetMouseButton(1))   // 
        {
            OnMouseRight();
        }
        if (Input.GetMouseButtonUp(1)) // 마우스 왼쪽 버튼 Up
        {
            OnMouseRightUp();
        }
    }
    public virtual void OnMouseLeft()
    {

    }
    public virtual void OnMouseLeftDown()
    {
        
    }
    public virtual void OnMouseLeftUp()
    {

    }
    public virtual void OnMouseRight()
    {

    }
    public virtual void OnMouseRightDown()
    {

    }
    public virtual void OnMouseRightUp()
    {

    }
}
