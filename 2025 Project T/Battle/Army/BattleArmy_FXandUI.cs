using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleArmy_FXandUI : MonoBehaviour
{
    [SerializeField] UI_Unit_Icon Origin_UnitIcon;
    [SerializeField] UI_DamageText DamageText;
    [SerializeField] Sprite IconImage;

    private UI_Unit_Icon UnitIcon = null;
    private UI_DamageText D_Text = null;
    private string ArmyIdx = string.Empty;
    public void Init(string idx)
    {
        UnitIcon = Instantiate(Origin_UnitIcon, Vector3.zero, Quaternion.identity);
        UnitIcon.transform.gameObject.SetActive(true);
        UnitIcon.transform.SetParent(Origin_UnitIcon.transform.parent);

        if (DamageText != null)
        {
            D_Text = Instantiate(DamageText, Vector3.zero, Quaternion.identity);
            D_Text.transform.gameObject.SetActive(true);
            D_Text.transform.SetParent(DamageText.transform.parent);
        }
        ArmyIdx = idx;
        UnitIcon.Set_IconImage(IconImage, ArmyIdx); 
    }
    public void Update_Icon(Transform unit)
    {
        if(UnitIcon!=null && unit!=null)
        {
            Vector3 screenPos = BattleEngine_Manager.Instance.CameraController.GetMainCamera().WorldToScreenPoint(unit.position );
            UnitIcon.transform.position = screenPos;
      
        }
        if (DamageText != null && D_Text != null)
        {
            Vector3 screenPos = BattleEngine_Manager.Instance.CameraController.GetMainCamera().WorldToScreenPoint(unit.position + new Vector3(0,1.7f,0));
            D_Text.transform.position = screenPos;

            BattleArmy Army = ArmyDataManager.Instance.GetBattleArmy(ArmyIdx);
            D_Text.GetText().text = Army.GetBattleArmyBattleData().HealthPoint + "/" + Army.GetBattleArmyBattleData().HealthMaxPoint;
        }
    }

    public Sprite GetIconImage() { return IconImage; }
}
