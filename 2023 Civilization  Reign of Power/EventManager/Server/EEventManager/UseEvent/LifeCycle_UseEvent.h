#pragma once
#include "Systems/LifeCycle/LifeCycle.h"


// �̺�Ʈ ����~
class CLifeCycle_UseEvent_End : public CLifeCycle
{
public:
	CLifeCycle_UseEvent_End(const ELifeCycleContentsType type, const INT32& kind, const INT64& completeTime, const ETimer_Type& timerType)
		: CLifeCycle(type, kind, completeTime, timerType)
	{}
	virtual ~CLifeCycle_UseEvent_End() {}

public:
	void OnCompleted(bool isUpdate) override;
};

class LifeCycle_UseEvent
{
};

