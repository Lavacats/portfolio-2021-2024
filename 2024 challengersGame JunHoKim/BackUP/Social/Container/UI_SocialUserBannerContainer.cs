using System.Collections.Generic;
using Beebyte.Obfuscator;
using PBSocialServer.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace PB.ClientParts
{
    public delegate void OnResizeSocialUserBannerEventHandler();
    public class UI_SocialUserBannerContainer :  UIRecycleViewController<UI_SocialBannerData>
    {        
        [Header("-------------UI Social User Banner Container---------------")]
        [SerializeField]
        private bool isMoveToFront = true;
        
        public LayoutElement layoutElement;
        public float targetHeight;

        private OnResizeSocialUserBannerEventHandler onResizeSocialUserBannerEventHandler;
        private eTabType tabType = eTabType.None;
        
        [SkipRename] readonly string FUNC_ON_ACTION = "OnAction";
        [SkipRename] readonly string FUNC_ON_SELECT = "OnSelect";
        
        public void SetData(OnResizeSocialUserBannerEventHandler resizeSocialUserBannerEventHandler, eTabType type = eTabType.None)
        {
            tableData.Clear();
            tabType = type;
            layoutElement.preferredHeight = 0f;
            onResizeSocialUserBannerEventHandler = resizeSocialUserBannerEventHandler;
            onResizeSocialUserBannerEventHandler?.Invoke();
            SetPreferredHeight();
        }
        public void OnItemDetailClick()
        {
            if (tableData.Count <= SelectIdx || SelectIdx < 0)
            {
                return;
            }
            // prev Item
            UnSelectCurrentItem();
            
            // current Item
            if (GetSelectCell(SelectIdx) is UI_Common_SocialBanner item)
            {
                item.SendMessage(FUNC_ON_SELECT, true, SendMessageOptions.DontRequireReceiver);
            }
        }
        public void UnSelectCurrentItem()
        {
            if (GetPreviousSelectCell() is UI_Common_SocialBanner item)
            {
                item.SendMessage(FUNC_ON_SELECT, false, SendMessageOptions.DontRequireReceiver);
            }
        }
        public void ActionCurrentItem()
        {
            if (tableData.Count <= SelectIdx || SelectIdx < 0)
            {
                return;
            }
            if (GetSelectCell(SelectIdx) is UI_Common_SocialBanner item)
            {
                if (item is not null)
                {
                    item.SendMessage(FUNC_ON_ACTION, item.CachedGameObject, SendMessageOptions.DontRequireReceiver);
                    UpdateScrollView();
                }   
            }
        }
        public void ChangeLeader(ulong userId)
        {
            if (tableData.Count > 0)
            {
                int idx = tableData.FindIndex(x => x.isLeader);
                if (idx >= 0)
                {
                    tableData[idx].isLeader = false;
                }
                idx = tableData.FindIndex(x => x.userData.Id == userId);
                if (idx >= 0)
                {
                    tableData[idx].isLeader = true;
                }
                UpdateScrollAndResize();   
            }
        }

        #region Item Data Control Function
        public void InitItem<T>(List<T> userDataList) where T : UserData
        {
            if (ReferenceEquals(userDataList, null) || tableData.Count > 0)
            {
                return;
            }
            for (int i = 0; i < userDataList.Count; i++)
            {
                UI_SocialBannerData eachParam = new UI_SocialBannerData();

                eachParam.userData = userDataList[i];
                eachParam.tabType = tabType;
                eachParam.isLeader = LobbyUserData.Instance.IsSquadLeader(userDataList[i].Id);
            
                tableData.Add(eachParam);
                if (isMoveToFront)
                {
                    if (userDataList[i].PlayState != (byte)eUserPlayState.Offline)
                    {
                        MoveToFront(userDataList[i].Id);
                    }
                }
            }
            if (userDataList.Count > 0)
            {
                UpdateScrollAndResize();   
            }
        }
        public void AddItem(UserData userData)
        {
            int idx = tableData.FindIndex(x => x.userData.Id == userData.Id);
            if (idx >= 0)
            {
                return;
            }
            UI_SocialBannerData param = new UI_SocialBannerData();
            param.userData = userData;
            param.tabType = tabType;
            param.isLeader = LobbyUserData.Instance.IsSquadLeader(userData.Id);
            
            tableData.Add(param);

            if (isMoveToFront)
            {
                if (userData.PlayState != (byte)eUserPlayState.Offline)
                {
                    MoveToFront(userData.Id);
                }
            }
            UpdateScrollAndResize();
        }
        public void UpdateItem(List<UserData> userDataList)
        {
            if (ReferenceEquals(userDataList, null))
            {
                return;
            }
            for (int i = 0; i < userDataList.Count; i++)
            {
                int idx = tableData.FindIndex(x => x.userData.Id == userDataList[i].Id);
                if (idx < 0)
                {
                    UI_SocialBannerData eachParam = new UI_SocialBannerData();
                    eachParam.userData = userDataList[i];
                    eachParam.tabType = tabType;
                    eachParam.isLeader = LobbyUserData.Instance.IsSquadLeader(userDataList[i].Id);
            
                    tableData.Add(eachParam);
                }
                else
                {
                    tableData[idx].userData = userDataList[i];
                }

                if (isMoveToFront)
                {
                    if (userDataList[i].PlayState == (byte)eUserPlayState.Offline)
                    {
                        MoveToEnd(userDataList[i].Id);
                    }
                    else
                    {
                        MoveToFront(userDataList[i].Id);
                    }
                }
            }
            if (userDataList.Count > 0)
            {
                UpdateScrollAndResize();   
            }
        }
        public void RemoveItem(ulong userId)
        {
            int idx = tableData.FindIndex(x => x.userData.Id == userId);
            if (idx < 0)
            {
                return;
            }
            tableData.RemoveAt(idx);
            
            UpdateScrollAndResize();
        }
        public void RemoveAllItem()
        {
            tableData.Clear();
            
            UpdateScrollAndResize();
        }
        private void MoveToFront(ulong userId)
        {
            int idx = tableData.FindIndex(x => x.userData.Id == userId);
            if (idx >= 0)
            {
                UI_SocialBannerData param = tableData[idx];
                tableData.RemoveAt(idx);
                tableData.Insert(0, param);
            }
        }
        private void MoveToEnd(ulong userId)
        {
            int idx = tableData.FindIndex(x => x.userData.Id == userId);
            if (idx >= 0)
            {
                UI_SocialBannerData param = tableData[idx];
                tableData.RemoveAt(idx);
                tableData.Add(param);
            }
        }
        public bool IsExistData()
        {
            if (tableData.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion
        
        #region Scroll Control Function
        
        public void UpdateScrollView()
        {
            InitializeTableView();
            ForceUpdateCells();
        }
        public void UpdateScrollAndResize()
        {
            UpdateScrollView();
            onResizeSocialUserBannerEventHandler?.Invoke();
            SetPreferredHeight();
        }
        public void SetPreferredHeight()
        {
            if (layoutElement == null)
                return;
			
            layoutElement.preferredHeight = targetHeight;
        }
        protected override float GetCellHeightAtIndex(int index)
        {
            float cellHeight = 0f;
            if (!ReferenceEquals(null, cellBase))
            {
                RectTransform curCellRect = cellBase.GetComponent<RectTransform>();
                if (!ReferenceEquals(null, curCellRect))
                {
                    cellHeight = curCellRect.rect.height;
                }
            }
            return cellHeight;
        }
        
        #endregion
    }
}