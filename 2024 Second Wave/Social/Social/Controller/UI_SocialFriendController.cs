using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class SocialFriendControllerData
    {
        public UI_GuideBtnController_Renewal guideBtnControllerForKeyboardMouse;
        public UI_GuideBtnController_Renewal guideBtnControllerForConsole;
        public OnSocialViewCloseEventHandler onSocialViewCloseEventHandler;
    }
    public class UI_SocialFriendController : UIComponent, IInputKeyEventListener
    {  
        [SerializeField]
        private UI_Common_SocialBanner localPlayerInfoItem;

        [SerializeField]
        private UI_SocialMenuContainer requestFriendsInfoItem;

        [SerializeField]
        private UI_SocialMenuContainer receiveFriendsInfoItem;

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

        private UI_GuideBtnController_Renewal guideBtnControllerForKeyboardMouse;
        private UI_GuideBtnController_Renewal guideBtnControllerForConsole;
        
        private List<UI_GuideBtnData_Renewal> guideBtnDataListForConsole = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> guideBtnDataListForKeyboardMouse = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> guideBtnDataListExpanded = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> guideBtnDataListCollapsed = new List<UI_GuideBtnData_Renewal>();

        private readonly string SOCIAL_FREIND_LOCALKEY = "UI_SOCIAL_FRIEND_SW";
        private readonly string SOCIAL_FREIND_REQUEST_SENT_LOCALKEY = "UI_FRIEND_REQUEST_SENT";
        private readonly string SOCIAL_FREIND_RECEIVE_REQUEST_LOCALKEY = "UI_FRIEND_REICIEVED_REQUEST";

        private InputListSelector currentInputListSelector = null;

        private UI_SocialMenuContainer currentInfoItem = null;
        private bool isSelectInfoItem = false;

        public bool IsTopListener => true;
        public bool IsBlockInputKeyEvent => false;
        
        public void SetData(SocialFriendControllerData controllerData)
        {
            SetEventHandler();
            InitGuideBtn();
            ChangeUserProfile();
            SetInputListSelector();
            
            float maxHeight = contentRectTransform.rect.size.y - 3 * (layoutGroup.spacing + requestFriendsInfoItem.MinHeight+requestFriendsInfoItem.contentPadding);
            
            SetItemSizeData(requestFriendsInfoItem, maxHeight);
            
            maxHeight = contentRectTransform.rect.size.y - 3 * (layoutGroup.spacing + receiveFriendsInfoItem.MinHeight+receiveFriendsInfoItem.contentPadding);

            SetItemSizeData(receiveFriendsInfoItem, maxHeight);
            
            maxHeight = contentRectTransform.rect.size.y - 3 * (layoutGroup.spacing + friendsInfoItem.MinHeight+friendsInfoItem.contentPadding);

            SetItemSizeData(friendsInfoItem, maxHeight);

            if (controllerData != null)
            {          
                onSocialViewCloseEventHandler += controllerData.onSocialViewCloseEventHandler;
                guideBtnControllerForKeyboardMouse = controllerData.guideBtnControllerForKeyboardMouse;
                guideBtnControllerForConsole = controllerData.guideBtnControllerForConsole;
            }
            
            List<UserData> data = LobbyUserData.Instance.GetRequestFriendsList();//보낸 친구 요청
            requestFriendsInfoItem.SetData(SOCIAL_FREIND_REQUEST_SENT_LOCALKEY, ConvertFriendCount(data.Count),
                OnToggleTabChanged, data, "SYS_FRIEND_EMPTY_LIST", eTabType.Friend); 
            
            data = LobbyUserData.Instance.GetReceiveFriendsList(); //받은 친구 요청
            receiveFriendsInfoItem.SetData(SOCIAL_FREIND_RECEIVE_REQUEST_LOCALKEY, ConvertFriendCount(data.Count),
                OnToggleTabChanged, data, "SYS_FRIEND_EMPTY_LIST", eTabType.Friend);
            
            data = LobbyUserData.Instance.GetFriendsList();//친구 목록
            friendsInfoItem.SetData(SOCIAL_FREIND_LOCALKEY,
                ConvertFriendCount(data.Count, LobbyUserData.Instance.GetCountFriendsOnline()), OnToggleTabChanged,
                data, "SYS_FRIEND_EMPTY_LIST", eTabType.Friend); 
        }

        public void SetEventHandler()
        {
            UIHandler.UIEvenHelper.AddOnReceiveChangeUserStatusEventHandler(ChangeUserProfile);             // 유저 프로필 갱신
            UIHandler.UIEvenHelper.AddOnUserCollectionEventHandler(ChangeUserProfile);                      // 유저 프로필 갱신
            UIHandler.UIEvenHelper.AddOnUserAccountLevelUpEventHandler(OnUserAccountLevelUp);               // 유저 프로필 레벨업
            
            UIHandler.UIEvenHelper.AddOnReceiveAllFriendListEventHandler(UpdateFriendList);                 // 친구 목록 갱신
       
            UIHandler.UIEvenHelper.AddOnReceiveFriendListEventHandler(AddFriendRow);                        // 친구 목록 추가
            UIHandler.UIEvenHelper.AddOnReceiveAddReceiveFriendListEventHandler(AddReceiveFriendRow);       // 초대받은 친구 추가
            UIHandler.UIEvenHelper.AddOnReceiveAddRequestFriendListEventHandler(AddRequestFriendRow);       // 초대한 친구 삭제
            
            UIHandler.UIEvenHelper.AddOnReceiveRemoveReceiveFriendListEventHandler(RemoveReceiveFriendRow); // 초대받은 친구 삭제
            UIHandler.UIEvenHelper.AddOnReceiveRemoveRequestFriendListEventHandler(RemoveRequestFriendRow); // 초대한 친구 삭제
            UIHandler.UIEvenHelper.AddOnReceiveRemoveFriendListEventHandler(RemoveFriendRow);               // 친구 목록 제거
            
            UIHandler.UIEvenHelper.AddOnReceiveInitFriendListEventHandler(InitSocialFriendData);            // 친구 데이터 초기화
            UIHandler.UIEvenHelper.AddOnReceiveRemoveAllFriendEventHandler(RemoveAllFriendData);            // 모든 데이터 삭제
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
                        SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.CLOSE);
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
                                if (currentInputListSelector == receiveFriendsInfoItem.InputListSelector)
                                {
                                    if (receiveFriendsInfoItem.IsSelect)
                                    {
                                        guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                                    }
                                    else
                                    {
                                        guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                                    }
                                }
                                else if (currentInputListSelector == requestFriendsInfoItem.InputListSelector)
                                {
                                    if (requestFriendsInfoItem.IsSelect)
                                    {
                                        guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                                    }
                                    else
                                    {
                                        guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                                    }
                                }
                                else if (currentInputListSelector == friendsInfoItem.InputListSelector)
                                {
                                    if (friendsInfoItem.IsSelect)
                                    {
                                        guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                                    }
                                    else
                                    {
                                        guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                                    }
                                }
                            }
                        }
                        
                        if (type == eInputControlType.South)
                        {
                            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.SELECT);
                        }
                        else
                        {
                            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.FOCUS_LIST);   
                        }
                        break;
                    }
                }
            }
            else
            {
                if (type == eInputControlType.North)
                {
                    OnClickAddFriendButton();
                    SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.OPEN);
                }
            }

            return false;
        }

        private void OnClickAddFriendButton()
        {
            UIHandler.Instance.LoadUI<UI_Popup_AddFriends>(new UIPopupAddFriendsParam()
            {
                IsSystemPopup = true,
                closeEventHandler = null,
                sortingOrder = UIHandler.CalcSortingOrder(70),
            }, null,true);
        }
        private void OnClickCloseButton()
        {
            onSocialViewCloseEventHandler?.Invoke();
        }
        public void ChangeGuidButtonList(SocialFriendControllerData controllerData)
        {
            if (controllerData != null)
            {
                onSocialViewCloseEventHandler += controllerData.onSocialViewCloseEventHandler;
                guideBtnControllerForKeyboardMouse = controllerData.guideBtnControllerForKeyboardMouse;
                guideBtnControllerForConsole = controllerData.guideBtnControllerForConsole;
            }
            OnDeviceChange();
        }
        private void OnDeviceChange()
        {
            InitConsoleControl();
            switch (GameSettings.CurrentDevice)
            {
                case eSupportedDevice.PS:
                case eSupportedDevice.XBOX:
                    currentInputListSelector.Select(true);
                    guideBtnControllerForConsole.SetActive(true);
                    guideBtnControllerForKeyboardMouse.SetActive(false);
                    guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
                    break;
                case eSupportedDevice.KEYBOARD_MOUSE:
                case eSupportedDevice.ANDROID:
                case eSupportedDevice.IOS:
                    currentInputListSelector.Select(false);
                    guideBtnControllerForConsole.SetActive(false);
                    guideBtnControllerForKeyboardMouse.SetActive(true);
                    guideBtnControllerForKeyboardMouse.SetData(guideBtnDataListForKeyboardMouse);
                    break;
            }
        }
        private void UpdateFriendList()
        {
            List<UserData> data = LobbyUserData.Instance.GetFriendsList();
            friendsInfoItem.UpdateData(ConvertFriendCount(data.Count, LobbyUserData.Instance.GetCountFriendsOnline()),
                LobbyUserData.Instance.GetUpdateFriendsList());
            data = LobbyUserData.Instance.GetReceiveFriendsList();
            receiveFriendsInfoItem.UpdateData(ConvertFriendCount(data.Count),
                LobbyUserData.Instance.GetUpdateReceiveFriendsList());
            data = LobbyUserData.Instance.GetRequestFriendsList();
            requestFriendsInfoItem.UpdateData(ConvertFriendCount(data.Count),
                LobbyUserData.Instance.GetUpdateRequestFriendsList());
            LobbyUserData.Instance.ClearUpdateFriendList();
        }
        
        public void RemoveEventHandler(OnSocialViewCloseEventHandler handler)
        {
            onSocialViewCloseEventHandler -= handler;
            
            RemoveAllFriendData();
            
            UIHandler.UIEvenHelper.RemoveOnReceiveChangeUserStatusEventHandler(ChangeUserProfile);          // 유저 프로필 갱신        
            UIHandler.UIEvenHelper.RemoveOnUserCollectionEventHandler(ChangeUserProfile);                   // 유저 프로필 갱신
            UIHandler.UIEvenHelper.RemoveOnUserAccountLevelUpEventHandler(OnUserAccountLevelUp);            // 유저 프로필 레벨업
    
            UIHandler.UIEvenHelper.RemoveOnReceiveAllFriendListEventHandler(UpdateFriendList);              // 친구 목록 갱신

            UIHandler.UIEvenHelper.RemoveOnReceiveFriendListEventHandler(AddFriendRow);                     // 친구 목록 추가
            UIHandler.UIEvenHelper.RemoveOnReceiveAddReceiveFriendListEventHandler(AddReceiveFriendRow);    // 초대받은 친구 추가
            UIHandler.UIEvenHelper.RemoveOnReceiveAddRequestFriendListEventHandler(AddRequestFriendRow);    // 초대한 친구 삭제
            
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveReceiveFriendListEventHandler(RemoveReceiveFriendRow);// 초대받은 친구 삭제
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveRequestFriendListEventHandler(RemoveRequestFriendRow);// 초대한 친구 삭제
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveFriendListEventHandler(RemoveFriendRow);            // 친구 목록 제거
            
            UIHandler.UIEvenHelper.RemoveOnReceiveInitFriendListEventHandler(InitSocialFriendData);         // 친구 데이터 초기화
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveAllFriendEventHandler(RemoveAllFriendData);         // 모든 데이터 삭제
        }
        private void InitSocialFriendData()
        {
            List<UserData> data = LobbyUserData.Instance.GetFriendsList();
            friendsInfoItem.InitData(SOCIAL_FREIND_LOCALKEY,
                ConvertFriendCount(data.Count, LobbyUserData.Instance.GetCountFriendsOnline()), data);
            data = LobbyUserData.Instance.GetReceiveFriendsList();
            receiveFriendsInfoItem.InitData(SOCIAL_FREIND_RECEIVE_REQUEST_LOCALKEY, ConvertFriendCount(data.Count), data);
            data = LobbyUserData.Instance.GetRequestFriendsList();
            requestFriendsInfoItem.InitData(SOCIAL_FREIND_REQUEST_SENT_LOCALKEY, ConvertFriendCount(data.Count), data);
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
            OnDeviceChange();
            
            if (currentInputListSelector is null)
            {
                currentInputListSelector = inputListSelector;
            }
            currentInputListSelector.Initialize();
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
            requestFriendsInfoItem.InitSelectItem();
            receiveFriendsInfoItem.InitSelectItem();
            ForceUpdateHeight();
        }
        public void ForceUpdateHeight()
        {
            requestFriendsInfoItem.ForceUpdateScrollHeight();
            receiveFriendsInfoItem.ForceUpdateScrollHeight();
            friendsInfoItem.ForceUpdateScrollHeight();
        }
        private void OnToggleTabChanged(GameObject go)
        {
            requestFriendsInfoItem.OnValueChanged(go);
            receiveFriendsInfoItem.OnValueChanged(go);
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
            };
            guideBtnDataListForKeyboardMouse = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.North },
                        { eSupportedDevice.PS, eInputControlType.North },
                    }, 
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_ADDFRIEND",null, OnClickAddFriendButton),
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
            };
            
            guideBtnDataListForConsole.AddRange(guideBtnDataListForKeyboardMouse);
            guideBtnDataListExpanded.AddRange(guideBtnDataListForKeyboardMouse);
            guideBtnDataListCollapsed.AddRange(guideBtnDataListForKeyboardMouse);

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
                currentInputListSelector = requestFriendsInfoItem.InputListSelector;
            }
            currentInputListSelector.Initialize();
            OnDeviceChange();

            ForceUpdateHeight();
        }
        #region  Menu InputListSelectr Function
        // 각 메뉴 ( 친구 , 초대한, 초대받은 ) 버튼 입력 처리에 대한 함수
        private void SetInputListSelector()
        {
            inputListSelector.ClearEventHandler();
            inputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, RefreshGuideBtnData);
            
            inputListSelector.Add(localPlayerInfoItem.CachedGameObject);
            
            requestFriendsInfoItem.InputListSelector.ClearEventHandler();
            requestFriendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuRequestFriend);
            requestFriendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuRequestFriend);
            
            receiveFriendsInfoItem.InputListSelector.ClearEventHandler();
            receiveFriendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuReceiveFriend);
            receiveFriendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuReceiveFriend);
            
            friendsInfoItem.InputListSelector.ClearEventHandler();
            friendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuFriendList);
            friendsInfoItem.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuFriendList);
        }
        
        private void OnDownSocialMenuRequestFriend(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (requestFriendsInfoItem.IsSelect && requestFriendsInfoItem.GetChildCount() > 0 && requestFriendsInfoItem.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = requestFriendsInfoItem;
                requestFriendsInfoItem.MoveChild(true);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else
            {
                isSelectInfoItem = false;
                requestFriendsInfoItem.InitSelectItem();
                currentInputListSelector = receiveFriendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (receiveFriendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        private void OnDownSocialMenuReceiveFriend(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (receiveFriendsInfoItem.IsSelect && receiveFriendsInfoItem.GetChildCount() > 0 && receiveFriendsInfoItem.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = receiveFriendsInfoItem;
                receiveFriendsInfoItem.MoveChild(true);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else
            {
                isSelectInfoItem = false;
                receiveFriendsInfoItem.InitSelectItem();
                currentInputListSelector = friendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (friendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        private void OnDownSocialMenuFriendList(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() > 0 && friendsInfoItem.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = friendsInfoItem;
                friendsInfoItem.MoveChild(true);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if ((friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() <= 0) || !friendsInfoItem.IsSelect)
            {
                currentInputListSelector.Select(true);
            }
        }
        
        private void OnUpSocialMenuRequestFriend(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (requestFriendsInfoItem.IsSelect && requestFriendsInfoItem.GetChildCount() > 0 && requestFriendsInfoItem.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = requestFriendsInfoItem;
                requestFriendsInfoItem.MoveChild(false);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if(requestFriendsInfoItem.IsSelect && requestFriendsInfoItem.GetChildCount() > 0 && !requestFriendsInfoItem.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                requestFriendsInfoItem.InitSelectItem();
                currentInputListSelector = requestFriendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (requestFriendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
            else
            {
                isSelectInfoItem = false;
                requestFriendsInfoItem.InitSelectItem();
                currentInputListSelector = inputListSelector;
                currentInputListSelector.Select(true);
                
                
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
        }
        private void OnUpSocialMenuReceiveFriend(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (receiveFriendsInfoItem.IsSelect && receiveFriendsInfoItem.GetChildCount() > 0 && receiveFriendsInfoItem.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = receiveFriendsInfoItem;
                receiveFriendsInfoItem.MoveChild(false);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if (requestFriendsInfoItem.IsSelect && requestFriendsInfoItem.GetChildCount() > 0 && requestFriendsInfoItem.CanMoveChild(false, true))
            {
                isSelectInfoItem = true;
                receiveFriendsInfoItem.InitSelectItem();
                requestFriendsInfoItem.MoveChild(false);
                currentInputListSelector = requestFriendsInfoItem.InputListSelector;
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }else if(receiveFriendsInfoItem.IsSelect && receiveFriendsInfoItem.GetChildCount() > 0 && !receiveFriendsInfoItem.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                receiveFriendsInfoItem.InitSelectItem();
                currentInputListSelector = receiveFriendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (receiveFriendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
            else
            {
                isSelectInfoItem = false;
                receiveFriendsInfoItem.InitSelectItem();
                currentInputListSelector = requestFriendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (requestFriendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
         private void OnUpSocialMenuFriendList(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() > 0 && friendsInfoItem.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = friendsInfoItem;
                friendsInfoItem.MoveChild(false);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if (receiveFriendsInfoItem.IsSelect && receiveFriendsInfoItem.GetChildCount() > 0 && receiveFriendsInfoItem.CanMoveChild(false, true))
            {
                isSelectInfoItem = true;
                friendsInfoItem.InitSelectItem();
                receiveFriendsInfoItem.MoveChild(false);
                currentInputListSelector = receiveFriendsInfoItem.InputListSelector;
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }else if(friendsInfoItem.IsSelect && friendsInfoItem.GetChildCount() > 0 && !friendsInfoItem.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                friendsInfoItem.InitSelectItem();
                currentInputListSelector = friendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (friendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
            else
            {
                isSelectInfoItem = false;
                friendsInfoItem.InitSelectItem();
                currentInputListSelector = receiveFriendsInfoItem.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (receiveFriendsInfoItem.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        
        private void RefreshGuideBtnData(bool isSelect)
        {
            // 가이드 버튼 펼치기,접기 상태 갱신
            currentInputListSelector.Select(false);
            currentInputListSelector = requestFriendsInfoItem.InputListSelector;
            currentInputListSelector.Select(true);
            if (requestFriendsInfoItem.IsSelect)
            {
                guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
            }
            else
            {
                guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
            }
        }

        #endregion
        
        #region FriendData Add Remove
        private void AddFriendRow(UserData data)
        {
            friendsInfoItem.AddData(ConvertFriendCount(LobbyUserData.Instance.GetFriendsList().Count, LobbyUserData.Instance.GetCountFriendsOnline()),
                data);
        }

        private void AddRequestFriendRow(UserData data)
        {
            requestFriendsInfoItem.AddData(ConvertFriendCount(LobbyUserData.Instance.GetRequestFriendsList().Count), data);
        }

        private void AddReceiveFriendRow(UserData data)
        {
            receiveFriendsInfoItem.AddData(ConvertFriendCount(LobbyUserData.Instance.GetReceiveFriendsList().Count), data);
        }

        private void RemoveRequestFriendRow(ulong userId)
        {
            requestFriendsInfoItem.RemoveData(ConvertFriendCount(LobbyUserData.Instance.GetRequestFriendsList().Count), userId);
        }

        private void RemoveReceiveFriendRow(ulong userId)
        {
            receiveFriendsInfoItem.RemoveData(ConvertFriendCount(LobbyUserData.Instance.GetReceiveFriendsList().Count), userId);
        }

        private void RemoveFriendRow(ulong userId)
        {
            friendsInfoItem.RemoveData(
                ConvertFriendCount(LobbyUserData.Instance.GetFriendsList().Count, LobbyUserData.Instance.GetCountFriendsOnline()), userId);
        }

        private void RemoveAllFriendData()
        {
            friendsInfoItem.RemoveAllData(ConvertFriendCount(0, 0));
            requestFriendsInfoItem.RemoveAllData(ConvertFriendCount(0));
            receiveFriendsInfoItem.RemoveAllData(ConvertFriendCount(0));
        }
        #endregion
    }
}