using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera_CameraMove 
{
    private float moveSpeed = 10f; // ī�޶� �̵� �ӵ�
    private float boundaryMargin = 10f; // ȭ�� ������ ī�޶� �̵��ϱ� ������ ����
    public void Update(Camera mainCamera)
    {
        // ����Ű �Է��� �޾� ī�޶� �̵�
        float horizontal = 0f;
        float vertical = 0f;

        // ����Ű �Է¿� ���� ���� �̵�
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
        }

        // ����Ű �Է¿� ���� ���� �̵�
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            vertical = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            vertical = -1f;
        }

        // �̵� ����
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // ī�޶� �̵���Ŵ
        mainCamera.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
