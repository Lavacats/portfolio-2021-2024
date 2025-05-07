using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skill_Icon : MonoBehaviour
{
    [SerializeField] Image SkillImage;
    [SerializeField] Image SkillCoolTime;
    [SerializeField] Image Buff_Icon;
    [SerializeField] Text SkillCoolTime_Text;
    public string ArmyIdx = string.Empty;
    private BattleArmy Army = null;
    public void Init(BattleArmy army)
    {
        SkillImage.sprite = army.GetArmyUI().GetIconImage();
        ArmyIdx = army.GetArmyIdx();
        Army = army;
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.BUFF_START, OnEvent_BuffStart);
        BaseEventManager.Instance.AddEvent(BaseEventManager.EVENT_BASE.BUFF_END, OnEvent_BuffEnd);

    }
    public void OnDestroy()
    {
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.BUFF_START, OnEvent_BuffStart);
        if (BaseEventManager.Instance != null) BaseEventManager.Instance.RemoveEvent(BaseEventManager.EVENT_BASE.BUFF_END, OnEvent_BuffEnd);

    }
    public void OnClick_SkillButton()
    {
        Debug.Log("이름" + ArmyIdx);


        SkillCoolTime.gameObject.SetActive(true);
        StartCoroutine(CountdownCoroutine(Army.GetBattleArmyBattleData().SkillCoolTime));
        BaseEventManager.Instance.OnEvent(BaseEventManager.EVENT_BASE.ON_EVENT_SKILL, ArmyIdx);
    }
    public void OnEvent_BuffStart(object value)
    {
        if (value == null) return;

        string armyIdx = (string)value;

        if (armyIdx != ArmyIdx) return;
        Sprite bufficon = Army.GetBattleArmyBattleData().BuffIcon;
        Buff_Icon.sprite = bufficon;

        Buff_Icon.transform.parent.gameObject.SetActive(true);
    }
    public void OnEvent_BuffEnd(object value)
    {
        if (value == null) return;
        Buff_Icon.transform.parent.gameObject.SetActive(false);
    }
    IEnumerator CountdownCoroutine(float duration)
    {
        float timer = duration;

        while (timer > 0f)
        {
            SkillCoolTime_Text.text = Mathf.Ceil(timer).ToString();
            yield return null;  // 다음 프레임까지 대기
            timer -= Time.deltaTime;
        }
        SkillCoolTime.gameObject.SetActive(false);
        SkillCoolTime_Text.text = "0";
    }
}
