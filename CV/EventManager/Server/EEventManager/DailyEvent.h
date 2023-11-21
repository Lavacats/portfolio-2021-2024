//DailyEvent.h
//매일 이벤트. 
//주간 단위로 완료되며, 매일 세부 이벤트가 변경된다.
#pragma once

#include "IEvent.h"

enum EnumDailyEvent
{
	DailyEvnetNone		= 0,
	DailyEventMonDay	= 1,
	DailyEventTuesDay	= 2,
	DailyEventWednesDay = 3,
	DailyEventThursDay	= 4,
	DailyEventFriDay	= 5,
	DailyEventSaturDay	= 6,
	DailyEventSunDay	= 7,
};
class CDailyEvent : public IEvent
{	
public: // ctor & dtor
	CDailyEvent(INT64 id, INT32 kind, INT32 subKind, INT64 startTime, INT64 endTime, INT64 scheduleID = 0);
	CDailyEvent(BASE::EVENT_INFO *info, INT64 startTime, INT64 endTime);
	CDailyEvent(BASE::EVENT_INFO *info);
	~CDailyEvent();
public: // virtual methods
	virtual void OnEvent(INT64 userID, int conditionKind, int conditionValue, int value, int tireValue = 0) override;
	virtual void GmCommandDaily(INT64 userID, int dailyPoint);
	virtual void Complete();
	virtual void Update();
	virtual void UpdateEventSchedule(INT64 startTime, INT64 endTime);
	virtual void ReloadEvent();
private:
	INT32 CalculateEventPoint(INT64 userID, INT32 conditionKind, INT32 subCondition, INT32 value);
	INT32 CalculateEventTotalPoint(INT64 userID, INT32 conditionKind, INT32 subCondition, INT32 value);
	void Send_DB_DAILY_EVENT_POINT_UPDATE_REQ(INT64 userID, int addPoint);

	INT64 CalculateAccumlatePoint(INT64 userID, INT32 conditionKind);
	bool IsAccumlateEvent(INT32 conditionkind);
	void Send_DB_DAILY_EVENT_POINT_VALUE_SET_REQ(INT64 userID, INT32 conditionKind, INT32 addPointvalue, INT64 prevTotalValue ,INT64 TotalValue);
	
public:
	void SendDailyPoint(INT64 userID, INT32 addPoint);
	void InitDailyEventInfo();
	void LoadDailyEventInfo();
};