using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public delegate void OnSelectedBlockItemEventHandler(UI_UserBlockItem_Renewal userBlockItem);
    public delegate void OnActionBlockItemEventHandler(UI_UserBlockItem_Renewal userBlockItem);
    public class UI_UserBlockItem_Renewal : UIComponent
    {
       [SerializeField]
        private UILocalizedText userNicNameText = null;
        [SerializeField]
        private UILocalizedText userNicNameTextSelect = null;
        [SerializeField]
        private Image hoverImage = null;
        [SerializeField]
        private Image choiceImage = null;
        
        private OnSelectedBlockItemEventHandler onSelectedBlockItemEventHandler;
        private OnActionBlockItemEventHandler onActionBlockItemEventHandler;

        private ulong userId = 0;
        public ulong UserId => userId;

        private int dataNumber = 0;
        public int DataNumber => dataNumber;

        public void SetData(int dataNumber, string nicName, ulong userId, OnActionBlockItemEventHandler actionBlockItemHandler = null, OnSelectedBlockItemEventHandler selectedBlockItemHandler = null )
        {
            userNicNameText.Text = nicName;
            userNicNameTextSelect.Text = nicName;
            this.dataNumber = dataNumber;
            this.userId = userId;
            onSelectedBlockItemEventHandler = selectedBlockItemHandler;
            onActionBlockItemEventHandler = actionBlockItemHandler;
            
            hoverImage.SetActive(false);
            choiceImage.SetActive(false);
        }

        protected override void OnSelect(bool isSelect)
        {
             base.OnSelect(isSelect);

            if (isSelect)
            {
                hoverImage.SetActive(true);
                if (GameSettings.CurrentDevice == eSupportedDevice.XBOX ||
                    GameSettings.CurrentDevice == eSupportedDevice.PS)
                {
                    SetEnableImage(isSelect);
                    onActionBlockItemEventHandler?.Invoke(this);
                }
            }
            else
            {
                hoverImage.SetActive(false);
                if (GameSettings.CurrentDevice == eSupportedDevice.XBOX ||
                    GameSettings.CurrentDevice == eSupportedDevice.PS)
                {
                    SetEnableImage(isSelect);
                }
            }
        }

        protected override void OnClick()
        {
            OnItemClick();
        }

        protected override void OnAction(GameObject go)
        {
            base.OnAction(go);
            
            OnItemClick();
        }
        public void SetEnableImage(bool isActive)
        {
            choiceImage.SetActive(isActive);
        }
        private void OnItemClick()
        {
            SetEnableImage(true);
            onActionBlockItemEventHandler?.Invoke(this);
        }
        public string GetUserNicName()
        {
            return userNicNameText.Text;
        }
    }
}