#pragma once
#include "Systems/LifeCycle/LifeCycle.h"

class CLifeCycle_SeasonEventPeriod_First : public CLifeCycle
{
public:
	CLifeCycle_SeasonEventPeriod_First(const ELifeCycleContentsType type, const INT32& kind, const INT64& completeTime, const ETimer_Type& timerType)
		: CLifeCycle(type, kind, completeTime, timerType)
	{}
	virtual ~CLifeCycle_SeasonEventPeriod_First() {}

public:
	void OnCompleted(bool isUpdate) override;
};

class CLifeCycle_SeasonEventPeriod_Second : public CLifeCycle
{
public:
	CLifeCycle_SeasonEventPeriod_Second(const ELifeCycleContentsType type, const INT32& kind, const INT64& completeTime, const ETimer_Type& timerType)
		: CLifeCycle(type, kind, completeTime, timerType)
	{}
	virtual ~CLifeCycle_SeasonEventPeriod_Second() {}

public:
	void OnCompleted(bool isUpdate) override;
};

class LifeCycle_SeasonEvent
{
};

