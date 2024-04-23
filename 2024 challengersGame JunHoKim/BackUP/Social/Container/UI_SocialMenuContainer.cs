using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PB.ClientParts
{
    public class UI_SocialMenuContainer  :  UISocialUserBannerAccordian
    {
        [SerializeField]
        private UI_SocialTabInfoItem infoTabItem;        
        [SerializeField]
        private InputListSelector inputListSelector = null;
        [SerializeField]
        private ScrollRect scrollView;
        [SerializeField]
        private RectTransform scrollViewRectTransform;
        [SerializeField]
        private GameObject selectedObj;

        private bool isSelect = false;
        public bool IsSelect => isSelect;
        public InputListSelector InputListSelector => inputListSelector;
        private OnButtonClickEventHandler onButtonClickEventHandler = null;
        
        public float contentMaxHeight;
        public float contentPadding;
        private float prefferHeight;
        private readonly float noDataHeight = 0f;
        
        public void SetData<T>(string localKey, string countText, OnButtonClickEventHandler handler,List<T> datas, 
            string noDataLocalKey, eTabType type = eTabType.None, bool isToggleOn = false) where T : UserData
        {
            SetDefaultData(type);
            InitData(localKey, countText, datas);
            SetToggleEventHandler(handler, isToggleOn);
            SetNoDataDesc();
            inputListSelector?.Clear();
            inputListSelector?.Add(CachedGameObject);
        }

        private void SetDefaultData( eTabType type)
        {
            btn = infoTabItem.uiButton;
            accordionItem.SetData(SetScrollView, type);
            SetBtnEventHandler();
        }
        public void InitData<T>(string localKey, string countText, List<T> datas) where T : UserData
        {
            infoTabItem.SetData(localKey, countText);
            accordionItem.InitItem(datas);
            SetNoDataDesc();
        }
        private void SetToggleEventHandler(OnButtonClickEventHandler handler, bool isToggleOn)
        {
            onButtonClickEventHandler = handler;
            if (isToggle)
            {
                btn.SetOnClickEventHandler(handler, ClientConst.UI.Sound.SELECT);
                if (isToggleOn)
                {
                    OnValueChanged(btn.CachedGameObject);
                }
            }
        }
        protected override void OnAction(GameObject go)
        {
            onButtonClickEventHandler?.Invoke(btn.CachedGameObject);
        }
        protected override void OnSelect(bool select)
        {
            selectedObj.SetActive(select);
        }
        public GameObject GetButtonGameObject()
        {
            return btn.CachedGameObject;
        }
        protected override void TransitionToState(eState state)
        {
            if (state == eState.Expanded)
            {
                isSelect = true;
            }
            else
            {
                isSelect = false;
            }
            base.TransitionToState(state);
        }
        
        #region  Accordion Item Function
        
        public void SetNoDataDesc()
        {
            if (accordionItem.IsExistData())
            {
                accordionItem.UpdateScrollAndResize();
            }
            else
            {
                SetScrollViewNoData(noDataHeight);
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewRectTransform);
            }
        }
        public void ChangeLeaderData(ulong userId)
        {
            accordionItem.ChangeLeader(userId);
        }
        public int GetChildCount()
        {
            return accordionItem.TableDataCount;
        }
        public bool CanMoveChild(bool isDown, bool isStartEnd = false)
        {
            if (isDown)
            {
                return !accordionItem.IsEnd;
            }
            else
            {
                if (isStartEnd && accordionItem.SelectIdx < 0)
                {
                    return true;
                }
                return !accordionItem.IsStart;
            }
        }
        public void MoveChild(bool isDown)
        {
            if (isDown)
            {
                accordionItem.SelectIdx++;
            }
            else
            {
                if (accordionItem.IsStart)
                {
                    accordionItem.SelectIdx = accordionItem.TableDataCount;
                }
                else
                {
                    accordionItem.SelectIdx--;
                }
            }
            accordionItem.OnItemDetailClick();
        }
        
        #endregion

        #region Item Data Control Function
        
        public void UpdateData(string countText, List<UserData> datas)
        {
            infoTabItem.UpdateCountText(countText);
            accordionItem.UpdateItem(datas);
        }
        public void AddData(string countText, UserData data)
        {
            infoTabItem.UpdateCountText(countText);
            accordionItem.AddItem(data);
            SetNoDataDesc();
        }
        public void RemoveData(string countText, ulong userId)
        {
            infoTabItem.UpdateCountText(countText);
            accordionItem.RemoveItem(userId);
            btn.ClearBtnEvent();
            onButtonClickEventHandler = null;
            SetNoDataDesc();
        }
        public void RemoveAllData(string countText)
        {
            infoTabItem.UpdateCountText(countText);
            accordionItem.RemoveAllItem();
            btn.ClearBtnEvent();
            onButtonClickEventHandler = null;
            SetNoDataDesc();
        }
        public void InitSelectItem()
        {
            accordionItem.InitSelectIdx();
            accordionItem.UnSelectCurrentItem();
        }
        public void ActionCurrentItem()
        {
            accordionItem.ActionCurrentItem();
        }

        #endregion
        
        #region  Scroll Control Function 
        
        public void ForceUpdateScrollHeight()
        {
            accordionItem.ForceUpdateCellsTop();
            SetNoDataDesc();
        }
        public void SetScrollView()
        {
            if (scrollView.content.rect.height < contentMaxHeight)
            {
                scrollViewRectTransform.sizeDelta =
                    new Vector2(scrollViewRectTransform.sizeDelta.x, scrollView.content.sizeDelta.y);
                prefferHeight = scrollView.content.rect.height + verticalPadding;
            }
            else
            {
                scrollViewRectTransform.sizeDelta = new Vector2(scrollViewRectTransform.sizeDelta.x, contentMaxHeight);
                prefferHeight = contentMaxHeight + contentPadding;
            }

            accordionItem.targetHeight = prefferHeight;
            accordionItem.UpdateScrollView();
            if (currentState == eState.Expanded)
            {
                layoutElement.preferredHeight = prefferHeight + MinHeight;
            }
        }

        public void SetScrollViewNoData(float height)
        {
            scrollViewRectTransform.sizeDelta = new Vector2(scrollViewRectTransform.sizeDelta.x, height);
            prefferHeight = height + verticalPadding;
            accordionItem.targetHeight = prefferHeight;
            if (currentState == eState.Expanded)
            {
                layoutElement.preferredHeight = prefferHeight + MinHeight;
                StartTween(layoutElement.preferredHeight);
            }
            accordionItem.SetPreferredHeight();
        }

        #endregion
    }
}