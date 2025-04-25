using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public class UI_SocialUserProfileSquadItem : UIComponent
    {
        [SerializeField]
        private RawImage bannerImg = null;

        [SerializeField]
        private RawImage symbolImg = null;

        [SerializeField]
        private GameObject squadLeaderObj = null;

        private UserData linkedData = null;
        public UserData LinkedData => linkedData;

        public void SetData(UserData data, bool isSquadLeader)
        {
            if (data == null)
            {
                return;
            }

            linkedData = data;
            
            SetBanner(data.EquipBanner);
            SetSymbol(data.EquipAvatar);
            SetSquadLeader(isSquadLeader);
        }

        public void SetBanner(int banner)
        {
            bannerImg.enabled = banner != 0;
            if (banner != 0)
            {
                ClientCollectionRawData rawData = ClientTableManager.CollectionTable.GetTableCollectionRawData(banner);
                if (rawData != null)
                {
                    bannerImg.texture = ClientResourceManager.Instance.GetCollectionTexture(eCollectionType.Banner, rawData.fileName);    
                }
                else
                {
                    bannerImg.enabled = false;
                }
            }
        }

        public void SetSymbol(int symbol)
        {
            symbolImg.enabled = symbol != 0;
            if (symbol != 0)
            {
                ClientCollectionRawData rawData = ClientTableManager.CollectionTable.GetTableCollectionRawData(symbol);
                if (rawData != null)
                {
                    symbolImg.texture = ClientResourceManager.Instance.GetCollectionTexture(eCollectionType.Avatar, rawData.fileName);    
                }
                else
                {
                    symbolImg.enabled = false;
                }
            }
        }

        public void SetSquadLeader(bool isSquadLeader)
        {
            squadLeaderObj.SetActive(isSquadLeader);
        }

        public override void SetActive(bool isActive)
        {
            if (!isActive)
            {
                linkedData = null;
            }
            base.SetActive(isActive);
        }
    }
    
}