using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class SocialConsoleFriendControllerData
    {
        public UI_GuideBtnController_Renewal guideBtnControllerForConsole;
        public OnSocialViewCloseEventHandler onSocialViewCloseEventHandler;
    }
    public class UI_SocialConsoleFriendController : UIComponent, IInputKeyEventListener
    {
        [SerializeField]
        private UI_Common_SocialBanner localPlayerInfoItem;
        [SerializeField] 
        private VerticalLayoutGroup layoutGroup;
        [SerializeField]
        private UI_SocialMenuContainer friendsInfoItem;
        [SerializeField]
        private InputListSelector inputListSelector = null;
        [SerializeField]
        private RectOffset padding;
        [SerializeField]
        private RectTransform contentRectTransform;

        
        private OnSocialViewCloseEventHandler onSocialViewCloseEventHandler;
        private InputListSelector currentInputListSelector = null;
        private UI_SocialMenuContainer currentInfoItem = null;
        private bool isSelectInfoItem = false;

        public bool IsTopListener => true;
        public bool IsBlockInputKeyEvent => false;
        
        private UI_GuideBtnController_Renewal guideBtnController;
        
        private List<UI_GuideBtnData_Renewal> guideBtnDataListForConsole = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> guideBtnDataListExpanded = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> guideBtnDataListCollapsed = new List<UI_GuideBtnData_Renewal>();
        
        
        public void SetData(SocialConsoleFriendControllerData controllerData)
         {
             SetEventHandler(); 
             InitGuideBtn();
             ChangeUserProfile();

             SetItemSizeData(friendsInfoItem,
                contentRectTransform.rect.size.y - (layoutGroup.spacing + friendsInfoItem.MinHeight+friendsInfoItem.contentPadding));

             if (controllerData != null)
             {
                 guideBtnController = controllerData.guideBtnControllerForConsole;
                 onSocialViewCloseEventHandler += controllerData.onSocialViewCloseEventHandler;
             }

             List<PlatformUserData> data = LobbyUserData.Instance.GetPlatformFriendsList(); // 플랫폼 친구 목록
              friendsInfoItem.SetData("UI_SOCIAL_FRIEND_SW",
                  ConvertFriendCount(data.Count, LobbyUserData.Instance.GetCountPlatformFriendsOnline()), OnToggleTabChanged,
                  data, "SYS_FRIEND_EMPTY_LIST", eTabType.PlatformFriend, true);
         
              SetInputListSelector();
              InitConsoleControl();
        }
        private void SetEventHandler()
        {
            UIHandler.UIEvenHelper.AddOnReceiveChangeUserStatusEventHandler(ChangeUserProfile);         //유저 프로필 갱신
            UIHandler.UIEvenHelper.AddOnUserCollectionEventHandler(ChangeUserProfile);                  //유저 프로필 갱신
            UIHandler.UIEvenHelper.AddOnUserAccountLevelUpEventHandler(OnUserAccountLevelUp);           //유저 레벨 갱신
            
            UIHandler.UIEvenHelper.AddOnReceivePlatformFriendListEventHandler(AddFriendRow);            //친구 목록 추가
            UIHandler.UIEvenHelper.AddOnReceiveUpdatePlatformFriendListEventHandler(UpdateFriendList);  //친구 목록 갱신
            UIHandler.UIEvenHelper.AddOnReceiveRemovePlatformFriendListEventHandler(RemoveFriendRow);   //친구 목록 삭제
            UIHandler.UIEvenHelper.AddOnReceiveInitPlatformFriendListEventHandler(InitFriendList);      //친구 목록 초기화
            UIHandler.UIEvenHelper.AddOnReceiveRemoveAllPlatformFriendEventHandler(RemoveAllFriendData);//친구 목록 모두 삭제
        }
        public bool OnInputEvent(eInputControlType type, bool isPressed, Vector2 movement)
        {
            if (isPressed)
            {
                switch (type)
                {
                    case eInputControlType.East:
                    {
                        OnClickCloseButton();
                        break;
                    }
                    case eInputControlType.DPadDown:
                    case eInputControlType.DPadUp:
                    case eInputControlType.LeftStickDown:
                    case eInputControlType.LeftStickUp:
                    case eInputControlType.South:
                    {
                        if (isSelectInfoItem && type == eInputControlType.South)
                        {
                            currentInfoItem.ActionCurrentItem();
                        }
                        else
                        {
                            currentInputListSelector.OnInputEvent(type.ChangeInputListSelectorType());
                             if (type == eInputControlType.South)
                            {
                                if (currentInputListSelector == friendsInfoItem.InputListSelector)
                                {
                                    if (friendsInfoItem.IsSelect)
                                    {
                                        guideBtnController.SetData(guideBtnDataListExpanded);
                                    }
                                    else
                                    {
                                        guideBtnController.SetData(guideBtnDataListCollapsed);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return false;
        }
        private void InitGuideBtn()
        {
            guideBtnDataListForConsole = new List<UI_GuideBtnData_Renewal>()
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
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_SELECT",null, null),
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
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_CLOSE", null,OnClickCloseButton)
            };
            // 열린 상태
            guideBtnDataListExpanded = new List<UI_GuideBtnData_Renewal>()
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
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_FOLD",null, null),
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
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_CLOSE",null, null),
            };
            
            // 접힌 상태
            guideBtnDataListCollapsed = new List<UI_GuideBtnData_Renewal>()
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
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_UNFOLD",null, null),
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
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_CLOSE",null, null),
            };
        }
        private void OnClickCloseButton()
        {
            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.BACK);
            onSocialViewCloseEventHandler?.Invoke();
        }
        private void InitConsoleControl()
        {
            if (currentInputListSelector is not null)
            {
                currentInputListSelector.Select(false);
            }
            currentInputListSelector = inputListSelector;
            currentInputListSelector.Initialize();
            isSelectInfoItem = false;
            friendsInfoItem.InitSelectItem();
            ForceUpdateHeight();
        }
        private void UpdateFriendList()
        {
            List<PlatformUserData> data = LobbyUserData.Instance.GetPlatformFriendsList();
            friendsInfoItem.UpdateData(ConvertFriendCount(data.Count, LobbyUserData.Instance.GetCountPlatformFriendsOnline()),
                LobbyUserData.Instance.GetUpdatePlatformFriendsList());
        }
        private void InitFriendList()
        {
            List<PlatformUserData> data = LobbyUserData.Instance.GetPlatformFriendsList();
            friendsInfoItem.InitData("UI_SOCIAL_FRIEND_SW",
                ConvertFriendCount(data.Count, LobbyUserData.Instance.GetCountPlatformFriendsOnline()), data);
        }

        public void RemoveEventHandler(OnSocialViewCloseEventHandler handler)
        {
            onSocialViewCloseEventHandler -= handler;
            
            RemoveAllFriendData();
            
            UIHandler.UIEvenHelper.RemoveOnReceiveChangeUserStatusEventHandler(ChangeUserProfile);          //유저 프로필 갱신
            UIHandler.UIEvenHelper.RemoveOnUserCollectionEventHandler(ChangeUserProfile);                   //유저 프로필 갱신
            UIHandler.UIEvenHelper.RemoveOnUserAccountLevelUpEventHandler(OnUserAccountLevelUp);            //유저 레벨 갱신
            
            UIHandler.UIEvenHelper.RemoveOnReceivePlatformFriendListEventHandler(AddFriendRow);             //친구 목록 추가
            UIHandler.UIEvenHelper.RemoveOnReceiveUpdatePlatformFriendListEventHandler(UpdateFriendList);   //친구 목록 갱신
            UIHandler.UIEvenHelper.RemoveOnReceiveRemovePlatformFriendListEventHandler(RemoveFriendRow);    //친구 목록 삭제
            UIHandler.UIEvenHelper.RemoveOnReceiveInitPlatformFriendListEventHandler(InitFriendList);       //친구 목록 초기화
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveAllPlatformFriendEventHandler(RemoveAllFriendData); //친구 목록 모두 삭제
        }
        private void ChangeUserProfile()
        {
            localPlayerInfoItem.SetData(LobbyUserData.Instance.GetUserSocialData());
        }
        private void OnUserAccountLevelUp(int level)
        {
            localPlayerInfoItem.UpdateLevel(level);
        }
        private void SetItemSizeData(UI_SocialMenuContainer item, float maxHeight)
        {
            item.contentMaxHeight = maxHeight;
            item.verticalPadding = padding.top + padding.bottom;
            item.contentPadding = layoutGroup.spacing;
        }
        private void OnEnable()
        {
            InitConsoleControl();
            
            if (currentInputListSelector is null)
            {
                currentInputListSelector = inputListSelector;
            }
            currentInputListSelector.Initialize();
        }
        private void OnDisable()
        {
            InputHandler.RemoveEventListener(this);
        }
        public void ForceUpdateHeight()
        {
            friendsInfoItem.ForceUpdateScrollHeight();
        }
        private void OnToggleTabChanged(GameObject go)
        {
            friendsInfoItem.OnValueChanged(go);
        }
        private string ConvertFriendCount(int total, int online = -1)
        {
            if (online < 0)
            {
                return $" {total.ToString()}";
            }

            return $" {online.ToString()}/{total.ToString()}";
        }

        public void Enter()
        {
            CachedGameObject.SetActive(true);
            if (LobbyUserData.Instance.IsMaxGroupReceiveCount())
            {
                UIPopupSystemMessageParam param = new UIPopupSystemMessageParam();
                param.messageInfo = new UILocalizedTextInfo("SYS_GROUP_RECEIVED_REQUEST_IS_FULL");
                param.sortingOrder = UIHandler.CalcSortingOrder(95);
                UIHandler.Instance.LoadUI<UI_Popup_SystemMessage>(param, null, true);
            }
            
            if (currentInputListSelector is null)
            {
                currentInputListSelector = friendsInfoItem.InputListSelector;
            }
            currentInputListSelector.Initialize();
            InitConsoleControl();

            ForceUpdateHeight();
        }
        
        #region Menu InputListSelector Function

         private void SetInputListSelector()
        {
            inputListSelector.ClearEventHandler();
            inputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, RefreshGuideBtnData);
            
            inputListSelector.Add(localPlayerInfoItem.CachedGameObject);
            
            friendsInfoItem.InputListSelector.ClearEventHandler();
            friendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuPlatFormFriend);
            friendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuPlatFormFriend);
        }
        private void RefreshGuideBtnData(bool isSelect)
        {
            currentInputListSelector.Select(false);
            currentInputListSelector = friendsInfoItem.InputListSelector;
            currentInputListSelector.Select(true);
            if (friendsInfoItem.IsSelect)
            {
                guideBtnController.SetData(guideBtnDataListExpanded);
            }
            else
            {
                guideBtnController.SetData(guideBtnDataListCollapsed);
            }
        }
        private void OnDownSocialMenuPlatFormFriend(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() > 0 && friendsInfoItem.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = friendsInfoItem;
                friendsInfoItem.MoveChild(true);
                guideBtnController.SetData(guideBtnDataListForConsole);
            }
            else if ((friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() <= 0) || !friendsInfoItem.IsSelect)
            {
                currentInputListSelector.Select(true);
            }
        }
        private void OnUpSocialMenuPlatFormFriend(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() > 0 && friendsInfoItem.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = friendsInfoItem;
                friendsInfoItem.MoveChild(false);
                guideBtnController.SetData(guideBtnDataListForConsole);
            }
            else if(friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() > 0 && !friendsInfoItem.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                friendsInfoItem.InitSelectItem();
                currentInputListSelector = friendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                if (friendsInfoItem.IsSelect)
                {
                    guideBtnController.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnController.SetData(guideBtnDataListCollapsed);
                }
            }
            else
            {
                isSelectInfoItem = false;
                friendsInfoItem.InitSelectItem();
                currentInputListSelector = inputListSelector;
                currentInputListSelector.Select(true);
           
                guideBtnController.SetData(guideBtnDataListForConsole);
            }
        }


        #endregion

        #region PlatformFriend Add Remove
        private void AddFriendRow(UserData data)
        {
            friendsInfoItem.AddData(
                ConvertFriendCount(LobbyUserData.Instance.GetPlatformFriendsList().Count,
                    LobbyUserData.Instance.GetCountPlatformFriendsOnline()), data);
        }
        private void RemoveFriendRow(ulong userId)
        {
            friendsInfoItem.RemoveData(
                ConvertFriendCount(LobbyUserData.Instance.GetPlatformFriendsList().Count, LobbyUserData.Instance.GetCountPlatformFriendsOnline()), userId);
        }
        private void RemoveAllFriendData()
        {
            friendsInfoItem.RemoveAllData(ConvertFriendCount(0, 0));
        }
        #endregion
    }
}