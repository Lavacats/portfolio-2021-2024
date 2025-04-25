using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera_CameraMove 
{
    private float moveSpeed = 10f; // 카메라 이동 속도
    private float boundaryMargin = 10f; // 화면 끝에서 카메라가 이동하기 시작할 마진
    public void Update(Camera mainCamera)
    {
        // 방향키 입력을 받아 카메라를 이동
        float horizontal = 0f;
        float vertical = 0f;

        // 방향키 입력에 따라 수평 이동
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
        }

        // 방향키 입력에 따라 수직 이동
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            vertical = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            vertical = -1f;
        }

        // 이동 벡터
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // 카메라를 이동시킴
        mainCamera.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
