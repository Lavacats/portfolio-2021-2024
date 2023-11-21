#include "GameAfx.h"
#include "UseEventManager.h"
#include "LifeCycle_UseEvent.h"
#include "Server/Util-ServerList.h"
#include "Season/SeasonManager.h"

CUseEventManager::CUseEventManager()
{

}

CUseEventManager::~CUseEventManager()
{

}

void CUseEventManager::Update()
{
	ProvideMissedReward();
}

void CUseEventManager::OnDBComplete()
{
	RegisterScheduler(); 
	OnDBComplete_MissedReward();

}

void CUseEventManager::RegisterScheduler()
{
	for (auto iter = BASE::USE_EVENT_INFO_DATA.begin(); iter != BASE::USE_EVENT_INFO_DATA.end(); ++iter)
	{
		// 연대기 시간체크도 여기서
		int groupKind = iter->second->groupKind;
		int m_chronicle = iter->second->eventChronicle;
		int m_chronicleTime = ( 0== m_chronicle ? 0 : GLOBAL::CHRONICLE_MANAGER.GetChronicleOpenTime((EChronicleType)m_chronicle));
		long m_endTime = m_chronicleTime + iter->second->useEventEndTime;
		
		// 라이프 사이클 추가
		const auto& End_lifeCycle = make_shared<CLifeCycle_UseEvent_End>(ELifeCycleContentsType::UseEvent_End, static_cast<INT32>(groupKind), m_endTime, ETimer_Type::Utc);
		CLifeCycleManager::Instance()->Insert(End_lifeCycle);
	}
}

bool CUseEventManager::IsDurationUseEvent(int groupKind)
{
	//bool IsDuration = false;
	INT64 curTimeUTC = GetDueDay_UTC(0);

	const auto useEventInfo = BASE::GET_USE_EVENT_INFO_DATA(groupKind);

	if (useEventInfo == nullptr)
		return false;

	int m_season = useEventInfo->eventSeason;
	int m_chronicle = useEventInfo->eventChronicle;
	int m_chronicleTime = (0 == m_chronicle ? 0 : GLOBAL::CHRONICLE_MANAGER.GetChronicleOpenTime((EChronicleType)m_chronicle));
	long m_startTime = m_chronicleTime + useEventInfo->useEventStartTime;
	long m_endTime = m_chronicleTime + useEventInfo->useEventEndTime;

	if (m_season != CSeasonManager::Instance()->GetSeasonNo())
	{
		if(useEventInfo ->eventSeason!=0)
			return false;
	}
	// 시간검사
	if (curTimeUTC < m_startTime)
		return false;

	if (curTimeUTC > m_endTime)
		return false;

	return true;
}

void CUseEventManager::OnDBComplete_MissedReward()
{
	auto& repo = CUserManager::Instance()->GetRepository();
	for (auto iter = repo.begin(); iter != repo.end(); ++iter)
	{
		const auto& game_user = iter->second;
		if (nullptr == game_user) continue;

		for (auto iter = BASE::USE_EVENT_INFO_DATA.begin(); iter != BASE::USE_EVENT_INFO_DATA.end(); ++iter)
		{
			int groupKind = iter->second->groupKind;
			int m_chronicle = iter->second->eventChronicle;
			int m_chronicleTime = (0 == m_chronicle ? 0 : GLOBAL::CHRONICLE_MANAGER.GetChronicleOpenTime((EChronicleType)m_chronicle));
			long m_endTime = m_chronicleTime + iter->second->useEventEndTime;
			INT64 curTimeUTC = GetDueDay_UTC(0);

			if (curTimeUTC > m_endTime)
			{
				if (ASE_INSTANCE(game_user, CUserUseEventManager)->IsMissedEventReward(groupKind) == true)
				{
					PushMissedRewardUser(groupKind, game_user->UID());
				}
			}
		}
	}
}

void CUseEventManager::PushMissedRewardUser(INT32 groupKind, INT64 userID)
{
	const auto& game_user = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(game_user))
		return;

	if (ASE_INSTANCE(game_user, CUserUseEventManager)->IsMissedEventReward(groupKind) == false)
		return;

	auto iterF = m_missedRewardUserList.find(groupKind);
	if (iterF == m_missedRewardUserList.end())
		iterF = m_missedRewardUserList.emplace(groupKind, std::queue<INT64>()).first;

	auto& userList = iterF->second;
	userList.emplace(userID);
}

void CUseEventManager::ProvideMissedReward()
{
	std::map<INT64, std::vector<PROTOCOL::ITEM_INFO>> rewardUserReward;

	for (auto& [groupKind, UserQueue] : m_missedRewardUserList)
	{
		INT32 count = 0;

		while (count < PROVIDE_REWARD_ONE_CYCLE && UserQueue.size() > 0)
		{
			auto userId = UserQueue.front();

			UserQueue.pop();

			const auto& game_user = CUserManager::Instance()->Seek(userId);
			if (IS_NULL(game_user))
				continue;


			ASE_INSTANCE(game_user, CUserUseEventManager)->SendUserUseFinishEventReward(groupKind);
			++count;
		}
	}
}

void CUseEventManager::SendServerUserUseEventReward(INT32 groupKind)
{
	auto& repo = CUserManager::Instance()->GetRepository();
	for (auto iter = repo.begin(); iter != repo.end(); ++iter)
	{
		const auto& game_user = iter->second;
		if (nullptr == game_user) continue;

		PushMissedRewardUser(groupKind,game_user->UID());
	}
}

