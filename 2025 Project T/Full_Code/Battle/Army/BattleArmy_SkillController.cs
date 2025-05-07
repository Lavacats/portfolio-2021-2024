using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BattleArmy_SkillController 
{
    [SerializeField] UI_SkillUI skillUI;



    public void Init(List<BattleArmy> armyList)
    {
        if (armyList.Count != 3) return;
        skillUI.Init(armyList);

        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, OnEvent_SkillEvent);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, OnEvent_SkillEvent_End);
    }
    public void OnDestroy()
    {
        skillUI.OnDestroy();
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, OnEvent_SkillEvent);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, OnEvent_SkillEvent_End);
    }

    private void OnEvent_SkillEvent(object value)
    {
        if (value == null) return;

        string armyIdx = (string)value;
        BattleArmy casterArmy = ArmyDataManager.Instance.GetBattleArmy(armyIdx);

        Transform chracter = casterArmy.GetBattleUnitController().GetHeroUnit().GetBattleUnitAnimation().GetBaseModel().transform;
        int layer = LayerMask.NameToLayer("Character");
        SetLayerRecursively(chracter, layer);


        Time.timeScale = 0;
        casterArmy.Update_ArmyState(E_ARMY_STATE.SKILL);
    }
    private void OnEvent_SkillEvent_End(object value)
    {
        string armyIdx = (string)value;
        Time.timeScale = 1;
        BattleArmy casterArmy = ArmyDataManager.Instance.GetBattleArmy(armyIdx);

        Transform chracter = casterArmy.GetBattleUnitController().GetHeroUnit().GetBattleUnitAnimation().GetBaseModel().transform;
        int layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(chracter, layer);
        ArmyDataManager.Instance.Engine.GetEngine_SKill().Skill_AfterEffect(armyIdx);

    }
    void SetLayerRecursively(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;

        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
}
