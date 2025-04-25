using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PB.ClientParts
{
    public class UIPopupChattingParam : UIPopupBaseParam
    {
        public bool isShowUserPopup=false;
    }
    
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_Chatting")]
    public class UI_Popup_Chatting : UIPopupBase
    {
        [SerializeField] 
        private UI_ChattingController chattingController;
        [SerializeField]
        private Animation popupAnimation = null;
        [SerializeField]
        private AnimationClip popupStartAnimationClip = null;
        [SerializeField]
        private AnimationClip popupEndAnimationClip = null;
        [SerializeField]
        private GameObject lastChatObj = null;
        [SerializeField]
        private GameObject chattingBoxObj = null;
        [SerializeField]
        protected GameObject dimObj = null;
        [SerializeField] 
        private UI_ChattingItem_Renewal lastChatItem=null;
        [SerializeField] 
        private UI_ControllerBtn_Renewal inputFieldControllerBtn;
        [SerializeField] 
        private UI_NotificationGuideBtnController noticeGuideBtnController = null;

        private List<UI_GuideBtnData_Renewal> pcGuideBtnDataList = null;
        private List<UI_GuideBtnData_Renewal> consoleGuideBtnDataList = null;
        private List<UI_GuideBtnData_Renewal> guideBtnDataListForConsoleItemSelect = null;
        
   
        private bool isShowUserPopup = false;
        private bool isChatting = false;
        public bool IsChatting => isChatting;
        
        private readonly float hideTimeLastMsgObj = 5.0f;
        private Coroutine hideLastObjCoroutine;

        public override void OnCreate()
        {
            base.OnCreate();
            
            SetGuideBtnData();
            
            inputFieldControllerBtn.SetData( new UI_ControllerBtnData_Renewal(
                new Dictionary<eSupportedDevice, eInputControlType>()
                {
                    { eSupportedDevice.XBOX , eInputControlType.South},
                    { eSupportedDevice.PS , eInputControlType.South},
                },
                true, 
                new Dictionary<eSupportedDevice, bool>()
                {
                    { eSupportedDevice.KEYBOARD_MOUSE, false},
                    { eSupportedDevice.ANDROID, false}
                }, 
                eGameInputEvent.None));
                       
            ChattingManager.Instance.AddOnReceiveChatEventHandler(RefreshLastChatMsg);
        }
        public override void OnSetup(UIPopupBaseParam param)
        {
            base.OnSetup(param);
            if (param is UIPopupChattingParam chatPopupParam)
            {
                this.isShowUserPopup = chatPopupParam.isShowUserPopup;
            }
        }
        public override void OnClose()
        {
            base.OnClose();
            ChattingManager.Instance.RemoveOnReceiveChatEventHandler(RefreshLastChatMsg);
        }
        public override bool OnInputEvent(eInputControlType types, bool isPressed, Vector2 movement)
        {
            if (isPressed && GameSettings.CurrentDevice != eSupportedDevice.KEYBOARD_MOUSE)
            {
                if (ReferenceEquals(noticeGuideBtnController, null))
                {
                    return false;
                }
                if (chattingController.IsFocusChattingItem)
                {
                    switch (types)
                    {
                        case eInputControlType.South: //선택
                        {
                            chattingController.ShowUserPopup();
                            return true;
                        }
                        case eInputControlType.East: //뒤로
                        {
                            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.BACK);
                            chattingController.OnDeselectItem();
                            noticeGuideBtnController.SetData(consoleGuideBtnDataList);
                            break;
                        }
                        case eInputControlType.DPadUp: //채팅 이력
                        {
                            chattingController.SelectIdx--;
                            chattingController.OnChattingItemSelect();
                            return true;
                        }
                        case eInputControlType.DPadDown: //채팅 이력
                        {
                            chattingController.SelectIdx++;
                            chattingController.OnChattingItemSelect();
                            return true;
                        }
                    }
                }
                else
                {
                    switch (types)
                    {
                        case eInputControlType.South: //채팅 입력
                        {
                            chattingController.OnFocusChatInputField();
                            return true;
                        }
                        case eInputControlType.DPadLeft: //채널 변경
                        {
                            chattingController.OnChangeInputChannel(-1);
                            return true;
                        }
                        case eInputControlType.DPadRight: //채널 변경
                        {
                            chattingController.OnChangeInputChannel(1);
                            return true;
                        }
                        case eInputControlType.East: //닫기
                        {
                            CloseChattingPopup();
                            break;
                        }
                        case eInputControlType.DPadUp: //채팅 이력
                        {
                            chattingController.OnItemSelectAtFirst(-1);
                            if (chattingController.IsFocusChattingItem)
                            {
                                noticeGuideBtnController.SetData(guideBtnDataListForConsoleItemSelect);
                            }
                            else
                            {
                                noticeGuideBtnController.SetData(consoleGuideBtnDataList);
                            }
                            return true;
                        }
                        case eInputControlType.DPadDown: //채팅 이력
                        {
                            chattingController.OnItemSelectAtFirst(1);
                            if (chattingController.IsFocusChattingItem)
                            {
                                noticeGuideBtnController.SetData(guideBtnDataListForConsoleItemSelect);
                            }
                            else
                            {
                                noticeGuideBtnController.SetData(consoleGuideBtnDataList);
                            }
                            return true;
                        }
                    }
                }
            }
            else if (isPressed && GameSettings.CurrentDevice == eSupportedDevice.KEYBOARD_MOUSE)
            {
                switch (types)
                {
                    case eInputControlType.Enter: // 채팅 송신
                    {
                        chattingController.SendChatMessage();
                        return true;
                    }
                    case eInputControlType.Space: // 단축키 입력
                    {
                        chattingController.OnStartChatCommandAction();
                        return true;
                    }
                    case eInputControlType.Tab: // 채널 변경
                    {
                        chattingController.OnChangeInputChannel(1);
                        return true;
                    }
                    case eInputControlType.Escape:
                    {
                        CloseChattingPopup();
                        return true;
                    }
                }
            }
            return false;
        }
        public void RefreshLastChatMsg(ChatData msg)
        {
            if (!isChatting)
            {
                lastChatItem.SetChattingItemChatData(msg, false);
            }
        }
        public void EnterChattingPopup()
        {
            popupAnimation.Play(popupStartAnimationClip.name);
            InputHandler.AddEventListener(this);
            ChattingManager.Instance.RefreshChatCount(false);
            chattingController.Enter();
        }
        private void SetGuideBtnData()
        {
            consoleGuideBtnDataList = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.DPadLR },
                        { eSupportedDevice.PS, eInputControlType.DPadLR },
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_INPUTCHANNEL", null, null),
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.DPadTB },
                        { eSupportedDevice.PS, eInputControlType.DPadTB },
                    },
                    new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false },
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_SELECT_TARGET", null, null),
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
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_BACK", null, null)
            };
            pcGuideBtnDataList = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE , eInputControlType.Escape},
                    }, new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false},
                        { eSupportedDevice.PS, false },
                        { eSupportedDevice.XBOX, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_BACK", null, CloseChattingPopup)
            };
            guideBtnDataListForConsoleItemSelect = new List<UI_GuideBtnData_Renewal>()
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
                    }, eGameInputEvent.None,false, "UI_BUTTON_SELECT", null,null),
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
                    }, eGameInputEvent.None,false, "UI_BUTTON_BACK",null,null),
            };
        }
        public void OnViewChattingPopup(bool isShowLastMsg)
        {
            CachedGameObject.SetActive(true);

            // param 갱신후 현재 POPUP 형태가 어느것인지에 따라 호출
            if (isShowLastMsg == true)
            {
                if (!isChatting)
                {
                    // 최신 메시지 5초간 보여주기
                    lastChatObj.SetActive(true);
                    dimObj.SetActive(false);
                    InputHandler.RemoveEventListener(this); // 최신 메시지를 보여줄때는 InputHandler를 사용하지않는다.
                    if (hideLastObjCoroutine != null)
                    {
                        StopCoroutine(hideLastObjCoroutine);
                        hideLastObjCoroutine = null;
                    }
                    hideLastObjCoroutine = StartCoroutine(CoCheckHideLastMsgObj());
                }
            }
            else
            {
                // 채팅창 오픈
                lastChatObj.SetActive(false);
                chattingBoxObj.SetActive(true);
                dimObj.SetActive(true);
                chattingController.SetData(isShowUserPopup);

                isChatting = true;
                
                OnDeviceChange();
                EnterChattingPopup();
            }
        }
        private IEnumerator CoCheckHideLastMsgObj()
        {
            yield return new WaitForSeconds(hideTimeLastMsgObj);
            lastChatObj.SetActive(false);
        }
        public void CloseChattingPopup()
        {
            isChatting = false;
            chattingController.RemoveEventHandler();
            StartCoroutine(CoCloseChattingPopup(popupEndAnimationClip));
        }
        private IEnumerator CoCloseChattingPopup(AnimationClip clip)
        {
            popupAnimation.Play(clip.name);
            yield return new WaitForSeconds(clip.length);
            if (hideLastObjCoroutine != null)
            {
                StopCoroutine(hideLastObjCoroutine);
                hideLastObjCoroutine = null;
            }
            OnClose();
        }
        private void OnDeviceChange()
        {
            if (!ReferenceEquals(noticeGuideBtnController,null))
            {
                switch (GameSettings.CurrentDevice)
                {
                    case eSupportedDevice.PS:
                    case eSupportedDevice.XBOX:
                        inputFieldControllerBtn.SetActive(true);
                        noticeGuideBtnController.SetData(consoleGuideBtnDataList);
                        break;
                    case eSupportedDevice.KEYBOARD_MOUSE:
                        inputFieldControllerBtn.SetActive(false);
                        chattingController.OnDeselectItem();
                        noticeGuideBtnController.SetData(pcGuideBtnDataList);
                        break;
                }
            }
        }
    }
}