using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PB.ClientParts.Rest;
using PBRest.Contracts;

namespace PB.ClientParts
{
    public class UIPopupBlockListParam : UIPopupBaseParam
    {
        public List<RestBlockUserInfo> blockUserInfoList = null;
        
    }
    [PrefabPath("UI_Renewal/Prefab/Popup/UI_Popup_BlockList")]
    public class UI_Popup_BlockList : UIPopupBase
    {
        [SerializeField]
        private UILocalizedText titleText = null;
        [SerializeField]
        private InputListSelector inputListSelector = null;
        [SerializeField]
        private List<UI_UserBlockItem_Renewal> userBlockItemList = null;
        [SerializeField]
        private UILocalizedText pageText = null;
        [SerializeField]
        private UILocalizedText userCountText = null;
        [SerializeField]
        private UI_GuideBtnController_Renewal guideBtnControllerConsole = null;
        [SerializeField]
        private UI_GuideBtnController_Renewal guideBtnControllerPC = null;
        
        [SerializeField]
        private GameObject leftArrowImageConsole = null;
        [SerializeField]
        private GameObject rightArrowImageConsole = null;

        [SerializeField]
        private Image leftArrowImage = null;
        [SerializeField]
        private UIButton leftArrowBtn = null;
        [SerializeField]
        private Image rightArrowImage = null;
        [SerializeField]
        private UIButton rightArrowBtn = null;

        public InputListSelector InputListSelector => inputListSelector;
        private List<RestBlockUserInfo> blockUserInfoDataList = new List<RestBlockUserInfo>();

        private List<UI_GuideBtnData_Renewal> dataForConsole = new List<UI_GuideBtnData_Renewal>();
        private List<UI_GuideBtnData_Renewal> dataForKeyboardMouse = new List<UI_GuideBtnData_Renewal>();
        
        private UI_UserBlockItem_Renewal currentItem = null;

        private int pageMaxCount = 0;
        private int pageCurrentCount = 0;
        private int currentListCount = 0;
        private bool isSelectState = false;


        private readonly string deactiveArrowColor = "#3b4049";
        private readonly string activeArrowColor = "#fffff2";

        public override void OnSetup(UIPopupBaseParam param)
        {
            pageCurrentCount = 0;
            pageMaxCount = 0;
            currentListCount = 0;
            currentItem = null;
            isSelectState = false;
            
            titleText.LocalKey = "UI_BLOCK_LIST";
            
            blockUserInfoDataList.Clear();
            if (param is UIPopupBlockListParam popupBlockListParam)
            {
                if (popupBlockListParam.blockUserInfoList != null)
                {
                    for (int i = 0; i < popupBlockListParam.blockUserInfoList.Count; ++i)
                    {
                        RestBlockUserInfo info = new RestBlockUserInfo();
                        info.Name = popupBlockListParam.blockUserInfoList[i].Name;
                        info.TargetId = popupBlockListParam.blockUserInfoList[i].TargetId;
                        blockUserInfoDataList.Add(info);
                    }

                    SettingUserBlockItemList();
                }
                else
                {
                    CGLog.Log("[UI_UserBlockListContainer] blockUserInfoDataList is Null");
                    for (int i = 0; i < userBlockItemList.Count; ++i)
                    {
                        userBlockItemList[i].SetActive(false);
                    }

                    pageText.Text = $"{1}<color=#565d66>/{1}</color>";
                    userCountText.Text = $"{0} / {100}";
                }
            }

            leftArrowBtn.ClearBtnEvent();
            rightArrowBtn.ClearBtnEvent();
            leftArrowBtn.SetOnClickEventHandler(ClickLeftArrowBtn);
            rightArrowBtn.SetOnClickEventHandler(ClickRightArrowBtn);
            leftArrowImage.color = Util.Color.ToColor(deactiveArrowColor);
            rightArrowImage.color = Util.Color.ToColor(activeArrowColor);
            
            BtnDataInit();
            guideBtnControllerConsole.SetData(dataForConsole);
            guideBtnControllerPC.SetData(dataForKeyboardMouse);
            OnDeviceChange();
        }

        public void SettingUserBlockItemList()
        {
            inputListSelector.Clear();
            
            pageMaxCount = blockUserInfoDataList.Count / 10;
            if (blockUserInfoDataList.Count % 10 > 0)
            {
                ++pageMaxCount;
            }

            for (int i = 0; i < userBlockItemList.Count; ++i)
            {
                userBlockItemList[i].SetActive(false);
            }

            //차단 유저가 하나라도 없으면 return
            if (pageMaxCount == 0)
            {
                pageText.Text = $"{pageCurrentCount+1}<color=#565d66>/{pageMaxCount+1}</color>";
                userCountText.Text = $"{blockUserInfoDataList.Count} / {100}";
                return;
            }

            if (pageCurrentCount == pageMaxCount)
            {
                --pageCurrentCount;
            }

            currentListCount = pageCurrentCount * 10;

            for (int i = 0; i < userBlockItemList.Count; ++i)
            {
                if (currentListCount > blockUserInfoDataList.Count - 1) break;
                userBlockItemList[i].SetData(currentListCount, blockUserInfoDataList[currentListCount].Name,
                    blockUserInfoDataList[currentListCount].TargetId, SetCurrentBlockItem);
                userBlockItemList[i].SetActive(true);
                inputListSelector.Add(userBlockItemList[i].CachedGameObject);
                
                ++currentListCount;
            }
            pageText.Text = $"{pageCurrentCount+1}<color=#565d66>/{pageMaxCount}</color>";
            userCountText.Text = $"{blockUserInfoDataList.Count} / {100}";
        }

        public override bool OnInputEvent(eInputControlType types, bool isPressed, Vector2 movement)
        {
            if (isPressed)
            {
                switch (types)
                {
                    case eInputControlType.LeftStickDown:
                    case eInputControlType.DPadDown:
                    case eInputControlType.DPadUp:
                    case eInputControlType.LeftStickUp:
                    {
                        if (blockUserInfoDataList == null || blockUserInfoDataList.Count == 0) break;

                        inputListSelector.OnInputEvent(types.ChangeInputListSelectorType());
                        break;
                    }

                    case eInputControlType.DPadLeft:
                    {
                        ClickLeftArrowBtn(null);
                        break;
                    }
                    case eInputControlType.DPadRight:
                    {
                        ClickRightArrowBtn(null);
                        break;
                    }

                    case eInputControlType.South:
                    {
                        UnBlockBtnClick();
                        break;
                    }

                    case eInputControlType.East:
                    {
                        OnClose();
                        break;
                    }
                }
            }

            return false;
        }

        private void ClickLeftArrowBtn(GameObject go)
        {
            if (blockUserInfoDataList == null || blockUserInfoDataList.Count == 0) return;
            
            if (pageCurrentCount > 0)
            {
                --pageCurrentCount;
                SettingUserBlockItemList();
                if (pageCurrentCount == 0)
                {
                    leftArrowImage.color = Util.Color.ToColor(deactiveArrowColor);
                }
            }

            if (pageMaxCount-1 > pageCurrentCount)
            {
                rightArrowImage.color = Util.Color.ToColor(activeArrowColor);
            }
        }
        
        private void ClickRightArrowBtn(GameObject go)
        {
            if (blockUserInfoDataList == null || blockUserInfoDataList.Count == 0) return;
            
            if (pageMaxCount-1 > pageCurrentCount)
            {
                ++pageCurrentCount;
                SettingUserBlockItemList();
                
                if (pageCurrentCount == pageMaxCount-1)
                {
                    rightArrowImage.color = Util.Color.ToColor(deactiveArrowColor);
                }
            }

            if (pageCurrentCount > 0 )
            {
                leftArrowImage.color = Util.Color.ToColor(activeArrowColor);
            }
        }

        private void BtnDataInit()
        {
            dataForConsole = new List<UI_GuideBtnData_Renewal>()
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
                    }, eGameInputEvent.None, false, "UI_BUTTON_UNBLOCK", null, UnBlockBtnClick),
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
                    }, eGameInputEvent.None,false, "UI_BUTTON_UNBLOCK",null, UnBlockBtnClick),
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

        private void SetCurrentBlockItem(UI_UserBlockItem_Renewal item)
        {
            if (currentItem == item) return;
            
            if (currentItem != null)
            {
                currentItem.SetEnableImage(false);
            }
            currentItem = item;

            if (isSelectState == false)
            {
                isSelectState = true;
            }
        }

        private void UnBlockBtnClick()
        {
            //차단 해제 서버에 요청 하고 리스트에서 삭제
            if (blockUserInfoDataList.Count == 0 || blockUserInfoDataList == null) return;

            if (currentItem != null )
            {
                ClientRestConnectManager.Instance.StartConnectCoroutine(RestAPI.ReqReleaseBlockUser(currentItem.UserId, (resp) =>
                {
                    if (resp.ReturnCode == (int)PBRestReturnCode.SUCCESS)
                    {
                        UIPopupSystemMessageParam param = new UIPopupSystemMessageParam();
                        param.messageInfo = new UILocalizedTextInfo("SYS_BLOCK_USER_UNBLOCKED", currentItem.GetUserNicName());
                        param.sortingOrder = UIHandler.CalcSortingOrder(95);
                        UIHandler.Instance.LoadUI<UI_Popup_SystemMessage>(param, null, true);
                        
                        
                        
                        blockUserInfoDataList.RemoveAt(currentItem.DataNumber);
                        GamePlayUserDataManager.Instance.RemoveBlockUserInfoList(currentItem.UserId, currentItem.GetUserNicName());
                        currentItem = null;
                        isSelectState = false;
                        //ChangeGuideBtn();
                        SettingUserBlockItemList();     
                    }
                    else
                    {
                        CGLog.LogError($"[UI_UserBlockListContainer] Not SUCCESS ReqReleaseBlockUser : {resp.ReturnCode}");
                    }

                }));
            }
        }

        private void OnDeviceChange()
        {
            switch (GameSettings.CurrentDevice)
            {
                case eSupportedDevice.PS:
                case eSupportedDevice.XBOX:
                    guideBtnControllerConsole.SetActive(true);
                    guideBtnControllerPC.SetActive(false);
                    leftArrowImageConsole.SetActive(true);
                    rightArrowImageConsole.SetActive(true);
                    break;
                case eSupportedDevice.KEYBOARD_MOUSE:
                case eSupportedDevice.ANDROID:
                case eSupportedDevice.IOS:
                    guideBtnControllerConsole.SetActive(false);
                    guideBtnControllerPC.SetActive(true);
                    leftArrowImageConsole.SetActive(false);
                    rightArrowImageConsole.SetActive(false);
                    break;
            }
        }
    }
}