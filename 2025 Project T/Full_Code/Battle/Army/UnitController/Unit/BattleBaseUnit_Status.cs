using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BattleBaseUnit_Status 
{
    // 유닛 State 정보
    [SerializeField]private float UnitSpeed = 1.0f;
    [SerializeField] private float AttackRange = 5.0f;
    [SerializeField] private Battle_MapPixel ArmyCell_Pixel = null;
    [SerializeField] GameObject ArmyPixel = null;
    private BattleBaseUnit CurUnit = null;

    public SpawnObject ThrowObject = null;
    public Queue<SpawnObject> ObjectPool = new Queue<SpawnObject>();
    public BattleBaseUnit TargetUnit = null;
    public E_AttackType AttackType = E_AttackType.Hit_Animation;


    public void Init(Battle_MapPixel pixel, BattleBaseUnit curUnit)
    {
        ArmyCell_Pixel = pixel;
        MeshRenderer renderer = pixel.ShowPixel.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
        ArmyPixel = pixel.ShowPixel.gameObject;
        CurUnit = curUnit;
    }
    public void Update_Move(Transform unitTransform,Transform targetTransform, Action<E_UNIT_STATE,E_OrderType> UpdateState,E_UNIT_STATE state,E_OrderType orderType = E_OrderType.Auto)
    {
        if (targetTransform != null)
        {
            if (Vector3.Distance(targetTransform.position, unitTransform.position) > 0.1f)
            {
                // 1. 목표 방향을 계산
                Vector3 direction = (targetTransform.position - unitTransform.position).normalized;

                // 2. 현재 방향에서 목표 방향으로 부드럽게 회전
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    unitTransform.rotation = Quaternion.Slerp(unitTransform.rotation, targetRotation, 10 * Time.deltaTime);
                }
                // 3. 앞으로 이동
                unitTransform.position += unitTransform.forward * UnitSpeed * Time.deltaTime;

                UpdateState?.Invoke(E_UNIT_STATE.Move, orderType);
            }
            else
            {
                UpdateState?.Invoke(state,E_OrderType.Auto);
            }
        }
    }
    public void Update_Look(Transform unitTransform,Vector3 targetTransform)
    {
        // 1. 목표 방향을 계산
        Vector3 direction = (targetTransform - unitTransform.position).normalized;

        // 2. 현재 방향에서 목표 방향으로 부드럽게 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            unitTransform.rotation = Quaternion.Slerp(unitTransform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    public void OnDamage(float Damage)
    {
        // 공방 처리

        Debug.Log("크아악");
    }
    public float GetAttackRage() { return AttackRange; }
    public BattleBaseUnit GetCurUnit() { return CurUnit; }
    public Battle_MapPixel GetUnitPixel() { return ArmyCell_Pixel; }
}
