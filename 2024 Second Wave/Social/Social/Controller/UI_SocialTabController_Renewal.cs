using UnityEngine;
using System.Collections.Generic;
using PB.ClientParts.Platform;
using PBRest.Contracts;

namespace PB.ClientParts
{
    public delegate void OnClickTabClickButtonEventHandler(eSocialTabItemType type);
    public class UI_SocialTabController_Renewal : UIComponent
    {
        [SerializeField] 
        private GameObject gameObjectSquadTab=null;
        [SerializeField] 
        private GameObject gameObjectSocialTabPC=null;
        [SerializeField] 
        private GameObject gameObjectSocialTabXbox=null;
        
        [SerializeField] 
        private UI_SocialTabItem_Renewal socialTabButtonSquad;
        [SerializeField] 
        private UI_SocialTabItem_Renewal socialTabButtonFriend;
        
        [SerializeField] 
        private UI_SocialTabItem_Renewal socialTabButtonXboxFriend;
        [SerializeField] 
        private UI_SocialTabItem_Renewal socialTabButtonSWFriend;
        
        [SerializeField] 
        private UI_ControllerBtn_Renewal leftControllerBtn = null;
        [SerializeField] 
        private UI_ControllerBtn_Renewal rightControllerBtn = null;
        
        [SerializeField] 
        private UI_ControllerBtn_Renewal consoleLeftControllerBtn = null;
        [SerializeField] 
        private UI_ControllerBtn_Renewal consoleRightControllerBtn = null;
        
        public void SetData(OnClickTabChangeEventHandler onClickTabChangeEventHandler)
        {
            SocialTabItemData socialTabItemSquadData = new SocialTabItemData();
            socialTabItemSquadData.onClickTabChangeEventHandler = onClickTabChangeEventHandler;
            socialTabItemSquadData.onClickTabClickButtonEventHandler = SelectSocialTabItem;
            socialTabItemSquadData.socialTabItemType = eSocialTabItemType.Squad;

            socialTabButtonSquad.SetData(socialTabItemSquadData);

            SocialTabItemData socialTabItemSocialData = new SocialTabItemData();
            socialTabItemSocialData.onClickTabChangeEventHandler = onClickTabChangeEventHandler;
            socialTabItemSocialData.onClickTabClickButtonEventHandler = SelectSocialTabItem;
            socialTabItemSocialData.socialTabItemType = eSocialTabItemType.Friend;

            socialTabButtonFriend.SetData(socialTabItemSocialData);
            
            SocialTabItemData consoleSocialTabItem_XboxFriend_Data = new SocialTabItemData();
            consoleSocialTabItem_XboxFriend_Data.onClickTabChangeEventHandler = onClickTabChangeEventHandler;
            consoleSocialTabItem_XboxFriend_Data.onClickTabClickButtonEventHandler = SelectSocialTabItem;
            consoleSocialTabItem_XboxFriend_Data.socialTabItemType = eSocialTabItemType.XboxFriend;

            socialTabButtonXboxFriend.SetData(consoleSocialTabItem_XboxFriend_Data);
            
            SocialTabItemData consoleSocialTabItem_SWFriend_Data = new SocialTabItemData();
            consoleSocialTabItem_SWFriend_Data.onClickTabChangeEventHandler = onClickTabChangeEventHandler;
            consoleSocialTabItem_SWFriend_Data.onClickTabClickButtonEventHandler = SelectSocialTabItem;
            consoleSocialTabItem_SWFriend_Data.socialTabItemType = eSocialTabItemType.SWFriend;

            socialTabButtonSWFriend.SetData(consoleSocialTabItem_SWFriend_Data);
            
            UIHandler.UIEvenHelper.AddOnReceiveSocialDataEventHandler(RefreshSocialCountData);
            
            SetControllerBtnData();
            OnDeviceChange();
            RefreshSocialCountData();
        }

        public override void OnClose()
        {
            UIHandler.UIEvenHelper.RemoveOnReceiveSocialDataEventHandler(RefreshSocialCountData);
            base.OnClose();
        }
        
        private void SetControllerBtnData()
        {
            leftControllerBtn.SetData(
                new UI_ControllerBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.LeftBumper},
                        { eSupportedDevice.PS , eInputControlType.LeftBumper},
                        { eSupportedDevice.KEYBOARD_MOUSE, eInputControlType.Q},
                    },
                    false,
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false },
                    }, eGameInputEvent.None));

            rightControllerBtn.SetData(
                new UI_ControllerBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.RightBumper},
                        { eSupportedDevice.PS , eInputControlType.RightBumper},
                        { eSupportedDevice.KEYBOARD_MOUSE ,eInputControlType.E},
                    },
                    false,
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false },
                    }, eGameInputEvent.None));
            consoleLeftControllerBtn.SetData(
                new UI_ControllerBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.LeftTrigger},
                        { eSupportedDevice.PS , eInputControlType.LeftTrigger},
                    },
                    false,
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false },
                    }, eGameInputEvent.None));

            consoleRightControllerBtn.SetData(
                new UI_ControllerBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.RightTrigger},
                        { eSupportedDevice.PS , eInputControlType.RightTrigger},
                    },
                    false,
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false },
                    }, eGameInputEvent.None));
        }
        
        public void SelectSocialTabItem(eSocialTabItemType type)
        {
            switch (type)
            {
                case eSocialTabItemType.Friend:
                {
                    socialTabButtonFriend.OnSelect();
                    socialTabButtonSquad.UnSelect();
                    // 엑스박스 환경일경우
                    if (PlatformManager.Instance.GetPlatformType() == PlatformType.Xbox)
                    {
                        gameObjectSocialTabXbox.SetActive(true);
                        gameObjectSocialTabPC.SetActive(false);
                        socialTabButtonXboxFriend.UnSelect();
                        socialTabButtonSWFriend.OnSelect();
                    }
                    else
                    {
                        gameObjectSocialTabPC.SetActive(true);
                    }
                    gameObjectSquadTab.SetActive(false);
                    break;
                }
                case eSocialTabItemType.Squad:
                {
                    gameObjectSocialTabXbox.SetActive(false);
                    gameObjectSocialTabPC.SetActive(false);
                    gameObjectSquadTab.SetActive(true);
                    socialTabButtonSquad.OnSelect();
                    socialTabButtonFriend.UnSelect();
                    break;
                }
                case eSocialTabItemType.XboxFriend:
                {
                    socialTabButtonSquad.UnSelect();
                    socialTabButtonXboxFriend.OnSelect();
                    socialTabButtonSWFriend.UnSelect();
                    break;
                }
                case eSocialTabItemType.SWFriend:
                {
                    socialTabButtonSquad.UnSelect();
                    socialTabButtonXboxFriend.UnSelect();
                    socialTabButtonSWFriend.OnSelect();
                    break;
                }
            }
        }
        public void ChangeLabel(eSocialTabItemType type)
        {
            switch (type)
            {
                case eSocialTabItemType.Squad:
                {
                    gameObjectSquadTab.SetActive(true);
                    gameObjectSocialTabPC.SetActive(false);
                    gameObjectSocialTabXbox.SetActive(false);
                    socialTabButtonSquad.ShowActiveButton(true);
                    socialTabButtonFriend.ShowActiveButton(false);
                    break;
                }
                case eSocialTabItemType.Friend:
                {
                    gameObjectSquadTab.SetActive(false);
                    socialTabButtonSquad.ShowActiveButton(false);
                    socialTabButtonFriend.ShowActiveButton(true);
                    if (PlatformManager.Instance.GetPlatformType() == PlatformType.Xbox)
                    {
                        gameObjectSocialTabPC.SetActive(false);
                        gameObjectSocialTabXbox.SetActive(true);
                    }
                    else
                    {
                        gameObjectSocialTabPC.SetActive(true);
                        gameObjectSocialTabXbox.SetActive(false);
                    }
                    break;
                }
            }
        }
        private void OnDeviceChange()
        {
            leftControllerBtn.ForceDeviceChange();
            rightControllerBtn.ForceDeviceChange();
            consoleLeftControllerBtn.ForceDeviceChange();
            consoleRightControllerBtn.ForceDeviceChange();
        }

        private void RefreshSocialCountData()
        {
            int squadCount = LobbyUserData.Instance.GetReceiveGroupsList().Count;
            int friendCount = LobbyUserData.Instance.GetReceiveFriendsList().Count;
            socialTabButtonSquad.RefreshSocialTabCount(squadCount);
            socialTabButtonFriend.RefreshSocialTabCount(friendCount);
        }
    }
}
