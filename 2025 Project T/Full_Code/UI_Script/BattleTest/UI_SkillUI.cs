using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillUI : MonoBehaviour
{
    [SerializeField] UI_Skill_Icon Skill_1;
    [SerializeField] UI_Skill_Icon Skill_2;
    [SerializeField] UI_Skill_Icon Skill_3;
    [SerializeField] Image Skill_BackGround;
    [SerializeField] Image Skill_Image;
    [SerializeField] GameObject CameraTexture;
    public void Init(List<BattleArmy> playerArmy)
    {
        Skill_1.Init(playerArmy[0]);
        Skill_2.Init(playerArmy[1]);
        Skill_3.Init(playerArmy[2]);

        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, OnEvent_SkillEvent);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, OnEvent_SkillEvent_End);
    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.AMRY_STATE_SKILL_START, OnEvent_SkillEvent);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.ARMY_STATE_SKILL_END, OnEvent_SkillEvent_End);
    }

    private void OnEvent_SkillEvent(object value)
    {
        if (value == null) return;
        string armyIdx = (string)value;
        BattleArmy casterArmy = ArmyDataManager.Instance.GetBattleArmy(armyIdx);
        Skill_Image.sprite = casterArmy.GetBattleArmyBattleData().SkillImage;
        Skill_BackGround.gameObject.SetActive(true);
        CameraTexture.SetActive(true);
    }
    private void OnEvent_SkillEvent_End(object value)
    {
        Skill_BackGround.gameObject.SetActive(false);
        CameraTexture.SetActive(false);
    }

}
