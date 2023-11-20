using System.Collections.Generic;
//using Assets.Scripts.Network.WAS;

using UnityEngine;
using System.Linq;
using PROTOCOL.FLATBUFFERS;
using BaseTable;
using DEFINE;
using NLibCs;
using System.Collections;
using System;
using Contents.ContentState;
using Contents.User;
using NdreamPayment;
using WasServer.Common;
using FlatBuffers;
using PROTOCOL.GAME.ID;
using Contents.Territory;
using Skill.Tree;

public class UseEventManager : NrTSingleton<UseEventManager>
{
    public class UseEventInfo
    {
        public long eventStartTime = 0;
        public long eventEndTime = 0;
        public UseEventInfo(long m_startTime,long m_endTime)
        {
            eventStartTime = m_startTime;
            eventEndTime = m_endTime;
        }

    }

    public Dictionary<int/*groupKind*/, int/*point*/>               dicUseEventpoint         = new Dictionary<int, int>();
    public Dictionary<int/*groupKind*/, List<int/*pointGrade*/>>    dicUseRewardEvent   = new Dictionary<int, List<int>>();
    public Dictionary<int/*groupKind*/, UseEventInfo/*eventInfo*/>  dicUseEventinfo     = new Dictionary<int, UseEventInfo>();        

    public int selectEventGroupKind = 0;
    public int showEventKind = 0;
    UseEventManager() { }
    public void InsertUseEventInfo(int eventGroupKind, int eventPoint)
    {
        if (eventGroupKind != 0)
        {
            if (dicUseEventpoint.ContainsKey(eventGroupKind))
            {
                dicUseEventpoint[eventGroupKind] = eventPoint;
            }
            else
            {
                dicUseEventpoint.Add(eventGroupKind, eventPoint);
            }
        }
    }
    public void InsertEventTime(int eventGroupKind, long staratTime , long endTime)
    {
        if (eventGroupKind != 0)
        {
            var refreshEventInfo = new UseEventInfo(staratTime, endTime);
            if (dicUseEventinfo.ContainsKey(eventGroupKind))
            {
                dicUseEventinfo[eventGroupKind] = refreshEventInfo;
            }
            else
            {
                dicUseEventinfo.Add(eventGroupKind, refreshEventInfo);
            }
        }
    }
    public void InsertUseEventReward(int eventKind, int eventStep)
    {
        if (eventKind != 0)
        {
            if (dicUseRewardEvent.ContainsKey(eventKind))
            {
                if (!dicUseRewardEvent[eventKind].Contains(eventStep))
                    dicUseRewardEvent[eventKind].Add(eventStep);
            }
            else
            {
                List<int> listStetp = new List<int>();
                listStetp.Add(eventStep);
                dicUseRewardEvent.Add(eventKind, listStetp);
            }
        }
    }
    public void Set_Use_Event_Get_NFY(PROTOCOL.FLATBUFFERS.GS_USE_EVENT_NFY nfy)
    {
        var eventGroupKind = nfy.EventGroupKind;
        var evetnPoint = nfy.EventPoint;

        if (dicUseEventpoint.ContainsKey(eventGroupKind))
            dicUseEventpoint[eventGroupKind] = evetnPoint;

    }
    public void Set_Use_Event_Get_LOGIN_NFY(PROTOCOL.FLATBUFFERS.GS_USE_EVENT_LOGIN_NFY nfy)
    {
        dicUseEventpoint.Clear();
        dicUseRewardEvent.Clear();
        var curSeason = SeasonPassManager.Instance.GetCurSeason();
        var curChronicleType = UserBase.User.CurChronicleType;
        var curUserLevel = UserBase.User.LORD.GetLevel();

        // 클리어 추가
        for (int i = 0; i < nfy.EventListLength; ++i)
        {
            var eventInfo = nfy.GetEventList(i);

            if (eventInfo != null)
            {
                InsertUseEventInfo(eventInfo.EventGroupKind, eventInfo.EventPoint);
                InsertEventTime(eventInfo.EventGroupKind, eventInfo.EventStartTime,eventInfo.EventEndTime);

                for (int step = 0; step < eventInfo.PointGradeListLength; step++)
                {
                    InsertUseEventReward(eventInfo.EventGroupKind, eventInfo.GetPointGradeList(step));
                }
            }
        }
    }
    public void Set_Use_Event_Get_ACK(PROTOCOL.FLATBUFFERS.GS_USE_EVENT_INFO_ACK ack)
    {
        for (int i = 0; i < ack.EventListLength; ++i)
        {
            var eventInfo = ack.GetEventList(i);

            if (eventInfo != null)
            {
                selectEventGroupKind = eventInfo.EventGroupKind;
                if (!dicUseEventpoint.ContainsKey(eventInfo.EventGroupKind))
                {
                    InsertUseEventInfo(eventInfo.EventGroupKind, eventInfo.EventPoint);

                    InsertEventTime(eventInfo.EventGroupKind, eventInfo.EventStartTime, eventInfo.EventEndTime);
                }

                for (int step = 0; step < eventInfo.PointGradeListLength; step++)
                {
                    InsertUseEventReward(eventInfo.EventGroupKind, eventInfo.GetPointGradeList(step));
                }
            }
        }
    }
    public void InsertRewardInfo(int eventKind, int groupKind, int poingGrade)
    {
        if (dicUseRewardEvent.ContainsKey(groupKind))
        {
            if (!dicUseRewardEvent[groupKind].Contains(poingGrade))
                dicUseRewardEvent[groupKind].Add(poingGrade);
        }
        else
        {
            List<int> rewardList = new List<int>();
            rewardList.Add(poingGrade);

            dicUseRewardEvent.Add(groupKind, rewardList);
        }
        AddItemAndRefreshUseEventReward(eventKind, groupKind, poingGrade);
    }
    public void AddItemAndRefreshUseEventReward(int eventKind, int groupKind, int poingGrade)
    {
        var EventList = UseEventManager.Instance.GetEventList(groupKind);

        foreach (var eventInfo in EventList)
        {
            if (eventInfo.i32EventKind != eventKind)
                continue;

            if (eventInfo.i32GroupKind != groupKind)
                continue;

            if (eventInfo.i64PointGrade != poingGrade)
                continue;

            List<ObtainItemBase> addItems = new List<ObtainItemBase>();

            for (int i=0; i<eventInfo.vecReward.Count;i++)
            {
                Inventory.Instance.AddItemNumAndRefreshNewItem(eventInfo.vecReward[i].Key, eventInfo.vecReward[i].Value);
              
                addItems.Add(new ObtainItemBase(eventInfo.vecReward[i].Key, eventInfo.vecReward[i].Value));
            }

            ObtainItem_ControllerEx.EnQueueEvent(addItems, ObtainEventEx.ContentsType.CONTENTS_COMMON);
            break;
        }
    }
    public int Get_CanReceiveRewardCount(int groupKind)
    {
        int cntReward = 0;
        int canReceiveReward = 0;
        int receivedRewardCnt = 0;
        int eventPoint = 0;
     
        if(dicUseEventpoint.ContainsKey(groupKind))
            eventPoint = dicUseEventpoint[groupKind];

        var EventList = UseEventManager.Instance.GetEventList(groupKind);

        if (EventList == null)
            return 0;

        foreach (var eventInfo in EventList)
        {
            if (eventInfo.i64PointGrade <= eventPoint) 
                canReceiveReward++;
        }

        if(dicUseRewardEvent.ContainsKey(groupKind))
            receivedRewardCnt = dicUseRewardEvent[groupKind].Count;

        cntReward = canReceiveReward - receivedRewardCnt;

        if (cntReward < 0) cntReward = 0;

        return  cntReward;
    }
    public int Get_IndexCanReciveReward()
    {
        int _index = -1;
        int eventPoint = 0;
        var EventList = UseEventManager.Instance.GetEventList(selectEventGroupKind);

        if (dicUseEventpoint.ContainsKey(selectEventGroupKind))
        {
            eventPoint = dicUseEventpoint[selectEventGroupKind];
        }

        if (Get_CanReceiveRewardCount(selectEventGroupKind) == 0)
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                if (EventList[i].i64PointGrade <= eventPoint)
                {
                    _index = i;
                }
            }
        }
        else
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                if (EventList[i].i64PointGrade <= eventPoint)
                {
                    bool checkReceive = false;
                    if (dicUseRewardEvent.ContainsKey(selectEventGroupKind))
                    {
                        foreach (var rewardInfo in dicUseRewardEvent[selectEventGroupKind])
                        {
                            if (rewardInfo == EventList[i].i64PointGrade)
                            {
                                checkReceive = true;
                                break;
                            }
                        }
                        if (!checkReceive)
                        {
                            _index = i;
                            break;
                        }
                    }
         
                }
            }
        }
        return _index;
    }
    public int Get_TotalEventRewardCount()
    {
        int allCnt = 0;
        foreach (var eventInfo in dicUseEventpoint)
        {
            allCnt += Get_CanReceiveRewardCount(eventInfo.Key);
        }

         return allCnt;
    }
    public int Get_UseEventPoint(int groupKind)
    {
        int eventPoint = 0;
        if (dicUseEventpoint.ContainsKey(groupKind))
            eventPoint = dicUseEventpoint[groupKind];

        return eventPoint; 
    }
    public bool IsRewaredUseEvent(int groupKind,int pointgrade)
    {
        bool result = false;

        if (dicUseRewardEvent.ContainsKey(groupKind))
        {
            foreach (var rewardInfo in dicUseRewardEvent[groupKind])
            {
                if (rewardInfo == pointgrade)
                {
                    result = true;
                    break;
                }
            }
        }

        return result;
    }

    public List<NrUseEventData> GetEventList(Int32 _groupKind)
    {
        List<NrUseEventData> lstEventData = new List<NrUseEventData>();
        var CurDate = PublicMethod.GetNowDate_Utc();
        var eventStartTime = UseEventManager.Instance.dicUseEventinfo[_groupKind].eventStartTime;
        var eventEndTime = UseEventManager.Instance.dicUseEventinfo[_groupKind].eventEndTime;

        var tableUseEventList = TableUseEventInfo.Instance.GetEventList(_groupKind);

        foreach (var useEventData in tableUseEventList)
        {
            if (useEventData.i32GroupKind != _groupKind)
                continue;

            if (PublicMethod.GetDueDate_Utc(eventStartTime) > CurDate || PublicMethod.GetDueDate_Utc(eventEndTime) < CurDate)
                continue;

            lstEventData.Add(useEventData);
        }
        return lstEventData;
    }

    public List<NrUseEventData> GetCurEventList()
    {
        List<NrUseEventData> lstEventData = new List<NrUseEventData>();
        var CurDate = PublicMethod.GetNowDate_Utc();
        var curChronicleType = UserBase.User.CurChronicleType;

        foreach (var useEventData in UseEventManager.Instance.dicUseEventinfo)
        {
            if (TableUseEventInfo.Instance.GetEventInfo(useEventData.Key) == null)
                continue;

            if (TableUseEventInfo.Instance.GetEventInfo(useEventData.Key).i32Chronicle > (int) curChronicleType)
            {
                if (TableUseEventInfo.Instance.GetEventInfo(useEventData.Key).i32Chronicle != 0)
                    continue;
            }

            if (PublicMethod.GetDueDate_Utc(useEventData.Value.eventStartTime) > CurDate || PublicMethod.GetDueDate_Utc(useEventData.Value.eventEndTime) < CurDate)
                continue;

            lstEventData.Add(TableUseEventInfo.Instance.GetEventInfo(useEventData.Key));
        }
        return lstEventData;
    }
}
