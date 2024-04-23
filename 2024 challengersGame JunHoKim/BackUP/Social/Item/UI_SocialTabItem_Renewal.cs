using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public enum eSocialTabItemType
    {
        Squad,
        Friend,
        XboxFriend,
        SWFriend,
    }
    public class SocialTabItemData
    {
        public OnClickTabChangeEventHandler onClickTabChangeEventHandler;
        public OnClickTabClickButtonEventHandler onClickTabClickButtonEventHandler;
        public eSocialTabItemType socialTabItemType;
    }
    public class UI_SocialTabItem_Renewal : UIComponent
    {
        [SerializeField] 
        private GameObject selectItemGameObject;
        [SerializeField] 
        private GameObject hoverItemGameObject;
        [SerializeField] 
        private GameObject redDotGameObject;
        [SerializeField] 
        private Text redDotText;

        private eSocialTabItemType socialTabItemType = eSocialTabItemType.Squad;
        private OnClickTabChangeEventHandler onClickTabChangeEventHandler;
        private OnClickTabClickButtonEventHandler onClickTabClickButtonEventHandler;
        public void SetData(SocialTabItemData data)
        {
            onClickTabChangeEventHandler = data.onClickTabChangeEventHandler;
            socialTabItemType = data.socialTabItemType;
            onClickTabClickButtonEventHandler = data.onClickTabClickButtonEventHandler;
        }
        
        protected override void OnClick()
        {
            base.OnClick();
            OnSelect();
            onClickTabClickButtonEventHandler?.Invoke(socialTabItemType);
        }

        protected override void OnHover(bool isHover)
        {
            hoverItemGameObject.SetActive(isHover);
        }
        public void RefreshSocialTabCount(int count)
        {
            if (count > 0)
            {
                redDotText.text = ConvertStringToCount(count);
                redDotGameObject.SetActive(true);
            }
            else
            {
                redDotGameObject.SetActive(false);
            }
        }
        public void OnSelect()
        {
            selectItemGameObject.SetActive(true);
            onClickTabChangeEventHandler?.Invoke(socialTabItemType);
        }

        public void ShowActiveButton(bool isActive)
        {
            selectItemGameObject.SetActive(isActive);
        }
        public void UnSelect()
        {
            selectItemGameObject.SetActive(false);
        }
        private string ConvertStringToCount(int total)
        {
            return $" {total.ToString()}";
        }
    }
}
