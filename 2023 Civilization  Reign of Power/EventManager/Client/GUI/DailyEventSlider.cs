using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EVENT;
using DEFINE;
using Recycler;

public class DailyEventItemInfo
{
    public byte level;
    public UIButton button;
    public UISprite sprite;
    public UILabel label;
    public UILabel volumeLabel;
    public UILabel countLabel;
    public GameObject parent;
    public RecycleManager.Stock effect;
    public TweenScale stween_b;
    public TweenScale stween_s;
    public Int32 weekDay = 0;
    public UISprite SpriteReward;
    public UISprite SpriteCheck;

    static private string[] pointIcons =
    {
        "item_reward_box_1",
        "item_reward_box_1",
        "Item_Vacant_Box"
    };

    private Color32[] btnColor =
    {
        new Color32(168, 168, 168, 102),
        new Color32(255, 255, 255, 102)
    };

    public DailyEventItemInfo(Int32 _weekDay, byte _level, GameObject p, UIButton b, UISprite s, UILabel l, UISprite reward, UISprite check, RecycleManager.Stock e, UILabel volume, UILabel count, UIEventListener.BoolDelegate cbLongTouch = null)
    {
        level = _level;
        parent = p;
        button = b;
        sprite = s;
        label = l;
        effect = e;
        weekDay = _weekDay;
        SpriteReward = reward;
        SpriteCheck = check;
        volumeLabel = volume;
        countLabel = count;

        if (button != null)
            stween_b = button.GetComponent<TweenScale>();

        if (sprite != null)
            stween_s = sprite.GetComponent<TweenScale>();

        if (button != null)
        {
            UIEventListener.Get(button.gameObject).onClick += OnClick;
            UIEventListener.Get(button.gameObject).onPress += cbLongTouch;
        }
    }


    public long GetDailyEventPointFromWeekDay()
    {
        if (weekDay == EVENT.EventManager.Instance.CurrentWeekDay())
            return EVENT.EventManager.Instance.GetCurrentDailyEventPoint();
        else
            return 0;
    }

    public bool HasRewardStep(Int32 level)
    {
        if (weekDay == EVENT.EventManager.Instance.CurrentWeekDay())
            return EVENT.EventManager.Instance.HasRewardStep(level);

        return false;
    }


    public void ChangeSprite(DailyPoint.PointState state)
    {
        if (null == sprite)
            return;


        bool isGray = DailyPoint.PointState.REWARDED == state;

        var info = EVENT.EventManager.Instance.GetDailyEventData(weekDay);

        if (info != null )
        {
            var rewardItem = info.GetRewardItemFromStep(level);
            if (rewardItem.listRewardItem.Count <= 0)
                return;

            var itemInfo = BaseTable.TableItemInfo.Instance.Get(rewardItem.listRewardItem[0].itemKind);
            if (itemInfo != null)
            {
                //if (button != null) button.normalSprite = itemInfo.itemIcon;

				//gray처리를 할때 gray용 아틀라스로  변경시켜주는 로직이 있습니당
				//gray가 true 상태일때 아틀라스가 기본 아틀라스로 셋팅된후 
  				//아틀라스를 변경시켜 주지 않는 오류가 있어서 수정하였습니다. 
                sprite.grayscale = false;
                sprite.atlas = AtlasManager.Instance.Get(itemInfo.atlasName);
                sprite.spriteName = itemInfo.itemIcon;

                sprite.grayscale = isGray;
            }

            ChangeButtonSprite(isGray);
        }
    }

    public void ChangeRewardSprite(bool bRewardOn)
    {
        if (SpriteReward != null)
            SpriteReward.color = bRewardOn ? PublicUIMethod.Data.COLOR_TEXT_STATE_ORANGE.ColorValue : PublicUIMethod.Data.COLOR_TEXT_STATE_GRAY.ColorValue;
    }

    public void ChangeCheckSprite(bool bRewardOn)
    {
        if (SpriteCheck != null)
            SpriteCheck.SetActive(bRewardOn);
    }

    public void ChangeLabelColor(Color color)
    {
        if (label != null)
            label.color = color;
    }

    public void ChangeButtonSprite(bool bRewardOn)
    {
        if (button != null)
            button.defaultColor = bRewardOn ? PublicUIMethod.Data.COLOR_TEXT_STATE_GRAY.ColorValue : PublicUIMethod.Data.COLOR_TEXT_STATE_WHITE.ColorValue;
    }

    public void ChangeButtonScale(bool bRewardOn)
    {
        if (bRewardOn == false)
            return;

        if(button != null)
        {
            button.gameObject.transform.localScale = Vector3.one;
        }

        if(sprite != null)
        {
            sprite.gameObject.transform.localScale = Vector3.one;
        }
    }

    public void ChangeText(string t)
    {
        if (label != null)
            label.text = t;
    }

    public void ChangePos(float posX)
    {
        if (parent != null)
        {
            var prePos = parent.transform.localPosition;

            parent.transform.localPosition = new Vector3(posX, prePos.y, prePos.z);
        }
            
    }

    public void ChangeVolumeText(string volume)
    {
        if (volumeLabel != null)
        {
            if (volume == "0")
                volumeLabel.text = "";
            else 
                volumeLabel.text = volume;
        }
    }
    public void ChangeCountText(string count)
    {
        if (countLabel != null)
            countLabel.text = count;
    }

    public Vector3 Position()
    {
        if (parent == null)
            return Vector3.zero;
        else
            return parent.transform.localPosition;
    }

    public void Release()
    {
        ReleaseEffect();
        GameObject.DestroyImmediate(parent);
    }

    public void Update()
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        var state = GetPointState();
        
        ChangeSprite(state); //아이콘

        if (state == DailyPoint.PointState.COMPLEDTED)
        {
            SetEffect(true);
            ActivateTween(true);
            ChangeRewardSprite(true);
            ChangeCheckSprite(false);
            ChangeButtonSprite(false);
            ChangeButtonScale(false);
            ChangeLabelColor(PublicUIMethod.Data.COLOR_TEXT_STATE_WHITE.ColorValue);
        }
        else if(state == DailyPoint.PointState.REWARDED)
        {
            SetEffect(false);
            ActivateTween(false);
            ChangeRewardSprite(false);
            ChangeCheckSprite(true);
            ChangeButtonSprite(true);
            ChangeButtonScale(true);
            ChangeLabelColor(PublicUIMethod.Data.COLOR_TEXT_STATE_GRAY.ColorValue);
        }
        else
        {
            SetEffect(false);
            ActivateTween(false);
            ChangeRewardSprite(false);
            ChangeCheckSprite(false);
            ChangeButtonSprite(false);
            ChangeButtonScale(true);
            ChangeLabelColor(PublicUIMethod.Data.COLOR_TEXT_STATE_WHITE.ColorValue);
        }
    }

    Int32 _curRewardStep, _curDailyPoint, _curItemDailyPoint = 0;
    DailyPoint.PointState _state = DailyPoint.PointState.INVAILD;
    public void ShowLog()
    {
        string log = string.Format("<color=blue>[DailyEventInfo]</color> State({0}), Level({1}), CuRewardStep({2}), CurDailyPoint({3}), CurItemDailyPoint({4}) ", _state, level, _curRewardStep, _curDailyPoint, _curItemDailyPoint);
        NDebug.Log(log);
    }

    public DailyPoint.PointState GetPointState()
    {
        _state = GetPointState(out _curRewardStep, out _curDailyPoint, out _curItemDailyPoint);
		return _state;
    }
    private DailyPoint.PointState GetPointState(out Int32 curRewardStep, out Int32 curDailyPoint, out Int32 curItemDailyPoint)
    {
        curItemDailyPoint = curDailyPoint = curRewardStep = 0;

        var eventData = EVENT.EventManager.Instance.GetDailyEventData(weekDay);
        if (null == eventData)
            return DailyPoint.PointState.NOT_COMPLETED;


        // 현재 유저의 매일이벤트 포인트
        curDailyPoint = (int)GetDailyEventPointFromWeekDay() ;

        // 선택한 아이템의 포인트
        curItemDailyPoint = eventData.pointGrade[level];

        if(curDailyPoint < curItemDailyPoint)
            return DailyPoint.PointState.NOT_COMPLETED;

        // 보상 여부
        if (HasRewardStep(level + 1))
            return DailyPoint.PointState.REWARDED;

        return DailyPoint.PointState.COMPLEDTED;
    }

    private void OnClick(GameObject sender)
    {
        var state = GetPointState();
        switch(state)
        {
            case DailyPoint.PointState.NOT_COMPLETED:
                OpenRewardList();
                break;
            case DailyPoint.PointState.COMPLEDTED:
                Send_Server_Reward_Get();
                break;
            case DailyPoint.PointState.REWARDED:
                {
                    Update();
                    OpenRewardList();
                }
                break;
        }
    }

    private void Send_Server_Reward_Get()
    {
        //yangkichan //구현 필요 //서버로 리워드 겟 보내는 부분
        if (weekDay != EVENT.EventManager.Instance.CurrentWeekDay())
            return;

        SByte EventType = (SByte)EVENT.EventManager.Instance.CurrentWeekDay();  //current weekday

        Int32 curDailyPoint = EVENT.EventManager.Instance.dailyEventPoint;

        AudioManager.Instance.PlaySFX_NDT("sfx_ui_resourceacquisition", null);
 
        Func<object> offsetmethod = () =>
        {
            return PROTOCOL.FLATBUFFERS.GS_DAILY_EVENT_POINT_REWARD_SET_REQ.CreateGS_DAILY_EVENT_POINT_REWARD_SET_REQ(FlatBuffers.NFlatBufferBuilder.FBB, EventType, level + 1, curDailyPoint);
        };
        FlatBuffers.NFlatBufferBuilder.SendBytes<PROTOCOL.FLATBUFFERS.GS_DAILY_EVENT_POINT_REWARD_SET_REQ>(PROTOCOL.GAME.ID.ePACKET_ID.GS_DAILY_EVENT_POINT_REWARD_SET_REQ, offsetmethod);
        
    }

    private void OpenRewardList()
    {
        PublicSoundMethod.PlayClickCommon();

        var info = EVENT.EventManager.Instance.GetDailyEventData(weekDay);
        if(info != null)
        {
            var rewardItems = info.GetRewardItemFromStep(level);
            if (null == rewardItems.listRewardItem)
                return;

            if (0 >= rewardItems.listRewardItem.Count)
                return;

            var iteminfo = BaseTable.TableItemInfo.Instance.Get(rewardItems.listRewardItem[0].itemKind);
            if (null == iteminfo)
                return;

            var titleText = NLibCs.NTextManager.Instance.GetText(iteminfo.itemName);
            var descText = NLibCs.NTextManager.Instance.GetText(iteminfo.itemInstruction);

            ToolTipDlg.SetToolTipDlg(titleText, descText);
        }
    }

    private void SetEffect(bool flag)
    {
        if (flag)
        {
            if (effect._go != null)
            {
                effect._go.SetActive(true);
            }
            else
            {
                AcquireEffect();
            }
        }
        else
        {
            if (effect._go != null)
            {
                effect._go.SetActive(false);
            }
        }
    }

    private void AcquireEffect()
    {
        if (effect._go == null)
            effect = FXManager.Instance.AcquireInstanceByNDTKind(KindId.GetDataKindByPrefabName("FX_UI_Event_Receive_Icon"), button.gameObject, false);
    }
    private void ReleaseEffect()
    {
        if (effect._go != null)
        {
            FXManager.Instance.ReleaseInstance(effect._kind, effect._go);
            effect._kind = 0;
            effect._go = null;
        }
    }

    private void ActivateTween(bool state)
    {
        if (stween_b != null)
        {
            if(state == false)
                stween_b.enabled = false;
            else
                stween_b.enabled = true;

            stween_b.ResetToForwardBeginning();
        }

        if (stween_s != null)
        {
            if (state == false)
                stween_s.enabled = false;
            else
                stween_s.enabled = true;

            stween_s.ResetToForwardBeginning();
        }
    }
}

public class DailyEventSlider : UICom_Parent
{
    public enum Components
    {
        Slider_Progress_1,  //ok
        First_1,  //ok

        ScrollView_RewardPoint_1,  //ok

        Thumb_1,   //ok

        Background_1,  //ok
        Foreground_1,  //ok

        ScrollDrag_RewardPoint_1,    //ok
        Label_My_Point_1,  //ok
        //Sprite_Thumb,

        //근데 절망적인건 컴포넌트가 탭마다 다르다...
    }

    #region private member data
    private UIProgressBarEx _progressbar = null;
    private UIScrollViewEx _scrollview = null;
    
    private Vector4 _finalClipRegion;
    private GameObject _originalObj = null;
    private GameObject _thumb;
    private UIWidget _widget;
    private BoxCollider _box;
    private UILabel _pointlabel;

    private UISprite _button_sprite = null;

    private int FirstPos
    {
        get
        {
            return -290;
        }
    }
    private const int lastpos = 730;

    //한 화면에 보이는 갯수
    private int REWARD_COUNT_ON_SCREEN
    {
        get
        {
            return 6;
        }

    }

    private float REWARD_INTERVAL
    {
        get
        {
            return (float)(lastpos - FirstPos) / REWARD_COUNT_ON_SCREEN;
        }
    }

    private UISprite _background;
    private UISprite _foreground;

    private Dictionary<byte, DailyEventItemInfo> Points = new Dictionary<byte, DailyEventItemInfo>();

    private Coroutine _sliderRoutine = null;

    public Int32 _weekDay = 0;

    private float full_width = 0f;
    private float b_width = 0f;

    #endregion private member data
    
    private void OnDestory()
    {
        Clear();
    }
    
    #region overridden methods
    public override void BindUIControls()
    {
        base.BindUIControls();
        BindControl<Components>();

        _progressbar = GetControl<UIProgressBarEx>(Components.Slider_Progress_1);
        _scrollview = GetControl<UIScrollViewEx>(Components.ScrollView_RewardPoint_1);

        _originalObj = GetGameObject(Components.First_1);
        _thumb = GetGameObject(Components.Thumb_1);

        _background = GetControl<UISprite>(Components.Background_1);
        _foreground = GetControl<UISprite>(Components.Foreground_1);
        _widget = GetControl<UIWidget>(Components.ScrollDrag_RewardPoint_1);
        _box = GetControl<BoxCollider>(Components.ScrollDrag_RewardPoint_1);
        _pointlabel = GetControl<UILabel>(Components.Label_My_Point_1);        

        b_width = _scrollview.GetComponent<UIPanel>().width;

        //User.Instance.DPoint.OnChangeFirstPoint += OnChangeFirstPoint;  //yangchan //dummy
        //위치 재조정 델리게이트를 해주는 부분 (필요한지 확인)

        if (null != _originalObj)
        {
            Transform trs = _originalObj.transform.FindChildRecursive("Sprite_ItmeBG");
            if (null != trs) _button_sprite = trs.GetComponent<UISprite>();
        }

        if (_progressbar)
            _progressbar.IsIgnoreBorderSizeCompute = true;

        InitPoints();
        OnChangeFirstPoint();
        InitScrollPos();
    }

    public void SetWeekDay(Int32 weekDay)
    {
        _weekDay = weekDay;
    }

    public override void BindUIEvents()
    {
                        
    }
    #endregion overridden methods

    #region public methods    
    public void RefreshUI()
    {
        Clear();
        InitPoints();
        OnChangeFirstPoint();
        InitScrollPos();
    }

    public void Clear()
    {
        var iter = Points.GetEnumerator();
        while (iter.MoveNext())
            iter.Current.Value.Release();

        Points.Clear();
        StopAllCoroutines();

        _pressRoutine = null;
    }

    public void OnUpdate()
    {
        UpdateProgressBar();
        UpdatePoint();
        UpdateItemVolume();
        UpdateText();
        UpdateScroll();
    }

    public void Rewarded(byte level)
    {
        var p = Get(level);
        if (p != null)
            p.Update();
    }
    #endregion public methods

    #region private methods
    private void InitPoints()
    {
        if (_originalObj == null)
            return;

        //byte maxpointcount = BaseTable.TableDailyPointInfo.Instance.GetMaxCount(TerritoryBase.Territory.GetCastleLevel());
        //몇개의 포인트가 존재하는지...
        
        EVENT.DailyEventData eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay);  //yangkichan //dummy //월요일만
        if (eventData == null)
            return;

        byte maxpointcount = (byte)eventData.pointGrade.Count;

        CopyAndUpdatePoint(maxpointcount);
        _progressbar.value = 0;
        
    }
    private void CopyAndUpdatePoint(byte count)
    {
        if (count == 0)
            return;

        //원하는 포인트 개수만큼 copy
        CopyPoint(count);
        UpdatePoint();
    }
    private void CopyPoint(byte count)
    {
        _originalObj.SetActive(true);
        for (byte i = 0; i < count; ++i)
        {
            //byte level = (byte)(i + 1);
            byte level = (byte)i;  //yangkichan

            if (Points.ContainsKey(level))
                continue;

            var p = Copy(level);
            if (p != null)
                Points.Add(level, p);
        }

        _originalObj.SetActive(false);
    }

    private void UpdatePoint()
    {
        //yangkichan
        var iter = Points.GetEnumerator();
        while(iter.MoveNext())
        {
            var eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay);
            if (null == eventData) continue;

            int point = (iter.Current.Key < eventData.pointGrade.Count) ? eventData.pointGrade[iter.Current.Key] : 0;

            iter.Current.Value.Update();
            iter.Current.Value.ChangeText(PublicUIMethod.ThousandSeparateString(point)); //텍스트
        }
        
    }

    private void UpdateItemVolume()
    {
        var eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay);
        if (null == eventData)
            return;


        var iter = Points.GetEnumerator();
        while (iter.MoveNext())
        {
            int index = iter.Current.Key;
            int firstItemKind = eventData.listRewards[index].listRewardItem[0].itemKind;
            //int firstItemCount = eventData.listRewards[index].listRewardItem[0].itemCnt;

            long total = 0;
            long count = 0;

            BaseTable.NrItemDataInfo firstItemInfo = BaseTable.TableItemInfo.Instance.Get(firstItemKind);

            if (firstItemInfo == null)
                continue;

            for (int i = 0; i < eventData.listRewards[index].listRewardItem.Count; i++)
            {
                int itemKind = eventData.listRewards[index].listRewardItem[i].itemKind;
                int itemCount = eventData.listRewards[index].listRewardItem[i].itemCnt;

                BaseTable.NrItemDataInfo itemInfo = BaseTable.TableItemInfo.Instance.Get(itemKind);

                if (firstItemInfo.itemAddingType != 0)
                {
                    if (firstItemInfo.itemAddingType == itemInfo.itemAddingType)
                    {
                        total += itemInfo.itemvalue2 /** */;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                    else if ((GameDefine.eITEM_TYPE)itemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_NATIVE_RESOURCE
                        && firstItemInfo.itemvalue1 == itemInfo.itemKind)
                    {
                        //total += eventData.listRewards[index].listRewardItem[i].itemCnt;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                }

                else if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_NATIVE_RESOURCE)
                {
                    if (itemInfo.itemAddingType != 0 && itemInfo.itemvalue1 == firstItemInfo.itemKind)
                    {
                        total += itemInfo.itemvalue2 /** */;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                    else if (itemInfo.itemKind == firstItemInfo.itemKind)
                    {
                        //total += eventData.listRewards[index].listRewardItem[i].itemCnt;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                }

                else if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_RESOURCE
                    && (GameDefine.eITEM_USE_TYPE)firstItemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_ITEM)
                {
                    if ((GameDefine.eITEM_TYPE)itemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_TICKET
                        && (GameDefine.eITEM_USE_TYPE)itemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_VALUE)
                    {
                        if (firstItemInfo.itemvalue1 == itemInfo.itemKind)
                        {
                            //total += eventData.listRewards[index].listRewardItem[i].itemCnt;
                            count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                        }
                    }
                    else if (firstItemInfo.itemKind == itemInfo.itemKind)
                    {
                        total += itemInfo.itemvalue2 /** */;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                }

                else if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_TICKET
                    && (GameDefine.eITEM_USE_TYPE)firstItemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_VALUE)
                {
                    if ((GameDefine.eITEM_TYPE)itemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_RESOURCE
                        && (GameDefine.eITEM_USE_TYPE)itemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_ITEM)
                    {
                        if (itemInfo.itemvalue1 == firstItemInfo.itemKind)
                        {
                            total += itemInfo.itemvalue2 /** */;
                            count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                        }
                    }
                    else if (firstItemInfo.itemKind == itemInfo.itemKind)
                    {
                        //total += eventData.listRewards[index].listRewardItem[i].itemCnt;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                }

                else if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_ACCELATOR
                    && (GameDefine.eITEM_USE_TYPE)firstItemInfo.itemUseType != GameDefine.eITEM_USE_TYPE.ACCELATOR_MARCH)
                {
                    if (firstItemInfo.itemUseType == itemInfo.itemUseType)
                    {
                        total += itemInfo.itemvalue1 /**/;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                }

                else if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_BUFF &&
                         (GameDefine.eITEM_USE_TYPE)firstItemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_PACKAGE)
                {
                    if ((GameDefine.eITEM_TYPE)itemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_BUFF &&
                        (GameDefine.eITEM_USE_TYPE)itemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_PACKAGE)
                    {
                        total += itemInfo.itemvalue4 /** */;
                        count += eventData.listRewards[index].listRewardItem[i].itemCnt;
                    }
                }
            }

            if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_ACCELATOR &&
                (GameDefine.eITEM_USE_TYPE)firstItemInfo.itemUseType != GameDefine.eITEM_USE_TYPE.ACCELATOR_MARCH)
            {
                iter.Current.Value.ChangeVolumeText(PublicUIMethod.GetSimpleTimeString(-total));
                if(count>0)iter.Current.Value.ChangeCountText(PublicUIMethod.GetUnitizedString(count));
            }
            else if ((GameDefine.eITEM_TYPE)firstItemInfo.itemType == GameDefine.eITEM_TYPE.ITEM_TYPE_BUFF &&
                     (GameDefine.eITEM_USE_TYPE)firstItemInfo.itemUseType == GameDefine.eITEM_USE_TYPE.RESOURCE_PACKAGE)
            {
                iter.Current.Value.ChangeVolumeText(PublicUIMethod.GetSimpleTimeString(total));
                if (count > 0) iter.Current.Value.ChangeCountText(PublicUIMethod.GetUnitizedString(count));
            }
            else
            {
                if (count <= 0)
                {
                    count = eventData.listRewards[index].listRewardItem[0].itemCnt;
                }

                iter.Current.Value.ChangeVolumeText(PublicUIMethod.GetUnitizedString(total));
                if (count > 0) iter.Current.Value.ChangeCountText(PublicUIMethod.GetUnitizedString(count));
            }
        }
    }

    private void UpdateText()
    {
        Int64 point = 0;
        if (EVENT.EventManager.Instance.CurrentWeekDay() == _weekDay)
            point = EVENT.EventManager.Instance.GetCurrentDailyEventPoint();

        if (_pointlabel != null)
            _pointlabel.text = point.ToString();
    }

    private void UpdateScroll()
    {
    }

    private DailyEventItemInfo Copy(byte index)
    {
        var obj = GameObject.Instantiate(_originalObj);
        if (obj == null)
            return null;

        obj.transform.parent = _originalObj.transform.parent;
        obj.transform.localPosition = _originalObj.transform.localPosition;
        obj.transform.localScale = new Vector3(1, 1, 1);

        UIButton b = null;
        UISprite s = null;
        UILabel l = null;
        UILabel volume = null;
        UILabel count = null;
        UISprite r = null;
        UISprite c = null;


        var buttonObj = obj.transform.Find("Button_First_1");
        if (buttonObj != null)
            b = buttonObj.GetComponent<UIButton>();

        if (buttonObj != null)
        {
            var spriteObj = buttonObj.transform.Find("Sprite_First_1");
            if (spriteObj != null)
                s = spriteObj.GetComponent<UISprite>();

            var amountLabelObj = buttonObj.transform.Find("Label_ItemVol_1"); 
            if (amountLabelObj != null)
                count = amountLabelObj.GetComponent<UILabel>();

            var volumeLabelObj = buttonObj.transform.Find("Label_First_1");
            if (volumeLabelObj != null)
                volume = volumeLabelObj.GetComponent<UILabel>();

            var spriteRewardObj = buttonObj.transform.Find("Sprite_RewardGlow");
            if (spriteRewardObj != null)
                r = spriteRewardObj.GetComponent<UISprite>();

            var spriteCheckObj = buttonObj.transform.Find("Sprite_check");
            if (spriteCheckObj != null)
                c = spriteCheckObj.GetComponent<UISprite>();
        }

        var labelObj = obj.transform.Find("Label_Point");
        if (labelObj != null)
            l = labelObj.GetComponent<UILabel>();

        obj.SetActive(true);
        return new DailyEventItemInfo(_weekDay, index, obj, b, s, l, r, c, default, count, volume, OnPress);
    }

    #region Log
    private Coroutine _pressRoutine = null;
    private void OnPress(GameObject sender, bool press)
    {
        if (press)
        {
            if (_pressRoutine == null)
                _pressRoutine = StartCoroutine(StartPress(sender, press));
        }
        else
        {
            if (_pressRoutine != null)
            {
                StopCoroutine(_pressRoutine);
                _pressRoutine = null;
            }
        }
    }

    private IEnumerator StartPress(GameObject sender, bool press)
    {
        if (press == false)
            yield break;

        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        var iter = Points.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value.button.gameObject == sender)
            {
                iter.Current.Value.ShowLog();
                yield break;
            }
        }
    }
#endregion

    private void UpdateProgressBar()
    {
        //yangkichan  //포인트 계산하는 부분
        //int l = BaseTable.TableDailyPointInfo.Instance.GetMaxPoint(TerritoryBase.Territory.GetCastleLevel());


        var eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay);
        if (null == eventData) return;

        int itemCount = eventData.pointGrade.Count;

        if (itemCount <= 0)
            return;

        int maxPoint = (0 < eventData.pointGrade.Count) ? eventData.pointGrade[eventData.pointGrade.Count - 1] : 0;

        if (maxPoint == 0)
            _progressbar.value = 0;
        else
        {
            if (_sliderRoutine != null)
                StopCoroutine(_sliderRoutine);

            Int64 dailyEventPoint = (_weekDay == EVENT.EventManager.Instance.CurrentWeekDay()) ? EVENT.EventManager.Instance.GetCurrentDailyEventPoint() : 0;

            _sliderRoutine = StartCoroutine(ChangeProgressSmoth(0.5f, _progressbar.value, CalcDailyPointSliderValueByPoint(eventData.pointGrade, dailyEventPoint)));
        }
        
    }

    private void MoveScrollView(float delta)
    {
        UIPanel panel = _scrollview.GetComponent<UIPanel>();

        var offset = panel.clipOffset;// _scrollview.panel.clipOffset;

        if (0 == delta)
            offset.x = -panel.clipSoftness.x;
        else
            offset.x = delta;

        panel.clipOffset = offset;

        var pos = _scrollview.transform.localPosition;

        if (0 == delta)
            pos.x = panel.clipSoftness.x;
        else
            pos.x = -(delta);

        _scrollview.transform.localPosition = pos;


        //panel.clipOffset = Vector3.zero;
        //_scrollview.transform.localPosition = new Vector3(-delta, 0f, 0f);
        //panel.clipOffset = new Vector2(delta, 0f);
        //_scrollview.marginBegin = 10;
        _scrollview.RestrictWithinBounds(true);
        //_scrollview.marginBegin = 0;
    }

    private IEnumerator ChangeProgressSmoth(float duration, float s, float l)
    {
        float time = 0;
        var totalLength = l - s;

        while(time <= duration)
        {            
            var progress = time / duration;                        
            _progressbar.value = s + totalLength * progress;

            time += Time.deltaTime;
            yield return null;
        }

        _progressbar.value = s + totalLength;      
    }

    private float GetLastThumbPos()
    {
        var eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay);
        if (null == eventData)
            return 0f;

        return GetRewardBtnPosition(eventData.pointGrade.Count - 1, eventData.pointGrade.Count);
    }


    private void OnChangeFirstPoint()
    {
        var eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay); 
        if(null == eventData)
            return;
        
        var iter = Points.GetEnumerator();
        while(iter.MoveNext())
        {
            //다음 녀석과의 간격을 벌리자.
            iter.Current.Value.ChangePos(GetRewardBtnPosition(iter.Current.Key, eventData.pointGrade.Count));
        }

        //슬라이더의 길이를 바꿔준다.
        var width = eventData.pointGrade.Count * REWARD_INTERVAL;
        full_width = width;

        ChangeSliderLength(width);
        //스크롤뷰의 영역도 줄여줍니다.
        ChangeScrollViewArea(width);
    }

    private float GetRewardBtnPosition(int idx, int pointCnt)
    {
        idx = Math.Min(Math.Max(-1, idx), pointCnt - 1);
        if (-1 == idx) return FirstPos;

        return idx * REWARD_INTERVAL + FirstPos;
    }

    private float CalcDailyPointSliderValueByPoint(List<int> points, Int64 dailyEventPoint)
    {
        if (null == points || 0 == points.Count)
            return 0f;

        int findIdx = points.FindLastIndex((int tempPoint) => { return tempPoint <= dailyEventPoint; });

        long startPoint = 0;
        long nextPoint = 0;

        if (-1 == findIdx)
        {
            startPoint = 0;
            nextPoint = points[findIdx + 1];
        }
        else if ((points.Count-1) > findIdx)
        {
            startPoint = points[findIdx];
            nextPoint = points[findIdx + 1];
        }
        else
        {
            startPoint = points[findIdx];
            nextPoint = points[findIdx];
        }


        //획득가능한 보상포인트와 다음 포인트 사이의 비율을 구합니다. (0f ~ 1f) : [이미 마지막 보상포인트까지 얻었다면 1f]
        float ratio = (startPoint < nextPoint) ? Mathf.Clamp01((float)(dailyEventPoint - startPoint) / (float)(nextPoint - startPoint)) : 1f;


        // 전체 비율을 구합니다.
        ratio += (findIdx + 1);
        ratio /= points.Count;
        return Mathf.Clamp01(ratio);
    }

    private DailyEventItemInfo Get(byte level)
    {
        DailyEventItemInfo p = null;
        Points.TryGetValue(level, out p);
        return p;
    }

    private void ChangeSliderLength(float width)
    {
        if (_background != null)
            _background.width = (int)width;
        if (_foreground != null)
            _foreground.width = (int)width;
    }

    private void ChangeScrollViewArea(float width)
    {
        if(_widget != null)
            _widget.width = (int)width;

        if (_box != null)
        {
            _box.size = new Vector3(width, _box.size.y, _box.size.z);
            _box.center = new Vector3(width * 0.5f - b_width * 0.5f, _box.center.y, _box.center.z);                 
        }

        if (_progressbar != null)
        {
            var tm = _progressbar.transform.localPosition;
            _progressbar.transform.localPosition = new Vector3(width * 0.5f - b_width * 0.5f, tm.y, tm.z);
        }
    }

    //초기 스크롤 위치를 정해준다.
    private void InitScrollPos()
    {
        //yangkichan
        var iter = Points.GetEnumerator();

        byte index = 0;
        bool IsAllReward = true;

        while (iter.MoveNext())
        {
            if (iter.Current.Value.GetPointState() != DailyPoint.PointState.REWARDED)
            {
                index = (byte)iter.Current.Key;
                IsAllReward = false;
                break;
            }
        }

        if (IsAllReward)
            index = (byte)(Points.Count - 1);

        FocusToIndex(index);
    }
    private void FocusToIndex(byte index)
    {
        var p = Get(index);
        if (p == null)
            return;

        var eventData = EVENT.EventManager.Instance.GetDailyEventData(_weekDay);
        if (null == eventData)
            return;
        
        float pos = 0.0f;

        if (0 != index)
        {
            pos = index * REWARD_INTERVAL + FirstPos;
        }
        

        // 최대치 제한 [ WWM코드 ]
        float over_mount = full_width - _scrollview.panel.GetViewSize().x - pos;
        if (0.0f > over_mount)
        {
            //pos -= b_width - (full_width - pos);
            pos += over_mount;
        
            if (null != _button_sprite)
            {
                // 너무 딱 붙어서 여유값 추가
                pos += (_button_sprite.width ) ;
            }
        }
        
//         if (index == 0 || index == Points.Count - 1)
//             pos -= 63;

        MoveScrollView(pos);
    }

    private float GetThumbPos()
    {
        return _thumb == null ? 0 : _thumb.transform.localPosition.x;
    }

  
    #endregion private methods
}
