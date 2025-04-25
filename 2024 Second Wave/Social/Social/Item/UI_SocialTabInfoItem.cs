using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class UI_SocialTabInfoItem : UIComponent
    {
        [SerializeField]
        private UILocalizedText LocalizedText;
        [SerializeField]
        private Text CountText;
        [SerializeField]
        private Image arrowImg;

        public UIButton uiButton;
        public UIButton_Renewal uiButtonRenwal;

        public void SetData(string localKey, string countText)
        {
            LocalizedText.LocalKey = localKey;
            CountText.text = countText;
            uiButtonRenwal.SetHoverEventHandler(OnHover);
        }

        public void UpdateCountText(string countText)
        {
            CountText.text = countText;
        }

        protected override void OnHover(bool isHover)
        {
            base.OnHover(isHover);
        }
    }
}
