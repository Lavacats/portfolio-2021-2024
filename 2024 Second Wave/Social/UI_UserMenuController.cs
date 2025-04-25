using System.Collections.Generic;
using PB.ClientParts.Platform;
using PB.ClientParts.Social;
using PBRest.Contracts;
using PBSocialServer.Contracts;
using UnityEngine;
using PB.ClientParts.Rest;

namespace PB.ClientParts
{
    public delegate void OnClosePopupEventHandler();
    public class UI_UserMenuData
    {
        public bool isFriend = false;
        public bool isInviteFriend = false;
        public bool isConsiderFriend = false;
        public bool isGroupMember = false;
        public bool isGroupLeader = false;
        public bool isInConsiderGroup = false;
        public bool isTeamState = false;

        public UI_UserMenuData(bool isFriend, bool isInviteFriend, bool isConsiderFriend,
            bool isGroupMember, bool isGroupLeader,bool isInConsiderGroup,bool isTeamState)
        {
            this.isFriend = isFriend;
            this.isInviteFriend = isInviteFriend;
            this.isConsiderFriend = isConsiderFriend;
            this.isGroupMember = isGroupMember;
            this.isGroupLeader = isGroupLeader;
            this.isInConsiderGroup = isInConsiderGroup;
            this.isTeamState = isTeamState;
        }
    }
    public class UI_UserMenuController : UIComponent
    {
        [SerializeField]
        private List<UI_UserMenuItem_Renewal> items = new List<UI_UserMenuItem_Renewal>();
        [SerializeField]
        private List<string> itemsString = new List<string>();
        [SerializeField]
        private InputListSelector inputListSelector = null;
        [SerializeField]
        private RectTransform separator;
        
        private OnClosePopupEventHandler onClosePopupEventHandler; 
        private UserData userMenuData;
        private UIPopupKickUserParam kickUserConfirmPopupParam = new UIPopupKickUserParam();
        private UI_SmallDefaultPopupParam deleteFriendConfirmPopupParam = new UI_SmallDefaultPopupParam();
        
        private PlatformType myPlatformType;
        private bool isReceiveFriend = false;
        private bool isGroup = false;
        private bool isTeam = false;
        private eSceneType openSceneType = eSceneType.None;
        
        public InputListSelector InputListSelector => inputListSelector;
        
        public void SetData(UserData userData, UI_UserMenuData data,OnClosePopupEventHandler onClosePopupEventHandler, eSceneType openSceneType = eSceneType.None)
        {
            this.onClosePopupEventHandler = onClosePopupEventHandler;
            userMenuData = userData;
            isGroup = data.isGroupMember;
            isTeam = data.isTeamState;
            isReceiveFriend = data.isConsiderFriend;
            this.openSceneType = openSceneType;
            SetUserPopupMenuItem();
            SetDeleteFriendConfirmPopupParam();
            SetKickUserConfirmPopup();
            SetInputListItem();
        }
        public override void OnClose()
        {
            onClosePopupEventHandler?.Invoke();
        }
        protected override void OnClick()
        {
            base.OnClick();
            onClosePopupEventHandler?.Invoke();
        }
        protected override void OnHover(bool isHover)
        {
          
        }
        public void SetInputListItem()
        {
            inputListSelector.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsActive())
                {
                    items[i].FocusVisible(false);
                    inputListSelector.Add(items[i].CachedGameObject);
                }
            }
        }
        private void SetUserPopupMenuItem()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].CachedGameObject.SetActive(false);
                int itemTypeIdx = (int)items[i].userMenuItemType;
                switch (items[i].userMenuItemType)
                {
                   
                    case eUserMenuType.CancelInvite:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickCancelFriendRequest);
                        break;
                    }
                    case eUserMenuType.Accept:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickAcceptInvite);
                        break;
                    }
                    case eUserMenuType.Refuse:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickRefuseInvite);
                        break;
                    }
                    case eUserMenuType.InviteSquad:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickInviteSquad);
                        break;
                    }
                    case eUserMenuType.InviteFriend:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickFriendRequest);
                        break;
                    }
                    case eUserMenuType.ChangeGroupLeader:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickChangeGroupLeader);
                        break;
                    }
                    case eUserMenuType.KickGroup:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickKickGroupMember);
                        break;
                    }
                    case eUserMenuType.Whisper:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickWhisper);
                        break;
                    }
                    case eUserMenuType.ViewProfile:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickShowProfile);
                        break;
                    }
                    case eUserMenuType.DeleteFriend:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickDeleteFriend);
                        break;
                    }
                    case eUserMenuType.Report:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickReportUser);
                        break;
                    }
                    case eUserMenuType.CutOff:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickBlockUser);
                        break;
                    }
                    case eUserMenuType.CancelSquadInvite:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickCancelInviteGroup);
                        break;
                    }
                    case eUserMenuType.BlockList:
                    {
                        items[itemTypeIdx].SetData(itemsString[itemTypeIdx], OnClickShowBlockMember);
                        break;
                    }
                }
            }
        }
       
        #region Kick And Delete User Set
        
        private void SetKickUserConfirmPopup()
        {
            kickUserConfirmPopupParam.cancelBtnData =
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.East},
                        { eSupportedDevice.PS , eInputControlType.East},
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.IOS, false },
                        { eSupportedDevice.ANDROID, false },
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_CANCEL", null, OnCloseKickUserConfirmPopup);
                 
            kickUserConfirmPopupParam.confirmBtnData = 
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.South},
                        { eSupportedDevice.PS , eInputControlType.South},
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.IOS, false },
                        { eSupportedDevice.ANDROID, false },
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_KICKUSER", null, OnClickKickGroupMemberConfirm);
            
            kickUserConfirmPopupParam.titleTextInfo = new UILocalizedTextInfo("UI_PLAYERMENU_KICK");
            kickUserConfirmPopupParam.descTextInfo = new UILocalizedTextInfo("SYS_GROUP_GROUPMEMBER_KICK_CONFIRM");
            kickUserConfirmPopupParam.btnType = ePopupDefaultBtnType.YES_NO;
            kickUserConfirmPopupParam.confirmEventHandler = OnClickKickGroupMemberConfirm;
            kickUserConfirmPopupParam.cancelEventHandler = OnCloseKickUserConfirmPopup;
            kickUserConfirmPopupParam.IsSystemPopup = true;
        }
        private void SetDeleteFriendConfirmPopupParam()
        {
            deleteFriendConfirmPopupParam.cancelBtnData = new UI_GuideBtnData(
                new Dictionary<eSupportedDevice, eInputControlType>()
                {
                    { eSupportedDevice.XBOX , eInputControlType.East},
                    { eSupportedDevice.PS , eInputControlType.East},
                },false,
                "UI_BUTTON_CANCEL",
                new Dictionary<eSupportedDevice, bool>()
                {
                    { eSupportedDevice.KEYBOARD_MOUSE, false },
                    { eSupportedDevice.IOS, false },
                    { eSupportedDevice.ANDROID, false },
                },
                eGameInputEvent.None, OnCloseKickUserConfirmPopup);
            deleteFriendConfirmPopupParam.confirmBtnData = new UI_GuideBtnData(
                new Dictionary<eSupportedDevice, eInputControlType>()
                {
                    { eSupportedDevice.XBOX , eInputControlType.South},
                    { eSupportedDevice.PS , eInputControlType.South},
                },false, "UI_BUTTON_CONFIRM",
                new Dictionary<eSupportedDevice, bool>()
                {
                    { eSupportedDevice.KEYBOARD_MOUSE, false },
                    { eSupportedDevice.IOS, false },
                    { eSupportedDevice.ANDROID, false },
                },
                eGameInputEvent.None, OnClickDeleteFriendConfirm);
            deleteFriendConfirmPopupParam.titleTextInfo = new UILocalizedTextInfo("UI_PLAYERMENU_FRIEND_DELETE");
            deleteFriendConfirmPopupParam.descTextInfo =
                new UILocalizedTextInfo("SYS_FRIEND_DELETE_CONFIRM");
            deleteFriendConfirmPopupParam.descTextInfo2 = new UILocalizedTextInfo("");
            deleteFriendConfirmPopupParam.descTextInfo3= new UILocalizedTextInfo("");
            deleteFriendConfirmPopupParam.btnType = eDefaultPopupBtnType.Yes_No;
            deleteFriendConfirmPopupParam.confirmEventHandler = OnClickDeleteFriendConfirm;
            deleteFriendConfirmPopupParam.cancelEventHandler = OnCloseKickUserConfirmPopup;
            deleteFriendConfirmPopupParam.IsSystemPopup = true;
        }
        private void OnClickDeleteFriendConfirm()
        {
            SocialNetworkManager.Instance.ReqDeleteFriend(userMenuData.Id);
        }
        private void OnCloseKickUserConfirmPopup()
        {
            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.BACK);
        }
        private void OnClickKickGroupMemberConfirm()
        {
            SocialNetworkManager.Instance.ReqKickGroupMember(userMenuData.Id);
        }

        #endregion

        #region Set UserMenuData
        
        public void SetUserSocialData(UserData data)
        {
            userMenuData = data;
        }
        public void SetLocalPlayerMenu()
        {
            //todo 프로필 보기가 플랫폼 프로필 보기가 아니라 내 프로필 보기로 기획이 정해지면 작업할 것.
          
            items[(int)eUserMenuType.BlockList].SetActive(true);
        }
        public void SetShowProfileMenu()
        {
            if (myPlatformType == PlatformType.Xbox ||  myPlatformType == PlatformType.PlayStation)
            {
                if (userMenuData.Type == (int)myPlatformType)
                {
                    items[(int)eUserMenuType.ViewProfile].SetActive(true);    
                }
            }
        }
        public void SetOfflineMenu()
        {
            if (myPlatformType == PlatformType.Xbox ||  myPlatformType == PlatformType.PlayStation)
            {
                if (userMenuData.Type == (int)myPlatformType)
                {
                    items[(int)eUserMenuType.ViewProfile].SetActive(true);    
                }
            }
        }
        public void SetScoreViewMenu(bool isFriend)
        {
            if (isFriend)
            {
                items[(int)eUserMenuType.DeleteFriend].SetActive(true);
            }
            else
            {
                items[(int)eUserMenuType.InviteFriend].SetActive(true);
            }
            
            if (myPlatformType == PlatformType.Xbox ||  myPlatformType == PlatformType.PlayStation)
            {
                if (userMenuData.Type == (int)myPlatformType)
                {
                    items[(int)eUserMenuType.ViewProfile].SetActive(true);    
                }
            }
            
            items[(int)eUserMenuType.Report].SetActive(true);
            items[(int)eUserMenuType.CutOff].SetActive(true);
        }
        public void SetReceiveInviteMenu(bool isFriend, eTabType type)
        {
            isReceiveFriend = isFriend;
            items[(int)eUserMenuType.Accept].SetActive(true);
            items[(int)eUserMenuType.Refuse].SetActive(true);
        }

        // ===== Friend ======
        public void SetMyFriendMenu()
        {
            items[(int)eUserMenuType.Whisper].SetActive(true);
            SetDeleteFriendMenu();
        }
        public void SetNotMyFriendMenu()
        {
            items[(int)eUserMenuType.Whisper].SetActive(true);
        }
        public void SetDeleteFriendMenu()
        {
            items[(int)eUserMenuType.DeleteFriend].SetActive(true);
            SetFriendDefaultMenu();
        }
        public void SetFriendDefaultMenu()
        {
            if (myPlatformType == PlatformType.Xbox ||  myPlatformType == PlatformType.PlayStation)
            {
                if (userMenuData.Type == (int)myPlatformType)
                {
                    items[(int)eUserMenuType.ViewProfile].SetActive(true);    
                }
            }
            
            items[(int)eUserMenuType.Report].SetActive(true);
            items[(int)eUserMenuType.CutOff].SetActive(true);
        }
        public void SetRequestInviteFriendMenu()
        {
            items[(int)eUserMenuType.CancelInvite].SetActive(true);
        }
        public void SetFriendUserMenu(bool isFriend = false)
        {
            if (isFriend)
            {
                items[(int)eUserMenuType.DeleteFriend].SetActive(true);
            }
            else
            {
                items[(int)eUserMenuType.InviteFriend].SetActive(true);
            }
            items[(int)eUserMenuType.Report].SetActive(true);
            items[(int)eUserMenuType.CutOff].SetActive(true);
        }
        
        // ===== Squad ===== 
        public void SetIsTeam(bool isTeamCheck = false)
        {
            isTeam = isTeamCheck;
        }
        public void SetRequestInviteGroupMenu()
        {
            items[(int)eUserMenuType.CancelSquadInvite].SetActive(true);
        }
        public void SetGroupLeaderMenuOffline()
        {
            items[(int)eUserMenuType.KickGroup].SetActive(true);
        }
        public void SetNotGroupMemberMenu()
        {
            items[(int)eUserMenuType.InviteSquad].SetActive(true);
        }
        public void SetGroupLeaderMenu()
        {
            items[(int)eUserMenuType.ChangeGroupLeader].SetActive(true);
            SetGroupLeaderMenuOffline();
        }

        #endregion

        #region Click UserMenu Function
        private void OnClickCancelFriendRequest()
        {
            SocialNetworkManager.Instance.ReqCancelInviteFriend(userMenuData.Id);
            OnClose();
        }
        private void OnClickAcceptInvite()
        {
            if (isReceiveFriend)
            {
                SocialNetworkManager.Instance.ReqResponseInviteFriend(userMenuData.Id, true);
            }
            else
            {
                if (!LobbyUserData.Instance.IsSolo)
                {
                    UIPopupSystemMessageParam param = new UIPopupSystemMessageParam();
                    param.messageInfo = new UILocalizedTextInfo("SYS_GROUP_CANNOT_ACCEPT_ALREADY_IN_GROUP");
                    param.sortingOrder = UIHandler.CalcSortingOrder(95);
                    UIHandler.Instance.LoadUI<UI_Popup_SystemMessage>(param, null, true);
                }
                else
                {
                    if (userMenuData is ServerInviteUserData serverInviteUserData)
                    {
                        SocialNetworkManager.Instance.ReqServerInviteAccept(serverInviteUserData.Id, true, serverInviteUserData.ip, serverInviteUserData.port, serverInviteUserData.ToServer);
                    }
                    else
                    {
                        SocialNetworkManager.Instance.ReqResponseInviteGroup(userMenuData.Id, true);    
                    }
                }
            }
            OnClose();
        }
        private void OnClickRefuseInvite()
        {
            if (isReceiveFriend)
            {
                SocialNetworkManager.Instance.ReqResponseInviteFriend(userMenuData.Id, false);
            }
            else
            {
                SocialNetworkManager.Instance.ReqResponseInviteGroup(userMenuData.Id, false);
            }
            OnClose();
        }
        private void OnClickInviteSquad()
        {
            if (userMenuData.PlayState == (byte)eUserPlayState.Online)
            {
                if (PlatformManager.Instance.GetPlatformType() == PlatformType.Xbox && (int)PlatformManager.Instance.GetPlatformType() == userMenuData.Type)
                {
                    PlatformManager.Instance.PlatformInvitation(userMenuData);
                }
                else
                {
                    SocialNetworkManager.Instance.ReqInviteGroup(userMenuData.Id);
                }
                
                OnClose();
                return;
            }
            
            UIPopupSystemMessageParam param = new UIPopupSystemMessageParam();
            
            if (userMenuData.PlayState == (byte)eUserPlayState.Offline)//offline
            {
                param.messageInfo = new UILocalizedTextInfo("SYS_GROUP_CANNOT_INVITE_OFFLINE_USER");    
            }else
            {
                param.messageInfo = new UILocalizedTextInfo("SYS_GROUP_CANNOT_INVITE_TAGET_IS_PLAYING");
            }

            if (GamePlayUserDataManager.Instance.IsMatching)
            {
                param.messageInfo = new UILocalizedTextInfo("SYS_GROUP_CANNOT_INVITE_WHILE_MATCHING");
            }
         
            param.sortingOrder = UIHandler.CalcSortingOrder(95);
            UIHandler.Instance.LoadUI<UI_Popup_SystemMessage>(param, null, true);
            OnClose();
        }
        private void OnClickFriendRequest()
        {
            SocialNetworkManager.Instance.ReqFriendInvite(userMenuData.Name);
            OnClose();
        }
        private void OnClickChangeGroupLeader()
        {
            SocialNetworkManager.Instance.ReqChangeGroupLeader(userMenuData.Id);
            OnClose();
        }
        private void OnClickKickGroupMember()
        {
            kickUserConfirmPopupParam.descTextInfo =
                new UILocalizedTextInfo("SYS_GROUP_GROUPMEMBER_KICK_CONFIRM", userMenuData.Name);
            kickUserConfirmPopupParam.sortingOrder = UIHandler.CalcSortingOrder(80);
            UIHandler.Instance.LoadUI<UI_Popup_KickUser>(kickUserConfirmPopupParam, null,true);
            OnClose();
        }
        private void OnClickWhisper()
        {
            ChattingManager.Instance.OnOpenUserWhisperChatting(eChatType.ReceiveWhisper,userMenuData.Id);
           
            OnClose();
        }
        private void OnClickShowProfile()
        {
            if ((int)PlatformManager.Instance.GetPlatformType() == userMenuData.Type)
            {
                PlatformManager.Instance.PlatformRequestProfile(userMenuData.PlatformUniqueKey);
            }
            OnClose();
        }
        private void OnClickDeleteFriend()
        {
            deleteFriendConfirmPopupParam.descTextInfo =
                new UILocalizedTextInfo("SYS_FRIEND_DELETE_CONFIRM", userMenuData.Name);
            deleteFriendConfirmPopupParam.sortingOrder = UIHandler.CalcSortingOrder(80);
            UIHandler.Instance.LoadUI<UI_SmallDefaultPopup>(deleteFriendConfirmPopupParam, null);
            OnClose();
        }
        private void OnClickReportUser()
        {
            bool isUserInTeam = new bool();

            if (isGroup || this.isTeam)
            {
                isUserInTeam = true;
            }
            
            UIHandler.Instance.LoadUI<UI_Popup_UserReport>(new UIPopupUserReportParam()
            {
                IsSystemPopup = true,
                userName = userMenuData.Name,
                userId = userMenuData.Id,
                isTeamState = isTeam,
                openSceneType = this.openSceneType,
                closeEventHandler = OnClose,
                sortingOrder = UIHandler.CalcSortingOrder(72)
            }, null);
        }
        private void OnClickBlockUser()
        {
            UIHandler.Instance.LoadUI<UI_Popup_Block>(new UIPopupBlockParam()
            {
                IsSystemPopup = true,
                closeEventHandler = OnClose,
                kickUserName = userMenuData.Name,
                kickUserId = userMenuData.Id,
                sortingOrder = UIHandler.CalcSortingOrder(72),
            }, null,true);
        }
        private void OnClickCancelInviteGroup()
        {
            if (userMenuData is ServerInviteUserData serverInviteUserData)
            {
                SocialNetworkManager.Instance.ReqServerInviteAccept(userMenuData.Id, false);
            }
            else
            {
                SocialNetworkManager.Instance.ReqCancelInviteGroup(userMenuData.Id);
            }
            OnClose();
        }
        private void OnClickShowBlockMember()
        {
            ClientRestConnectManager.Instance.StartConnectCoroutine(RestAPI.ReqGetBlockUsers(resp =>
            {
                if (resp.ReturnCode == (int)PBRestReturnCode.SUCCESS)
                {
                    GamePlayUserDataManager.Instance.SetBlockUserInfoList(resp.BlockUsers);
                    
                    UIHandler.Instance.LoadUI<UI_Popup_BlockList>(new UIPopupBlockListParam()
                    {
                        IsSystemPopup = true,
                        closeEventHandler = null,
                        blockUserInfoList = resp.BlockUsers,
                        sortingOrder = UIHandler.CalcSortingOrder(70),
                    }, null,true);
                    OnClose();  
                }
                else
                {
                    CGLog.LogError($"[UI_LobbyDefaultView] Not SUCCESS ReqGetBlockUsers : {resp.ReturnCode}");
                    UIHandler.LoadServerErrorMessagePopup(resp.ReturnCode, null, UIHandler.CalcSortingOrder(95));
                    OnClose();  
                }
            }));
        }
        
        #endregion
    }
}  