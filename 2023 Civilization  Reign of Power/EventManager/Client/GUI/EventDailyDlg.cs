using UnityEngine;
using NLibCs;
using DEFINE;
using System;
using Contents.User;
using EVENT;
using Framework.EventSystem;
using GameEvent;
using RANKING;

public class EventDailyDlg : UIForm
{
    #region inspector

    // Tab
    [SerializeField] private UIButton[] _buttonTabs;
    [SerializeField] private UILabel[] _labelDays;
    [SerializeField] private UILabel[] _labelToday;
    [SerializeField] private UISprite[] _spriteTabOn;
    [SerializeField] private UISprite[] _spriteTabNotices;
    [SerializeField] private UIButton _buttonDetailInfo;

    // Waiting
    [SerializeField] private GameObject _objWaiting;
    [SerializeField] private UILabel _labelWaiting;

    // Content
    [SerializeField] private GameObject _objNow;
    [SerializeField] private GameObject[] _objExplanes;
    [SerializeField] private UIButton[] _buttonExplanes;
    [SerializeField] private UILabel _labelEndTime;
    [SerializeField] private UILabel _labelSubTitle;
    [SerializeField] private UILabel[] _labelExplanes;
    [SerializeField] private UISprite _spriteBaseLllust;
    [SerializeField] private UITexture _textureLllust;

    // Top Ranking
    [SerializeField] private UIButton[] _buttonTopRankers;
    [SerializeField] private UILabel[] _labelRankerPower;
    [SerializeField] private UILabel[] _labelRankerName;
    [SerializeField] private UISprite[] _spriteRankerPortrait;
    [SerializeField] private UISprite[] _spriteNoRanker;
    [SerializeField] private UILabel _labelNoTopRanker;

    // My Ranking
    [SerializeField] private GameObject _objMyRank;
    [SerializeField] private UIButton _buttonRank;
    [SerializeField] private UILabel _labelMyRank;
    [SerializeField] private UILabel _labelExplanation;
    [SerializeField] private UILabel _labelRankChange;
    [SerializeField] private UISprite _spriteUpArrow;
    [SerializeField] private UISprite _spriteDownArrow;

    // DailyPoint
    [SerializeField] private UIScrollViewEx _scrollPoint;
    [SerializeField] private UILabel _labelMyPoint;

    #endregion

    #region private member data

    private DailyEventSlider _dailyEventSlider = null;  //yangkichan

    private int _currentWeekDay = 1;        //오늘이 무슨요일 인지       ...days since Sunday - [0, 6]  (금토일 묶어서 처리 1 ~ 5)  
    private int _showWeekDay = 1;           //현재 표시하는 요일이벤트   ...days since Sunday - [0, 6]  (금토일 묶어서 처리 1 ~ 5)    

    #endregion

    #region override method

    [ReceiveGameEventAttribute(typeof(DailyEventDataUpdated))]
    protected override void OnDispatchGameEvent(Type eventID, IEventDispatchParam param)
    {
        if (typeof(DailyEventDataUpdated) == eventID)
        {
            Refresh(null);
        }
    }

    public override void BindUIEvents()
    {
        base.BindUIEvents();

        //BindEvent<UIButton>(Components.Button_Global, OpenWeeklyEventRanking);

        if (_buttonRank) EventDelegate.Add(_buttonRank.onClick, OnClick_Ranking);

        if (_buttonDetailInfo) EventDelegate.Add(_buttonDetailInfo.onClick, OnClick_Question);

        var index = 0;

        //탭 버튼이벤트 설정.
        foreach (var tab in _buttonTabs)
        {
            if (null == tab)
            {
                index++;
                continue;
            }

            EventDelegate del = new EventDelegate(this, "OnClick_Tab");
            del.parameters[0].value = index + 1;
            del.parameters[0].expectedType = typeof(int);
            EventDelegate.Add(tab.onClick, del, false);

            index++;
        }

        index = 0;
         //탭 버튼이벤트 설정.
        foreach (var ex in _buttonExplanes)
        {
            if (null == ex)
            {
                index++;
                continue;
            }

            EventDelegate del = new EventDelegate(this, "Click_Explain");
            del.parameters[0].value = index;
            EventDelegate.Add(ex.onClick, del, false);

            index++;
        }

        // 탑랭커 버튼 이벤트 설정
        foreach (var ranker in _buttonTopRankers)
        {
            if (null == ranker)
                continue;

            EventDelegate.Add(ranker.onClick, OnClick_Ranking);
        }
    }

    public override void Init()
    {
        base.Init();

        // 블러 예외 처리
        IsExceptionBlur = true;
        
        SettingWeekDays();

        // init dailySlider
        if (_scrollPoint)
        {
            var dailypointroot = _scrollPoint.gameObject;
            if (dailypointroot != null)
            {
                _dailyEventSlider = dailypointroot.SetComponent<DailyEventSlider>();
                _dailyEventSlider?.SetWeekDay(_currentWeekDay);
                _dailyEventSlider?.RefreshUI();
            }
        }

        // 불러오기를 실패했을 경우 기본 이미지를 적용해줍니다.
        SetActive(_spriteBaseLllust, true);
        SetActive(_textureLllust, false);
    }

    public override void Refresh(IUIParamBase param)
    {
        base.Refresh(param);
        
        UpdateDailyEventSlider();
        SetUIItems();
        RefreshNewMark();

        CancelInvoke("SetLeftTime");
        InvokeRepeating("SetLeftTime", 0, 1);

        TotalEventListDlg.FirstOpen = false;
        TotalEventListDlg.CloseAllSubEventDlg(this);

        SetPanelDepth_ByLastDepth();

        var listDlg = UIFormManager.Instance.FindUIForm<TotalEventListDlg>();
        if (listDlg != null && listDlg.Visible)
        {
            //SetPanelDepth_ByLastDepth(listDlg);
            listDlg.Refresh(null);
        }
    }

    public override void Open(IUIParamBase param)
    {
        base.Open(param);

        UpdateDailyEventSlider();
        SetUIItems();
        RefreshNewMark();

        CancelInvoke("SetLeftTime");
        InvokeRepeating("SetLeftTime", 0, 1);

        TotalEventListDlg.FirstOpen = false;
        TotalEventListDlg.CloseAllSubEventDlg(this);

        SetPanelDepth_ByLastDepth();

        var listDlg = UIFormManager.Instance.FindUIForm<TotalEventListDlg>();
        if (listDlg != null && listDlg.Visible)
        {
            //SetPanelDepth_ByLastDepth(listDlg);
            listDlg.Refresh(null);
        }
    }

    public override void Close()
    {
        base.Close();

        EVENT.EventManager.Instance.UpdateNotificationPrefs();

        //UIFormManager.Instance.CloseUIForm<MessageBoxDlg>();

    }

    public override void Close_End()
    {
        base.Close_End();

        if (CloseReason == ReasonType.Escape)
            TotalEventListDlg.CloseReasonEscape();
    }

    #endregion

    private void UpdateDailyEventSlider()
    {
        if (_dailyEventSlider != null)  //yangkichan
            _dailyEventSlider.OnUpdate();
    }

    private void SettingWeekDays()
    {
        // 현재요일과 UI에서 표시할 요일을 받아온다.
        _showWeekDay = _currentWeekDay = EVENT.EventManager.Instance.CurrentWeekDay(); 
    }

    private void SetUIItems()
    {
        //일반 제목과 포인트
        EVENT.DailyEventData data = EVENT.EventManager.Instance.GetDailyEventData(_showWeekDay);
        //if (_DailyEventControllers._LabelEventTitle) _DailyEventControllers._LabelEventTitle.text = data.name;       //타이틀
        SetText(ref _labelSubTitle, data.name); //설명.

        SetActive(ref _objNow, _showWeekDay == _currentWeekDay);

        for (var i = 0; i < _objExplanes.Length; ++i)
        {
            if (null == _objExplanes[i])
                continue;

            if (null == _labelExplanes[i])
                continue;

            if (i < data.listExplain.Count && !string.IsNullOrEmpty(data.listExplain[i]))
            {
                SetActive(ref _objExplanes[i], true);
                SetText(ref _labelExplanes[i], data.listExplain[i]);
            }
            else
            {
                SetActive(ref _objExplanes[i], false);
            }
        }

        //이미지 셋팅
        if (null != _textureLllust)
        {
            EventManager.Instance.ImageLoad(EventManager.DAILYIMAGE_KEY, data.imageUrl, (wwwdata, error) =>
            {
                if (wwwdata != null)
                {
                    SetActive(_spriteBaseLllust, false);
                    SetActive(_textureLllust, true);

                    ShowImage(wwwdata);
                }
                else
                {
                    // 불러오기를 실패했을 경우 기본 이미지를 적용해줍니다.
                    SetActive(_spriteBaseLllust, true);
                    SetActive(_textureLllust, false);
                    Debug.LogWarning(error);
                }
            });
        }

        // 딤드처리
        _objWaiting.Ex_SetActive(_showWeekDay != EVENT.EventManager.Instance.CurrentWeekDay());

        SetTabUi((DayOfWeek)_showWeekDay);

        var dailyEventPoint = (_showWeekDay == EVENT.EventManager.Instance.CurrentWeekDay()) ? EVENT.EventManager.Instance.GetCurrentDailyEventPoint() : 0;
        SetText(ref _labelMyPoint, PublicUIMethod.ThousandSeparateString(dailyEventPoint));  //일간 이벤트 포인트

        // 최소 랭킹 진입 조건을 써줍니다.
        var leastPoint = 0;
        if (data.pointGrade.Count > 3)
            leastPoint = data.pointGrade[2];
        else
        {
            foreach (int point in data.pointGrade)
            {
                if (leastPoint <= 0)
                    leastPoint = point;

                leastPoint = Mathf.Min(leastPoint, point);
            }
        }

        var myRank = GetDaliyRank();
        if (myRank > 0)
        {
            _objMyRank.Ex_SetActive(true);
            _labelExplanation.Ex_SetActive(false);
            SetText(ref _labelMyRank, string.Format(NTextManager.Instance.GetText("UI_DAILYEVENT_RANK_MY"), $"{myRank:n0}"));

            var change = RankManager.Instance.GetRank_Changed((int)GameDefine.eRANK_TYPE.DAILY_EVENT_RANK, GetRankKind(), UserBase.User.UID, myRank, out var isNew);
            if (isNew)
            {
                _spriteUpArrow.Ex_SetActive(false);
                _spriteDownArrow.Ex_SetActive(false);
                _labelRankChange.Ex_SetText(NTextManager.Instance.GetText("UI_RANKING_NEW_ENTRANT"));
            }
            else
            {
                _spriteUpArrow.SetActive(change > 0);
                _spriteDownArrow.SetActive(change < 0);
                _labelRankChange.Ex_SetText(change == 0 ? "-" : MathF.Abs(change).ToString());
            }
        }
        else
        {
            _objMyRank.Ex_SetActive(false);
            _labelExplanation.Ex_SetActive(true);
            SetText(ref _labelExplanation, string.Format(NTextManager.Instance.GetText("UI_FESTIVAL_EVENT_RANKING_MIN_01"), $"{leastPoint:n0}"));
        }
    }

    private void RefreshNewMark()
    {
        for (var i = 0; i < _spriteTabNotices.Length; ++i)
        {
            if (null == _spriteTabNotices[i])
                continue;

            var day = i + 1;
            if (day == EVENT.EventManager.Instance.CurrentWeekDay())
            {
                var eventData = EVENT.EventManager.Instance.GetDailyEventData(day);
                if (null == eventData)
                    continue;

                var rewardCheck = false;

                for (var j = 0; j < eventData.pointGrade.Count; ++j) 
                {
                    var level = j + 1;

                    var curPoint = EVENT.EventManager.Instance.GetCurrentDailyEventPoint();
                    var levelPoint = eventData.pointGrade[j];

                    if (curPoint < levelPoint)
                        continue;

                    if (true == EVENT.EventManager.Instance.HasRewardStep(level))
                        continue;

                    rewardCheck = true;
                    break;
                }

                SetActive(_spriteTabNotices[i], rewardCheck);
            }
            else
                SetActive(_spriteTabNotices[i], false);
        }
    }

    private int GetDaliyRank()
    {
        return RANKING.RankManager.Instance.GetMyEventRank(GetRankKind());
    }

    private int GetRankKind()
    {
        return _showWeekDay == 6 ? 0 : _showWeekDay;
    }
    
    private void ShowImage(Texture info)
    {
        if (null == info)
        {
            NDebug.Log("Imageinfo is null");
            return;
        }


        if (null == _textureLllust)
        {
            NDebug.Log("_textureLllust null");
            return;
        }

        _textureLllust.mainTexture = info;
        //_textureLllust.material.mainTexture = info;
    }

    private void ShowImageAlpha(Texture info)
    {
        if (null == info)
        {
            NDebug.Log("Imageinfo is null");
            return;
        }

        if (null == _textureLllust)
        {
            NDebug.Log("_textureLllust null");
            return;
        }

        _textureLllust.material.SetTexture("_MainTransTex", info);
    }


    private void SetLeftTime()
    {
        var time = PublicMethod.GetDueDay_UTC(0);
        var day = time / PublicMethod.ONE_DAY;
        long nextdaytime = 0;

        var strTimeFormat = string.Empty;

        if (_showWeekDay != _currentWeekDay)
        {
            var diffDay = 0;
            var tempCurrentDay = _currentWeekDay;

            while (tempCurrentDay != _showWeekDay)
            {
                ++tempCurrentDay;
                ++diffDay;

                if (EVENT.EventManager.Instance.DAILY_EVENT_DAY_COUNT < tempCurrentDay)
                {
                    //diffDay += 7 - EVENT.EventManager.Instance.DAILY_EVENT_DAY_COUNT;
                    var d = DateTime.UtcNow;
                    var n = 0;
                    switch (d.DayOfWeek)
                    {
                        case DayOfWeek.Saturday:
                            n = 1;
                            break;
                        case DayOfWeek.Sunday:
                            n = 0;
                            break;
                        default:
                            n = 7 - EVENT.EventManager.Instance.DAILY_EVENT_DAY_COUNT;
                            break;
                    }
                    diffDay += n;
                    tempCurrentDay = 1;
                }
            }
            nextdaytime = (day + diffDay) * PublicMethod.ONE_DAY;

            var lefttime = nextdaytime - time;

            SetActive(_labelWaiting, true);
            SetActive(_labelEndTime, false);
            SetText(ref _labelWaiting, string.Format(NTextManager.Instance.GetText("UI_DAILY_EVENT_TIME_HELP_01"), PublicMethod.TimeToString(lefttime)));
        }
        else
        {
            nextdaytime = (day + 1) * PublicMethod.ONE_DAY;

            DateTime d = DateTime.UtcNow;

            int n = 0;

            switch (d.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    n = 1;
                    break;
                case DayOfWeek.Sunday:
                    n = 0;
                    break;
            }

            //하루동안 남은 시간 계산
            if (EVENT.EventManager.Instance.DAILY_EVENT_DAY_COUNT == 6)
            {
                if (_currentWeekDay == 6)   //월, 화, 수, 목, 금, 토일 - 6개짜리 이벤트 일경우
                    nextdaytime += PublicMethod.ONE_DAY * n;
            }

            var lefttime = nextdaytime - time;

            SetActive(_labelEndTime,true);
            SetActive(_labelWaiting, false);
            SetText(ref _labelEndTime, PublicMethod.TimeToString(lefttime));
        }
    }

    
    private void OpenDailyEventRank(GameDefine.eEVENT_RANK_CATEGORY InRankCategory)
    {
        RANKING.RankManager.Instance.Send_Daily_Event_RankInfo_Get_Req(InRankCategory, true);
    }


    private void OnClick_Ranking()
    {
        PublicSoundMethod.PlayClickCommon();

        // 일일 이벤트 랭킹 페이지 오픈
        OpenDailyEventRank((GameDefine.eEVENT_RANK_CATEGORY)GetRankKind());
    }

    private void OnClick_Question()
    {
        PublicSoundMethod.PlayClickCommon(GameDefine.eCLICK_SOUND_TYPE.CLICK_SOUND_4);

        var contentskind = (int)GameDefine.eQuestionPopUpContentsType.DailyEvent;
        BaseQuestionDlg.OpenQuestionDlg(contentskind);
    }

    private void OnClick_Tab(int selectTab)
    {
        PublicSoundMethod.PlayClickCommon();

        _showWeekDay = selectTab;

        SetUIItems();
        SetLeftTime();
        RefreshNewMark();

        if (_dailyEventSlider)
        {
            _dailyEventSlider.SetWeekDay(_showWeekDay);
            _dailyEventSlider.RefreshUI();
        }
        UpdateDailyEventSlider();
    }

    private void Click_Explain(int i_Value)
    {
        var dailyData = EVENT.EventManager.Instance.GetDailyEventData(_showWeekDay);
        if (null == dailyData)
        {
            NDebug.Log("_dailyData is null" + _showWeekDay);
            return;
        }

        PublicSoundMethod.PlayClickCommon(GameDefine.eCLICK_SOUND_TYPE.CLICK_SOUND_4);
        
        // Out of Array.... 
        if (i_Value < 0 || dailyData.pointDetail.Count <= i_Value)
            return;

        // Out of Array.... 
        if (i_Value < 0 || dailyData.subTitle_PointDetail.Count <= i_Value)
            return;

        // 변원일, 주석을 풀어야 합니다. 메일이벤트에 보물 스킬이?? 문의해보자~!
        TotalEventPointInfoDlg.Open_For_DailyEventExplain(dailyData.subTitle_PointDetail[i_Value], dailyData.pointDetail[i_Value]);
    }

    public void DailyEventSliderUpdate()
    {
        if (_dailyEventSlider != null)
            _dailyEventSlider.OnUpdate();
    }

    private void SetTabUi(DayOfWeek weekDay)
    {
        var sprIdx = 0;
        switch (weekDay)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
            case DayOfWeek.Wednesday:
            case DayOfWeek.Thursday:
            case DayOfWeek.Friday:
                sprIdx = (int)weekDay - 1;
                break;
            case DayOfWeek.Saturday:
            case DayOfWeek.Sunday:
                sprIdx = 5;
                break;
        }

        for (var i = 0; i < _spriteTabOn.Length; ++i)
        {
            if (null == _spriteTabOn[i])
                continue;

            if (null == _labelDays[i])
                continue;

            SetActive(_spriteTabOn[i], i == sprIdx);
            _labelDays[i].Ex_SetTextColor(i == sprIdx ? PublicUIMethod.Data.COLOR_TEXT_STATE_WHITE.ColorValue : PublicUIMethod.GetHexToColor("BDAF9D"));
            _labelToday[i].Ex_SetActive(i + 1 == EVENT.EventManager.Instance.CurrentWeekDay());
        }

        SetTop3Ranker((int)weekDay);
    }

    private void SetTop3Ranker(int weekDay)
    {
        var eventData = EVENT.EventManager.Instance.GetDailyEventData(weekDay);
        if (null == eventData)
            return;
        
        var listTopRanker = eventData.listTopRankers;
        var topRankCount = 3;

        // 표시할 랭커가 없다면 라벨을 켜줍니다.
        if (listTopRanker.Count <= 0)
        {
            SetActive(_buttonTopRankers[0], false);
            SetActive(_buttonTopRankers[1], false);
            SetActive(_buttonTopRankers[2], false);

            _labelNoTopRanker.SetActive(true);
        }
        else
        {
            _labelNoTopRanker.SetActive(false);

            SetActive(_buttonTopRankers[0], true);
            SetActive(_buttonTopRankers[1], true);
            SetActive(_buttonTopRankers[2], true);

            for (var i = 0; i < topRankCount; i++)
            {
                if (null == _buttonTopRankers[i])
                    continue;

                if (null == _labelRankerName[i])
                    continue;

                if (null == _labelRankerPower[i])
                    continue;

                if (null == _spriteRankerPortrait[i])
                    continue;

                if (null == _spriteNoRanker[i])
                    continue;
                
                var isActive = i < listTopRanker.Count;

                SetActive(_spriteRankerPortrait[i], isActive);
                SetActive(_labelRankerName[i], isActive);
                SetActive(_labelRankerPower[i], isActive);

                if (!isActive)
                {
                    SetActive(_spriteNoRanker[i], true);
                }
                else
                {
                    SetSprite(ref _spriteRankerPortrait[i], "Atlas_Portrait_Leader", Character.Lord.GetLordUpperBodySpriteName(listTopRanker[i].PortraitId));
                    SetText(ref _labelRankerName[i], PublicUIMethod.MergeGuildAndLrodName(listTopRanker[i].NickName, listTopRanker[i].Name));
                    SetText(ref _labelRankerPower[i], PublicUIMethod.ThousandSeparateString(listTopRanker[i].RankPoint));
                }
            }
        }
    }

    //private void SetRankfluctuation()
    //{
    //    var prevRankKey = $"DailyEventRank_{UserBase.User.UID}_{_showWeekDay}";

    //    if (_showWeekDay != _currentWeekDay)
    //    {
    //        if (PlayerPrefs.HasKey(prevRankKey))
    //            PlayerPrefs.DeleteKey(prevRankKey);

    //        SetActive(_spriteUpArrow, false);
    //        SetActive(_spriteUpArrow, false);
    //        SetActive(_labelRankChange, false);
    //        return;
    //    }

    //    if (PlayerPrefs.HasKey(prevRankKey))
    //    {
    //        int prevRank = PlayerPrefs.GetInt(prevRankKey);
    //        int curRank = GetDaliyRank();

    //        if (prevRank == curRank || curRank == 0 || prevRank == 0) // 등수 등락이 없거나 1000위밖이면
    //        {
    //            SetActive(_spriteUpArrow, false);
    //            SetActive(_spriteUpArrow, false);
    //            SetActive(_labelRankChange, false);
    //        }
    //        else if (prevRank > curRank) // 순위가 오르면
    //        {
    //            SetActive(_spriteUpArrow, true);
    //            SetActive(_spriteDownArrow, false);
    //            SetActive(_labelRankChange, true);
    //            SetText(ref _labelRankChange, (prevRank - curRank).ToString());
    //        }
    //        else // 순위가 떨어지면
    //        {
    //            SetActive(_spriteUpArrow, false);
    //            SetActive(_spriteDownArrow, true);
    //            SetActive(_labelRankChange, true);
    //            SetText(ref _labelRankChange, (curRank - prevRank).ToString());
    //        }
    //    }
    //    else
    //    {
    //        SetActive(_spriteUpArrow, false);
    //        SetActive(_spriteUpArrow, false);
    //        SetActive(_labelRankChange, false);
    //    }

    //    PlayerPrefs.SetInt(prevRankKey, GetDaliyRank());
    //}
}


