using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EVENT;
using DEFINE;
using Recycler;
using FlatBuffers;
using BaseTable;
using NLibCs;
using Contents.User;
using PROTOCOL.COMMON;
using PROTOCOL.FLATBUFFERS;
using PROTOCOL.GAME.ID;
using Object = UnityEngine.Object;
public class UseEventRewardInfo 
{
    public UIButton rewardButton;
    public GameObject parentObjcet;
    public UISprite rewardBackGround;
    public UISprite rewardSelectBackGround;

    private UISheet _sheetReward;
    private NrUseEventData rewardList;

    public int _eventKind = 0;
    
    public UseEventRewardInfo(int eventKind,int groupKind,int eventPoint,GameObject _obj)
    {
        rewardList = TableUseEventInfo.Instance.GetEventInfo(groupKind,(int)eventKind);

        _eventKind = eventKind;

        parentObjcet = GameObject.Instantiate(_obj);
        _sheetReward = parentObjcet.GetComponent<UISheet>();
        rewardButton = parentObjcet.GetComponent<UIButton>();

        rewardBackGround = parentObjcet.Ex_FindChildRecursive("Sprite_ItemReceive").GetComponent<UISprite>();
        rewardSelectBackGround = parentObjcet.Ex_FindChildRecursive("Sprite_SelectBack").GetComponent<UISprite>();

        parentObjcet.transform.parent = _obj.transform.parent;
        parentObjcet.transform.localPosition = _obj.transform.localPosition;
        parentObjcet.transform.localScale = new Vector3(1, 1, 1);

        _sheetReward.Initialize();
        _sheetReward.RegisterHandlerValidator<UseEventRewardInfo.RewardSheetBlock>(UseEventRewardInfo.RewardSheetBlock.OBJECTID);

        // 이벤트 정보 추가
        SetEventItemInfo(eventKind, eventPoint, groupKind);
    }
    public void SetEventItemInfo(int eventKind, int eventPoint, int groupKind)
    {
        int curBlockPoint = TableUseEventInfo.Instance.GetPointGrade((int)eventKind);
        var receiveReward = UseEventManager.Instance.dicUseRewardEvent;
        bool isOverPoint = false;
        bool isReceived = false;

        if (eventPoint >= curBlockPoint)
        {
            if (rewardSelectBackGround)
            {
                rewardSelectBackGround.SetActive(true);
                isOverPoint = true;
            }
        }
 
        if (receiveReward.ContainsKey(groupKind))
        {
            List<int> rewardListvalue = new List<int>();
            receiveReward.TryGetValue(groupKind, out rewardListvalue);
            foreach (var rewarGrade in rewardListvalue)
            {
                if (rewarGrade == curBlockPoint)
                {
                    if (rewardSelectBackGround) rewardSelectBackGround.SetActive(false);
                    if (rewardBackGround) rewardBackGround.SetActive(true);

                    isReceived = true;
                }
            }
        }

        for (int i = 0; i < rewardList.vecReward.Count; i++)
        {
            var data = _sheetReward.AddItem<UseEventRewardInfo.ItemData>();

            var rewardItemInfo = TableItemInfo.Instance.Get(rewardList.vecReward[i].Key);
            if (rewardItemInfo == null) continue;

            data.useEventItemKind = rewardItemInfo.itemKind;
            data.useEventItemValue = rewardItemInfo.itemvalue1;
            data.useEventItemCount = rewardList.vecReward[i].Value;
            data.useEventKind = eventKind;
            data.usePointGrade = curBlockPoint;
            data.useEventGroupKind = groupKind;

            if (isOverPoint && !isReceived)
            {
                data.isReceiveReward = true;
            }
            else
            {
                data.isReceiveReward = false;
            }
        }
        _sheetReward.UpdateAllItems();
    }
    public float GetRewardPosition_Y()
    {
        float position_Y = 0;
        if(_sheetReward)position_Y = _sheetReward.gameObject.transform.localPosition.y;
        return position_Y;
    }

    public void ChangePos(float posX)
    {
        if (parentObjcet != null && _sheetReward != null)
        {
            _sheetReward.m_scrollView.transform.SetLayersRecursively("NGUI_2D_Overlay");
            _sheetReward.m_scrollView.panel.depth = parentObjcet.transform.parent.GetComponent<UIPanel>().depth + 10;
            _sheetReward.m_scrollView.enabled = false;

            parentObjcet.transform.localPosition = parentObjcet.transform.localPosition + new Vector3(0, -posX, 0);
        }
    }
    public void RefreshUI(int eventKind)
    {
        if (_sheetReward)
        {
            for (var i = 0; i < _sheetReward.itemCount; ++i)
            {
                var block = _sheetReward.GetSheetBlockHandler<UseEventRewardInfo.RewardSheetBlock>(i);
                if (null == block)
                    continue;
                if (_eventKind == eventKind)
                {
                    if (rewardSelectBackGround) rewardSelectBackGround.SetActive(false);
                    block.ReceiveItemUI();

                }
            }
            _sheetReward.UpdateAllItems();
        }
    }

    public void Release()
    {
        Object.DestroyImmediate(parentObjcet);
        parentObjcet = null;
    }


    public class RewardSheetBlock : UISheetBlockHandler
    {
        public const string OBJECTID = "SheetBlock_Item";

        static int useEventRewardMaxCount = 3;
        protected override bool useAutoBindingByRootBinder => true;

        private UISprite _spriteItem            = new UISprite();
        private UILabel _labelItemNum           = new UILabel();
        private UILabel _labelItemVol           = new UILabel();
        private UISprite _selectBackGround      = new UISprite();
        private UIButton _receiveItemButton     = new UIButton();
        private GameObject _rewardEffect         = new GameObject();

        public int itemindex = 0;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (_receiveItemButton) EventDelegate.Add(_receiveItemButton.onClick, OnClick);
        }
        protected override void OnUpdatedBlock(bool changedItem)
        {
            var data = GetItemData<ItemData>();
            if (data == null) return;

            var itemInfo = TableItemInfo.Instance.Get(data.useEventItemKind);
            if (itemInfo == null) return;

            SetSprite(0, itemInfo.atlasName, itemInfo.itemIcon, data.useEventItemCount,itemInfo.itemvalue2,data.isReceiveReward);

        }
        private void SetSprite(int index, string atlasName, string iconName, int itemCount, int itemVolume,bool itemReceive)
        {
            if ( _spriteItem == null) return;
            if (_labelItemNum == null) return;
            if (_labelItemNum == null) return;
            if (_labelItemVol == null) return;

            itemindex = index;
            if(itemVolume>0)_labelItemVol.text = PublicUIMethod.GetUnitizedString(itemVolume);
            _spriteItem.SetSprite(iconName, atlasName);
            _labelItemNum.text = itemCount.ToString();
            _rewardEffect.gameObject.SetActive(itemReceive);
        }
        private void OnClick()
        {
            var data = GetItemData<ItemData>();
            if (data == null) return;

            var iteminfo = BaseTable.TableItemInfo.Instance.Get(data.useEventItemKind);
            if (null == iteminfo)
                return;
            var titleText = NLibCs.NTextManager.Instance.GetText(iteminfo.itemName);
            var descText = NLibCs.NTextManager.Instance.GetText(iteminfo.itemInstruction);

            if (!UseEventManager.Instance.IsRewaredUseEvent(data.useEventGroupKind, data.usePointGrade) 
                && UseEventManager.Instance.Get_UseEventPoint(data.useEventGroupKind) >= data.usePointGrade)
            {
                // 수령 패킷 발사 ( EventKind / pointgrade ) 
                NFlatBufferBuilder.SendBytes<GS_USE_EVENT_REWARD_REQ>(ePACKET_ID.GS_USE_EVENT_REWARD_REQ, () => GS_USE_EVENT_REWARD_REQ.CreateGS_USE_EVENT_REWARD_REQ(FlatBuffers.NFlatBufferBuilder.FBB, data.useEventKind, data.usePointGrade, data.useEventGroupKind));
            }
            else
            {
                ToolTipDlg.SetToolTipDlg(titleText, descText);
            }
        }

        public void ReceiveItemUI()
        {
            _selectBackGround.SetActive(true);
            _rewardEffect.SetActive(false);
        }
    }
    public class ItemData : UISheet.ItemData
    {
        public int useEventItemKind;
        public int useEventItemValue;
        public int useEventItemCount;
        public int useEventKind;
        public int usePointGrade;
        public int useEventGroupKind;
        public bool isReceiveReward = false;
    }
}
public class EventGemUseSlider : UICom_Parent 
{
    private UIProgressBarEx     _progressbar    = null;
    private UIScrollViewEx       _scrollview     = null;
    private GameObject          _originalObj    = null;
    private UILabel             _pointlabel     = null;
    private BoxCollider         _box            = null;
    private Vector3             _scrollviewPos = new Vector3();
    private Vector2             _panelOffset = new Vector2();

    private Dictionary<int, UseEventRewardInfo> Points = new Dictionary<int, UseEventRewardInfo>();

    float lastpos = 0;
    float FirstPos = 0;

    int preMaxPoint = 0;
    int countList = 0;
    int curPouint = 0;
    int pointMax = 0;
    int maxCount = 0;

    public float rewardUILength=0;

    private int REWARD_COUNT_ON_SCREEN
    {
        get
        {
            return 5;   // 한 화면에 보일 보상의 개수
        }
    }
    private float REWARD_INTERVAL
    {
        get
        {
            return ((float)(lastpos - FirstPos) / REWARD_COUNT_ON_SCREEN + rewardUILength);
        }
    }

    public override void Init()
    {
        base.Init();

        if (_progressbar)
        {
            _progressbar.IsIgnoreBorderSizeCompute = true;
        }
    }
    private void OnDestory()
    {
        var iter = Points.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Value.Release();
        }
        Points.Clear();
    }
    public override void BindUIControls()
    {
        base.BindUIControls();
  
        _progressbar     = this.GetControl<UIProgressBarEx>("Slider_Progress");
        _scrollview      = GetControl<UIScrollViewEx>("ScrollDrag_RewardPoint");
        _box             = GetControl<BoxCollider>("ScrollDrag_RewardPoint");
        _pointlabel      = GetControl<UILabel>("Label_Gem_Point");
        _originalObj     = GetGameObject("Sheet_Item01");

        _scrollviewPos = _scrollview.transform.localPosition;
        _panelOffset = _scrollview.GetComponent<UIPanel>().clipOffset;
        lastpos = _box.size.y;
    }
   
    public void SetUI()
    {
        InitPoints();
        OnChangeRewardPosition();
        MoveScrollView();
        SetProgressValue();
    }
    public void RefreshUI()
    {
        OnDestory();
        SetUI();
    }
    public void RefreshItem(int eventKind)
    {
        var iter = Points.GetEnumerator();
        while (iter.MoveNext())
        {
            if(iter.Current.Value._eventKind == eventKind)
            {
                iter.Current.Value.RefreshUI(eventKind);
                break;
            }
        }
    }
    public void SetProgressValue()
    {
        if (countList == 0) return;
        if (pointMax == 0) return;

        float blockSizeValue = 1.0f / countList;                        // 프로그래스 바에서 블록 사이즈의 %정도

        float valueNextPoint = curPouint - preMaxPoint;                 // 현재 포인트와 다음 포인트 차이
        float valuePrePoint = pointMax - preMaxPoint;                   // 다음 포인트와 이전 포인트 차이
        float curPointSizeValue = (valueNextPoint) / (valuePrePoint);   // 현재 블록에서 프로그래스바가 차지하는 비률

        float curPointBlockCount = maxCount - 0.5f;                     // 포인트가 추월한 보상값들 (50/100/500/800) 높이에서 0.5만큼 뺀다 
        if (curPointBlockCount < 0)
        {
            curPointBlockCount = 0;
            curPointSizeValue = curPointSizeValue * 0.5f;               // 추월한 보상이 없는 경우 0 기준으로 진행
        }
        if(_progressbar)_progressbar.value = blockSizeValue * (curPointBlockCount) + blockSizeValue *curPointSizeValue; // 프로그래스바 위치 계산
    }

    private void InitPoints()
    {
        if (_originalObj == null)
            return;
        CreateReward();
        if(_progressbar)_progressbar.value = 0;
    }

    private void OnChangeRewardPosition()
    {
        var iter = Points.GetEnumerator();
        int pointSize = Points.Count;
        while (iter.MoveNext())
        {
            iter.Current.Value.ChangePos(GetRewardBtnPosition(iter.Current.Key, 1000, pointSize));
        }
        SetProgressbarValue();
    }
    private float GetRewardBtnPosition(int idx, int pointCnt,int pointCount)
    {
        if (-1 == idx) return FirstPos;

        float halfNum = (float)pointCount / 2.0f;

        return idx * REWARD_INTERVAL - halfNum * REWARD_INTERVAL ;
    }

    private void SetProgressbarValue()
    {
        // 프로그래스바 길이 고정
        float widthProgress = countList * REWARD_INTERVAL;                              //보상 카운트 x 보상아이콘 높이
        float halfNum = (float)(countList-1) / 2.0f;                                    //보상 카운트를 0기준으로 위/아래 세팅위해 반으로 나눔 ( 끝포인트 계산위해 1제거 )
        float newY = Points[0].GetRewardPosition_Y() - (halfNum * REWARD_INTERVAL)  + 2.0f;   // 첫번째보상 버튼 기준으로 값 세팅

        if (_progressbar != null)
        {
            _progressbar.transform.localPosition = new Vector3(_progressbar.transform.localPosition.x, newY, _progressbar.transform.localPosition.z);
            _progressbar.backgroundWidget.height = (int)widthProgress;
            _progressbar.foregroundWidget.height = (int)widthProgress;
            _progressbar.ForceUpdate();
        }
    }

    private void MoveScrollView()
    {
        UIPanel panel = _scrollview.GetComponent<UIPanel>();

        if (!panel)
            return;

        float widthProgress = (countList/2-2) * REWARD_INTERVAL;
        float moveValue = widthProgress;

        int focusIndex = UseEventManager.Instance.Get_IndexCanReciveReward();

        var _offset = _panelOffset;
        var m_pos = _scrollviewPos;

        moveValue -= focusIndex * REWARD_INTERVAL;

        _offset.y += moveValue;
        m_pos.y -= moveValue;

        panel.clipOffset = _offset;
        _scrollview.transform.localPosition = m_pos;
    }

    private void CreateReward()
    {
        _originalObj.SetActive(true);

        var eventListAll = UseEventManager.Instance.GetEventList(UseEventManager.Instance.selectEventGroupKind);
        bool checkMaxCount = false;

        countList = eventListAll.Count ;
        pointMax = 0;

        for (int i=0;i<eventListAll.Count;i++)
        {
            if (Points.ContainsKey(i))
                continue;

            UseEventManager.Instance.dicUseEventpoint.TryGetValue(eventListAll[i].i32GroupKind, out curPouint);
            _pointlabel.text = eventListAll[i].i64PointGrade.ToString();

            if (!checkMaxCount)
            {
                preMaxPoint = pointMax;
                pointMax = (int)eventListAll[i].i64PointGrade;
                maxCount = i ;
            }
            if (curPouint< eventListAll[i].i64PointGrade)
            {
                checkMaxCount = true;
            }

            var p = new UseEventRewardInfo((byte)eventListAll[i].i32EventKind, UseEventManager.Instance.selectEventGroupKind, curPouint,_originalObj);
            if (p != null)
                Points.Add(i, p);
        
        }
        _originalObj.SetActive(false);
    }
}
