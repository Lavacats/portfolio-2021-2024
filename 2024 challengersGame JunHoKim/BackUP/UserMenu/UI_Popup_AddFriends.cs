using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PB.ClientParts.Social;
using PB.ClientParts.Platform;

namespace PB.ClientParts
{
    public class UIPopupAddFriendsParam : UIPopupBaseParam
    {
        
    }
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_AddFriends")]
    public class UI_Popup_AddFriends : UIPopupBase
    {
        [SerializeField]
        private UILocalizedText titleText = null;

        [SerializeField]
        private UI_GuideBtnController_Renewal guideBtnControllerPC = null;
        
        [SerializeField]
        private UI_GuideBtnController_Renewal guideBtnControllerConsole = null;
        
        [SerializeField]
        private UI_ControllerBtn_Renewal controllerBtnController = null;

        
        [SerializeField]
        private FixedInputField inputField = null;

        [SerializeField]
        private UILocalizedText placeholder = null;
        
        public override bool IsBlockInputKeyEvent => true;

        private List<UI_GuideBtnData_Renewal> dataForConsole = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> dataForKeyboardMouse = new List<UI_GuideBtnData_Renewal>();
        
        public override void OnSetup(UIPopupBaseParam param)
        {
            titleText.LocalKey = "UI_PLAYERMENU_ADDFRIEND";
            placeholder.LocalKey = "UI_ADDFRIEND_INPUT";
            
            controllerBtnController.SetData( new UI_ControllerBtnData_Renewal(
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
            
            SetGuideData();
            guideBtnControllerConsole.SetData(dataForConsole);
            guideBtnControllerPC.SetData(dataForKeyboardMouse);
            OnDeviceChange();
            Enter();
        }

        public override bool OnInputEvent(eInputControlType type, bool isPressed, Vector2 movement)
        {
            if (isPressed)
            {
                switch (type)
                {
                    case eInputControlType.East:
                        OnClose();
                        SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.CLOSE);
                        break;
                    case eInputControlType.South:
                        OnClickInputActionButton();
                        SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.SELECT);
                        break;
                }
            }
            else
            {
                if (type == eInputControlType.North)
                {
                    OnClickSendRequestButton();
                    SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.CONFIRM);
                }
            }
            return false;
        }

        private void SetGuideData()
        {
            dataForConsole = new List<UI_GuideBtnData_Renewal>()
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
                        { eSupportedDevice.IOS, false },
                    }, eGameInputEvent.None,false, "UI_BUTTON_SENDREQUEST",null, OnClickSendRequestButton),
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
                    }, eGameInputEvent.None, false, "UI_BUTTON_CLOSE",null,OnClose),
            };
            dataForKeyboardMouse = new List<UI_GuideBtnData_Renewal>()
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
                        { eSupportedDevice.IOS, false },
                    }, eGameInputEvent.None,false, "UI_BUTTON_SENDREQUEST",null, OnClickSendRequestButton),
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
                    }, eGameInputEvent.None, false, "UI_BUTTON_CLOSE",null,OnClose),
            };
        }
        private void OnDeviceChange()
        {
            switch (GameSettings.CurrentDevice)
            {
                case eSupportedDevice.PS:
                case eSupportedDevice.XBOX:
                    controllerBtnController.SetActive(true);
                    guideBtnControllerConsole.SetActive(true);
                    guideBtnControllerPC.SetActive(false);
             
                    break;
                case eSupportedDevice.KEYBOARD_MOUSE:
                case eSupportedDevice.ANDROID:
                case eSupportedDevice.IOS:
                    controllerBtnController.SetActive(false);
                    guideBtnControllerConsole.SetActive(false);
                    guideBtnControllerPC.SetActive(true);
                    break;
            }
        }

        public void Enter()
        {
            InputHandler.AddEventListener(this);
            inputField.text = "";
            CachedRectTransform.SetActive(true);
            inputField.enabled = true;
        }
        private void OnClickInputActionButton()
        {
#if UNITY_GAMECORE
            PlatformManager.Instance.PlatformVirtualKeyboard(string.Empty, string.Empty, string.Empty, 0,0,
                (hresult, resultString) =>
                {
                    inputField.text = resultString;
                });
#elif UNITY_PS5
            PlatformManager.Instance.PlatformVirtualKeyboard(string.Empty, string.Empty, string.Empty, (int)Sony.PS5.Dialog.Ime.Option.DEFAULT, 0,
                (result, resultString) =>
                {
                    inputField.text = resultString;
                });
#else
            inputField.Select();
            inputField.ActivateInputField();
#endif
        }
        private void OnClickSendRequestButton()
        {
            SocialNetworkManager.Instance.ReqFriendInvite(inputField.text);
        }
        public override void OnClose()
        {
            inputField.text = "";
            base.OnClose();
        }
    }
}