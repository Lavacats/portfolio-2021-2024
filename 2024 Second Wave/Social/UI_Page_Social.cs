using UnityEngine;
using System.Collections.Generic;
using PBRest.Contracts;
using PB.ClientParts.Platform;

namespace PB.ClientParts
{
    public enum eSocialType
    {
        None = -1,
        Squad = 0,
        Chat = 1,
        Friend = 2,
        PlatformFriend = 3
    }
    
    public delegate void OnClickTabChangeEventHandler(eSocialTabItemType type);
        
    public class UIPageSocialParam : UIPageBaseParam
    {
        public eSocialType pageSocialType;
        public bool isAutoAcceptInvite = false;
        public UserData sendInviteUserData = null;
        public UIPageBase prevPage = null;
    }
    
    [PrefabPath("UI_Renewal/Prefab/Page/UI_Page_Social", InputHandler.eActionMapType.Lobby)]
    public class UI_Page_Social : UIPageBase
    {
        [Header("---UI Page Social---")]
        [SerializeField] 
        private UI_SocialTabController_Renewal socialTabController;
        [SerializeField] 
        private UI_SocialSquadController socialSquadContainer;
        [SerializeField] 
        private UI_SocialFriendController socialFriendContainer;
        [SerializeField] 
        private UI_SocialConsoleFriendController socialConsoleFriendContainer;
        [SerializeField]
        private Animation animationUI = null;
        
        [Space]
        [SerializeField]
        private UI_GuideBtnController_Renewal consoleGuideBtnController;
        [SerializeField] 
        private UI_GuideBtnController_Renewal pcGuideBtnController;
        
        private eSocialType currentType = eSocialType.Squad;
        private PlatformType currentPlatform = PlatformType.None;
        
        private static readonly string HIDE_ANIMATION_NAME = "UI_Page_Social_Hide";
        
        private List<UI_GuideBtnData_Renewal> pcGuideBtnDataList = null;
        private List<UI_GuideBtnData_Renewal> consoleGuideBtnDataList = null;
        
        private  bool isAutoAcceptInvite = false;
        private  UserData sendInviteUserData = null;
        private UIPageBase prevPage = null;
        public override bool IsTopListener => true;
        public override bool IsBlockInputKeyEvent => true;

        protected override void OnSetup(UIPageBaseParam param)
        {
            SetData(param);
            
            SetGuidButtonData();
            
            socialTabController.SetData(ChangeSocialTab);
            OnDeviceChange();
            InputHandler.AddEventListener(this);
     
            socialSquadContainer.SetData(new SocialSquadControllerData()
            {
                guideBtnControllerForKeyboardMouse = pcGuideBtnController,
                guideBtnControllerForConsole = consoleGuideBtnController,
                onSocialViewCloseEventHandler = MovePrevPage
            });
            socialFriendContainer.SetData(new SocialFriendControllerData()
                {
                    guideBtnControllerForKeyboardMouse = pcGuideBtnController,
                    guideBtnControllerForConsole = consoleGuideBtnController,
                    onSocialViewCloseEventHandler = MovePrevPage
                });
            socialConsoleFriendContainer.SetData(new SocialConsoleFriendControllerData()
            {
                guideBtnControllerForConsole = consoleGuideBtnController,
                onSocialViewCloseEventHandler = MovePrevPage
            });
            ChangeSocialTab(eSocialTabItemType.Friend);
            CheckAutoSquadAccept();
        }

        public void SetData(UIPageBaseParam param)
        {
            if (param is UIPageSocialParam pageParam)
            {
                currentType = pageParam.pageSocialType;
                this.isAutoAcceptInvite = pageParam.isAutoAcceptInvite;
                this.sendInviteUserData = pageParam.sendInviteUserData;
                this.prevPage = pageParam.prevPage;
            }
        }

        public override void OnClose()
        {
            socialSquadContainer.RemoveEventHandler(MovePrevPage);
            socialFriendContainer.RemoveEventHandler(MovePrevPage);
            socialConsoleFriendContainer.RemoveEventHandler(MovePrevPage);
            base.OnClose();
        }
        private void SetGuidButtonData()
        {
            consoleGuideBtnDataList = new List<UI_GuideBtnData_Renewal>()
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
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_UNFOLD", null, null),
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
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_EXIT_GROUP", null, null),
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
                    eGameInputEvent.None, false, "UI_BUTTON_CLOSE", null, null)
            };
            
            pcGuideBtnDataList = new List<UI_GuideBtnData_Renewal>()
            {
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE , eInputControlType.None},
                    }, new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false},
                        { eSupportedDevice.PS, false },
                        { eSupportedDevice.XBOX, false }
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_EXIT_GROUP", null, null),
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE , eInputControlType.None},
                    }, new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false},
                        { eSupportedDevice.PS, false },
                        { eSupportedDevice.XBOX, false }
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_ADDFRIEND", null, null),
                new UI_GuideBtnData_Renewal(
                    new Dictionary<eSupportedDevice, eInputControlType>()
                    {
                        { eSupportedDevice.KEYBOARD_MOUSE , eInputControlType.Escape},
                    }, new Dictionary<eSupportedDevice, bool>()
                    {
                        { eSupportedDevice.ANDROID, false},
                        { eSupportedDevice.PS, false },
                        { eSupportedDevice.XBOX, false }
                    },
                    eGameInputEvent.None, false, "UI_BUTTON_CLOSE", null, null)
            };
        }
        public override bool OnInputEvent(eInputControlType type, bool isPressed, Vector2 movement)
        {
            if (isPressed && GameSettings.CurrentDevice != eSupportedDevice.KEYBOARD_MOUSE)
            {
                switch (type)
                {
                    case eInputControlType.LeftBumper: //스쿼드
                    {
                        socialTabController.SelectSocialTabItem(eSocialTabItemType.Squad);
                        currentType = eSocialType.Squad;
                        return true;
                    }
                    case eInputControlType.RightBumper: //소셜
                    {
                        socialTabController.SelectSocialTabItem(eSocialTabItemType.Friend);
                        currentType = eSocialType.Friend;
                        return true;
                    }
                }
                if (PlatformManager.Instance.GetPlatformType() == PlatformType.Xbox)
                {
                    if (currentType == eSocialType.Friend || currentType == eSocialType.PlatformFriend)
                    {
                        switch (type)
                        {
                            case eInputControlType.LeftTrigger: //엑박친구
                            {
                                socialTabController.SelectSocialTabItem(eSocialTabItemType.XboxFriend);
                                currentType = eSocialType.PlatformFriend;
                                return true;
                            }
                            case eInputControlType.RightTrigger: //sw친구
                            {
                                socialTabController.SelectSocialTabItem(eSocialTabItemType.SWFriend);
                                currentType = eSocialType.Friend;
                                return true;
                            }
                        }
                    }
                }
            }
            else if (isPressed && GameSettings.CurrentDevice == eSupportedDevice.KEYBOARD_MOUSE)
            {
                switch (type)
                {
                    case eInputControlType.Q: //스쿼드
                    {
                        socialTabController.SelectSocialTabItem(eSocialTabItemType.Squad);
                        return true;
                    }
                    case eInputControlType.E: //소셜
                    {
                        socialTabController.SelectSocialTabItem(eSocialTabItemType.Friend);
                        return true;
                    }
                    case eInputControlType.Escape:
                    case eInputControlType.East:
                    {
                        MovePrevPage();
                        return true;
                    }
                }    
            }
            
            if (currentType == eSocialType.Squad)
            {
                socialSquadContainer.OnInputEvent(type, isPressed, movement);
            }
            else if (currentType == eSocialType.Friend)
            {
                socialFriendContainer.OnInputEvent(type, isPressed, movement);
            }
            else if (currentType == eSocialType.PlatformFriend)
            {
                socialConsoleFriendContainer.OnInputEvent(type, isPressed, movement);
            }
            return false;
        }

        public void CheckAutoSquadAccept()
        {
            if (isAutoAcceptInvite)
            {
                ChangeSocialTab(eSocialTabItemType.Squad);
                UIHandler.UIEvenHelper.TriggerOnReceiveChangeOpenGroupAccordionItemEventHandler(eAccordionType.ReceiveList);//그룹탭에서 받은 친구요청 아코디언 열기 
                UIHandler.UIEvenHelper.TriggerOnAutoGroupAcceptHandler(sendInviteUserData);//자동 그룹 수락 이벤트
            }
        }
        
        private void ChangeSocialTab(eSocialTabItemType type)
        {
            socialTabController.ChangeLabel(type);
            switch (type)
            {
                case eSocialTabItemType.Friend:
                {
                    currentType = eSocialType.Friend;
                    socialSquadContainer.CachedGameObject.SetActive(false);
                    socialFriendContainer.Enter();
                    socialFriendContainer.ChangeGuidButtonList(new SocialFriendControllerData()
                    {
                        guideBtnControllerForKeyboardMouse = pcGuideBtnController,
                        guideBtnControllerForConsole = consoleGuideBtnController,
                        onSocialViewCloseEventHandler = MovePrevPage
                    });
                    break;
                }
                case eSocialTabItemType.Squad:
                {
                    currentType = eSocialType.Squad;
                    socialSquadContainer.Enter();
                    socialFriendContainer.CachedGameObject.SetActive(false);
                    socialConsoleFriendContainer.CachedGameObject.SetActive(false);
                    socialSquadContainer.ChangeGuidButtonList(new SocialSquadControllerData()
                    {
                        guideBtnControllerForKeyboardMouse = pcGuideBtnController,
                        guideBtnControllerForConsole = consoleGuideBtnController,
                        onSocialViewCloseEventHandler = MovePrevPage
                    });
                    break;
                }
                case eSocialTabItemType.XboxFriend:
                {
                    currentType = eSocialType.PlatformFriend;
                    socialFriendContainer.CachedGameObject.SetActive(false);
                    socialConsoleFriendContainer.Enter();
                    break;
                }
                case eSocialTabItemType.SWFriend:
                {
                    currentType = eSocialType.Friend;
                    socialFriendContainer.Enter();
                    socialConsoleFriendContainer.CachedGameObject.SetActive(false);
                    break;
                }
            }
        }

        private void OnDeviceChange()
        { 
            switch (GameSettings.CurrentDevice)
            {
                case eSupportedDevice.PS:
                case eSupportedDevice.XBOX:
                    consoleGuideBtnController.SetActive(true);
                    pcGuideBtnController.SetActive(false);
                    consoleGuideBtnController.SetData(consoleGuideBtnDataList);
                    break;
                case eSupportedDevice.KEYBOARD_MOUSE:
                    consoleGuideBtnController.SetActive(false);
                    pcGuideBtnController.SetActive(true);
                    pcGuideBtnController.SetData(pcGuideBtnDataList);
                    break;
            }
        }

        private void MovePrevPage()
        {
            if (linkedSceneBase is not UI_LobbyScene_Renewal lobbyScene)
            {
                CGLog.LogError("Linked Scene Error");
                return;
            }
            lobbyScene.ViewPrevSocialPage(prevPage);
        }
    }
}