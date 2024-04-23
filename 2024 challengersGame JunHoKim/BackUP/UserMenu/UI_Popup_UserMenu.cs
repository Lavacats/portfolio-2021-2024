using System.Collections.Generic;
using PB.ClientParts.Platform;
using PBRest.Contracts;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PB.ClientParts
{
    public class UIPopupUserMenuParam : UIPopupBaseParam
    {
        public RectTransform targetRectTransform;
        public UserData targetUserData;
        public bool isMe;
        public bool isOffline = true;
        public eTabType tabType = eTabType.None;
        public eSceneType sceneType = eSceneType.None;
        
        public bool isFriend = false;
        public bool isInviteFriend = false;
        public bool isConsiderFriend = false;
        
        public bool isGroupMember = false;
        // 내가 그룹 리더일 때
        public bool isGroupLeader = false;
        public bool isInInviteGroup = false;
        public bool isInConsiderGroup = false;

        public bool isTeamState = false;
    }
    
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_UserMenu")]
    public class UI_Popup_UserMenu : UIPopupBase
    {
         [SerializeField]
        private UI_UserMenuController userMenuController = null;
        [SerializeField]
        private RectTransform userPopupContainer = null;
        [SerializeField]
        private RectTransform dimmedImg = null;
        [SerializeField] 
        private UI_GuideBtnController_Renewal guideBtnController;
        [SerializeField]
        private Image bannerImg = null;
        [SerializeField]
        private Text userName = null;
        [SerializeField]
        private Text gamerTag = null;
        [SerializeField]
        private Image platformImg = null;
        [SerializeField]
        private Animation popupAnimation = null;
        [SerializeField]
        private AnimationClip popupStartAnimationClip = null;
        [SerializeField]
        private AnimationClip popupEndAnimationClip = null;

        private List<UI_GuideBtnData_Renewal> guideBtnConsoleList = null;
        private List<UI_GuideBtnData_Renewal> guideBtnPCList = null;
        
        private readonly string xBoxIconPath = "Ico_Common_Platform_Xbox";
        private readonly string etcIconPath = "Ico_Common_Platform_ETC";

        private bool isScoreViewState = false;

        public override void OnSetup(UIPopupBaseParam param)
        {
            base.OnSetup(param);
          
            SetGuideBtnData();
      
            if (param is UIPopupUserMenuParam popupParam)
            {
                SetData(popupParam);    
            }
            SetEnter();
        }
        public override bool OnInputEvent(eInputControlType inputControlType, bool isPressed, Vector2 movement)
        {
            if (isPressed)
            {
                switch (inputControlType)
                {
                    case eInputControlType.DPadDown:
                    case eInputControlType.DPadUp:
                    case eInputControlType.South:
                    {
                        userMenuController.InputListSelector.OnInputEvent(inputControlType.ChangeInputListSelectorType());
                        if(inputControlType == eInputControlType.DPadDown ||
                           inputControlType == eInputControlType.DPadUp)
                        {
                            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.FOCUS_LIST);
                        }
                        else if (inputControlType == eInputControlType.South)
                        {
                            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.SELECT);
                        }
                    }
                        break;
                    case eInputControlType.East:
                    case eInputControlType.Escape:
                    {
                        userMenuController.OnClose();
                        SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.BACK);
                        OnClickCloseButton();
                        break;
                    }
                }
            }
            return false;
        }
        private void SetGuideBtnData()
        {
            guideBtnConsoleList = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.South },
                        { eSupportedDevice.PS, eInputControlType.South },
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                        { eSupportedDevice.IOS, false },
                    }, eGameInputEvent.None, false, "UI_BUTTON_SELECT", null, null),
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.East },
                        { eSupportedDevice.PS, eInputControlType.East },
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                        { eSupportedDevice.IOS, false },
                    }, eGameInputEvent.None, false, "UI_BUTTON_CLOSE", null, null),
            };
            guideBtnPCList = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE , eInputControlType.MouseLeftClick},
                    }, new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false},
                        { eSupportedDevice.PS, false },
                        { eSupportedDevice.XBOX, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_SELECT", null, OnClickSelectButton),
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE , eInputControlType.MouseLeftClick},
                    }, new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false},
                        { eSupportedDevice.PS, false },
                        { eSupportedDevice.XBOX, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_CLOSE", null, OnClickCloseButton),
            };
        }

        #region User Menu Data Setting

         public void SetData(UIPopupUserMenuParam param)
        {
            if (param.tabType == eTabType.Chatting ||
                param.tabType == eTabType.ScoreView ||
                param.tabType == eTabType.InGameView)
            {
                isScoreViewState = true;
            }
             
            userMenuController.SetData(
                param.targetUserData,
                new UI_UserMenuData(
                    param.isFriend,
                    param.isInviteFriend,
                    param.isConsiderFriend,
                    param.isGroupMember,
                    param.isGroupLeader,
                    param.isInConsiderGroup,
                    param.isTeamState), 
                OnClose, param.sceneType);
            
            // 자기 자신인지 체크
            if (param.isMe)
            {
                userMenuController.SetLocalPlayerMenu();
                CachedRectTransform.SetActive(true);
                userMenuController.SetInputListItem();
            }
            else
            {
                SetUserPopupProfile(param);
                SetUserMenuTabTypeData(param);
                SetUserMenuOffLineData(param);
                SetUserMenuFriendAndGroup(param);
            }
            
            userMenuController.SetInputListItem();
            
            if (IsActiveInHierarchy())
            {
                StartCoroutine(ShowMenuContainer());
            }
            userPopupContainer.position = param.targetRectTransform.position;
        }
        private void SetUserPopupProfile(UIPopupUserMenuParam param)
        {
            if (param.targetUserData != null)
            {
                userName.text = param.targetUserData.Name;
                if (PlatformManager.Instance.GetPlatformType() == PlatformType.Xbox)
                {
                    if (param.targetUserData.Type == (int)PlatformType.Xbox)
                    {
                        platformImg.sprite = ClientResourceManager.Instance.GetPlatformSprite(xBoxIconPath);
                    }
                    else
                    {
                        platformImg.sprite = ClientResourceManager.Instance.GetPlatformSprite(etcIconPath);
                    }

                    if (param.targetUserData is PlatformUserData platformUserData)
                    {
                        gamerTag.gameObject.SetActive(true);
                        gamerTag.text = platformUserData.PlatformName;
                    }
                    else
                    {
                        gamerTag.gameObject.SetActive(false);
                    }
                }
                else
                {
                    platformImg.SetActive(false);
                    gamerTag.gameObject.SetActive(false);
                }
                bannerImg.sprite = CollectionModule.GetCollectionItemIcon(param.targetUserData.EquipBanner);
                userMenuController.SetUserSocialData(param.targetUserData);
                userMenuController.SetIsTeam(param.isTeamState);
            }
        }
        private void SetUserMenuTabTypeData(UIPopupUserMenuParam param)
        {
            if (param.tabType == eTabType.PlatformFriend)
            {
                userMenuController.SetShowProfileMenu();
            }
            if (param.tabType ==eTabType.ScoreView)
            {
                userMenuController.SetScoreViewMenu(param.isFriend);
            }
            if (param.tabType != eTabType.InGameView)
            {
                userMenuController.SetFriendDefaultMenu();
            }
        }
        private void SetUserMenuOffLineData(UIPopupUserMenuParam param)
        {
            if (param.isOffline)
            {
                if (param.isFriend)
                {
                    userMenuController.SetDeleteFriendMenu();
                }

                if (param.isGroupLeader && param.isGroupMember)
                {
                    userMenuController.SetGroupLeaderMenuOffline();
                }
                userMenuController.SetOfflineMenu();
                CachedRectTransform.SetActive(true);
                
                //임시(베타 1차때 인게임에서 신고 차단 막기)
                if (param.tabType == eTabType.InGameView)
                {
                    userMenuController.SetFriendUserMenu(param.isFriend);
                }
                
                userMenuController.SetInputListItem();
            }
        }
        private void SetUserMenuFriendAndGroup(UIPopupUserMenuParam param)
        {
            if (param.isInviteFriend)
            {
                userMenuController.SetRequestInviteFriendMenu();
            }
            if (param.isInInviteGroup)
            {
                userMenuController.SetRequestInviteGroupMenu();
            }
            if (param.isConsiderFriend || param.isInConsiderGroup)
            {
                userMenuController.SetReceiveInviteMenu(param.isConsiderFriend, param.tabType);
            }
            
            if (param.isFriend)
            {
                userMenuController.SetMyFriendMenu();
            }
            else
            {
                if (!(param.isConsiderFriend || param.isInviteFriend))
                {
                    if (param.targetUserData != null)
                    {
                        if (param.targetUserData.Type == (byte)PlatformType.Xbox ||
                            param.targetUserData.Type == (byte)PlatformType.PlayStation)
                        {
                            if (param.targetUserData is PlatformUserData platformUserData)
                            {
                                if (!platformUserData.IsSWPlay)
                                {
                                    userMenuController.SetNotMyFriendMenu();
                                }
                            }
                        }
                        else
                        {
                            userMenuController.SetNotMyFriendMenu();
                        }
                    }
                }
            }
            
            if (!param.isGroupMember && !param.isInInviteGroup)
            {
                if (param.tabType != eTabType.Group)
                {
                    userMenuController.SetNotGroupMemberMenu();   
                }
            }

            if (param.isGroupLeader && param.isGroupMember)
            {
                userMenuController.SetGroupLeaderMenu();
            }
        }
        
        #endregion
        
        public void SetEnter()
        {
            InputHandler.AddEventListener(this);
            OnDeviceChange();
        }
        private IEnumerator ShowMenuContainer()
        {
            yield return null;
            userPopupContainer.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(userPopupContainer);
            dimmedImg.SetActive(true);
            //showHideAnimation.Play(showAnimationName);
        }
        private void OnClickSelectButton()
        {
            
        }
        private void OnClickCloseButton()
        {
            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.BACK);
            InputHandler.RemoveEventListener(this);
            UIHandler.Instance.RemoveUI(this);
        }
        private void OnDeviceChange()
        {
            switch (GameSettings.CurrentDevice)
            {
                case eSupportedDevice.PS:
                case eSupportedDevice.XBOX:
                    guideBtnController.SetActive(true);
                    guideBtnController.SetData(guideBtnConsoleList);
                    break;
                case eSupportedDevice.KEYBOARD_MOUSE:
                case eSupportedDevice.ANDROID:
                case eSupportedDevice.IOS:
                    guideBtnController.SetActive(false);
                    break;
            }
        }
    }
}
