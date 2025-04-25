using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum E_PixelState
{
    NONE,                   // 0 : �ʱ�ȭ ���� ���� �ȼ�
    MOVE_ENABLE,            // 1 : �̵�����
    MOVE_DISENABLE_TYPE_1,  // 2 : ���� ��ֹ� 
    MOVE_DISENABLE_TYPE_2,  // 3 : ��ֹ�( ����, ���� )
}

public class Battle_MapPixel 
{
    public Vector2Int  PixelIndex;
    public Vector2     PixelPos;
    public T_ShowPixel ShowPixel;

    public Battle_MapPixel(Vector2Int pixelIndex, Vector2 pixelPos)
    {
        PixelIndex = pixelIndex; ;
        PixelPos = pixelPos;
    }
}
