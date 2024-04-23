using System.Collections.Generic;
using PB.ClientParts.Rest;
using PBRest.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class UIPopupBlockParam : UIPopupBaseParam
    {
        public string kickUserName;
        public ulong kickUserId;

    }
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_Block")]
    public class UI_Popup_Block : UIPopupBase
    {
        [SerializeField]
        private UI_GuideBtnController_Renewal guideBtnControllerPC = null;
        [SerializeField]
        private UI_GuideBtnController_Renewal guideBtnControllerConsole = null;
        [SerializeField]
        private UILocalizedText titleText = null;
        [SerializeField]
        private Text nickNameText = null;
        
        private List<UI_GuideBtnData_Renewal> btnKeybordMouseDataList = null;
        private List<UI_GuideBtnData_Renewal> btnConsolDataList = null;

        private string nicName = null;
        private ulong blockUserId;

        private eSceneType currentSceneType = eSceneType.None;
        
        public override void OnSetup(UIPopupBaseParam param)
        {
            if (param is UIPopupBlockParam chatPopupParam)
            {
                blockUserId = chatPopupParam.kickUserId;
                nicName = chatPopupParam.kickUserName;
            }
            titleText.LocalKey = "UI_PLAYERMENU_BLOCK";
            nickNameText.text = nicName;
            BtnDataInit();
            guideBtnControllerPC.SetData(btnConsolDataList);
            guideBtnControllerPC.SetData(btnKeybordMouseDataList);
            OnDeviceChange();
        }

        private void OnDeviceChange()
        {
            if (GameSettings.CurrentDevice == eSupportedDevice.XBOX ||
                GameSettings.CurrentDevice == eSupportedDevice.PS)
            {
                guideBtnControllerConsole.SetActive(true);
                guideBtnControllerPC.SetActive(false);
            }
            else
            {
                guideBtnControllerConsole.SetActive(false);
                guideBtnControllerPC.SetActive(true);
            }
        }

        public override bool OnInputEvent(eInputControlType inputControlType, bool isPressed, Vector2 movement)
        {
            
            switch (inputControlType)
            {
                case eInputControlType.South:
                {
                    if (isPressed)
                    {
                        OnBlockBtnClick();
                    }
                    break;
                }
                case eInputControlType.East:
                {
                    OnClose();
                    break;
                }
            }

            return false;
        }
        
        private void BtnDataInit()
        {
            btnKeybordMouseDataList = new List<UI_GuideBtnData_Renewal>
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.South },
                        { eSupportedDevice.PS, eInputControlType.South },
                    },
                    new Dictionary<eSupportedDevice, bool>
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_BLOCK",null, OnBlockBtnClick ),
                
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.East },
                        { eSupportedDevice.PS, eInputControlType.East },
                    }, 
                    new Dictionary<eSupportedDevice, bool>
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_OUTGAME_CANCEL",null, OnClose),
            };
            
            btnConsolDataList = new List<UI_GuideBtnData_Renewal>
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.South },
                        { eSupportedDevice.PS, eInputControlType.South },
                    }, 
                    new Dictionary<eSupportedDevice, bool>
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false }
                    }, eGameInputEvent.None, false, "UI_BUTTON_BLOCK",null,OnBlockBtnClick),
                
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.XBOX, eInputControlType.East },
                        { eSupportedDevice.PS, eInputControlType.East },
                    }, 
                    new Dictionary<eSupportedDevice, bool>
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE, false },
                        { eSupportedDevice.ANDROID, false }
                    }, eGameInputEvent.None,false, "UI_BUTTON_BACK",null, OnClose),
            };
        }

        private void OnBlockBtnClick()
        {
            //차단 유저 카운트
            if (GamePlayUserDataManager.Instance.GetBlockUserCount() < 100)
            {
                //차단 보내기
                ClientRestConnectManager.Instance.StartConnectCoroutine(RestAPI.ReqRegisterBlockUser(blockUserId,
                    (resp) =>
                    {
                        if (resp.ReturnCode == (int)PBRestReturnCode.SUCCESS)
                        {
                            LobbyUserData.Instance.RemoveFriend(blockUserId);
                            LobbyUserData.Instance.RemoveConsiderFriend(blockUserId);
                            LobbyUserData.Instance.RemoveInviteFriend(blockUserId);
                            UIPopupSystemMessageParam param = new UIPopupSystemMessageParam();
                            param.messageInfo = new UILocalizedTextInfo("SYS_BLOCK_USER_BLOCKED", nicName);
                            param.sortingOrder = UIHandler.CalcSortingOrder(95);
                            UIHandler.Instance.LoadUI<UI_Popup_SystemMessage>(param, null, true);
                            
                            GamePlayUserDataManager.Instance.AddBlockUserInfoList(blockUserId, nicName);
                            if (currentSceneType == eSceneType.Game)
                            {
                                InGameEventHelper.TriggerOnPlayerBlockEventHandler(blockUserId);
                            }
                        }
                        else
                        {
                            CGLog.LogError($"[UI_UserBlockContainer] Not SUCCESS ReqRegisterBlockUser : {resp.ReturnCode}");
                        }
                    }));    
            }
            else
            {
                UIPopupSystemMessageParam param = new UIPopupSystemMessageParam();
                param.messageInfo = new UILocalizedTextInfo("SYS_BLOCK_LIST_IS_FULL");
                param.sortingOrder = UIHandler.CalcSortingOrder(95);
                UIHandler.Instance.LoadUI<UI_Popup_SystemMessage>(param, null, true);
            }
        }
    }
}