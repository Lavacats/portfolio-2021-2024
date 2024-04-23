using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PB.ClientParts.Social;

namespace PB.ClientParts
{
    public class SocialSquadControllerData
    {
        public UI_GuideBtnController_Renewal guideBtnControllerForKeyboardMouse;
        public UI_GuideBtnController_Renewal guideBtnControllerForConsole;
        public OnSocialViewCloseEventHandler onSocialViewCloseEventHandler;
    }
    public class UI_SocialSquadController : UIComponent ,IInputKeyEventListener 
    {
        [SerializeField] 
        private UI_SocialMenuContainer squadListContainer;
        [SerializeField] 
        private UI_SocialMenuContainer squadRequestListContainer;
        [SerializeField] 
        private UI_SocialMenuContainer squadReceiveListContainer;

        [SerializeField] 
        private VerticalLayoutGroup layoutGroup;
        
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

                
        private InputListSelector currentInputListSelector = null;

        private UI_SocialMenuContainer currentInfoItem = null;
        private bool isSelectInfoItem = false;
        
        public bool IsTopListener => true;
        public bool IsBlockInputKeyEvent => false;
        
        public void SetData(SocialSquadControllerData controllerData)
        {
            SetEventHandler();
            InitGuideBtn();
            
            float maxHeight = contentRectTransform.rect.size.y - 3 * (layoutGroup.spacing + squadListContainer.MinHeight+squadListContainer.contentPadding);

            SetItemSizeData(squadRequestListContainer, maxHeight);
            
            maxHeight = contentRectTransform.rect.size.y - 3 * (layoutGroup.spacing + squadReceiveListContainer.MinHeight+squadReceiveListContainer.contentPadding);
            SetItemSizeData(squadReceiveListContainer, maxHeight);
            
            maxHeight = contentRectTransform.rect.size.y - 3 * (layoutGroup.spacing + squadListContainer.MinHeight+squadListContainer.contentPadding);
            SetItemSizeData(squadListContainer, maxHeight);
            
            if (controllerData != null)
            {          
                onSocialViewCloseEventHandler += controllerData.onSocialViewCloseEventHandler;
                guideBtnControllerForKeyboardMouse = controllerData.guideBtnControllerForKeyboardMouse;
                guideBtnControllerForConsole = controllerData.guideBtnControllerForConsole;
            }
            
            List<UserData>data = LobbyUserData.Instance.GetGroupsList(); // 스쿼드 목록
            squadListContainer.SetData("UI_GROUP_GROUPMEMBERS", ConvertSquadCount(data.Count), OnToggleTabChanged, data, "SYS_TARGET_IS_NOT_EXIST", eTabType.Group);
    
            data = LobbyUserData.Instance.GetRequestGroupsList();    //보낸 스쿼드 요청
            squadRequestListContainer.SetData("UI_GROUP_INVITATION_SENT", ConvertSquadCount(data.Count), OnToggleTabChanged, data, "SYS_TARGET_IS_NOT_EXIST", eTabType.Group);
            
            data = LobbyUserData.Instance.GetReceiveGroupsList();    //받은 스쿼드 요청
            squadReceiveListContainer.SetData("UI_GROUP_REICIEVED_REQUEST", ConvertSquadCount(data.Count), OnToggleTabChanged, data, "SYS_TARGET_IS_NOT_EXIST", eTabType.Group);

            SetInputListSelector();
        }
    
        private void SetInputListSelector()
        {
            squadListContainer.InputListSelector.ClearEventHandler();
            squadListContainer.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuSquad);
            squadListContainer.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuSquad);
            
            squadRequestListContainer.InputListSelector.ClearEventHandler();
            squadRequestListContainer.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuRequest);
            squadRequestListContainer.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuRequest);
            
            squadReceiveListContainer.InputListSelector.ClearEventHandler();
            squadReceiveListContainer.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Down, OnDownSocialMenuReceive);
            squadReceiveListContainer.InputListSelector.AddEventHandler(InputListSelector.eLinkDirType.Up, OnUpSocialMenuReceive);
        }
        private void SetItemSizeData(UI_SocialMenuContainer item, float maxHeight)
        {
            item.contentMaxHeight = maxHeight;
            item.verticalPadding = padding.top + padding.bottom;
            item.contentPadding = layoutGroup.spacing;
        }
        public void SetEventHandler()
        {            
            UIHandler.UIEvenHelper.AddOnReceiveChangeOpenGroupAccordionItemEventHandler(ChangeAccordionType);   // 스쿼드 아코디언 오픈 
            UIHandler.UIEvenHelper.AddOnReceiveChangeGroupLeaderEventHandler(ChangeSquadLeader);                // 스쿼드장 교체
            UIHandler.UIEvenHelper.AddOnReceiveAllGroupListEventHandler(UpdateSquadList);                       // 스쿼드 리스트 갱신
            
            UIHandler.UIEvenHelper.AddOnReceiveRequestGroupEventHandler(AddRequestSquadData);                   // 초대한 스쿼드 정보 갱신
            UIHandler.UIEvenHelper.AddOnReceiveReceiveGroupEventHandler(AddReceiveSquadData);                   // 초대받은 스쿼드 정보 갱신
            UIHandler.UIEvenHelper.AddOnReceiveGroupEventHandler(AddSquadData);                                 // 스쿼드 정보 갱신
            
            UIHandler.UIEvenHelper.AddOnReceiveRemoveRequestGroupEventHandler(RemoveRequestSquadData);          // 초대받은 스쿼드 정보 삭제
            UIHandler.UIEvenHelper.AddOnReceiveRemoveReceiveGroupEventHandler(RemoveReceiveSquadData);          // 초대한 스쿼드 정보 삭제
            UIHandler.UIEvenHelper.AddOnReceiveRemoveGroupEventHandler(RemoveSquadData);                        // 스쿼드 정보 삭제
            UIHandler.UIEvenHelper.AddOnReceiveRemoveAllGroupEventHandler(RemoveAllSquadData);                  // 모든 스쿼드 정보 삭제
            
            UIHandler.UIEvenHelper.AddOnReceiveInitGroupListEventHandler(InitSquadList);                        // 스쿼드 리스트 초기화
            UIHandler.UIEvenHelper.AddOnAutoGroupAcceptHandler(AcceptSquadInvite);                              // 스쿼드 초대 수락
        }
        private void InitGuideBtn()
        {
            guideBtnDataListForConsole = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.South},
                        { eSupportedDevice.PS , eInputControlType.South},
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_SELECT",null, null),
            };
            
            UI_GuideBtnData_Renewal btnData = new UI_GuideBtnData_Renewal(new Dictionary<eSupportedDevice, eInputControlType>()
                {
                    { eSupportedDevice.XBOX , eInputControlType.North},
                    { eSupportedDevice.PS , eInputControlType.North},
                }, 
                new Dictionary<eSupportedDevice, bool>()
                {
                    { eSupportedDevice.KEYBOARD_MOUSE, false },
                    { eSupportedDevice.ANDROID, false },
                    { eSupportedDevice.IOS, false }
                }, eGameInputEvent.None,true, "UI_BUTTON_EXIT_GROUP",null, OnClickExitSquadButton);
            
            guideBtnDataListForConsole.Add(btnData);
            guideBtnDataListForKeyboardMouse = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX , eInputControlType.East},
                        { eSupportedDevice.PS , eInputControlType.East},
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                        { eSupportedDevice.IOS, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_CLOSE",null,OnClickCloseButton),
            };
            guideBtnDataListForConsole.AddRange(guideBtnDataListForKeyboardMouse);
            UI_GuideBtnData_Renewal btnData2 = new UI_GuideBtnData_Renewal(new Dictionary<eSupportedDevice, eInputControlType>()
                {
                    { eSupportedDevice.XBOX , eInputControlType.North},
                    { eSupportedDevice.PS , eInputControlType.North},
                }, 
                new Dictionary<eSupportedDevice, bool>()
                {
                    { eSupportedDevice.KEYBOARD_MOUSE, false },
                    { eSupportedDevice.ANDROID, false },
                    { eSupportedDevice.IOS, false }
                }, eGameInputEvent.None, false, "UI_BUTTON_EXIT_GROUP",null,OnClickExitSquadButton);
            guideBtnDataListForKeyboardMouse.Insert(0, btnData2);
            
            
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
            
            guideBtnDataListExpanded.AddRange(guideBtnDataListForKeyboardMouse);
            guideBtnDataListCollapsed.AddRange(guideBtnDataListForKeyboardMouse);

        }
        private void InitSquadList()
        {
            List<UserData> data = LobbyUserData.Instance.GetGroupsList();
            squadListContainer.InitData("UI_GROUP_GROUPMEMBERS",ConvertSquadCount(data.Count), data);
        }
        private void InitConsoleControl()
        {
            if (currentInputListSelector is not null)
            {
                currentInputListSelector.Select(false);
            }
            currentInputListSelector = squadListContainer.InputListSelector;
            currentInputListSelector.Initialize();
            isSelectInfoItem = false;
            squadListContainer.InitSelectItem();
            squadRequestListContainer.InitSelectItem();
            squadReceiveListContainer.InitSelectItem();
            ForceUpdateHeight();
            
            if (squadListContainer.IsSelect)
            {
                guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
            }
            else
            {
                guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
            }
        }
        
        public bool OnInputEvent(eInputControlType type, bool isPressed, Vector2 movement)
        {
            if (GameSettings.CurrentDevice == eSupportedDevice.PS || GameSettings.CurrentDevice == eSupportedDevice.XBOX)
            {
                if (isPressed)
                {
                    switch (type)
                    {
                        case eInputControlType.NorthHoldStart:
                        {
                            break;
                        }
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
                                    if (currentInputListSelector == squadRequestListContainer.InputListSelector)
                                    {
                                        if (squadRequestListContainer.IsSelect)
                                        {
                                            guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                                        }
                                        else
                                        {
                                            guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                                        }
                                    }
                                    else if (currentInputListSelector == squadReceiveListContainer.InputListSelector)
                                    {
                                        if (squadReceiveListContainer.IsSelect)
                                        {
                                            guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                                        }
                                        else
                                        {
                                            guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                                        }
                                    }
                                    else if (currentInputListSelector == squadListContainer.InputListSelector)
                                    {
                                        if (squadListContainer.IsSelect)
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
                    switch (type)
                    {
                        case eInputControlType.NorthHold:
                        {
                            OnClickExitSquadButton();
                            break;
                        }
                    }
                }
            }
            return false;
        }
        
        private void OnClickExitSquadButton()
        {
            SocialNetworkManager.Instance.ReqExitGroup();
            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.CANCEL);
        }
        private void OnClickCloseButton()
        {
            onSocialViewCloseEventHandler?.Invoke();
            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.CLOSE);
        }
        public void RemoveEventHandler(OnSocialViewCloseEventHandler handler)
        {         
            RemoveAllSquadData();
            onSocialViewCloseEventHandler -= handler;
   
            UIHandler.UIEvenHelper.RemoveOnReceiveChangeOpenGroupAccordionItemEventHandler(ChangeAccordionType);    // 스쿼드 아코디언 오픈
            UIHandler.UIEvenHelper.RemoveOnReceiveChangeGroupLeaderEventHandler(ChangeSquadLeader);                 // 스쿼드장 교체
            UIHandler.UIEvenHelper.RemoveOnReceiveAllGroupListEventHandler(UpdateSquadList);                        // 스쿼드 리스트 갱신
            
            UIHandler.UIEvenHelper.RemoveOnReceiveRequestGroupEventHandler(AddRequestSquadData);                    // 초대한 스쿼드 정보 갱신
            UIHandler.UIEvenHelper.RemoveOnReceiveReceiveGroupEventHandler(AddReceiveSquadData);                    // 초대받은 스쿼드 정보 갱신
            UIHandler.UIEvenHelper.RemoveOnReceiveGroupEventHandler(AddSquadData);                                  // 스쿼드 정보 갱신
            
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveRequestGroupEventHandler(RemoveRequestSquadData);           // 초대받은 스쿼드 정보 삭제
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveReceiveGroupEventHandler(RemoveReceiveSquadData);           // 초대한 스쿼드 정보 삭제
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveGroupEventHandler(RemoveSquadData);                         // 스쿼드 정보 삭제
            UIHandler.UIEvenHelper.RemoveOnReceiveRemoveAllGroupEventHandler(RemoveAllSquadData);                   // 모든 스쿼드 정보 삭제
            
            UIHandler.UIEvenHelper.RemoveOnReceiveInitGroupListEventHandler(InitSquadList);                         // 스쿼드 리스트 초기화
            UIHandler.UIEvenHelper.RemoveOnAutoGroupAcceptHandler(AcceptSquadInvite);                               // 스쿼드 초대 수락
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
                currentInputListSelector = squadListContainer.InputListSelector;
            }
            currentInputListSelector.Initialize();
            OnDeviceChange();

            ForceUpdateHeight();
        }
        
        public void ChangeGuidButtonList(SocialSquadControllerData controllerData)
        {
            if (controllerData != null)
            {
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
        private void UpdateSquadList()
        {
            List<UserData> data = LobbyUserData.Instance.GetGroupsList();
            squadListContainer.UpdateData(ConvertSquadCount(data.Count),LobbyUserData.Instance.GetUpdateGroupsList());
            data = LobbyUserData.Instance.GetReceiveGroupsList();
            squadReceiveListContainer.UpdateData(ConvertSquadCount(data.Count),LobbyUserData.Instance.GetUpdateReceiveGroupsList());
            data = LobbyUserData.Instance.GetRequestGroupsList();
            squadRequestListContainer.UpdateData(ConvertSquadCount(data.Count),LobbyUserData.Instance.GetUpdateRequestGroupsList());
            LobbyUserData.Instance.ClearUpdateGroupList();
        }
        public void ForceUpdateHeight()
        {
            squadRequestListContainer.ForceUpdateScrollHeight();
            squadReceiveListContainer.ForceUpdateScrollHeight();
            squadListContainer.ForceUpdateScrollHeight();
        }
        private void ChangeAccordionType(eAccordionType type)
        {
            GameObject go = null;
            switch (type)
            {
                case eAccordionType.List:
                    go = squadListContainer.GetButtonGameObject();
                    break;
                case eAccordionType.ReceiveList:
                    go = squadReceiveListContainer.GetButtonGameObject();
                    break;
                case eAccordionType.RequestList:
                    go = squadRequestListContainer.GetButtonGameObject();
                    break;
            }

            if (go is not null)
            {
                OnToggleTabChanged(go);
            }
        }
        private void OnToggleTabChanged(GameObject go)
        {
            squadRequestListContainer.OnValueChanged(go);
            squadReceiveListContainer.OnValueChanged(go);
            squadListContainer.OnValueChanged(go);
        }
        private void ChangeSquadLeader(ulong userId)
        {
            squadListContainer.ChangeLeaderData(userId);
        }
        private string ConvertSquadCount(int total)
        {
            return $" {total.ToString()}";
        }
        private void AcceptSquadInvite(UserData userData)
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
                if (userData is ServerInviteUserData serverInviteUserData)
                {
                    SocialNetworkManager.Instance.ReqServerInviteAccept(serverInviteUserData.Id, true, serverInviteUserData.ip, serverInviteUserData.port, serverInviteUserData.ToServer);
                }
                else
                {
                    SocialNetworkManager.Instance.ReqResponseInviteGroup(userData.Id, true);    
                }
            }
        }

        #region Menu InputListSelectr Function
        
        // 각 메뉴 ( 스쿼드 , 초대한, 초대받은 ) 버튼 입력 처리에 대한 함수
        private void OnDownSocialMenuSquad(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (squadListContainer.IsSelect && squadListContainer.GetChildCount() > 0 && squadListContainer.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = squadListContainer;
                squadListContainer.MoveChild(true);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else
            {
                isSelectInfoItem = false;
                squadListContainer.InitSelectItem();
                currentInputListSelector = squadRequestListContainer.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (squadRequestListContainer.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        private void OnDownSocialMenuRequest(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (squadRequestListContainer.IsSelect && squadRequestListContainer.GetChildCount() > 0 && squadRequestListContainer.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = squadRequestListContainer;
                squadRequestListContainer.MoveChild(true);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else
            {
                isSelectInfoItem = false;
                squadRequestListContainer.InitSelectItem();
                currentInputListSelector = squadReceiveListContainer.InputListSelector;
                currentInputListSelector.Select(true);
                
                if (squadReceiveListContainer.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        private void OnDownSocialMenuReceive(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (squadReceiveListContainer.IsSelect && squadReceiveListContainer.GetChildCount() > 0 && squadReceiveListContainer.CanMoveChild(true))
            {
                isSelectInfoItem = true;
                currentInfoItem = squadReceiveListContainer;
                squadReceiveListContainer.MoveChild(true);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
                
                if (squadReceiveListContainer.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
            else if ((squadReceiveListContainer.IsSelect && squadReceiveListContainer.GetChildCount() <= 0) || !squadReceiveListContainer.IsSelect)
            {
                currentInputListSelector.Select(true);
            }
        }
        
        private void OnUpSocialMenuSquad(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (squadListContainer.IsSelect && squadListContainer.GetChildCount() > 0 && squadListContainer.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = squadListContainer;
                squadListContainer.MoveChild(false);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if(squadListContainer.IsSelect && squadListContainer.GetChildCount() > 0 && !squadListContainer.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                squadListContainer.InitSelectItem();
                currentInputListSelector = squadListContainer.InputListSelector;
                currentInputListSelector.Select(true);
                if (squadListContainer.IsSelect)
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
                currentInputListSelector.Select(true);
            }
        }
        private void OnUpSocialMenuRequest(bool isSelect)
        {
            currentInputListSelector.Select(false);
            
            if (squadRequestListContainer.IsSelect && squadRequestListContainer.GetChildCount() > 0 && squadRequestListContainer.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = squadRequestListContainer;
                squadRequestListContainer.MoveChild(false);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if (squadListContainer.IsSelect && squadListContainer.GetChildCount() > 0 && squadListContainer.CanMoveChild(false, true))
            {
                isSelectInfoItem = true;
                squadRequestListContainer.InitSelectItem();
                squadListContainer.MoveChild(false);
                currentInputListSelector = squadListContainer.InputListSelector;
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if(squadRequestListContainer.IsSelect && squadRequestListContainer.GetChildCount() > 0 && !squadRequestListContainer.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                squadRequestListContainer.InitSelectItem();
                currentInputListSelector = squadRequestListContainer.InputListSelector;
                currentInputListSelector.Select(true);
                if (squadRequestListContainer.IsSelect)
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
                squadListContainer.InitSelectItem();
                currentInputListSelector = squadListContainer.InputListSelector;
                currentInputListSelector.Select(true);
                if (squadListContainer.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        private void OnUpSocialMenuReceive(bool isSelect)
        {
            currentInputListSelector.Select(false);
            if (squadReceiveListContainer.IsSelect && squadReceiveListContainer.GetChildCount() > 0 && squadReceiveListContainer.CanMoveChild(false))
            {
                isSelectInfoItem = true;
                currentInfoItem = squadReceiveListContainer;
                squadReceiveListContainer.MoveChild(false);
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if (squadRequestListContainer.IsSelect && squadRequestListContainer.GetChildCount() > 0 && squadRequestListContainer.CanMoveChild(false, true))
            {
                isSelectInfoItem = true;
                squadReceiveListContainer.InitSelectItem();
                squadRequestListContainer.MoveChild(false);
                currentInputListSelector = squadRequestListContainer.InputListSelector;
                guideBtnControllerForConsole.SetData(guideBtnDataListForConsole);
            }
            else if(squadReceiveListContainer.IsSelect && squadReceiveListContainer.GetChildCount() > 0 && !squadReceiveListContainer.CanMoveChild(false) && isSelectInfoItem)
            {
                isSelectInfoItem = false;
                squadReceiveListContainer.InitSelectItem();
                currentInputListSelector = squadReceiveListContainer.InputListSelector;
                currentInputListSelector.Select(true);
              
                if (squadReceiveListContainer.IsSelect)
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
                squadReceiveListContainer.InitSelectItem();
                currentInputListSelector = squadRequestListContainer.InputListSelector;
                currentInputListSelector.Select(true);
                if (squadRequestListContainer.IsSelect)
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListExpanded);
                }
                else
                {
                    guideBtnControllerForConsole.SetData(guideBtnDataListCollapsed);
                }
            }
        }
        
        #endregion
        
        #region SquadData Add Remove
        private void AddRequestSquadData(UserData data)
        {
            squadRequestListContainer.AddData(ConvertSquadCount(LobbyUserData.Instance.GetRequestGroupsList().Count), data);
        }
        private void AddSquadData(UserData data)
        {
            squadListContainer.AddData(ConvertSquadCount(LobbyUserData.Instance.GetGroupsList().Count), data);
        }
        private void AddReceiveSquadData(UserData data)
        {
            squadReceiveListContainer.AddData(ConvertSquadCount(LobbyUserData.Instance.GetReceiveGroupsList().Count), data);
        }
        private void RemoveRequestSquadData(ulong userId)
        {
            squadRequestListContainer.RemoveData(ConvertSquadCount(LobbyUserData.Instance.GetRequestGroupsList().Count), userId);
        }
        private void RemoveReceiveSquadData(ulong userId)
        {
            squadReceiveListContainer.RemoveData(ConvertSquadCount(LobbyUserData.Instance.GetReceiveGroupsList().Count), userId);
        }
        private void RemoveSquadData(ulong userId)
        {
            squadListContainer.RemoveData(ConvertSquadCount(LobbyUserData.Instance.GetGroupsList().Count), userId);
        }
        private void RemoveAllSquadData()
        {
            squadListContainer.RemoveAllData(ConvertSquadCount(0));
            squadReceiveListContainer.RemoveAllData(ConvertSquadCount(0));
            squadRequestListContainer.RemoveAllData(ConvertSquadCount(0));
        }
        #endregion
    }
}