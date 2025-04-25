using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BaseTable;
using Contents.User;
using DEFINE;
using RANKING;

namespace EVENT
{
    public enum EVENT_MESSAGE
    {
        EVENT_START,
        EVENT_END,
        END,
    }

    public class StepInfo
    {
        public byte step;
        public Int32 point;
        public Int32 castleLevel;
        public Int64 startTime;
        public Int64 endTime;
        public bool[] rewarded = new bool[15] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
    }

    public class DailyEventData
    {
        public int missionType;
        public int dayOfWeek;
        public string name;        
        public string desc;
        public string toptextname;

        public string imageUrl;    //이미지 URL
        public List<string> listExplain = new List<string>(); 
        public List<string> pointDetail = new List<string>();            //각 포인트 획득방식 //기획과 줄바꿈으로 스플릿하여 그리드로 보여줘야 함
        public List<string> subTitle_PointDetail = new List<string>();   //각 포인트 획득방식의 서브타이틀.
        public List<int> pointGrade = new List<int>();                   //각 보상별로 도달해야 하는 포인트 //서버에서 받아와야 한다 //yangchan
        public List<BaseTable.DailyEventRewardItem> listRewards = new List<BaseTable.DailyEventRewardItem>();
        public List<TopRankerInfo> listTopRankers = new List<TopRankerInfo>();
        
        public DailyEventData(
            int missionType
            , int dayOfWeek
            , string name
            , string desc
            , string imageUrl
            , string toptextname
            , List<string> listExplain 
            , List<string> pointDetail 
            , List<string> subTitle_PointDetail 
            , List<int> pointGrade
            , List<BaseTable.DailyEventRewardItem> listRewards 
            , List<TopRankerInfo> listTopRanker)
        {
            this.missionType = missionType;
            this.dayOfWeek = dayOfWeek;
            this.name = name;
            this.desc = desc;
            this.imageUrl = imageUrl;
            this.toptextname = toptextname;
            this.listExplain = listExplain;
            this.pointDetail = pointDetail;
            this.subTitle_PointDetail = subTitle_PointDetail;
            this.pointGrade = pointGrade;
            this.listRewards = listRewards;
            this.listTopRankers = listTopRanker;
        }

        public BaseTable.DailyEventRewardItem GetRewardItemFromStep(int i_step)
        {
            if (i_step < 0 || listRewards.Count <= i_step)
                return BaseTable.DailyEventRewardItem.emptyData;

            return listRewards[i_step];
        }
    }

    public class UseEventData
    {
        public int eventKind;
        public int groupKind;
        public int pointGrade;
        public string tab_URL;
        public string tab_Title;
        public string mainTitle_Text_Key;
        public string sub_Title_Text_Key;
        public string notice_Text_Key;

        public List<BaseTable.useEventRewardItem> listRewards = new List<BaseTable.useEventRewardItem>();

        public UseEventData(
              int eventKind
            , int groupKind
            , int pointGrade
            , string tab_URL
            , string tab_Title
            , string mainTitle_Text_Key
            , string sub_Title_Text_Key
            , string notice_Text_Key
            , List<BaseTable.useEventRewardItem> listRewards)
        {
            this.eventKind = eventKind;
            this.groupKind = groupKind;
            this.pointGrade = pointGrade;
            this.tab_URL = tab_URL;
            this.tab_Title = tab_Title;
            this.mainTitle_Text_Key = mainTitle_Text_Key;
            this.sub_Title_Text_Key = sub_Title_Text_Key;
            this.notice_Text_Key = notice_Text_Key;
            this.listRewards = listRewards;
        }

        public BaseTable.useEventRewardItem GetRewardItemFromStep(int i_step)
        {
            if (i_step < 0 || listRewards.Count <= i_step)
                return BaseTable.useEventRewardItem.emptyData;

            return listRewards[i_step];
        }
    }


    public class DailyEvent
    {
        public byte curStep;

        public Int64 startTime;
        public Int64 endTime;
        public Int64 eventID;
        public Int32 kind;
        public Int32 subKind;
        // 월= 1,화,수,목,금
        public SortedList<byte, StepInfo> stepInfos = new SortedList<byte, StepInfo>();    // 월,화,수,목,금

        // 토탈 일 랭킹 = 0, 월,화,수,목,금
        public SortedList<byte, Int32> dailyRankings = new SortedList<byte, Int32>();        // 토탈, 월, 화, 수, 목, 금
        // 토탈 주 랭킹
        public SortedList<byte, Int32> weekRankings = new SortedList<byte, Int32>();        // 토탈 주 랭킹

        public void Clear()
        {
            startTime = 0;
            endTime = 0;
            eventID = 0;
            kind = 0;
            subKind = 0;
            stepInfos.Clear();
            //rankings.Clear();
        }

        public int GetStepRanking(byte step)
        {
            return 0;
            //if (rankings.ContainsKey(step))
            //    return rankings[step];
            //else
            //    return 0;
        }
    }


    public class SeasonEventData
    {
        public Int64 _startTime = 0;
        public Int64 _completeTime = 0;
        public int _kind = 0;
        public string _title = string.Empty;
        public string _icon = string.Empty;
        public string _subtitle = string.Empty;
        public string _summary = string.Empty;

        public SeasonEventData(int kind, string title, string icon, string subtitle, string summary, Int64 starttime, Int64 completetime)
        {
            _kind = kind;
            _title = title;
            _icon = icon;
            _summary = summary;
            _subtitle = subtitle;
            _startTime = starttime;
            _completeTime = completetime;
        }
    }

    public class EventManager : NrTSingleton<EventManager>
    {
        /// <summary>
        /// 다운로드한 이미지  네임.
        /// </summary>
        [Serializable]
        class _LocalImageData
        {
            public string name;
        }

        /// <summary>
        /// 이미지 다운로드 리스트
        /// </summary>
        [Serializable]
        class LocalImageDatas
        {
            public List<_LocalImageData> localImageData = new List<_LocalImageData>();

            public void CleanUpFile(string key, List<string> paths)
            {
                List<_LocalImageData> tempPath = new List<_LocalImageData>();
                for (int i = 0; i < localImageData.Count; ++i)
                {
                    var localName = localImageData[i];
                    var findItem = paths.Find((v) => { if (v == localName.name) return true; return false; });
                    if (findItem == null)
                    {
                        tempPath.Add(localName);
                    }
                }

                for (int i = 0; i < tempPath.Count; ++i)
                {

                    string fullPath = EVENT.EventManager.Instance.Get_EventImageDirectory(key) + "/" + tempPath[i].name;

                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                    localImageData.Remove(tempPath[i]);
                }
            }
        }


        // 이벤트 확인 정보.
        [Serializable]
        struct DailyEventNotifyData
        {
            public Int64 nUTCTime;
            public Int32 nDayType;
            public DailyEventNotifyData(Int64 utcTime, Int32 dayType)
            {
                nUTCTime = utcTime;
                nDayType = dayType;
            }
        }


        #region private member data
        private List<IEvent>                        _runningEvents  = new List<IEvent>();
        private List<IEvent>                        _waitingEvents  = new List<IEvent>();
        public  Dictionary<int, DailyEventData>     _dailyEventData = new Dictionary<int, DailyEventData>();
        public List<TopRankerInfo>                  _weeklyRankData = new List<TopRankerInfo>();
        public Dictionary<int, UseEventData>        _useEventData = new Dictionary<int, UseEventData>();


        private int         weekDay                 = 1; // 1: 월 ~ 6: 주말
        public int          dailyEventPoint         = 0;
        public int          dailyEventWeeklyPoint   = 0;
        public List<Int32>  dailyEventRewardSteps   = new List<Int32>();

        public int          DAILY_EVENT_DAY_COUNT   = 6;
        public const string SEASONIMAGE_KEY         = "season";
        public const string DAILYIMAGE_KEY          = "daily";

        string DAILYEVENT_NOTIFY_PREF_KEY           => string.Format("daily_event_notify_{0}", UserBase.User.UID);

        DailyEventNotifyData    _notificationData                   = new DailyEventNotifyData(0, 0);
        LocalImageDatas         _localSeasonImageInfo               = new LocalImageDatas();
        LocalImageDatas         _localDailyImageInfo                = new LocalImageDatas();

        public Dictionary<int, SeasonEventData> _seasonEventData    = new Dictionary<int, SeasonEventData>();

        #endregion private member data


        #region constructor
        private EventManager()
        {

        }
        #endregion constructor


        #region public methods
        public void Init()
        {
            // 여기 호출 안되는 듯??? 나중에 주석처리 해버리자..

            InitEvent();
            ReqGetRunningEvent();
            Send_GS_DAILY_EVENT_DATA_GET_REQ(); //yangkichan_1213
            InitEventImage();
            
        }

        public bool GetIsNewDailyEvent()
        {
            var nPrevDay        = _notificationData.nUTCTime       / DEFINE.GameDefine.SECOND_PER_DAY;
            var nCurDay         = PublicMethod.GetDueDay_UTC(0)    / DEFINE.GameDefine.SECOND_PER_DAY;

            // 마지막으로 확인한 날짜와 현재 날짜 차이가 충분히 경과했다. (_notificationData.nDayType == CurrentWeekDay() 일 경우, 날짜로 체크한다.)
            var itsBeenAWeek    = nCurDay - nPrevDay >= DAILY_EVENT_DAY_COUNT;

            return _notificationData.nDayType != CurrentWeekDay() || itsBeenAWeek;
        }


        public void NotifyMessage(EVENT_MESSAGE message)
        {
            if (!ValidMessage(message))
                return;


            switch (message)
            {
                case EVENT_MESSAGE.EVENT_START:
                    break;
                case EVENT_MESSAGE.EVENT_END:
                    break;
            }
        }
        public void NotifyRunningList(PROTOCOL.FLATBUFFERS.GS_EVENT_RUNNING_LIST_GET_ACK ack)
        {
            for (int i = 0; i < ack.RunningEventKindLength; ++i)
                AWakeEvent(ack.GetRunningEventKind(i));
        }

        public void Add(BaseTable.NrEventInfoData info)
        {
            if (info.ConditionKind == 170 && info.ConditionValue == 1)
                return;

            var cofEvent = Get(info.Kind);
            if (cofEvent == null)
                _waitingEvents.Add(new IEvent(info));
        }
        public void Remove(int kind)
        {

        }
        public IEvent Get(int kind)
        {
            var cofEvent = Get(_waitingEvents, kind);
            if (cofEvent != null) return cofEvent;

            cofEvent = Get(_runningEvents, kind);
            return cofEvent;
        }

        public List<IEvent>.Enumerator GetEnum()
        {
            return _runningEvents.GetEnumerator();
        }
        public List<IEvent>.Enumerator GetWaitingEnum()
        {
            return _waitingEvents.GetEnumerator();
        }

        public int CurrentWeekDay()
        {
            // 1 : 월 ~ 6 : 주말
            return Math.Max(1, weekDay);
        }

        public DailyEventData GetCurrentDailyEventData()
        {
            return GetDailyEventData(CurrentWeekDay()); //yangkichan 
        }

        public long GetCurrentDailyEventPoint()
        {
            return EVENT.EventManager.Instance.dailyEventPoint;
        }

        public bool HasRewardStep(Int32 rewardStep)
        {
            return dailyEventRewardSteps.Contains(rewardStep);
        }

        public DailyEventData GetDailyEventData(int weekDay)  //1 = 월, 6 = 주말
        {
            if (_dailyEventData.ContainsKey(weekDay))
                return _dailyEventData[weekDay];
            else
                return null;
        }

        public int WaitingDailyRewardCount()
        {
            int count = 0;
            
            var data = GetCurrentDailyEventData();
            if (data != null)
            {
                int Stepindex = 0;
                // 최소 획득 점수가 되고 아직 보상 받은 것이 없을 경우
                foreach (int rewardGrade in data.pointGrade)
                {
                    if(rewardGrade <= dailyEventPoint &&
                        HasRewardStep(Stepindex + 1) == false)
                    {
                        ++count;
                    }

                    ++Stepindex;
                }
            }

            return count;
        }


        public void SetDailyEventData(
              int missionType
            , int dayOfWeek
            , List<int> pointGrade
            , string name
            , string desc
            , List<BaseTable.DailyEventRewardItem> rewards
            , List<string> pointDetail
            , string imageUrl
            , List<string> explains
            , List<string> subTitlePointDetail
            , string toptextname
            , List<TopRankerInfo> topRankers)
        {   
            DailyEventData updateData = new DailyEventData(missionType, dayOfWeek, name, desc, imageUrl, toptextname, explains, pointDetail, subTitlePointDetail, pointGrade, rewards, topRankers);

            if (_dailyEventData.ContainsKey(dayOfWeek))
                _dailyEventData[dayOfWeek] = updateData;
            else
                _dailyEventData.Add(dayOfWeek, updateData);
        }

        /// <summary>
        /// 다운받은 매일이벤트 이미지 체크. 쓸모없는 것은 삭제.
        /// </summary>
        private void Check_For_Delete_DailyEventImage()
        {
#if UNITY_ANDROID
            string platform = "_and.assetbundle";
#elif UNITY_STANDALONE_WIN
            string platform = "_win.assetbundle";
#else
            string platform = "_ios.assetbundle";
#endif

            List<string> imageList = new List<string>();

            var iterData = _dailyEventData.GetEnumerator();
            while (iterData.MoveNext())
            {
                var info = iterData.Current.Value;
                if (null == info)
                    continue;

                imageList.Add(string.Format("{0}{1}", info.imageUrl, platform));
            }

            ServerGet_EventImageList(DAILYIMAGE_KEY, imageList);
        }


        #region season event
        /// <summary>
        /// 시즌이벤트 이미지 로드
        /// </summary>
        /// 
        public void ImageLoad(string key, string packageImage, Action<Texture2D, string> callBack, bool IsForceDownLoad = true)
        {

            //#if UNITY_ANDROID
            //            string platform = "_and.assetbundle";
            //#elif UNITY_STANDALONE_WIN
            //            string platform = "_win.assetbundle";
            //#else
            //            string platform = "_ios.assetbundle";
            //#endif

            //            string image_path = string.Format(
            //                "{0}/event_image/{1}/{2}{3}",
            //                WOTURL.S3URL, key, packageImage, platform);

            string url = string.Format("{0}/event_image/{1}/", WOTSettings.TargetCDNURL, key);
 
            // 이미지 로딩 
            HttpImageDownloadManager.Instance.Download(url, packageImage, callBack, key, IsForceDownLoad);
        }

        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /*public void ServerGet_GS_SEASON_EVENT_FREE_GOLD_ACK(PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_FREE_GOLD_ACK ack)
        {
            if (ack == null)
            {
                NDebug.Log("GS_SEASON_EVENT_FREE_GOLD_ACK is null");
                return;
            }
        }*/

        /// <summary>
        /// Season Event Free Gold Time으로 무료 보상을 받을 지 체크해서 받을 수 있으면 true를 리턴한다.
        /// 제작 : 심규화
        /// </summary>
        /// <returns></returns>
        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        public bool Get_Season_Event_Free_Gold_Time_Check()
        {
            long curtime = PublicMethod.GetDueDay_UTC(0);

            if (curtime - UserBase.User.SeasonEventFreeItemRewardedTime > TableConstData.Instance.CONST_FREE_REWARD_LIMITED_TIME)
                return true;

            return false;
        }

        /// <summary>
        /// Season Event Free Gold{ 일 수랑 비교해서 파라미터의 일 수가 스타트 타임보다 높으면 트루 리턴}
        /// 제작 : 심규화
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="plusDay"></param>
        /// <returns></returns>
        public bool Get_Time_Remaining_Compare(long startTime, int plusDay)
        {
            long beforethirtyDay = PublicMethod.ConvertTimeDay_UTC(plusDay);

            return (PublicMethod.GetDueDay_UTC(beforethirtyDay) > startTime);  // 30일 이 후 이벤트는 보여주지 않는다.
        }

        /// <summary>
        /// 현재 NoticeBoard안에 표시될 Event의 수를 리턴합니다.
        /// 제작 : 심규화
        /// </summary>
        /// <param name="ack"> SeasonEventlist의 ack </param>
        /// <returns></returns>
        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /*public int Get_Season_Event_List_Count(PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_LIST_ACK ack)
        {
            int count = 0;
            for (int i = 0; i < ack.EventInfoLength; i++)
            {
                var sEvent = ack.GetEventInfo(i);
                if (sEvent != null && sEvent.Kind > 0 && sEvent.Kind != 6)
                {
                    if (PublicMethod.leftUTCSeconds(sEvent.StartTime) > 0)
                    {
                        if (Get_Time_Remaining_Compare(sEvent.StartTime, TableConstData.Instance.CONST_SEASON_EVENT_VIEW_VALUE))  // 30일 이 후 이벤트는 보여주지 않는다.
                        {
                            count++;
                        }
                        else
                            continue;
                    }
                    else
                        count++;
                }
            }

            return count;
        }*/

        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /*public void ServerGet_GS_SEASON_EVENT_LIST_ACK(PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_LIST_ACK ack)
        {
            if (ack == null)
            {
                NDebug.Log("GS_SEASON_EVENT_LIST_ACK is null");
                return;
            }

            _seasonEventData.Clear();

            int count = Get_Season_Event_List_Count(ack);

            if (Get_Season_Event_Free_Gold_Time_Check() == false && count == 0)
            {
                var seasondlg = UIFormManager.Instance.FindUIForm<SeasonEventDlg>();

                if (null != seasondlg)
                    seasondlg.Close();
                PublicUIMethod.SetPopUpAlramMsg(NLibCs.NTextManager.Instance.GetText("UI_SEASON_EVENT_NOTICE_NULL"));

                return;
            }

            for (int i = 0; i < ack.EventInfoLength; ++i)
            {
                var info = ack.GetEventInfo(i);
                if (null == info)
                    continue;

                _seasonEventData.Add(info.Kind,
                    new SeasonEventData(
                    info.Kind,
                    TKString.BytesString(info.GetTitleBytes().Value),
                    TKString.BytesString(info.GetIconNameBytes().Value),
                    TKString.BytesString(info.GetSubTitleBytes().Value),
                    TKString.BytesString(info.GetSummaryBytes().Value),
                    info.StartTime, info.CompleteTime));
            }

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
            string platform = "_and.assetbundle";
#else
            string platform = "_ios.assetbundle";
#endif

            List<string> imageList = new List<string>();

            for (int i = 0; i < ack.EventInfoLength; ++i)
            {
                var info = ack.GetEventInfo(i);
                if (null == info)
                    continue;

                string imageName = TKString.BytesString(info.GetImageBytes().Value) + platform;
                imageList.Add(imageName);
            }

            ServerGet_EventImageList(SEASONIMAGE_KEY, imageList);

            var seasoneventDlg = UIFormManager.Instance.OpenUIForm<SeasonEventDlg>();
            if (null != seasoneventDlg)
                seasoneventDlg.Update_SeasonEventList();
        }*/

        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /*public void Get_GS_SEASON_EVENT_DETAIL_INFO_ACK(PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_DETAIL_INFO_ACK ack)
        {
            if (ack == null)
                return;
            string title = string.Empty;
            string explain = string.Empty;
            string imagename = string.Empty;

            if (null != ack.GetTitleBytes())
                title = TKString.BytesString(ack.GetTitleBytes().Value);
            if (null != ack.GetExplainBytes())
                explain = TKString.BytesString(ack.GetExplainBytes().Value);
            if (null != ack.GetImageBytes())
                imagename = TKString.BytesString(ack.GetImageBytes().Value);


            title = PublicUIMethod.Replace_NewLineTexts(title);
            explain = PublicUIMethod.Replace_NewLineTexts(explain);

            int kind = ack.Kind;

            _ImageInfo info = new _ImageInfo();

            Int64 starttime = 0;
            Int64 completetime = 0;
            if (EVENT.EventManager.Instance._seasonEventData.ContainsKey(kind))
            {
                starttime = EVENT.EventManager.Instance._seasonEventData[kind]._startTime;
                completetime = EVENT.EventManager.Instance._seasonEventData[kind]._completeTime;
            }

            var seasonDetailDlg = UIFormManager.Instance.OpenUIForm<SeasonEventDetailDlg>();
            if (null != seasonDetailDlg)
            {
                seasonDetailDlg.SetEventInfo(title, explain, starttime, completetime);
            }

            ImageLoad(info, SEASONIMAGE_KEY, imagename, (data, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    var detaildlg = UIFormManager.Instance.GetUIForm<SeasonEventDetailDlg>();
                    if (null != detaildlg)
                    {
                        detaildlg.SetBannerImage(info);
                    }

                }
                else
                {
                    Debug.LogWarning(error);
                }
            });

        }*/

        /// <summary>
        /// 시즌이벤트 이미지 리스트에 포함안된 로컬 이미지 파일 삭제.
        /// </summary>
        public void ServerGet_EventImageList(string key, List<string> imageList)
        {
            string strLocalImageData = PlayerPrefs.GetString(key);
            switch (key)
            {
                case SEASONIMAGE_KEY:
                    if (!string.IsNullOrEmpty(strLocalImageData))
                        _localSeasonImageInfo = JsonUtility.FromJson<LocalImageDatas>(strLocalImageData);
                    CleanUpImage(ref _localSeasonImageInfo, imageList, key);
                    break;
                case DAILYIMAGE_KEY:
                    if (!string.IsNullOrEmpty(strLocalImageData))
                        _localDailyImageInfo = JsonUtility.FromJson<LocalImageDatas>(strLocalImageData);

                    CleanUpImage(ref _localDailyImageInfo, imageList, key);
                    break;
            }
        }

        /// <summary>
        /// 시즌이벤트 다운받은 로컬 이미지. PlayerPref에 저장
        /// </summary>
        public void Save_EventImageFile(string key, string imagename)
        {
            _LocalImageData _data = new _LocalImageData();
            _data.name = imagename;
            switch (key)
            {
                case SEASONIMAGE_KEY:
                    _localSeasonImageInfo.localImageData.Add(_data);
                    PlayerPrefs.SetString(key, JsonUtility.ToJson(_localSeasonImageInfo));
                    break;
                case DAILYIMAGE_KEY:
                    _localDailyImageInfo.localImageData.Add(_data);
                    PlayerPrefs.SetString(key, JsonUtility.ToJson(_localDailyImageInfo));
                    break;
            }
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 시즌이벤트 리스트 요청.
        /// </summary>
        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /*public void Send_GS_SEASON_EVENT_LIST_REQ()
        {
            Func<object> offsetmethod = () =>
            {
                return PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_LIST_REQ.CreateGS_SEASON_EVENT_LIST_REQ(FlatBuffers.NFlatBufferBuilder.FBB, User.Instance.UID);
            };
            FlatBuffers.NFlatBufferBuilder.SendBytes<PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_LIST_REQ>(PROTOCOL.GAME.ID.ePACKET_ID.GS_SEASON_EVENT_LIST_REQ, offsetmethod);

        }*/

        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /*public void Send_GS_SEASON_EVENT_FREE_GOLD_REQ()
        {
            Func<object> offsetmethod = () =>
            {
                return PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_FREE_GOLD_REQ.CreateGS_SEASON_EVENT_FREE_GOLD_REQ(FlatBuffers.NFlatBufferBuilder.FBB, User.Instance.UID);
            };
            FlatBuffers.NFlatBufferBuilder.SendBytes<PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_FREE_GOLD_REQ>(PROTOCOL.GAME.ID.ePACKET_ID.GS_SEASON_EVENT_FREE_GOLD_REQ, offsetmethod);
        }*/

        // 변원일, 시즌 이벤트 관련은 우선 주석처리 하였습니다.
        /* public void Send_GS_SEASON_EVENT_DETAIL_INFO_REQ(int kind)
        {
            Func<object> offsetmethod = () =>
            {
                return PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_DETAIL_INFO_REQ.CreateGS_SEASON_EVENT_DETAIL_INFO_REQ(FlatBuffers.NFlatBufferBuilder.FBB, kind);
            };
            FlatBuffers.NFlatBufferBuilder.SendBytes<PROTOCOL.FLATBUFFERS.GS_SEASON_EVENT_DETAIL_INFO_REQ>(PROTOCOL.GAME.ID.ePACKET_ID.GS_SEASON_EVENT_DETAIL_INFO_REQ, offsetmethod);
        }*/

        public string Get_EventImageDirectory(string key)
        {
            return Application.persistentDataPath + "/eventimage/" + key;
        }
        #endregion season event

        #endregion public methods

        #region private methods
        private void InitEvent()
        {
            //일단 ...테이블에 있는 것을 이벤트로 편집해서 넣어야겠지...?
            var iter = BaseTable.TableEventInfo.Instance.GetEnum();
            while (iter.MoveNext())
                Add(iter.Current.Value);
        }
        private void ClearEvents()
        {
            _runningEvents.Clear();
            _waitingEvents.Clear();
        }

        private IEvent Get(List<IEvent> list, int kind)
        {
            return list.Find((first) =>
            {
                return first.Kind.Equals(kind);
            });
        }
        private bool ValidMessage(EVENT_MESSAGE message)
        {
            return message != EVENT_MESSAGE.END;
        }

        private void AWakeEvent(int kind)
        {
            var cofEvent = Get(_waitingEvents, kind);
            if (cofEvent != null)
            {
                _runningEvents.Add(cofEvent);
                _waitingEvents.Remove(cofEvent);
            }
        }

        public void ReqGetRunningEvent()
        {
            var fbb = FlatBuffers.NFlatBufferBuilder.FBB;

            System.Func<object> offsetmethod = () =>
            {
                PROTOCOL.FLATBUFFERS.GS_EVENT_RUNNING_LIST_GET_REQ.StartGS_EVENT_RUNNING_LIST_GET_REQ(fbb);
                return PROTOCOL.FLATBUFFERS.GS_EVENT_RUNNING_LIST_GET_REQ.EndGS_EVENT_RUNNING_LIST_GET_REQ(fbb);
            };
            FlatBuffers.NFlatBufferBuilder.SendBytes<PROTOCOL.FLATBUFFERS.GS_EVENT_RUNNING_LIST_GET_REQ>(PROTOCOL.GAME.ID.ePACKET_ID.GS_EVENT_RUNNING_LIST_GET_REQ, offsetmethod);
        }

        public void Send_GS_DAILY_EVENT_DATA_GET_REQ()  //yangkichan_1213
        {
            System.Func<object> OffsetMethod = () =>
            {
                object offset = PROTOCOL.FLATBUFFERS.GS_DAILY_EVENT_DATA_GET_REQ.CreateGS_DAILY_EVENT_DATA_GET_REQ(FlatBuffers.NFlatBufferBuilder.FBB, 0);
                return offset;
            };
            FlatBuffers.NFlatBufferBuilder.SendBytes<PROTOCOL.FLATBUFFERS.GS_DAILY_EVENT_DATA_GET_REQ>(PROTOCOL.GAME.ID.ePACKET_ID.GS_DAILY_EVENT_DATA_GET_REQ, OffsetMethod);
        }

        public void AddDailyEventData(PROTOCOL.FLATBUFFERS.GS_DAILY_EVENT_DATA_GET_ACK ack)
        {   
            DAILY_EVENT_DAY_COUNT = ack.DailyEventLength;

            for (int i = 0; i < ack.DailyEventLength; i++)
            {
                int missionType = ack.GetDailyEvent(i).MissionType;
                int dayOfWeek = ack.GetDailyEvent(i).WeekDay;
                List<int> pointGrade = new List<int>();
                List<BaseTable.DailyEventRewardItem> rewards = new List<BaseTable.DailyEventRewardItem>();
                List<string> listExplain = new List<string>(ack.GetDailyEvent(i).ExplainsLength);
                List<string> listPointDetail = new List<string>(ack.GetDailyEvent(i).PointDetailLength);
                List<string> listPointDetailSubTitle = new List<string>(ack.GetDailyEvent(i).SubTitleExplainLength);
                List<TopRankerInfo> listTopRankers = new List<TopRankerInfo>();

                string name;
                string desc;
                string imageUrl;
                string toptextname;

                for (int j = 0; j < ack.GetDailyEvent(i).PointGradeLength; j++)
                {
                    pointGrade.Add(ack.GetDailyEvent(i).GetPointGrade(j));
                }

                for (int j = 0; j < ack.GetDailyEvent(i).RewardsLength; j++)
                {
                    var rewardInfo = ack.GetDailyEvent(i).GetRewards(j);
                    if (null == rewardInfo) continue;

                    BaseTable.DailyEventRewardItem rewardItem = new BaseTable.DailyEventRewardItem(rewardInfo.Step);
                    for (int k = 0; k < rewardInfo.RewardIDLength && k < rewardInfo.RewardNumLength; ++k)
                    {
                        BaseTable.RewardItem item = new BaseTable.RewardItem();
                        item.itemKind = rewardInfo.GetRewardID(k);
                        item.itemCnt = rewardInfo.GetRewardNum(k);
                        rewardItem.listRewardItem.Add(item);
                    }
                    rewards.Add(rewardItem);                    
                }
              
                for (int j = 0; j < ack.GetDailyEvent(i).ExplainsLength; ++j)
                {
                    listExplain.Add(NLibCs.NTextManager.Instance.GetText(TKString.BytesString(ack.GetDailyEvent(i).GetExplains(j).GetExplainBytes().Value)));
                }

                for (int j = 0; j < ack.GetDailyEvent(i).PointDetailLength; ++j)
                {
                    listPointDetail.Add(NLibCs.NTextManager.Instance.GetText(TKString.BytesString(ack.GetDailyEvent(i).GetPointDetail(j).GetExplainBytes().Value)));
                }

                for (int j = 0; j < ack.GetDailyEvent(i).SubTitleExplainLength; ++j)
                {
                    listPointDetailSubTitle.Add(NLibCs.NTextManager.Instance.GetText(TKString.BytesString(ack.GetDailyEvent(i).GetSubTitleExplain(j).GetExplainBytes().Value)));
                }

                name = NLibCs.NTextManager.Instance.GetText(TKString.BytesString(ack.GetDailyEvent(i).GetNameBytes().Value));
                desc = NLibCs.NTextManager.Instance.GetText(TKString.BytesString(ack.GetDailyEvent(i).GetDescBytes().Value));
                imageUrl = (TKString.BytesString(ack.GetDailyEvent(i).GetImageUrlBytes().Value)) + ".png";
                toptextname = NLibCs.NTextManager.Instance.GetText(TKString.BytesString(ack.GetDailyEvent(i).GetTopTextNameBytes().Value));

                for (int j = 0; j < ack.GetDailyEvent(i).TopRankerLength; ++j)
                    listTopRankers.Add(new TopRankerInfo(ack.GetDailyEvent(i).GetTopRanker(j)));

                RankManager.Instance.UpdateMyRank((int)GameDefine.eRANK_TYPE.DAILY_EVENT_RANK, ack.GetDailyEvent(i).MyRank.Kind, ack.GetDailyEvent(i).MyRank);

                SetDailyEventData(missionType, dayOfWeek, pointGrade, name, desc, rewards, listPointDetail, imageUrl, listExplain, listPointDetailSubTitle, toptextname, listTopRankers);
            }

            weekDay = ack.CurWeek;

            // 주간 랭킹 설정
            _weeklyRankData.Clear();
            for (int i = 0; i < ack.WeeklyTopRankerLength; ++i)
                _weeklyRankData.Add(new TopRankerInfo(ack.GetWeeklyTopRanker(i)));
            RankManager.Instance.UpdateMyRank((int)GameDefine.eRANK_TYPE.DAILY_EVENT_RANK, ack.WeeklyMyRank.Kind, ack.WeeklyMyRank);

            // 매일이벤트 데이터 전체를 UI 열때마다 새로 받고있다.;; 개선이 필요한듯?
            InitNotificationPrefs();
            Check_For_Delete_DailyEventImage();
        }

        public void PointUpdate(int dailyPoint, int weeklyPoint)
        {
            dailyEventPoint = dailyPoint;
            dailyEventWeeklyPoint = weeklyPoint;
        }

        public bool IsExistRewards()
        {
            // 매이 이벤트에서 수령 가능한 보상이 있는지
            if (EVENT.EventManager.Instance == null)
                return false;

            // 현재 유저의 매일이벤트 포인트
            Int32 curDailyPoint = EVENT.EventManager.Instance.dailyEventPoint;

            DailyEventData data = GetCurrentDailyEventData();

            if (data == null)
                return false;

            int iHasCount = 0;

            for (int i = 0; i < data.pointGrade.Count; i++)
            {
                Int32 curItemDailyPoint = EVENT.EventManager.Instance.GetCurrentDailyEventData().pointGrade[i];

                if (curDailyPoint <= curItemDailyPoint)
                    continue;

                if (HasRewardStep(i + 1))
                    continue;

                ++iHasCount;
            }

            return 0 < iHasCount; 
        }

        public int GetTotalEventReddot()
        {
            var reddotCount = 0;

            // 매일 이벤트
            reddotCount += WaitingDailyRewardCount();

            // 시즌 이벤트
            reddotCount += ChloeStoryEventManager.Instance.Get_CanReceiveCurSeasonEventReward();

            // 최강 문명
            reddotCount += QuestManager.Instance.ChallengeEventList.EventListNewMissionCount(DEFINE.GameDefine.CHALLENGE_EVENT_TYPE.CHALLENGE_EVENT_NORMAL);
            reddotCount += QuestManager.Instance.ChallengeEventList.EventListNewMissionCount(DEFINE.GameDefine.CHALLENGE_EVENT_TYPE.CHALLENGE_EVENT_EXTREME);

            // 사전 예약
            reddotCount += SPECIALEVENT.UserDataMgr.GetAllSpecialEventCompleteCount();

            // 주간 출석부
            reddotCount += TableAttendInfo.Instance.IsNormalReddotCount();

            // 특별 출석부
            reddotCount += TableAttendInfo_Premium.Instance.IsPremiumReddotCount(0);
            reddotCount += TableAttendInfo_Premium.Instance.IsPremiumReddotCount(1);

            // 시즌 패스
            reddotCount += SeasonPassManager.Instance.GetMissionRewardableCount();
            reddotCount += SeasonPassManager.Instance.GetTierRewardableCount();

            // 성장 이벤트
            reddotCount += GrowthEventManager.Instance.GetTotalRewardableCount();

            // 사용 이벤트
            reddotCount += UseEventManager.Instance.Get_TotalEventRewardCount();

            return reddotCount;
        }

        public bool IsNewDailyEvent()
        {
            var nPrevUTC = PlayerPrefs.GetInt(DAILYEVENT_NOTIFY_PREF_KEY, 0);
            var nCurUTC  = PublicMethod.GetDueDay_UTC(0);

            return false;
        }


        /// <summary>
        /// 다운로드한 이벤트 이미지중 사용하지 않는 것 삭제.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="imageList"></param>
        /// <param name="key"></param>
        private void CleanUpImage(ref LocalImageDatas data, List<string> imageList, string key)
        {
            if (null == data)
                return;

            data.CleanUpFile(key, imageList);
            PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 이미지 정보 불러오기.
        /// </summary>
        private void InitEventImage()
        {
            string strLocalImageData = PlayerPrefs.GetString(SEASONIMAGE_KEY);
            if (!string.IsNullOrEmpty(strLocalImageData))
                _localSeasonImageInfo = JsonUtility.FromJson<LocalImageDatas>(strLocalImageData);

            strLocalImageData = PlayerPrefs.GetString(DAILYIMAGE_KEY);
            if (!string.IsNullOrEmpty(strLocalImageData))
                _localDailyImageInfo = JsonUtility.FromJson<LocalImageDatas>(strLocalImageData);
        }

        /// <summary>
        /// 알림 데이터 갱신!!!
        /// </summary>
        void InitNotificationPrefs()
        {
            var notificationJson = PlayerPrefs.GetString(DAILYEVENT_NOTIFY_PREF_KEY, string.Empty);
            if (!string.IsNullOrEmpty(notificationJson))
                _notificationData = JsonUtility.FromJson<DailyEventNotifyData>(notificationJson);
            else
                _notificationData = new DailyEventNotifyData(0, 0);
        }


        public void UpdateNotificationPrefs()
        {
            _notificationData.nDayType = CurrentWeekDay();
            _notificationData.nUTCTime = PublicMethod.GetDueDay_UTC(0);

            PlayerPrefs.SetString(DAILYEVENT_NOTIFY_PREF_KEY, JsonUtility.ToJson(_notificationData));
        }


        #endregion private methods




    }

}
