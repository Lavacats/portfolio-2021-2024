#pragma once

#include "Systems/LifeCycle/LifeCycleManager.h"
#include "NxBase/pattern/Singleton.h"

struct UseEventScheduleInfo
{
public:
	UseEventScheduleInfo(INT64 startTime, INT64 endTime)
		: m_startTime(startTime), m_endTime(endTime) {}

	INT64 m_startTime = 0;
	INT64 m_endTime = 0;
};


class CUseEventManager : public nx::Singleton<CUseEventManager>
{

	const int PROVIDE_REWARD_ONE_CYCLE = 4;

private:
	std::map<INT32, std::queue<INT64>> m_missedRewardUserList;	// ����̺�Ʈ �̼��� ���� �л� ���� ������

public:
	CUseEventManager();
	~CUseEventManager();

public:
	void Update();

	void OnDBComplete();
	void RegisterScheduler();
	bool IsDurationUseEvent(int groupKind);
	void OnDBComplete_MissedReward();

	void PushMissedRewardUser(INT32 groupKind, INT64 userID);
	void ProvideMissedReward();

	void SendServerUserUseEventReward(INT32 groupKind);
};

