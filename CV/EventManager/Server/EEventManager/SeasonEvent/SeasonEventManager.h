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
	std::unordered_map<pair<Int64/*userID*/, int/*SeasonEvnetMissionKind*/>, std::shared_ptr<SeasonEventMissionData>> repo_SeasonEvent;	//  User와 MissionKind(카드1,2,3,4)에 있는 미션들을 관리한다
	std::unordered_map<Int64/*userID*/, std::vector<std::shared_ptr<SeasonEventRewardData>>>  repo_SeasonEventReward;	// Mission에 대한 보상값을 관리한다.

	std::map<INT32, std::queue<INT64>> m_missedRewardUserList;	// 시즌이벤트 미수령 보상 분산 수령 데이터

public:

	int GetCurPeriod();
	int GetPreviosPeriod(int period);

	void Update();
	void OnDBComplete();
	void DeleteSeasonEvent_Info(INT64 userID, int seasonEventMissonKind);
	void DeleteSeasonEvent_Reward(INT64 userID);


	INT64 GetSeasonEventValue(int64_t userID, int seasonEventMissionKind); 			// 시즌이벤트 미션 값을 가져온다
	INT32 GetSeasonEventRewardState(int64_t userID, int seasonEventKind);			// 보상 상태를 가져온다.
	INT64 GetSeasonEventRewardTime(int64_t userID, int seasonEventKind);			// 보상 수령 시각을 가져온다.

	bool FindSeasonEventReward(int64_t userId, int seasonEventKind);				// 시즌이벤트 보상 값 ( 수령/미수령/모든보상수령) 이 있는지 확인.
	std::vector<PROTOCOL::ITEM_INFO>  FindSeasonKindReward(int seasonKind);			// 시즌이벤트 보상 아이템 정보 가져온다.

	// 클라이언트로 ACK / NFY
	void SendUserSeasonEventInfoClinetACK(int64_t userID, int showUI);
	void SendUserSeasonEventInfoACK(int64_t userID, int showUI, int curSeason, int curPeriod, std::vector < std::shared_ptr<SeasonEventRewardData >>, std::vector < std::shared_ptr<SeasonEventMissionData >>);

	void SendUserSeasonEventInfoClinetNFY(int64_t userID);
	void SendUserSeasonEventInfoNFY(int64_t userID, int curSeason, int curPeriod, std::vector < std::shared_ptr<SeasonEventRewardData >>, std::vector < std::shared_ptr<SeasonEventMissionData >>);

	// 시즌 이벤트 미션 체크 함수
	void ProcessMission(INT64 i64UserID, int iConditionKind, int iConditionValue, int iTargetValue, Quest::SETTYPE setType);

	// DB 로드
	void LoadToDB_Season_Event(int64_t userID, int32_t SeasonKind, int SeasonMissionKind,
		INT16 Mission_1_Kind, INT64 Mission_1_value, INT16 Mission_2_Kind, INT64 Mission_2_value, INT16 Mission_3_Kind, INT64 Mission_3_value,
		INT16 completeMission, int64_t RewardTime);

	// 시즌 이벤트 보상 수령 처리 ( 유저 요청 Req [클릭] )
	void UpdateUserSeasonEventInfo_Reward(int64_t userID, INT16 userSeasonEventKind, INT16 completeEventValue, const QueryPackLinker::SharedPtr& load_balancer = nullptr);	// 1. 보상 수령 여부 처리
	void SendToClientUserSeasonEventReward(INT64 userID, INT16 userSeasonEventKind);																						// 2. 클라이언트  ACK
	void UpdateToDBuserSeasonEventReward(INT64 userID, INT16 userSeasonEventKind, const QueryPackLinker::SharedPtr& load_balancer = nullptr);								// 3. DB상태 변경

	// 시즌 이벤트 로그인시 호출
	void UserLoginData(INT64 userID);
																																											
	// 시즌 이벤트 미수령 보상 수령
	void RegisterScheduler();																			// 시즌시작시 라이프 사이클 송신
	CLifeCycle::SharedPtr MakeSeasonEventScheduler(SeasonEventPeriodState type, INT64 completeTime);	// 라이프사이클 Scheduler 생성
	INT64 MakeEndTimeByType(const INT64 startTime, SeasonEventPeriodState type);						// 전반기/하반기 시간 계산
	void SendServerUserSeasonEventReward(INT16 season, INT16 prevPeriod);								// 이벤트 종료시 미수령 보상 송신

	// 미수령 보상 분산 송신
	void PushMissedRewardUser(INT32 period, INT64 userID);
	void ProvideMissedReward();
	void SendUserMissedReward(INT64 userID);

	// 시즌 이벤트 치트키 
	void Cheat_SetSeasonEvent(INT64 userID, INT32 cardNum ,INT32 MissionNum,int missionValue);
	void UpdateDBdata(INT64 userID, INT16 userSeasonEventKind);

};

