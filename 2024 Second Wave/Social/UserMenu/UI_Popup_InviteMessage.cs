using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class UIPopupInviteMessageParam : UIPopupBaseParam
    {
        public UILocalizedTextInfo messageInfo;
        public bool isOnlyTextMsg = false;
        public UserData userData;
        public string senderUserName;
    }
    
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_InviteMessage")]
    public class UI_Popup_InviteMessage : UIPopupBase
    {
        [SerializeField]
        private RectTransform popupMessageButton = null;
        [SerializeField]
        private RectTransform popupMessageOnlyText = null;
        [SerializeField]
        private Animation showHideAnimation = null;
        
        [Header("---Message Button---")]
        [SerializeField]
        private Text senderUserNickNameText = null;
        [SerializeField]
        private UILocalizedText localizedText = null;
        [SerializeField]
        private UI_GuideBtn_Renewal guideBtn = null;
      

        [Header("---Message Text Only---")]
        [SerializeField]
        private Text  senderUserNickNameText_Only= null;
        [SerializeField]
        private UILocalizedText localizedText_Only = null;
        public override bool IsBlockInputKeyEvent => false;
        
        private readonly string showAnimationName = "UI_Common_InviteMessage_Show";
        private readonly string hideAnimationName = "UI_Common_InviteMessage_Hide";
        
        private readonly int showPopupTime = 5;
        private float passedTime = 0;
        private UserData userData = null;
        
        public override void OnSetup(UIPopupBaseParam param)
        {
            base.OnSetup(param);
            
            UIHandler.UIEvenHelper.AddOnReceiveInvitePopupEventHandler(Enter);
            
            guideBtn.SetData(new UI_GuideBtnData_Renewal(
                new Dictionary<eSupportedDevice, eInputControlType>()
                {
                    { eSupportedDevice.XBOX , eInputControlType.Select},
                    { eSupportedDevice.PS , eInputControlType.Select},
                },
                new Dictionary<eSupportedDevice, bool>()
                {
                    { eSupportedDevice.KEYBOARD_MOUSE, false },
                }, eGameInputEvent.None, false,"UI_BUTTON_ACCEPT",null,OnClickConfirmButton));
            
            
            if (param is UIPopupInviteMessageParam popupParam)
            {
                SetData(popupParam);
            }

            showHideAnimation.Play(showAnimationName);
        }
        public override void OnClose()
        {
            base.OnClose();
            
            UIHandler.UIEvenHelper.RemoveOnReceiveInvitePopupEventHandler(Enter);
        }
        
        public void Enter(UIPopupInviteMessageParam param)
        {
            InputHandler.AddEventListener(this);
            SetData(param);
            popupMessageButton.SetActive(true);
            showHideAnimation.Play(showAnimationName);
            if (param.isOnlyTextMsg)
            {
                popupMessageButton.SetActive(false);
                popupMessageOnlyText.SetActive(true);
            }
            else
            {
                popupMessageButton.SetActive(true);
                popupMessageOnlyText.SetActive(false);
            }
        }

        private void SetData(UIPopupInviteMessageParam param)
        {
            passedTime = 0;
            guideBtn.CachedRectTransform.SetActive(!param.isOnlyTextMsg);
            
            if (param.isOnlyTextMsg)
            {
                senderUserNickNameText_Only.text = param.senderUserName;
                localizedText_Only.LocalParams = param.messageInfo.localKeyParameters;
                localizedText_Only.LocalKey = param.messageInfo.localKey;
                popupMessageButton.SetActive(false);
                popupMessageOnlyText.SetActive(true);
            }
            else
            {
                senderUserNickNameText.text = param.senderUserName;
                localizedText.LocalParams = param.messageInfo.localKeyParameters;
                localizedText.LocalKey = param.messageInfo.localKey;
                popupMessageButton.SetActive(true);
                popupMessageOnlyText.SetActive(false);
            }

            userData = param.userData;
        }

        private void Update()
        {
            if (enabled)
            {
                passedTime += Time.deltaTime;
                if (passedTime > showPopupTime)
                {
                    popupMessageButton.SetActive(false);
                    popupMessageOnlyText.SetActive(false);
                }
            }
        }

        public void OnClickConfirmButton()
        {
            //소셜 탭 오픈 ( 그룹 UI 선택 상태로 오픈 )

            UI_Page_Social pageSocial = UIHandler.Instance.GetUI<UI_Page_Social>();
            if (!ReferenceEquals(null, pageSocial))
            {
                if (!pageSocial.gameObject.activeInHierarchy)
                {
                    UIHandler.UIEvenHelper.TriggerOnClickShowSocialPageEventHandler(eSocialType.Squad,true,userData);
                }
                else
                {
                    // 이미 열린 상태면 Group으로 갱신
                    pageSocial.SetData(new UIPageSocialParam()
                    {
                        pageSocialType = eSocialType.Squad,
                        isAutoAcceptInvite = true,
                        sendInviteUserData = userData,
                    });
                    pageSocial.CheckAutoSquadAccept();
                }
            }
            else
            {
                UIHandler.UIEvenHelper.TriggerOnClickShowSocialPageEventHandler(eSocialType.Squad,true,userData);
            }

            StartCoroutine(Hide());
        }

        public IEnumerator Hide()
        {
            InputHandler.Instance.SetActionMap(beforeActionMapType);
            showHideAnimation.Play(hideAnimationName);
            yield return new WaitForSeconds(showHideAnimation.clip.length);
            InputHandler.RemoveEventListener(this);
            popupMessageButton.SetActive(false);
            popupMessageOnlyText.SetActive(false);
        }
    }
}