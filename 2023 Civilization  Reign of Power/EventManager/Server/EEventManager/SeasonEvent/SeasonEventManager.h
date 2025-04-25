#pragma once
#include "NxBase/pattern/Singleton.h"
#include <unordered_map>

class CSeasonEventManager : public nx::Singleton<CSeasonEventManager>
{
	const int PROVIDE_REWARD_ONE_CYCLE = 4;
	const int PERIOD_DURATION = 28;
	const int CARD_MISSION_COUNT = 3;
	const int CARD_NUM = 4;

public:
	enum SeasonEventRewardState
	{
		None,
		CanReceiveReward,
		ReceivedRewad,
		ShowAllCompletedMissionReward
	};
	enum SeasonEventPeriodState
	{
		Not_Open_Season = -1,
		Period_First,
		Period_second,
		End_Season
	};

	struct SeasonEventMissionData;
	struct SeasonEventRewardData;

private:
	std::unordered_map<pair<Int64/*userID*/, int/*SeasonEvnetMissionKind*/>, std::shared_ptr<SeasonEventMissionData>> repo_SeasonEvent;	//  User�� MissionKind(ī��1,2,3,4)�� �ִ� �̼ǵ��� �����Ѵ�
	std::unordered_map<Int64/*userID*/, std::vector<std::shared_ptr<SeasonEventRewardData>>>  repo_SeasonEventReward;	// Mission�� ���� ������ �����Ѵ�.

	std::map<INT32, std::queue<INT64>> m_missedRewardUserList;	// �����̺�Ʈ �̼��� ���� �л� ���� ������

public:

	int GetCurPeriod();
	int GetPreviosPeriod(int period);

	void Update();
	void OnDBComplete();
	void DeleteSeasonEvent_Info(INT64 userID, int seasonEventMissonKind);
	void DeleteSeasonEvent_Reward(INT64 userID);


	INT64 GetSeasonEventValue(int64_t userID, int seasonEventMissionKind); 			// �����̺�Ʈ �̼� ���� �����´�
	INT32 GetSeasonEventRewardState(int64_t userID, int seasonEventKind);			// ���� ���¸� �����´�.
	INT64 GetSeasonEventRewardTime(int64_t userID, int seasonEventKind);			// ���� ���� �ð��� �����´�.

	bool FindSeasonEventReward(int64_t userId, int seasonEventKind);				// �����̺�Ʈ ���� �� ( ����/�̼���/��纸�����) �� �ִ��� Ȯ��.
	std::vector<PROTOCOL::ITEM_INFO>  FindSeasonKindReward(int seasonKind);			// �����̺�Ʈ ���� ������ ���� �����´�.

	// Ŭ���̾�Ʈ�� ACK / NFY
	void SendUserSeasonEventInfoClinetACK(int64_t userID, int showUI);
	void SendUserSeasonEventInfoACK(int64_t userID, int showUI, int curSeason, int curPeriod, std::vector < std::shared_ptr<SeasonEventRewardData >>, std::vector < std::shared_ptr<SeasonEventMissionData >>);

	void SendUserSeasonEventInfoClinetNFY(int64_t userID);
	void SendUserSeasonEventInfoNFY(int64_t userID, int curSeason, int curPeriod, std::vector < std::shared_ptr<SeasonEventRewardData >>, std::vector < std::shared_ptr<SeasonEventMissionData >>);

	// ���� �̺�Ʈ �̼� üũ �Լ�
	void ProcessMission(INT64 i64UserID, int iConditionKind, int iConditionValue, int iTargetValue, Quest::SETTYPE setType);

	// DB �ε�
	void LoadToDB_Season_Event(int64_t userID, int32_t SeasonKind, int SeasonMissionKind,
		INT16 Mission_1_Kind, INT64 Mission_1_value, INT16 Mission_2_Kind, INT64 Mission_2_value, INT16 Mission_3_Kind, INT64 Mission_3_value,
		INT16 completeMission, int64_t RewardTime);

	// ���� �̺�Ʈ ���� ���� ó�� ( ���� ��û Req [Ŭ��] )
	void UpdateUserSeasonEventInfo_Reward(int64_t userID, INT16 userSeasonEventKind, INT16 completeEventValue, const QueryPackLinker::SharedPtr& load_balancer = nullptr);	// 1. ���� ���� ���� ó��
	void SendToClientUserSeasonEventReward(INT64 userID, INT16 userSeasonEventKind);																						// 2. Ŭ���̾�Ʈ  ACK
	void UpdateToDBuserSeasonEventReward(INT64 userID, INT16 userSeasonEventKind, const QueryPackLinker::SharedPtr& load_balancer = nullptr);								// 3. DB���� ����

	// ���� �̺�Ʈ �α��ν� ȣ��
	void UserLoginData(INT64 userID);
																																											
	// ���� �̺�Ʈ �̼��� ���� ����
	void RegisterScheduler();																			// ������۽� ������ ����Ŭ �۽�
	CLifeCycle::SharedPtr MakeSeasonEventScheduler(SeasonEventPeriodState type, INT64 completeTime);	// ����������Ŭ Scheduler ����
	INT64 MakeEndTimeByType(const INT64 startTime, SeasonEventPeriodState type);						// ���ݱ�/�Ϲݱ� �ð� ���
	void SendServerUserSeasonEventReward(INT16 season, INT16 prevPeriod);								// �̺�Ʈ ����� �̼��� ���� �۽�

	// �̼��� ���� �л� �۽�
	void PushMissedRewardUser(INT32 period, INT64 userID);
	void ProvideMissedReward();
	void SendUserMissedReward(INT64 userID);

	// ���� �̺�Ʈ ġƮŰ 
	void Cheat_SetSeasonEvent(INT64 userID, INT32 cardNum ,INT32 MissionNum,int missionValue);
	void UpdateDBdata(INT64 userID, INT16 userSeasonEventKind);

};

