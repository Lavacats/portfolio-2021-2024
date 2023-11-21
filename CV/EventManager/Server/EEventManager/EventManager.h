//EventManager.h
//이벤트들을 관리하는 매니저. 
#pragma once

//forward declaration
class IEvent;

enum EVENT_MESSAGE
{
	EVENT_START,
	EVENT_END,

	END,
};



class CEventManager
{
private: // private member data	
	std::list<IEvent *>				m_waitingEvents;
	std::list<IEvent *>				m_runningEvents;	
	
	INT64 oldDate	= 0;
	bool  m_Initialized = false;	// 서버 시작시 단한번 db로부터 필요한 데이터를 받아오면 true. 이벤트는 initialize되기전에는 동작하지 않아야 한다.
	INT32 m_DailyEventWeeks = 0;	// 매일 이벤트를 진행한지 몇주가 되었는지 기록.

public: // ctor & dtor
	CEventManager();
	~CEventManager();


public: // general methods
	SRESULT Initailize();

	void AddWaitingEvent(IEvent *cofEvent);
	void AddWaitingEvent(INT64 eventID, INT32 eventKind, INT32 eventSubKind, INT64 startTime, INT64 endTime, bool isRun);
	void DeleteWaitingEvent(INT64 scheduleID);
	void UpdateEventSchedule(INT64 eventID, INT32 eventKind, INT32 eventSubKind, INT64 startTime, INT64 endTime, bool isRun);
	void ReloadDailyEvent();
	void Update();

	void GetRunningEvents(CUser *user);
	void CompleteLoadNDT();

	void AddPointDailyEvent(INT64 userID, INT32 eventKind, int dailyPoint);
	void OnEvent(INT64 userID, int conditionKind, int subCondition, int eventValue, int tireValue = 0);
	void GetActiveEventKindList(std::vector<INT32>& eventList);

	bool IsActive(INT32 eventKind);
	int GetEventRewardLogType(INT32 conditionKind);
public:	// test

	INT64 testUserIndex = 0;
	INT64 GMcommondWeekDay = 0;
	void TestEvent();
	void Test_ThanksgivingStart(INT64 i64Second);
	void Test_ThanksgivingEnd(INT64 i64Second);
	void SendClientDailyEventData(CUser * user, INT32 showUI);  //클라이언트에게 데일리 이벤트 데이터(ndt) 전송
	void SendDBDailyEventRewardSet(CUser * user, const void * pData);  //매일 이벤트 포인트 보상을 지급 한다.

	void SendRankReward_DailyMissionByGM(INT64 userID, INT32 eventType);

private: // private methods		
	void InitEvent();
	void Release();
	void ReleaseEvent(INT32 eventKind);
	bool ReleaseRunningEvent(INT32 eventKind);
	bool ReleaseWaitingEvent(INT32 eventKind);
	void ReleaseEvents();
	void ReleaseWaitingEvent();
	void ReleaseRunningEvent();

	void UpdateEventSchedule(INT64 currentTime);
	void UpdateRunningEvent(INT64 currentTime);

	void BroadcastEventMessage(EVENT_MESSAGE message);

	void MakeSeasonEventInfoFbb(flatbuffers::FlatBufferBuilder &fbb, IEvent* seasonEvent, CUser* pUser, std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_EVENT_INFO>>& vecEvent, bool IsCompleted);
	void MakeContentEventInfoFbb(flatbuffers::FlatBufferBuilder &fbb, IEvent* seasonEvent, CUser* pUser, std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_EVENT_INFO>>& vecEvent, bool IsCompleted);

public: // to Client
	void Send_DB_CONTENTS_EVENT_LIST_REQ(CUser * pUser);
	void Send_GS_SEASON_EVENT_LIST_ACK(const PROTOCOL::FLATBUFFERS::DB_CONTENT_EVENT_LIST_GET_ACK * pAck);
	void Send_GS_SEASON_EVENT_DETAIL_INFO_ACK(CUser * pUser, const PROTOCOL::FLATBUFFERS::GS_SEASON_EVENT_DETAIL_INFO_REQ * pReq);
	void Send_DB_CONTENT_EVENT_GET_REQ(CUser * pUser, const PROTOCOL::FLATBUFFERS::GS_SEASON_EVENT_DETAIL_INFO_REQ * pReq);
	void Send_GS_CONTENT_EVENT_DETAIL_INFO_ACK(const PROTOCOL::FLATBUFFERS::DB_CONTENT_EVENT_GET_ACK * paCK);

	int GetEventTypeByEventKind(int EventKind);
	bool IsOccupyCityEvent(int EventKind);
	void OnEventContents(INT64 userID, int conditionKind, const std::vector<INT32> CompareValues, int IncreaseValue = 0);
	void CheckEquipmentEventContents(INT64 userID, PROTOCOL::EQUIPMENT_INFO &Info);
	void SetEventContentsCondition(std::vector<INT32>& o_Values, INT32 Value1, INT32 Value2 = 0, INT32 Value3 = 0, INT32 Value4 = 0, INT32 Value5 = 0);

	bool IsContentEventCompleted(INT32 ContentKind, const flatbuffers::Vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::CONTENT_EVENT_COUNT_INFO>> *EventListInfos);
public:
	// 매일이벤트 주차 관련 함수들.
	int		GetDailyEventWeeks() const { return m_DailyEventWeeks; }
	void	SetDailyEventWeeks(int weeks) { m_DailyEventWeeks = weeks; }
	void	Send_DB_DAILY_EVENT_WEEK_SET_REQ();
	//void	Recv_DB_DAILY_EVENT_WEEK_SET_ACK(const PROTOCOL::FLATBUFFERS::DB_DAILY_EVENT_WEEK_SET_ACK* pAck);
};