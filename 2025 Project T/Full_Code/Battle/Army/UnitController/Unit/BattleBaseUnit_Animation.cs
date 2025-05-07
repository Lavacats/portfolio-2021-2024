using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleBaseUnit_Animation
{
    [SerializeField] private BaseModel unitModel;       // À¯´Ö ÇÁ¸®ÆÕ ¸ðµ¨

    public void Init(Action hitEvent)
    {
        unitModel.Init(hitEvent);
    }
    public void Update()
    {

    }
    public void Set_StateAnimation(E_UNIT_STATE state)
    {
        switch (state)
        {
            case E_UNIT_STATE.Idle:
                {
                    unitModel.PlayAnimation("Idle");
                }
                break;
            case E_UNIT_STATE.Move:
                {
                    unitModel.PlayAnimation("Move");
                }
                break;
            case E_UNIT_STATE.Attack:
                {
                    unitModel.PlayAnimation("Attack");
                }
                break;
            case E_UNIT_STATE.SKILL:
                {
                    unitModel.PlayAnimation("Skill");
                    unitModel.GetAnimator().updateMode = AnimatorUpdateMode.UnscaledTime;
                    
                }
                break;
        }
    }
    public BaseModel GetBaseModel() { return unitModel; }
}
