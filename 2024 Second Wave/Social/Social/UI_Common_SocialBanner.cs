using PB.ClientParts.Platform;
using PBRest.Contracts;
using PBSocialServer.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class UI_SocialBannerData
    {
        public UserData userData;
        public bool isLeader = false;
        public eTabType tabType = eTabType.None;
    }
    public class UI_Common_SocialBanner : UI_RecycleViewItem<UI_SocialBannerData>
    {
        [SerializeField]
        private bool isMine;
        [SerializeField]
        private Text userName;
        [SerializeField]
        private Text userLevel;
        [SerializeField]
        private UILocalizedText titleText;
        [SerializeField]
        private Text gamerTag;
        [SerializeField]
        private Image platformImage;
        [SerializeField]
        private Image statusImg;
        [SerializeField]
        private UILocalizedText statusText;
        [SerializeField]
        private Image selectedImg;
        [SerializeField]
        private UI_SocialUserProfileSquadItem userProfileItem;
        [SerializeField]
        private Image offlineImage;
        [SerializeField]
        private GameObject rankImage;
        
        private UserData thisSocialUserData;

        [SerializeField] 
        private eTabType tabType = eTabType.None;

        private readonly string XBoxIconPath = "Ico_Common_Platform_Xbox";
        private readonly string EtcIconPath = "Ico_Common_Platform_ETC";
        
        public void SetData(UserData data)
        {
            // 랭크 시스템이 없기 때문에 랭크 이미지를 꺼둔다.
            rankImage.SetActive(false);
            // 차단 유저 체크
            if (GamePlayUserDataManager.Instance.ContainsKeyBlockUserNickNameDic(data.Name))
            {
                userName.text = Const.String.BlockUserNic;
            }
            else
            {
                userName.text = data.Name;
                userLevel.text = data.Level.ToString();
            }

            //xbox만 아이콘이 보여야 함
            if (PlatformManager.Instance.GetPlatformType() == PlatformType.Xbox)
            {
                if (data.Type == (byte)PlatformType.Xbox)
                {
                    platformImage.sprite = ClientResourceManager.Instance.GetPlatformSprite(XBoxIconPath);
                }
                else
                {
                    platformImage.sprite = ClientResourceManager.Instance.GetPlatformSprite(EtcIconPath);
                }

                if (data is PlatformUserData platformUserData)
                {
                    gamerTag.gameObject.SetActive(true);
                    gamerTag.text = platformUserData.PlatformName;

                    if (platformUserData.IsSWPlay || platformUserData.PlayState == (byte)eUserPlayState.Offline)
                    {
                        SetStatus((eUserPlayState)platformUserData.PlayState, platformUserData.IsSWPlay);
                    }
                }
                else
                {
                    gamerTag.gameObject.SetActive(false);
            
                    SetStatus((eUserPlayState)data.PlayState);
                }
            }
            else
            {
                SetStatus((eUserPlayState)data.PlayState);
                platformImage.SetActive(false);
                gamerTag.gameObject.SetActive(false);
            }
            
            // 유저 정보 세팅
            thisSocialUserData = data;
            userProfileItem.SetData(data,LobbyUserData.Instance.SocialData.Id == thisSocialUserData.Id);
            
            if (data.EquipTitle != 0)
            {
                ClientCollectionRawData rawData = ClientTableManager.CollectionTable.GetTableCollectionRawData(data.EquipTitle);
                    
                if (rawData != null)
                {
                    if (rawData.index == 400000)
                    {
                        titleText.Text = string.Empty;
                    }
                    else
                    {
                        titleText.LocalKey = rawData.nameLocalKey;
                    }
                }
                else
                {
                    titleText.Text = string.Empty;
                }
            }
        }
        public override void UpdateContent(UI_SocialBannerData itemData)
        {
            tabType = itemData.tabType;
            SetData(itemData.userData);
            if (tabType == eTabType.Group)
            {
                userProfileItem.SetData(itemData.userData, itemData.isLeader);
            }
        }
        private void SetStatus(eUserPlayState state, bool isSwPlay = true)
        {
            if (state == eUserPlayState.Offline)
            {
                statusImg.sprite = ClientResourceManager.Instance.GetRenewalCommonSprite("Ico_Square_01");
                statusText.LocalKey = "UI_SOCIAL_OFFLINE";
                offlineImage.SetActive(true);
                return;
            }
            
            statusImg.sprite = ClientResourceManager.Instance.GetRenewalCommonSprite("Ico_Square_02");
            offlineImage.SetActive(false);

            if (!isSwPlay)
            {
                statusText.LocalKey = "UI_SOCIAL_ONLINE";
                return;
            }

            eGameMode mode = eGameMode.None;
            
            switch (state)
            {
                case eUserPlayState.Domination:
                    mode = eGameMode.DominationMode;
                    break;
                case eUserPlayState.Conquer:
                    mode = eGameMode.ConquerMode;
                    break;
                case eUserPlayState.StoneGrab:
                    mode = eGameMode.StoneGrabMode;
                    break;
                case eUserPlayState.Escort:
                    mode = eGameMode.EscortMode;
                    break;
                default:
                    statusText.LocalKey = "UI_SOCIAL_PLAYING_LOBBY";
                    return;
            }
            statusText.LocalKey = "UI_SOCIAL_PLAYING_NORMAL";
            statusText.LocalParams = new object[] { UILocalizedText.GetLocalizationText(UIUtil.GetGameModeLocalKey(mode)) };
        }
        public void UpdateLevel(int level)
        {
            userLevel.text=level.ToString();
        }
        protected override void OnSelect(bool isSelect)
        {
            OnHover(isSelect);
            selectedImg.SetActive(isSelect);
        }
        protected override void OnAction(GameObject go)
        {
            OnClick();
        }
        protected override void OnClick()
        {
            base.OnClick();
            
            // 유저 메뉴 팝업 
            UIPopupUserMenuParam param = new UIPopupUserMenuParam();
            param.targetRectTransform = CachedRectTransform;
            param.isMe = isMine || thisSocialUserData.Id == LobbyUserData.Instance.SocialData.Id;
            param.targetRectTransform = CachedRectTransform;
            param.isOffline = thisSocialUserData.PlayState == (byte)eUserPlayState.Offline;
            param.targetUserData = thisSocialUserData;
            param.tabType = tabType;
            param.IsSystemPopup = true;
          
            param = LobbyUserData.Instance.FillParamDataFriend(param, thisSocialUserData.Id);
            param = LobbyUserData.Instance.FillParamDataGroup(param, thisSocialUserData.Id);
            
            param.sortingOrder = UIHandler.CalcSortingOrder(70);
   
            UIHandler.Instance.LoadUI<UI_Popup_UserMenu>(param, null,true);    
        }
        protected override void OnHover(bool isHover)
        {
            if (isHover)
            {
                SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.FOCUS_LIST);
            }
            if (!isMine)
            {
                selectedImg.SetActive(isHover);
            }
        }
    }
}