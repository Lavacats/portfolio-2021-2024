using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
public class BattleArmyCell : T_ShowCell
{
    private string ArmyIdx = string.Empty;
    private Vector3 PrePos = Vector3.zero;
    private Vector3 NextPos = Vector3.zero;

    public Test_BattleMap_ShowPixel ShowPixelController = new Test_BattleMap_ShowPixel();
    public Dictionary<Vector2, Battle_MapPixel> Dic_MapPixel = new Dictionary<Vector2, Battle_MapPixel>();
    public Dictionary<Vector2Int, Battle_MapPixel> Dic_IndexPixel = new Dictionary<Vector2Int, Battle_MapPixel>();

    private Action<E_ARMY_STATE> UpdateState;

    void Start()
    {
        PrePos = this.transform.position;
        NextPos = this.transform.position;
    }
    public void Cell_Update(E_ARMY_STATE sate)
    {
        if (sate == E_ARMY_STATE.Move)
        {
            if (Vector3.Distance(NextPos, this.transform.position) > 0.1f)
            {
                // 1. 목표 방향을 계산
                Vector3 direction = (NextPos - transform.position).normalized;

                // 2. 현재 방향에서 목표 방향으로 부드럽게 회전
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1000 * Time.deltaTime);
                }

                // 3. 앞으로 이동
                transform.position += transform.forward * 1.3f * Time.deltaTime;
            }
            else
            {
                UpdateState?.Invoke(E_ARMY_STATE.Idle);
            }
        }
    }
    public void Init(string idx, Action<E_ARMY_STATE> updateState)
    {
        ArmyIdx = idx;
        UpdateState = updateState;
        Battle_MapDataManager mpaData = Battle_MapDataManager.Instance;
        for (int i = 0; i < mpaData.PixelCountX; i++)
        {
            for (int j = 0; j < mpaData.PixelCountY; j++)
            {
                float posX = (i - mpaData.PixelCountX / 2f + 0.5f) * mpaData.PixelSIze + this.transform.position.x;
                float posZ = (j - mpaData.PixelCountY / 2f + 0.5f) * mpaData.PixelSIze + this.transform.position.z;

                Vector2Int pixelIndex = new Vector2Int(i, j);
                Vector2 pixelPos = new Vector2(posX, posZ);
                Vector3 pixelPos3 = new Vector3(posX, 0, posZ);

                Battle_MapPixel mapPixel = new Battle_MapPixel(pixelIndex, pixelPos);

                ShowPixelController.ADD_Pixel(pixelPos3, pixelIndex, Vector2Int.zero, this.transform, mapPixel, true, false);
                if (ShowPixelController.DIc_ShowPixel.ContainsKey(new Vector2(pixelPos3.x, pixelPos3.z)))
                {
                    mapPixel.ShowPixel = ShowPixelController.DIc_ShowPixel[new Vector2(pixelPos3.x, pixelPos3.z)];
                }

                Dic_IndexPixel[new Vector2Int(i, j)] = mapPixel;
                Dic_MapPixel[pixelPos] = mapPixel;
            }
        }
    }
    public void Update_NextPos(Vector3 pos)
    {
        PrePos = NextPos;
        NextPos = pos;
    }
    public string GetArmyIdx() { return ArmyIdx; }
}
