using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class UIPopupKickUserParam : UIPopupDefaultParam
    {
        public string kickUserName;
        
    }
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_KickUser")]
    public class UI_Popup_KickUser : UI_Popup_Default
    {
        [SerializeField]
        private Text nickNameText = null;
        
        public override void OnSetup(UIPopupBaseParam param)
        {
            base.OnSetup(param);
            if (param is UIPopupKickUserParam popupKickUserParam)
            {
                nickNameText.text = popupKickUserParam.kickUserName;
            }
        }
    }
}
