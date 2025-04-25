#pragma once

/// <summary>
/// 유저 사용 이벤트 ( 특별 이벤트 , 잼소모 ) 
/// </summary>

class CUserUseEventInfo
{
	int groupKind = 0;
	int evetnPoint = 0;
	int minLV = 0;
	int maxLV = 0;
	INT64 startTime = 0;
	INT64 endTime = 0;

public:
	using SharedPtr = std::shared_ptr<CUserUseEventInfo>;
	using WeakPtr = std::weak_ptr<CUserUseEventInfo>;

	auto GetGroupKind() const ->const decltype(groupKind)& { return groupKind; }
	auto SetGroupKind(const decltype(groupKind)& o) { groupKind = o; }

	auto GetEventPoint() const ->const decltype(evetnPoint)& { return evetnPoint; }
	auto SetEventPoint(const decltype(evetnPoint)& o) { evetnPoint = o; }

	auto GetStartTime() const ->const decltype(startTime)& { return startTime; }
	auto SetStartTime(const decltype(startTime)& o) { startTime = o; }

	auto GetEndTime() const ->const decltype(endTime)& { return endTime; }
	auto SetEndTime(const decltype(endTime)& o) { endTime = o; }

	auto GetMinLv() const ->const decltype(minLV)& { return minLV; }
	auto SetMinLv(const decltype(minLV)& o) { minLV = o; }

	auto GetMaxLV() const ->const decltype(maxLV)& { return maxLV; }
	auto SetMaxLV(const decltype(maxLV)& o) { maxLV = o; }
};

class CUserUseEventReward
{
	int eventKind = 0;
	int groupKind = 0;
	int pointGrade = 0;

public:
	using SharedPtr = std::shared_ptr<CUserUseEventReward>;
	using WeakPtr = std::weak_ptr<CUserUseEventReward>;

	auto GetEventKind() const ->const decltype(eventKind)& { return eventKind; }
	auto SetEventKind(const decltype(eventKind)& o) { eventKind = o; }

	auto GetGroupKind() const ->const decltype(groupKind)& { return groupKind; }
	auto SetGroupKind(const decltype(groupKind)& o) { groupKind = o; }

	auto GetEventRewardStep() const ->const decltype(pointGrade)& { return pointGrade; }
	auto SetEventRewardStep(const decltype(pointGrade)& o) { pointGrade = o; }

};

class CUserUseEventManager : public CUserContent
{
	struct UseEventInfo
	{
		using SharedPtr = std::shared_ptr<UseEventInfo>;
		using WeakPtr = std::weak_ptr<UseEventInfo>;

		using RepoEventInfo = std::unordered_map<int/*groupKind*/, SharedPtr>;

		CUserUseEventInfo::SharedPtr useEvent_Info = nullptr;

		RepoEventInfo::iterator iter_1;
	};

	struct UseEventReward
	{
		using SharedPtr = std::shared_ptr<UseEventReward>;
		using WeakPtr = std::weak_ptr<UseEventReward>;

		using RepoEventReward = std::unordered_map<pair<Int64/*eventKind*/, Int64/*groupKind*/>, SharedPtr>;

		CUserUseEventReward::SharedPtr useEvent_Reward = nullptr;

		RepoEventReward::iterator iter_1;
	};

	UseEventInfo::RepoEventInfo			m_repo_UseEvent_Info;
	UseEventReward::RepoEventReward		m_repo_UseEvent_Reward;

public:

	CUserUseEventManager();
	~CUserUseEventManager();

	auto GetUseEventInfoRepository() -> decltype(m_repo_UseEvent_Info)& { return m_repo_UseEvent_Info; }
	auto GetUseEventRewardRepository() -> decltype(m_repo_UseEvent_Reward)& { return m_repo_UseEvent_Reward; }

	auto Seek_Info(const INT64 groupKind)->CUserUseEventInfo::SharedPtr;
	auto Seek_Reward(const INT64 eventKind, const INT64 groupKind)->CUserUseEventReward::SharedPtr;

	void Insert_Info(const Int64 groupKind, const Int64 eventPoint);

	void Insert_Reward(const Int64 eventKind, const Int64 pointStep);

	void OnQuestEvent(int iConditionKind, int iConditionValue, int iTargetValue, Quest::SETTYPE setType);
	void SetEmpty();

	void SetUpUseEventUser();
	void SendUserUseEventInfoClientACK(CUser* pUser, int groupKind);
	void SendUserUseEventRewardClientACK(CUser* pUser, int eventKind, int groupKind, int poingGrade);

	void SendUserUseEventClientNFY(int eventGroupKind);

	void SendUserUseFinishEventReward(int eventGroupKind);	// Life cycle 호출 함수

	std::vector<INT64> GetGroupKindEvent_Condition(int iConditionKind, int iConditionValue);


	bool IsMissedEventReward(int groupKind);
};