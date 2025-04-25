#include "GameAfx.h"

#include "ContentsEvent.h"
#include "DailyEvent.h"
#include "IEvent.h"
#include "ThanksGivingEvent.h"
#include "OBJECTS/Mail/Util-Mail.h"
#include "_DatabaseThreadManager/DBQUERY/MASTERDB/P_T_DAILY_EVENT_WEEK_SET.h"
#include "Contents/Item/Util-Item.h"
#include "DailyPoint/Util-DailyPoint.h"
#include "OBJECTS/Rank/DailyRankerContainer.h"
#include "Season/SeasonManager.h"
#include "Contents/SeasonEvent/SeasonEventManager.h"

CEventManager::CEventManager()
{

}

CEventManager::~CEventManager()
{
	Release();
}

SRESULT CEventManager::Initailize()
{
	return 0;
}

void CEventManager::CompleteLoadNDT()
{

}

bool CEventManager::IsOccupyCityEvent(int EventKind)
{
	bool returnValue = false;
	switch (EventKind)
	{

	case GAME::CONTENTS_OCCUPY_CITY_1:
	case GAME::CONTENTS_OCCUPY_CITY_2:
	case GAME::CONTENTS_OCCUPY_CITY_3:
	case GAME::CONTENTS_OCCUPY_CITY_4:
	case GAME::CONTENTS_OCCUPY_CITY_5:
	case GAME::CONTENTS_OCCUPY_CITY_6:
	case GAME::CONTENTS_OCCUPY_CITY_7:
	case GAME::CONTENTS_OCCUPY_CITY_8:
	case GAME::CONTENTS_OCCUPY_CITY_9:
	case GAME::CONTENTS_OCCUPY_CITY_10:
	case GAME::CONTENTS_OCCUPY_CITY_11:
	case GAME::CONTENTS_OCCUPY_CITY_12:
	case GAME::CONTENTS_OCCUPY_CITY_13:
	case GAME::CONTENTS_OCCUPY_CITY_14:
	case GAME::CONTENTS_OCCUPY_CITY_15:
	case GAME::CONTENTS_OCCUPY_CITY_16:
	case GAME::CONTENTS_OCCUPY_CITY_17:
	case GAME::CONTENTS_OCCUPY_CITY_18:
	case GAME::CONTENTS_OCCUPY_CITY_19:
	case GAME::CONTENTS_OCCUPY_CITY_20:
	case GAME::CONTENTS_OCCUPY_CITY_21:
	case GAME::CONTENTS_OCCUPY_CITY_22:
	case GAME::CONTENTS_OCCUPY_CITY_23:
	case GAME::CONTENTS_OCCUPY_CITY_24:
	case GAME::CONTENTS_OCCUPY_CITY_25:
	case GAME::CONTENTS_OCCUPY_CITY_26:
	case GAME::CONTENTS_OCCUPY_CITY_27:
		returnValue = true;
		break;

	default:
		break;
	}
	return returnValue;
}

int CEventManager::GetEventTypeByEventKind(int EventKind)
{
	int EventType = 0;
	switch (EventKind)
	{
	case GAME::EVENT_EVERYDAY:
		EventType = GAME::EVENT_TYPE_EVERYDAY;
		break;
	case GAME::EVENT_THANKSGIVING:
	case GAME::EVENT_CHRISTMAS:
	case GAME::EVENT_NEWYEARSDAY:
	case GAME::EVENT_LUNARYEAR:
	case GAME::EVENT_VALENTINE:
	case GAME::EVENT_ANNIVERSARY_100:
	case GAME::EVENT_APRILFOOLS_DAY:
	case GAME::EVENT_EASTER:
	case GAME::EVENT_ROYAL_RESEARCH:
		EventType = GAME::EVENT_TYPE_SEASON;
		break;
	case GAME::CONTENTS_MANUFACTURE:
	case GAME::CONTENTS_OCCUPY_CITY_1:
	case GAME::CONTENTS_OCCUPY_CITY_2:
	case GAME::CONTENTS_OCCUPY_CITY_3:
	case GAME::CONTENTS_OCCUPY_CITY_4:
	case GAME::CONTENTS_OCCUPY_CITY_5:
	case GAME::CONTENTS_OCCUPY_CITY_6:
	case GAME::CONTENTS_OCCUPY_CITY_7:
	case GAME::CONTENTS_OCCUPY_CITY_8:
	case GAME::CONTENTS_OCCUPY_CITY_9:
	case GAME::CONTENTS_OCCUPY_CITY_10:
	case GAME::CONTENTS_OCCUPY_CITY_11:
	case GAME::CONTENTS_OCCUPY_CITY_12:
	case GAME::CONTENTS_OCCUPY_CITY_13:
	case GAME::CONTENTS_OCCUPY_CITY_14:
	case GAME::CONTENTS_OCCUPY_CITY_15:
	case GAME::CONTENTS_OCCUPY_CITY_16:
	case GAME::CONTENTS_OCCUPY_CITY_17:
	case GAME::CONTENTS_OCCUPY_CITY_18:
	case GAME::CONTENTS_OCCUPY_CITY_19:
	case GAME::CONTENTS_OCCUPY_CITY_20:
	case GAME::CONTENTS_OCCUPY_CITY_21:
	case GAME::CONTENTS_OCCUPY_CITY_22:
	case GAME::CONTENTS_OCCUPY_CITY_23:
	case GAME::CONTENTS_OCCUPY_CITY_24:
	case GAME::CONTENTS_OCCUPY_CITY_25:
	case GAME::CONTENTS_OCCUPY_CITY_26:
	case GAME::CONTENTS_OCCUPY_CITY_27:
	case GAME::CONTENTS_OCCUPY_CITY_28:
	case GAME::CONTENTS_TRADE_PROFIT:
	case GAME::CONTENTS_EQUIPMENT_LOOTING:
	case GAME::CONTENTS_GUILD_RESEARCH_DONATION:
	case GAME::CONTENTS_MANUFACTURE2:
		EventType = GAME::EVENT_TYPE_CONTENTS;
		break;

	default:
		break;
	}

	return EventType;

}

void CEventManager::UpdateEventSchedule(INT64 eventID, INT32 eventKind, INT32 eventSubKind, INT64 startTime, INT64 endTime, bool isRun)
{
	bool isWaiting = false;
	if (isRun)
	{
		for (auto iter = m_waitingEvents.begin(); iter != m_waitingEvents.end(); iter++)
		{
			if (IS_NOT_NULL(*iter))
			{
				if ((*iter)->Kind() == eventKind)
				{
					(*iter)->UpdateEventSchedule(startTime, endTime);
					isWaiting = true;
					break;
				}
			}
		}
	}
	else
	{
		ReleaseWaitingEvent(eventKind);
	}

	bool isRunning = ReleaseRunningEvent(eventKind);
	if (isRun && isRunning && isWaiting == false)
		AddWaitingEvent(eventID, eventKind, eventSubKind, startTime, endTime, isRun);
}

void CEventManager::ReloadDailyEvent()
{
	for (auto iter = m_runningEvents.begin(); iter != m_runningEvents.end(); iter++)
	{
		if (IS_NOT_NULL(*iter))
		{
			if ((*iter)->Kind() == GAME::EVENT_EVERYDAY)
			{
				(*iter)->ReloadEvent();
				break;
			}
		}
	}
}


void CEventManager::AddWaitingEvent(INT64 eventID, INT32 eventKind, INT32 eventSubKind, INT64 startTime, INT64 endTime, bool isRun)
{
	if (eventKind == GAME::EVENT_THANKSGIVING
		|| eventKind == GAME::EVENT_CHRISTMAS
		|| eventKind == GAME::EVENT_NEWYEARSDAY
		|| eventKind == GAME::EVENT_LUNARYEAR
		|| eventKind == GAME::EVENT_EVERYDAY
		|| eventKind == GAME::EVENT_VALENTINE
		|| eventKind == GAME::EVENT_ANNIVERSARY_100)
	{
		const auto& eventInfo = BASE::EVENT_INFO_DATA.find(eventKind);

		if (BASE::EVENT_INFO_DATA.end() == eventInfo)
			return;

		// 매일 이벤트는 항상 열리는 것인가???
		if (eventKind == GAME::EVENT_EVERYDAY)
		{
			eventInfo->second = std::make_shared<BASE::EVENT_INFO>();
			eventInfo->second->Kind = GAME::EVENT_EVERYDAY;
		}

		if (eventKind != eventInfo->second->Kind)
			return;

		bool alreadyHave = false;
		for (auto& cofEvent : m_waitingEvents)
		{
			if (cofEvent->Kind() == eventKind)
			{
				alreadyHave = true;
				break;
			}
		}
		for (auto& cofEvent : m_runningEvents)
		{
			if (cofEvent->Kind() == eventKind)
			{
				alreadyHave = true;
				break;
			}
		}

		if (alreadyHave == true)
			UpdateEventSchedule(eventID, eventKind, 0, startTime, endTime, isRun);
		else if (isRun == true)
		{
			if (eventKind == GAME::EVENT_EVERYDAY)
			{
				AddWaitingEvent(new CDailyEvent(eventInfo->second.get(), startTime, endTime));
			}
			else
				AddWaitingEvent(new IEvent(eventInfo->second.get(), startTime, endTime));
		}
	}
}

void CEventManager::AddWaitingEvent(IEvent* cofEvent)
{
	CEventPeriod::PeriodResult period = cofEvent->IsValid(GetDueDay_UTC(0));
	if (period == CEventPeriod::NOT_YET
		|| period == CEventPeriod::INTIME)
		m_waitingEvents.push_back(cofEvent);
}

void CEventManager::DeleteWaitingEvent(INT64 scheduleID)
{
	INT64 currentTime = GetDueDay_UTC(0);
	for (auto iter = m_waitingEvents.begin(); iter != m_waitingEvents.end(); )
	{
		if ((*iter)->IsValid(currentTime) == CEventPeriod::INTIME)
		{
			iter++;
		}
		else if ((*iter)->ScheduleID() == scheduleID)
		{
			iter = m_waitingEvents.erase(iter--);
		}
	}
}

void CEventManager::Update()
{
	/*if (m_Initialized == false)
	return;*/
	AUTO_WORK_TIME_CHECK();

	DWORD _memory = GLOBAL::PROCESS_MONITOR.GetMemoryUsage() / 1024;

	//업데이트 하면서 이벤트의 기간을 체크하자. 
	INT64 currentTime = GetDueDay_UTC(0);

	UpdateEventSchedule(currentTime);
	UpdateRunningEvent(currentTime);

	INT64 check_memory = abs((INT64)(_memory - GLOBAL::PROCESS_MONITOR.GetMemoryUsage() / 1024));
	if (check_memory > 100)
	{
		TLNET_LOG(boost::log::trivial::severity_level::debug, "CEventManager::Update");
	}
}


void CEventManager::GetRunningEvents(CUser* pUser)
{
	if (IS_NOT_NULL(pUser))
	{
		NEW_FLATBUFFER(GS_EVENT_RUNNING_LIST_GET_ACK, pPacket);
		pPacket.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
		{
			std::vector<INT32> runningEvents;

			for (const auto& cofEvent : m_runningEvents)
				runningEvents.push_back(cofEvent->Kind());

			return PROTOCOL::FLATBUFFERS::CreateGS_EVENT_RUNNING_LIST_GET_ACK(fbb, fbb.CreateVector(runningEvents));
		});

		SEND_ACTIVE_USER(pUser, pPacket);
	}
}
//void CEventManager::OnEvent_MAKE_TANK_BY_KIND(INT64 userID, INT32 kind, INT32 count)
//{
//	if (GLOBAL::IsBattleRoyalServer() == true)
//		return;
//
//	if (userID <= 0)
//	{
//		TLNET_LOG(boost::log::trivial::severity_level::error, "UID InValid By OnEvent_MAKE_TANK_BY_KIND. Add UID:%ld Kind:%d", userID, kind);
//		return;
//	}
//
//	if (count <= 0)
//	{
//		TLNET_LOG(boost::log::trivial::severity_level::error, "Count Error By OnEvent_MAKE_TANK_BY_KIND. Add UID:%ld Kind:%d", userID, kind);
//		return;
//	}
//
//	auto tankInfo = BASE::GET_TANK_DATA(kind, 1);
//	if (tankInfo == nullptr)
//	{
//		TLNET_LOG(boost::log::trivial::severity_level::error, "TankInfo Is Null By OnEvent_MAKE_TANK_BY_KIND. Add UID:%ld Kind:%d", userID, kind);
//		return;
//	}
//
//	int conditionKind = 0;
//	int subCondition = 0;
//	int eventValue = 0;
//	int tireValue = 0;
//
//	// 탱크의 티어와 클래스를 합쳐서 ConditionKind를 만들어줍니다.
//	conditionKind = static_cast<int>(GAME::eEVENTCONDITION_TYPE::BUILD_TANK_TIER);
//	int tankClass = tankInfo->TANKCLASS;
//	int tankTier = tankInfo->TIER;
//	eventValue = count;
//
//	// 탱크 티어는 10의 자리 클래스는 일의 자리입니다.
//	subCondition = ((tankTier * 10) + tankClass);
//
//	if (subCondition <= 0)
//	{
//		TLNET_LOG(boost::log::trivial::severity_level::error, "subCondition Error By OnEvent_MAKE_TANK_BY_KIND. Add UID:%ld TankKind:%d", userID, kind);
//		return;
//	}
//
//	OnEvent(userID, conditionKind, subCondition, eventValue, tireValue);
//}

void CEventManager::OnEvent(INT64 userID, int conditionKind, int subCondition, int eventValue, int tireValue)
{
	if (GLOBAL::IsBattleRoyalServer() == true)
		return;

	for (const auto& runningEvent : m_runningEvents)
	{
		runningEvent->OnEvent(userID, conditionKind, subCondition, eventValue, tireValue);
	}
}

void CEventManager::AddPointDailyEvent(INT64 userID, INT32 eventKind, int dailyPoint)
{
	if (GLOBAL::IsBattleRoyalServer() == true)
		return;

	for (const auto& runningEvent : m_runningEvents)
	{
		if (eventKind == runningEvent->Kind())
			runningEvent->GmCommandDaily(userID, dailyPoint);
	}
}

//InitEvent가 아니라 GM커맨드용 테스트 함수이다....;;;;
void CEventManager::InitEvent()
{
	for (const auto& [fst, eventInfo] : BASE::EVENT_INFO_DATA)
	{
		if (eventInfo->Kind == 1)
			AddWaitingEvent(new CDailyEvent(eventInfo.get()));
		else
			AddWaitingEvent(new IEvent(eventInfo.get()));
	}
	if (m_waitingEvents.size() == 0 && m_runningEvents.size() == 0)
		return;

	m_Initialized = true;
}

void CEventManager::SendRankReward_DailyMissionByGM(INT64 userID, INT32 eventType)
{

	if (GLOBAL::IsBattleRoyalServer() == true)
		return;

	//현재 매일 이벤트 week 관련 값들 
	int curEventWeeks = GLOBAL::EVENT_MANAGER.GetDailyEventWeeks();
	int curEventWeeklyID = GLOBAL::GetDailyEventWeeklyID(GetDueDay_UTC(0));
	int missionType = BASE::GetDailyEventMissionType(curEventWeeks);

	STRING title;
	STRING contents;

	std::vector < BASE::DAILY_EVENT_REWARD> rewards;

	if (eventType == 0) //주간이벤트
	{
		//std::vector<JSON::JsonMailParam> titleParam;
		//titleParam.push_back(JSON::JsonMailParam(1, _T("$0$")));

		title = CMailManager::GetMailString(_T("SYSTEM_MAIL_TITLE_DAILYEVENT_002_NEW"), true, nullptr);

		std::vector<JSON::JsonMailParam> param;
		param.push_back(JSON::JsonMailParam(1, _T("$0$")));

		contents = CMailManager::GetMailString(_T("SYSTEM_MAIL_CONTENTS_DAILYEVENT_002"), true, &param);

		rewards = BASE::GET_DAILY_EVENT_WEEK_REWARD_LIST(missionType);
	}
	else //매일이벤트
	{
		auto rankinfo = BASE::GetDailyEventMissionInfos(missionType, eventType);
		if (rankinfo == nullptr)
			return;

		//std::vector<JSON::JsonMailParam> titleParam;
		//titleParam.push_back(JSON::JsonMailParam(1, _T("$0$")));

		title = CMailManager::GetMailString(_T("SYSTEM_MAIL_TITLE_DAILYEVENT_001_NEW"), true, nullptr);

		std::vector<JSON::JsonMailParam> param;
		param.push_back(JSON::JsonMailParam(2, rankinfo->asMissionInfo[0].Name));
		param.push_back(JSON::JsonMailParam(1, _T("$0$")));

		contents = CMailManager::GetMailString(_T("SYSTEM_MAIL_CONTENTS_DAILYEVENT_001"), true, &param);

		rewards = BASE::GET_DAILY_EVENT_REWARD_LIST(missionType, eventType);
	}

	// 랭킹 정보를 DB에 업데이트
	CRankMgr::Instance()->Send_DB_Prev_Daily_Rank_List_Set_Req(eventType, curEventWeeklyID);
	
}

void CEventManager::Release()
{
	ReleaseEvents();
}

void CEventManager::ReleaseEvent(INT32 eventKind)
{
	ReleaseWaitingEvent(eventKind);
	ReleaseRunningEvent(eventKind);
}

bool CEventManager::ReleaseWaitingEvent(INT32 eventKind)
{
	bool isDelete = false;
	for (auto iter = m_waitingEvents.begin(); iter != m_waitingEvents.end(); iter++)
	{
		IEvent* pEvent = (*iter);
		if (IS_NOT_NULL(pEvent) && pEvent->Kind() == eventKind)
		{
			m_waitingEvents.erase(iter);
			SAFE_DELETE(pEvent);
			isDelete = true;
			break;
		}
	}

	return isDelete;
}

bool CEventManager::ReleaseRunningEvent(INT32 eventKind)
{
	bool isDelete = false;
	for (auto iter = m_runningEvents.begin(); iter != m_runningEvents.end(); iter++)
	{
		IEvent* pEvent = (*iter);
		if (IS_NOT_NULL(pEvent) && pEvent->Kind() == eventKind)
		{
			m_runningEvents.erase(iter);
			pEvent->Complete();
			SAFE_DELETE(pEvent);
			isDelete = true;
			break;
		}
	}

	return isDelete;
}

void CEventManager::ReleaseEvents()
{
	ReleaseWaitingEvent();
	ReleaseRunningEvent();
}

void CEventManager::ReleaseWaitingEvent()
{
	for (auto& cofEvent : m_waitingEvents)
		SAFE_DELETE(cofEvent);
	m_waitingEvents.clear();
}

void CEventManager::ReleaseRunningEvent()
{
	for (auto& cofEvent : m_runningEvents)
	{
		cofEvent->Complete();
		SAFE_DELETE(cofEvent);
	}
	m_runningEvents.clear();
}

void CEventManager::UpdateEventSchedule(INT64 currentTime)
{
	for (auto iter = m_waitingEvents.begin(); iter != m_waitingEvents.end(); ++iter)
	{
		if ((*iter)->IsValid(currentTime) == CEventPeriod::INTIME)
		{
			m_runningEvents.push_back(*iter);
			(*iter)->Initialize();
			iter = m_waitingEvents.erase(iter);

			break;
		}
	}
}

void CEventManager::UpdateRunningEvent(INT64 currentTime)
{
	for (auto iter = m_runningEvents.begin(); iter != m_runningEvents.end(); )
	{
		CEventPeriod::PeriodResult result = (*iter)->IsValid(currentTime);
		if (result == CEventPeriod::NOT_YET)
		{
			m_waitingEvents.push_back(*iter);
			iter = m_runningEvents.erase(iter);

		}
		else if (result == CEventPeriod::INTIME)
		{
			(*iter)->Update();
			iter++;
		}
		else if (result == CEventPeriod::OVER)
		{
			IEvent* pEvent = (*iter);
			iter = m_runningEvents.erase(iter);
			pEvent->Complete();
		}
	}
}

void CEventManager::BroadcastEventMessage(EVENT_MESSAGE message)
{

}

void CEventManager::GetActiveEventKindList(std::vector<INT32>& eventList)
{
	eventList.clear();
	for (auto runningEvent : m_runningEvents)
	{
		if (runningEvent->IsValid(GetDueDay_UTC(0)) == CEventPeriod::INTIME)
			eventList.push_back(runningEvent->Kind());
	}
}

bool CEventManager::IsActive(INT32 eventKind)
{
	for (auto runningevent : m_runningEvents)
	{
		if (runningevent->Kind() == eventKind &&
			runningevent->IsValid(GetDueDay_UTC(0)) == CEventPeriod::INTIME)
		{
			return true;
		}
	}

	return false;
}

int CEventManager::GetEventRewardLogType(INT32 conditionKind)
{
	/*REASON_COLLECT_ISLE_EVENT_ITEM_GET = 1411,
	REASON_GUILD_RESEARCH_ASSET_UPDATE_CONTRIBUTION__EVENT_ITEM_GET = 1412,
	REASON_EMPIRE_ATTACK_EVENT_ITEM_GET = 1413,
	REASON_TERRITORY_NPC_MEMBER_ALL_EVENT_ITEM_GET = 1414,
	REASON_GUILD_QUOTES_TRADE_UPLOAD_EVENT_ITEM_GET = 1415,
	REASON_TRADE_ITEM_BUY_RESOURCE_USE_EVENT_ITEM_GET = 1416,*/

	int logType = DB_LOG::REASON_DEFAULT_EVENT_ITEM_GET;
	switch (conditionKind)
	{
	case GAME::COLLECT_RESOURCEALL:
		logType = DB_LOG::REASON_COLLECT_ISLE_EVENT_ITEM_GET;
		break;
	case GAME::COND_KILLMON_COUNT:
		//logType = DB_LOG::REASON_COLLECT_ISLE_EVENT_ITEM_GET;
		break;
	case GAME::DONATION_GUILD:
		logType = DB_LOG::REASON_GUILD_RESEARCH_ASSET_UPDATE_CONTRIBUTION__EVENT_ITEM_GET;
		break;
	case GAME::SUPPORT_GUILD_WORK:
		//logType = DB_LOG::REASON_COLLECT_ISLE_EVENT_ITEM_GET;
		break;
	//case GAME::CREATE_EQUIP:
	//	logType = DB_LOG::REASON_PRODUCT_EQUIPMENT_RESOURCE_USE_EVENT_ITEM_GET;
	//	break;
	case GAME::NPC_ATTACK_COUNT:
		logType = DB_LOG::REASON_EMPIRE_ATTACK_EVENT_ITEM_GET;
		break;
	case GAME::EMPIRE_KILL:
		//logType = DB_LOG::REASON_COLLECT_ISLE_EVENT_ITEM_GET;
		break;
	default:
		break;
	}

	return logType;
}

//void CEventManager::RankingNotRewardListAck(const PROTOCOL::FLATBUFFERS::DB_RANKING_NOT_REWARDED_LIST_GET_ACK* ack)
//{
//	if (IS_FAILED(ack->Result()))
//	{
//		TLNET_LOG(boost::log::trivial::severity_level::debug, "Error : %d, ServerID : %d", ack->Result(), ack->ServerID());
//	}
//	else
//	{
//		if (ack->NotRewards() == nullptr || ack->NotRewards()->Length() == 0)
//			return;
//
//
//		//임시코드...
//
//		//현재 매일 이벤트 week 관련 값들 
//		int curEventWeeks = GLOBAL::EVENT_MANAGER.GetDailyEventWeeks();
//		int curEventWeeklyID = GLOBAL::GetDailyEventWeeklyID(GetDueDay_UTC(0));
//
//
//		for (UINT32 i = 0; i < ack->NotRewards()->Length(); ++i)
//		{
//			auto rank_unit = ack->NotRewards()->Get(i);
//			if (rank_unit == nullptr)
//				continue;
//
//			//랭킹보상 처리해야 할 미션이 이전 주에 해당하는 내용일 경우 보정처리.
//			//MAIL쪽에 WeeklyID와 현재 WeeklyID의 차이를 비교해서 몇주차인지 계산합니다. (음수는 무시)
//			int diffWeeklyID = max(curEventWeeklyID - rank_unit->WeeklyNumber(), 0);
//			int prevEventWeeks = curEventWeeks - diffWeeklyID;
//			int missionType = BASE::GetDailyEventMissionType(prevEventWeeks);
//
//			STRING title;
//			STRING contents;
//
//			std::vector < BASE::DAILY_EVENT_REWARD> rewards;
//
//			if (rank_unit->EventType() == 0) //주간이벤트
//			{
//				std::vector<JSON::JsonMailParam> titleParam;
//				titleParam.push_back(JSON::JsonMailParam(1, _T("$0$")));
//
//				title = CMailManager::GetMailString(_T("SYSTEM_MAIL_TITLE_DAILYEVENT_002_NEW"), true, &titleParam);
//
//				std::vector<JSON::JsonMailParam> param;
//				param.push_back(JSON::JsonMailParam(1, _T("$0$")));
//
//				contents = CMailManager::GetMailString(_T("SYSTEM_MAIL_CONTENTS_DAILYEVENT_002"), true, &param);
//
//				rewards = BASE::GET_DAILY_EVENT_WEEK_REWARD_LIST(missionType);
//			}
//			else //매일이벤트
//			{
//				auto rankinfo = BASE::GetDailyEventMissionInfos(missionType, rank_unit->EventType());
//				if (rankinfo == nullptr)
//					continue;
//
//				std::vector<JSON::JsonMailParam> titleParam;
//				titleParam.push_back(JSON::JsonMailParam(1, _T("$0$")));
//
//				title = CMailManager::GetMailString(_T("SYSTEM_MAIL_TITLE_DAILYEVENT_001_NEW"), true, &titleParam);
//
//				std::vector<JSON::JsonMailParam> param;
//				param.push_back(JSON::JsonMailParam(2, rankinfo->asMissionInfo[0].Name));
//				param.push_back(JSON::JsonMailParam(1, _T("$0$")));
//
//				contents = CMailManager::GetMailString(_T("SYSTEM_MAIL_CONTENTS_DAILYEVENT_001"), true, &param);
//
//				rewards = BASE::GET_DAILY_EVENT_REWARD_LIST(missionType, rank_unit->EventType());
//			}
//
//			NEW_FLATBUFFER(DB_RANKING_REWARD_MAIL_SEND_REQ, pPacket);
//			pPacket.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
//			{
//				std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::DAILY_EVENT_RANKING_REWARD>> rewardOffsets;
//
//				for (auto& reward : rewards)
//				{
//					STRING appendings;
//					if (reward.Valid())
//					{
//						appendings = CMailManager::WrapItemToString(reward.Rewards);
//						rewardOffsets.push_back(PROTOCOL::FLATBUFFERS::CreateDAILY_EVENT_RANKING_REWARD(fbb, reward.RankingMin, reward.RankingLimit, CREATE_FLATBUFFER_RAWSTR(appendings.c_str())));
//					}
//				}
//
//				return PROTOCOL::FLATBUFFERS::CreateDB_RANKING_REWARD_MAIL_SEND_REQ(fbb,
//					GLOBAL::GS_INFO.SVID,
//					rank_unit->WeeklyNumber(),
//					rank_unit->EventType(),
//					CREATE_FLATBUFFER_RAWSTR(title.c_str()),
//					CREATE_FLATBUFFER_RAWSTR(contents.c_str()),
//					fbb.CreateVector(rewardOffsets),
//					GetDueDay_UTC(0),
//					GetDueDay_UTC(0) + 86400 * 30,
//					GetDueDay_UTC(0),
//					GAME::eMAIL_TYPE::MAIL_TYPE_APPENDING,
//					GAME::eMAIL_CATEGORY::NOTICE);
//			});
//
//			SEND_DBA_MAIL(pPacket);
//		}
//	}
//}

void CEventManager::Test_ThanksgivingStart(INT64 i64Seconds)
{
	ReleaseEvents();
	InitEvent();
	Update();

	for (auto runningEvent : m_waitingEvents)
	{
		if (runningEvent->Kind() == 2)
		{
			runningEvent->GetPeriod()->SetStartTime(GetDueDay_UTC(i64Seconds));
			break;
		}
	}
}

void CEventManager::Test_ThanksgivingEnd(INT64 i64Seconds)
{
	for (auto runningEvent : m_runningEvents)
	{
		if (runningEvent->Kind() == 2)
		{
			runningEvent->GetPeriod()->SetEndTime(GetDueDay_UTC(i64Seconds));
			break;
		}
	}
}

void CEventManager::TestEvent()
{

	if (m_Initialized == false)
		return;
	INT64 testIDs[] = {
		400559,
		400553,
		400551,
		400549,
		400545,
		350942,
		350940,
		350936,
		350934,
		350845,
		350836,
		350833,
		350804,
		350801,
		350798,
		350794,
		350792,
		350784,
		350781,
		350779,
		350777,
		350743,
		346529,
		346521,
		346517,
		305979,
		275119,
		275115,
		275113,
		275111,
		275073,
		275062,
		275059,
		275038,
		275029,
		275013,
		275012,
		275008,
		275004,
		274998,
		274996,
		274994,
		274992,
		274990,
		274988,
		274975,
		270992,
		266770,
		266601,
		266532,
		266507,
		262503,
		262351,
		262340,
		262338,
		262334,
		233846,
		229844,
		225774,
		161447 };

	OnEvent(testIDs[testUserIndex++], 150, 0, CRandManager::INDEX(EnumRandType::EVENT_BASE, 100) + 1);
	if (testUserIndex >= 50)
		testUserIndex = 0;
}


void CEventManager::SendClientDailyEventData(CUser* pUser, INT32 showUI)
{
	if (IS_NULL(pUser))
		return;

	if (pUser->CheckDailyPointRefreshData() == false)
	{
		pUser->RefreshDailyEventDatas();
	}

	int Commandlevel = pUser->Territory().GetBuildLevelFromKind(GAME::KIND_BUILD_COMMAND);
	if (Commandlevel <= 0)
		return;

	const auto uid = pUser->UID();

	NEW_FLATBUFFER(GS_DAILY_EVENT_DATA_GET_ACK, pPACKET);

	pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto // FlatBuffers 의 offset 을 리턴한다.
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::DAILY_EVENT_DATA>> tempRewardPointVector;

		INT32 curWeekNum = GLOBAL::GetDailyEventWeeklyID(GetDueDay_UTC(0));
		INT32 curWeekDay = GLOBAL::GetDailyEventWeekDay(GetDueDay_UTC(0));
		INT32 curDailyPoint = pUser->GetDailyEventPoint();
		INT32 curWeeklyPoint = pUser->GetWeeklyDailyPoint();

		std::vector<INT32> vecDailyRewardStep = pUser->GetDailyEventRewardStep(curWeekNum, curWeekDay);

		for (int k = 1; k <= BASE::GAME_CONST_DATA.CONST_MAIL_EVENT_MISSION_COUNT; k++)
		{
			//요일 (1 월 ~ 6 주말)
			int dayOfWeek = k;

			//미션타입을 가져옵니다. 
			int missionType = BASE::GetDailyEventMissionType(GLOBAL::EVENT_MANAGER.GetDailyEventWeeks());

			BASE::DAILY_EVENT_MISSION_INFO_ARRAY* missionArray = BASE::GetDailyEventMissionInfos(missionType, dayOfWeek);

			if (IS_NULL(missionArray))
				continue;

			auto eventMissionDatasByLevel = missionArray->GetEventMissionDataByLevel(Commandlevel);
			const decltype(std::get<0>(eventMissionDatasByLevel))& vecDailyMissionInfo = std::get<0>(eventMissionDatasByLevel);
			const decltype(std::get<1>(eventMissionDatasByLevel))& pointDataVector = std::get<1>(eventMissionDatasByLevel);
			const decltype(std::get<2>(eventMissionDatasByLevel))& vecRewardItem = std::get<2>(eventMissionDatasByLevel);

			//데이터가 하나도 없을경우.
			if (vecDailyMissionInfo.size() <= 0)
				continue;

			//널체크!! 
			BASE::DAILY_EVENT_MISSION_INFO* pMissionInfo = vecDailyMissionInfo[0];
			if (nullptr == pMissionInfo)
				continue;

			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::DAILY_EVENT_EXPLAIN>> vecDailyEventExplains;
			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::DAILY_EVENT_EXPLAIN>> vecDailyPointDetails;
			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::DAILY_EVENT_EXPLAIN>> vecDailyPointDetailsSubTitle;
			vecDailyEventExplains.reserve(vecDailyMissionInfo.size());
			vecDailyPointDetails.reserve(vecDailyMissionInfo.size());
			for (size_t i = 0; i < vecDailyMissionInfo.size(); ++i)
			{
				if (nullptr == vecDailyMissionInfo[i]) continue;

				vecDailyEventExplains.emplace_back(PROTOCOL::FLATBUFFERS::CreateDAILY_EVENT_EXPLAIN(fbb, CREATE_FLATBUFFER_RAWSTR(vecDailyMissionInfo[i]->Desc)));
				vecDailyPointDetails.emplace_back(PROTOCOL::FLATBUFFERS::CreateDAILY_EVENT_EXPLAIN(fbb, CREATE_FLATBUFFER_RAWSTR(vecDailyMissionInfo[i]->PointDetail)));
				vecDailyPointDetailsSubTitle.emplace_back(PROTOCOL::FLATBUFFERS::CreateDAILY_EVENT_EXPLAIN(fbb, CREATE_FLATBUFFER_RAWSTR(vecDailyMissionInfo[i]->PointDetailSubtitle)));
			}


			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::DAILY_EVENT_REWARD>> vecDailyEventRewards;
			vecDailyEventRewards.reserve(vecRewardItem.size());

			for (size_t i = 0; i < vecRewardItem.size(); ++i)
			{
				vector<int> rewardIDVector;			rewardIDVector.reserve(vecRewardItem[i].Rewards.size());
				vector<int> rewardNumVector;		rewardNumVector.reserve(vecRewardItem[i].Rewards.size());

				for (size_t rewardIdx = 0; rewardIdx < vecRewardItem[i].Rewards.size(); ++rewardIdx)
				{
					rewardIDVector.push_back(vecRewardItem[i].Rewards[rewardIdx].Kind);
					rewardNumVector.push_back(vecRewardItem[i].Rewards[rewardIdx].Count);
				}

				vecDailyEventRewards.emplace_back(PROTOCOL::FLATBUFFERS::CreateDAILY_EVENT_REWARD(fbb, vecRewardItem[i].Step, fbb.CreateVector(rewardIDVector), fbb.CreateVector(rewardNumVector)));
			}

			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::TOP_RANKER>> vecDailyEventTopRankers;
			flatbuffers::Offset<PROTOCOL::FLATBUFFERS::MYRANKING> myDailyRank;
			vecDailyEventTopRankers.reserve(3);
			auto rankKind = dayOfWeek;
			const auto& rankContainer = dynamic_pointer_cast<CDailyRankerContainer>(CRankMgr::Instance()->GetRankerContainer(GAME::eRANK_TYPE::DAILY_EVENT_RANK, rankKind));
			if (nullptr != rankContainer)
			{
				std::vector<stBaseRanker> vecRanks;
				rankContainer->GetDailyRankerList(rankKind, vecRanks, 3);
				for (auto ranker : vecRanks)
				{
					const auto& user = CUserManager::Instance()->GetByUID(ranker.m_nUniqueKey);
					if (nullptr == user)
						continue;

					const auto nickName = CGuildManager::Instance()->GetGuildNickNameFromPersonID(user->UID());

					vecDailyEventTopRankers.emplace_back(PROTOCOL::FLATBUFFERS::CreateTOP_RANKER(fbb, static_cast<int>(GAME::eRANK_TYPE::DAILY_EVENT_RANK), rankKind, ranker.m_nPoint, ranker.m_nUniqueKey,
						0, user->GetLordPortraitIndex(), 0, 0, 0, CREATE_FLATBUFFER_RAWSTR(user->GetLordName()), CREATE_FLATBUFFER_RAWSTR(nickName)));
				}

				rankContainer->GetDailyMyRanking(fbb, uid, rankKind, myDailyRank);
			}

			TCHAR szDesc[128] = { 0, };
			TCHAR szAppend[32] = { 0, };
			_STRCPY(szDesc, L"UI_DAILYEVENT_EXPLAIN_WARDS_");
			_stprintf(szAppend, L"%02d", k + 1);
			_tcscat(szDesc, szAppend);

			auto dailyEventData = PROTOCOL::FLATBUFFERS::CreateDAILY_EVENT_DATA(fbb
				, missionType
				, dayOfWeek
				, fbb.CreateVector(pointDataVector)
				, CREATE_FLATBUFFER_RAWSTR(pMissionInfo->Name)
				, CREATE_FLATBUFFER_RAWSTR(pMissionInfo->Desc)
				, fbb.CreateVector(vecDailyEventRewards)
				, fbb.CreateVector(vecDailyPointDetails)
				, CREATE_FLATBUFFER_RAWSTR(pMissionInfo->ImageUrl)
				, fbb.CreateVector(vecDailyEventExplains)
				, fbb.CreateVector(vecDailyPointDetailsSubTitle)
				, CREATE_FLATBUFFER_RAWSTR(pMissionInfo->TopTextName)
				, fbb.CreateVector(vecDailyEventTopRankers)
				, myDailyRank
			);

			tempRewardPointVector.push_back(dailyEventData);
		}
		
		flatbuffers::Offset<PROTOCOL::FLATBUFFERS::MYRANKING> myWeeklyRank;
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::TOP_RANKER>> vecWeeklyEventTopRankers;
		const auto& rankContainer = dynamic_pointer_cast<CDailyRankerContainer>(CRankMgr::Instance()->GetRankerContainer(GAME::eRANK_TYPE::DAILY_EVENT_RANK, 0));
		if (nullptr != rankContainer)
		{
			std::vector<stBaseRanker> vecRanks;
			rankContainer->GetDailyRankerList(0, vecRanks, 3);
			for (auto ranker : vecRanks)
			{
				const auto& user = CUserManager::Instance()->GetByUID(ranker.m_nUniqueKey);
				if (nullptr == user)
					continue;

				const auto nickName = CGuildManager::Instance()->GetGuildNickNameFromPersonID(user->UID());

				vecWeeklyEventTopRankers.emplace_back(PROTOCOL::FLATBUFFERS::CreateTOP_RANKER(fbb, static_cast<int>(GAME::eRANK_TYPE::DAILY_EVENT_RANK), 0, ranker.m_nPoint, ranker.m_nUniqueKey,
					0, user->GetLordPortraitIndex(), 0, 0, 0, CREATE_FLATBUFFER_RAWSTR(user->GetLordName()), CREATE_FLATBUFFER_RAWSTR(nickName)));
			}

			rankContainer->GetDailyMyRanking(fbb, uid, 0, myWeeklyRank);
		}

		return PROTOCOL::FLATBUFFERS::CreateGS_DAILY_EVENT_DATA_GET_ACK(fbb, fbb.CreateVector(tempRewardPointVector)
			, showUI, curWeekDay, curDailyPoint, fbb.CreateVector(vecDailyRewardStep), curWeeklyPoint, fbb.CreateVector(vecWeeklyEventTopRankers), myWeeklyRank);
	}
	);
	SEND_ACTIVE_USER(pUser, pPACKET);
}

//매일 이벤트 포인트 보상을 지급 한다.
void CEventManager::SendDBDailyEventRewardSet(CUser* pUser, const void* pData)
{
	if (IS_NULL(pUser))
		return;

	auto req = PROTOCOL::FLATBUFFERS::GetGS_DAILY_EVENT_POINT_REWARD_SET_REQ(pData);

	INT32 weekNum = GLOBAL::GetDailyEventWeeklyID(GetDueDay_UTC(0));
	INT32 weekDay = GLOBAL::GetDailyEventWeekDay(GetDueDay_UTC(0));

	//현재 UTC 시간과 마지막으로 업데이트되었던 시간이 맞지 않으면 이벤트 포인트 & 보상정보 초기화.
	if (pUser->CheckDailyPointRefreshData() == false)
	{
		pUser->RefreshDailyEventDatas();
	}

	INT32 rewardStep = req->RewardStep();
	auto funcCompare = [&](INT32 iValue)->bool { return iValue == rewardStep; };

	// 중복값이 있으면 잘못된 요청입니다.
	// 미리 메모리 선처리 하지 않는 이유는 DB 값 갱신 하는 부분에서 추가로 예외처리 진행하기 때문입니다.
	auto vecReward = pUser->GetDailyEventRewardStep(weekNum, weekDay);
	if (std::find_if(vecReward.begin(), vecReward.end(), funcCompare) != vecReward.end())
	{
		TLNET_LOG(boost::log::trivial::severity_level::debug, "DailyEventRewardError UserID : %d, Step : %d, WeekDay : %d", pUser->UID(), rewardStep, weekDay);
		return;
	}

	auto rewardMissionInfo = BASE::GetDailyEventMissionInfosByRewardStep(BASE::GetDailyEventMissionType(GLOBAL::EVENT_MANAGER.GetDailyEventWeeks()), weekDay, rewardStep);

	auto  pMissionInfo = std::get<BASE::DAILY_EVENT_MISSION_INFO_ARRAY::MI_INFO>(rewardMissionInfo);
	INT32 rewardPoint = std::get<BASE::DAILY_EVENT_MISSION_INFO_ARRAY::MI_POINT>(rewardMissionInfo);
	auto  pRewardArray = std::get<BASE::DAILY_EVENT_MISSION_INFO_ARRAY::MI_REWARDARRAY>(rewardMissionInfo);
	INT32 curDailyPoint = pUser->GetDailyEventPoint();

	//체크!! (널체크, 포인트 체크, 레벨체크)
	if (nullptr == pMissionInfo
		|| nullptr == pRewardArray
		|| curDailyPoint < rewardPoint
		|| pUser->GetCastleLevel() < pMissionInfo->LevelMin)
		return;

	INT64 oil_add = 0;
	INT64 iron_add = 0;
	INT64 silver_add = 0;
	INT64 gold_add = 0;
	PROTOCOL::ASSET addAsset;



	std::vector<Item> items;

	//// value 수치에 맞춰 1종의 아이템 지급
	for (auto& reward : (*pRewardArray))
	{
		if (reward.Kind <= 0)
			continue;

		if (reward.Count <= 0)
			continue;


		auto itemInfo = BASE::GET_ITEM_DATA(reward.Kind);
		if (itemInfo == nullptr)
			continue;

		switch (itemInfo->i32ITEM_TYPE)
		{
		case GAME::ITEM_NATIVE_RESOURCE: //직접 지급한다!
		{
			if (itemInfo->i32ITEM_KIND == 1) oil_add += reward.Count;
			else if (itemInfo->i32ITEM_KIND == 2) iron_add += reward.Count;
			else if (itemInfo->i32ITEM_KIND == 3) silver_add += reward.Count;
			else if (itemInfo->i32ITEM_KIND == 4) gold_add += reward.Count;
		}
		break;

		default:  //아이템으로 지급한다.
		{
			items.push_back(Item(reward.Kind, reward.Count));
		}
		break;

		}
	}

	auto asset_add = PROTOCOL::ASSET(oil_add, iron_add, silver_add, gold_add);

	Util::DailyPoint::DailyEventPointRewardSet(
		pUser->GetSharedPtr(),
		GLOBAL::GetDailyEventWeeklyID(GetDueDay_UTC(0)),
		GLOBAL::GetDailyEventWeekDay(GetDueDay_UTC(0)),
		rewardStep,
		items,
		curDailyPoint,
		asset_add);
}
/////////////////////////////////////////////////////////////////////////////////////////
void CEventManager::Send_GS_SEASON_EVENT_LIST_ACK(const PROTOCOL::FLATBUFFERS::DB_CONTENT_EVENT_LIST_GET_ACK* pAck)
{
	CUser* pUser = CUserManager::Instance()->FindByUID(pAck->UserID());
	if (IS_NULL(pUser))
	{
		return;
	}

	if (IS_ACTIVE_USER(pUser))
	{
		NEW_FLATBUFFER(GS_SEASON_EVENT_LIST_ACK, pPACKET);
		pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto // FlatBuffers 의 offset 을 리턴한다.
		{
			bool alreadyAdded_OccupyCityEvent = false;
			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_EVENT_INFO>> eventVector;
			for (auto waitingEvent : m_waitingEvents)
			{
				if (IS_NOT_NULL(waitingEvent))
				{
					if (GetEventTypeByEventKind(waitingEvent->Kind()) == GAME::EVENT_TYPE_SEASON)
					{
						MakeSeasonEventInfoFbb(fbb, waitingEvent, pUser, eventVector, false);
					}
					else if (GetEventTypeByEventKind(waitingEvent->Kind()) == GAME::EVENT_TYPE_CONTENTS)
					{
						if (waitingEvent->Kind() == GAME::CONTENTS_TRADE_PROFIT)
							continue;

						if (IsOccupyCityEvent(waitingEvent->Kind()) == true)
						{
							if (alreadyAdded_OccupyCityEvent == false)
							{
								MakeContentEventInfoFbb(fbb, waitingEvent, pUser, eventVector, false);
								alreadyAdded_OccupyCityEvent = true;
							}
						}
						else
						{
							MakeContentEventInfoFbb(fbb, waitingEvent, pUser, eventVector, false);
						}
					}
				}
			}

			alreadyAdded_OccupyCityEvent = false;
			for (auto runningEvent : m_runningEvents)
			{
				if (IS_NOT_NULL(runningEvent))
				{
					if (GetEventTypeByEventKind(runningEvent->Kind()) == GAME::EVENT_TYPE_SEASON)
					{
						MakeSeasonEventInfoFbb(fbb, runningEvent, pUser, eventVector, false);
					}
					else if (GetEventTypeByEventKind(runningEvent->Kind()) == GAME::EVENT_TYPE_CONTENTS)
					{
						if (runningEvent->Kind() == GAME::CONTENTS_TRADE_PROFIT)
							continue;

						if (IsOccupyCityEvent(runningEvent->Kind()) == true)
						{
							if (alreadyAdded_OccupyCityEvent == false)
							{
								MakeContentEventInfoFbb(fbb, runningEvent, pUser, eventVector, false);
								alreadyAdded_OccupyCityEvent = true;
							}
						}
						else
						{
							bool IsCompleted = IsContentEventCompleted(runningEvent->Kind(), pAck->EventCountInfos());
							MakeContentEventInfoFbb(fbb, runningEvent, pUser, eventVector, IsCompleted);
						}
					}
				}
			}
			return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_LIST_ACK(fbb,
				fbb.CreateVector(eventVector));
		});
		SEND_ACTIVE_USER(pUser, pPACKET);
	}
}

void CEventManager::MakeContentEventInfoFbb(flatbuffers::FlatBufferBuilder& fbb, IEvent* contentsEvent, CUser* pUser, std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_EVENT_INFO>>& vecEvent, bool IsCompleted)
{
	if (IS_NULL(contentsEvent))
	{
		SPDLOG_ERROR("[MakeSeasonEventInfoFbb]seasonEvent NULL!!");
		return;
	}

	if (IS_NULL(pUser))
		return;

	const auto& eventInfo = BASE::CONTENTS_EVENT_INFO_DATA.find(contentsEvent->Kind());

	if (eventInfo == BASE::CONTENTS_EVENT_INFO_DATA.end())
	{
		SPDLOG_ERROR("[MakeSeasonEventInfoFbb]eventInfo NULL!! UserID : {}, Kind : {}", contentsEvent->Kind());
		return;
	}

	vecEvent.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_EVENT_INFO(fbb,
		contentsEvent->Kind(),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Title),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->SubTitle),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Icon_Img),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Story),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Popup_IMG),
		contentsEvent->GetPeriod()->StartTime(),
		contentsEvent->GetPeriod()->EndTime(),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Icon_Atlas),
		(true == IsCompleted) ? 1 : 0));

}

void CEventManager::MakeSeasonEventInfoFbb(flatbuffers::FlatBufferBuilder& fbb, IEvent* seasonEvent, CUser* pUser, std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_EVENT_INFO>>& vecEvent, bool IsCompleted)
{
	if (IS_NULL(seasonEvent))
	{
		SPDLOG_ERROR("[MakeSeasonEventInfoFbb]seasonEvent NULL!!");
		return;
	}

	if (IS_NULL(pUser))
		return;

	const auto& eventInfo = BASE::EVENT_INFO_DATA.find(seasonEvent->Kind());

	if (eventInfo == BASE::EVENT_INFO_DATA.end())
	{
		SPDLOG_ERROR("[MakeSeasonEventInfoFbb]eventInfo NULL!! UserID : {}, Kind : {}", seasonEvent->Kind());
		return;
	}

	//매일 이벤트는 거릅니다.
	if (eventInfo->second->Kind == GAME::EVENT_EVERYDAY)
	{
		return;
	}

	vecEvent.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_EVENT_INFO(fbb,
		seasonEvent->Kind(),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Title),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->SubTitle),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Icon_Img),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Story),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Popup_IMG),
		seasonEvent->GetPeriod()->StartTime(),
		seasonEvent->GetPeriod()->EndTime(),
		CREATE_FLATBUFFER_RAWSTR(eventInfo->second->Icon_Atlas),
		(true == IsCompleted) ? 1 : 0));
}


void CEventManager::Send_GS_SEASON_EVENT_DETAIL_INFO_ACK(CUser* pUser, const PROTOCOL::FLATBUFFERS::GS_SEASON_EVENT_DETAIL_INFO_REQ* pReq)
{
	if (IS_NULL(pUser))
	{
		return;
	}
	if (IS_NULL(pReq))
	{
		return;
	}

	int eventKind = 0;
	eventKind = pReq->Kind();
	if (eventKind <= 0)
	{
		//_ERROR_LOG_FILE("Error.txt", "[Send_GS_SEASON_EVENT_DETAIL_INFO_ACK]seasonEvent kind < 0, Kind : " + pReq->Kind());
		SPDLOG_ERROR("Error.txt - [Send_GS_SEASON_EVENT_DETAIL_INFO_ACK]seasonEvent kind < 0, Kind : {}", pReq->Kind());
		return;
	}
	IEvent* seasonEvent = NULL;
	for (auto waitingEvent : m_waitingEvents)
	{
		if (IS_NOT_NULL(waitingEvent))
		{
			if (waitingEvent->Kind() == eventKind)
			{
				seasonEvent = waitingEvent;
				break;
			}
		}
	}

	if (IS_NULL(seasonEvent))
	{
		for (auto runningEvent : m_runningEvents)
		{
			if (IS_NOT_NULL(runningEvent))
			{
				if (runningEvent->Kind() == eventKind)
				{
					seasonEvent = runningEvent;
					break;
				}
			}
		}
	}

	if (IS_NOT_NULL(seasonEvent))
	{
		if (const auto& eventInfo = BASE::EVENT_INFO_DATA.find(seasonEvent->Kind()); eventInfo == BASE::EVENT_INFO_DATA.end())
		{
			//_ERROR_LOG_FILE("Error.txt", "[MakeSeasonEventInfoFbb]eventInfo NULL!! UserID : %ld, Kind : %d", seasonEvent->Kind());
			SPDLOG_ERROR("Error.txt - [MakeSeasonEventInfoFbb]eventInfo NULL!! UserID : {}, Kind : {}", pUser->UID(), seasonEvent->Kind());
			return;
		}
	}


	// 변원일, 시즌 이벤트는 우선 주석 처리 하였습니다.
	/*NEW_FLATBUFFER(GS_SEASON_EVENT_DETAIL_INFO_ACK, pPACKET);
	pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto // FlatBuffers 의 offset 을 리턴한다.
	{
		if (IS_NOT_NULL(eventInfo))
		{
			return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_DETAIL_INFO_ACK(fbb,
				true,
				eventInfo->Kind,
				CREATE_FLATBUFFER_RAWSTR(GetText(pUser->GetLanguageType(), eventInfo->Title)),
				CREATE_FLATBUFFER_RAWSTR(eventInfo->Popup_IMG),
				CREATE_FLATBUFFER_RAWSTR(GetText(pUser->GetLanguageType(), eventInfo->Popup_Description))
			);
		}
		else
			return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_DETAIL_INFO_ACK(fbb, false, 0, 0, 0, 0);

	});
	SEND_ACTIVE_USER(pUser, pPACKET);*/
}

void CEventManager::SetEventContentsCondition(std::vector<INT32>& o_Values, INT32 Value1, INT32 Value2, INT32 Value3, INT32 Value4, INT32 Value5)
{
	o_Values.clear();
	int Values[] = { Value1, Value2, Value3, Value4, Value5 };
	for (int Idx = 0; Idx < GAME::CONTENT_EVENT_CONDITION_EXPRESS_MAX; Idx++)
	{
		o_Values.push_back(Values[Idx]);
	}
}

void CEventManager::CheckEquipmentEventContents(INT64 userID, PROTOCOL::EQUIPMENT_INFO& Info)
{
	//장비 제작/강화 컨텐츠 이벤트설정
	std::vector<int> CompareValues = std::vector<INT32>();
	for (int i = 0; i < Info.SlotCount(); i++)
	{
		//슬롯에 포함된 특정제작 재료 및 티어, 슬롯갯수 조건체크
		CompareValues.clear();
		SetEventContentsCondition(CompareValues, Info._optKind[i], Info.SlotCount(), Info._grade); //버프종류,슬롯,티어
		OnEventContents(userID, GAME::eCONTENTSCONDITION_TYPE::EQUIP_MANUFACTURE, CompareValues);
	}

	//티어, 슬롯갯수 조건체크
	CompareValues.clear();
	SetEventContentsCondition(CompareValues, 0, Info.SlotCount(), Info._grade); //슬롯,티어
	OnEventContents(userID, GAME::eCONTENTSCONDITION_TYPE::EQUIP_MANUFACTURE_2, CompareValues);
}

void CEventManager::OnEventContents(INT64 userID, INT32 conditionKind, const std::vector<INT32> CompareValues, INT32 IncreaseValue)
{
	if (CompareValues.empty())
		return;

	for (const auto& runningEvent : m_runningEvents)
	{
		if (GAME::EVENT_TYPE_CONTENTS == GetEventTypeByEventKind(runningEvent->Kind()))
		{
			((CContentsEvent*)runningEvent)->SetEventContentsCompareData(CompareValues);
			((CContentsEvent*)runningEvent)->OnEventContents(userID, conditionKind, IncreaseValue);
		}
	}
}

bool CEventManager::IsContentEventCompleted(INT32 ContentKind, const flatbuffers::Vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::CONTENT_EVENT_COUNT_INFO>>* EventListInfos)
{
	bool IsCompleted = false;
	for (const auto& EventListInfo : *EventListInfos)
	{
		if (ContentKind == EventListInfo->EventKind())
		{
			const auto& contentEvent = BASE::CONTENTS_EVENT_INFO_DATA.find(EventListInfo->EventKind());
			if (contentEvent == BASE::CONTENTS_EVENT_INFO_DATA.end())
				return IsCompleted;

			INT32 currentCount = EventListInfo->CurrentCount();
			if (currentCount >= contentEvent->second->ResultCount)
			{
				IsCompleted = true;
				break;
			}
		}
	}

	return IsCompleted;
}

void CEventManager::Send_DB_CONTENTS_EVENT_LIST_REQ(CUser* pUser)
{
	if (IS_NULL(pUser))
		return;

	NEW_FLATBUFFER(DB_CONTENT_EVENT_LIST_GET_REQ, pPACKET);
	pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto // FlatBuffers 의 offset 을 리턴한다.
	{
		return PROTOCOL::FLATBUFFERS::CreateDB_CONTENT_EVENT_LIST_GET_REQ(fbb, GLOBAL::GS_INFO.SVID, pUser->UID(), GetDueDay_UTC(0));
	});
	SEND_DBA(pPACKET);
}

void CEventManager::Send_DB_CONTENT_EVENT_GET_REQ(CUser* pUser, const PROTOCOL::FLATBUFFERS::GS_SEASON_EVENT_DETAIL_INFO_REQ* pReq)
{
	if (IS_NULL(pUser))
	{
		return;
	}
	if (IS_NULL(pReq))
	{
		return;
	}

	int eventKind = 0;
	eventKind = pReq->Kind();
	if (eventKind <= 0)
	{
		SPDLOG_ERROR("[Send_DB_CONTENT_EVENT_GET_REQ] contentEvent kind < 0, Kind : {}", pReq->Kind());
		return;
	}
	if (GetEventTypeByEventKind(eventKind) != GAME::EVENT_TYPE_CONTENTS)
	{
		SPDLOG_ERROR("[Send_DB_CONTENT_EVENT_GET_REQ] eventType is not contentEvent, eventType : {}", GAME::EVENT_TYPE_CONTENTS);
		return;
	}

	IEvent* contentEvent = NULL;
	for (auto waitingEvent : m_waitingEvents)
	{
		if (IS_NOT_NULL(waitingEvent))
		{
			if (waitingEvent->Kind() == eventKind)
			{
				contentEvent = waitingEvent;
				break;
			}
		}
	}

	if (IS_NULL(contentEvent))
	{
		for (auto runningEvent : m_runningEvents)
		{
			if (IS_NOT_NULL(runningEvent))
			{
				if (runningEvent->Kind() == eventKind)
				{
					contentEvent = runningEvent;
					break;
				}
			}
		}
	}

	if (IS_NOT_NULL(contentEvent))
	{
		const auto& eventInfo = BASE::CONTENTS_EVENT_INFO_DATA.find(contentEvent->Kind());
		if (eventInfo == BASE::CONTENTS_EVENT_INFO_DATA.end())
		{
			SPDLOG_ERROR("[Send_DB_CONTENT_EVENT_GET_REQ]eventInfo NULL!! UserID : {}, Kind : {}", contentEvent->Kind());
			return;
		}

		NEW_FLATBUFFER(DB_CONTENT_EVENT_GET_REQ, pPACKET);
		pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto // FlatBuffers 의 offset 을 리턴한다.
		{
			return PROTOCOL::FLATBUFFERS::CreateDB_CONTENT_EVENT_GET_REQ(fbb, GLOBAL::GS_INFO.SVID, pUser->UID(), contentEvent->Kind(), 0, GetDueDay_UTC(0));
		});
		SEND_DBA(pPACKET);
	}
}

void CEventManager::Send_GS_CONTENT_EVENT_DETAIL_INFO_ACK(const PROTOCOL::FLATBUFFERS::DB_CONTENT_EVENT_GET_ACK* pAck)
{
	CUser* pUser = CUserManager::Instance()->FindByUID(pAck->UserID());
	if (IS_NULL(pUser))
	{
		return;
	}
	auto eventInfo = pAck->EventCountInfo();
	if (IS_NULL(eventInfo))
	{
		return;
	}

	if (IS_ACTIVE_USER(pUser))
	{
		NEW_FLATBUFFER(GS_SEASON_EVENT_DETAIL_INFO_ACK, pPACKET);
		pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto // FlatBuffers 의 offset 을 리턴한다.
		{
			const auto& contentEvent = BASE::CONTENTS_EVENT_INFO_DATA.find(eventInfo->EventKind());
			if (contentEvent != BASE::CONTENTS_EVENT_INFO_DATA.end())
			{
				INT32 currentCount = eventInfo->CurrentCount();
				if (currentCount > contentEvent->second->ResultCount)
					currentCount = contentEvent->second->ResultCount;

				std::wstring strLocalizeText = GetText(pUser->GetLanguageType(), contentEvent->second->Popup_Description);
				if (!strLocalizeText.empty() && IsOccupyCityEvent(contentEvent->second->Kind) == false)
				{
					if (eventInfo->IsComplete())
					{
						strLocalizeText = GetText(pUser->GetLanguageType(), contentEvent->second->Popup_DescCompleted);
					}
					else
					{
						STRING numberBuffer = STRING(to_wstring(currentCount));
						UTIL::ReplaceAll(strLocalizeText, _T("{0}"), numberBuffer);
					}
				}
				return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_DETAIL_INFO_ACK(fbb,
					true,
					contentEvent->second->Kind,
					CREATE_FLATBUFFER_RAWSTR(GetText(pUser->GetLanguageType(), contentEvent->second->Title)),
					CREATE_FLATBUFFER_RAWSTR(contentEvent->second->Popup_IMG),
					CREATE_FLATBUFFER_RAWSTR(strLocalizeText.c_str())
				);
			}
			else
				return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_DETAIL_INFO_ACK(fbb, false, 0, 0, 0, 0);

		});
		SEND_ACTIVE_USER(pUser, pPACKET);
	}

}






void	CEventManager::Send_DB_DAILY_EVENT_WEEK_SET_REQ()
{
	/*NEW_FLATBUFFER(DB_DAILY_EVENT_WEEK_SET_REQ, pPACKET);
	pPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto{
		return PROTOCOL::FLATBUFFERS::CreateDB_DAILY_EVENT_WEEK_SET_REQ(fbb
			, GLOBAL::GS_INFO.SVID, m_DailyEventWeeks);
	});
	SEND_DBA(pPACKET);*/

	QueryPacker packer;

	const auto query = std::make_shared<QP_T_DAILY_EVENT_WEEK_SET>();

	query->m_nServerID = GLOBAL::GS_INFO.SVID;
	query->m_nWeeks = m_DailyEventWeeks;

	packer.Add_Server(query);

	packer << [query, DailyEventWeeks = m_DailyEventWeeks]()
	{
		if (false == query->IsOK())
		{
			SPDLOG_ERROR("excute query fail : {}", query->GetErrCode());
			return;
		}

		const auto weeks = query->m_nWeeks;

		int result = 10;

		if (0 < query->GetSET_1().size())
		{
			auto& setInfo = query->GetSET_1().front();
			result = setInfo.m_nResult;
		}

		//값은 선처리되었다. 
		if (IS_FAILED(result))
		{
			//실패했을 경우에는 모니터링툴에 에러 로그를 찍어준다. 
			NxC_Agent::Get()->SendEventLogFormat(EVENT_LOG_ERROR_OCCUR, L"Recv_DB_DAILY_EVENT_WEEK_SET_ACK failed : %d, serverWeek : %d, packetWeek : %d"
				, result, DailyEventWeeks, weeks);
			TLNET_LOG(boost::log::trivial::severity_level::debug, L"Recv_DB_DAILY_EVENT_WEEK_SET_ACK failed : %d, serverWeek : %d, packetWeek : %d"
				, result, DailyEventWeeks, weeks);
		}
	};

	packer.Request();
}

//void	CEventManager::Recv_DB_DAILY_EVENT_WEEK_SET_ACK(const PROTOCOL::FLATBUFFERS::DB_DAILY_EVENT_WEEK_SET_ACK* pAck)
//{
//	if (nullptr == pAck)
//		return;
//
//	//값은 선처리되었다. 
//	if (IS_FAILED(pAck->Result()))
//	{
//		//실패했을 경우에는 모니터링툴에 에러 로그를 찍어준다. 
//		NxC_Agent::Get()->SendEventLogFormat(EVENT_LOG_ERROR_OCCUR, L"Recv_DB_DAILY_EVENT_WEEK_SET_ACK failed : %d, serverWeek : %d, packetWeek : %d"
//			, pAck->Result(), m_DailyEventWeeks, pAck->Weeks());
//		TLNET_LOG(boost::log::trivial::severity_level::debug, L"Recv_DB_DAILY_EVENT_WEEK_SET_ACK failed : %d, serverWeek : %d, packetWeek : %d"
//			, pAck->Result(), m_DailyEventWeeks, pAck->Weeks());
//	}
//}