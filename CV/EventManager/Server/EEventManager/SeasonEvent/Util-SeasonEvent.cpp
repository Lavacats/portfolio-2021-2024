#include "GameAfx.h"
#include "Util-SeasonEvent.h"
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_Season_Info_Fetch_Server.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_Season_Info_Update.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_Season_Point_Fetch_Server.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_Season_Rank_Fetch_Server.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_Season_Point_Update.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_SEASON_RANK_UPDATE.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_USER_SEASON_FETCH_SERVER.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_USER_SEASON_FETCH_USER.h>
#include <_DatabaseThreadManager/DBQUERY/GDB/P_T_USER_SEASON_UPDATE.h>

#include "SeasonEventManager.h"
#include "Server/Util-ServerList.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_SEASON_POINT_LIMIT_FETCH_SERVER.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_SEASON_POINT_LIMIT_UPDATE.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_SEASON_VICOTRY_HISTORY_FETCH_SERVER.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_SEASON_VICOTRY_HISTORY_UPDATE.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_USER_SEASON_VICTORY_REWARD_FETCH_SERVER.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_USER_SEASON_VICTORY_REWARD_UPDATE.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_USER_SEASON_VICTORY_REWARD_FETCH_USER.h"

void Util::SeasonEvent::SetData_Season_Event(const TLDB::CQuery::SharedPtr& query)
{
	if (const auto query_server = std::dynamic_pointer_cast<QP_T_SEASON_EVENT_FETCH_SERVER>(query))
	{
		for (const auto& row : query_server->GetSET_1())
		{
			CSeasonEventManager::Instance()->LoadToDB_Season_Event(
				row.m_nUserID,						// userId
				row.m_nSeason,						// season
				row.m_nSeasonEventKind,				// seasonMissionKind
				row.m_nMission__Kind[0],			// seasonEvent_1_kind
				row.m_nMission__Value[0],			// seasonEvent_1_value
				row.m_nMission__Kind[1],			// seasonEvent_2_kind
				row.m_nMission__Value[1],			// seasonEvent_2_value
				row.m_nMission__Kind[2],			// seasonEvent_3_kind
				row.m_nMission__Value[2],			// seasonEvent_3_value
				row.m_nCompleteMission,
				GLOBAL::TIME_DB2UTC(row.m_tmRewardTime)
			);
		}
	}
	else if (const auto query_server = std::dynamic_pointer_cast<QP_T_SEASON_EVENT_FETCH_USER>(query))
	{
		for (const auto& row : query_server->GetSET_1())
		{
			CSeasonEventManager::Instance()->LoadToDB_Season_Event(
				row.m_nUserID,						// userId
				row.m_nSeason,						// season
				row.m_nSeasonEventKind,				// seasonMissionKind
				row.m_nMission__Kind[0],			// seasonEvent_1_kind
				row.m_nMission__Value[0],			// seasonEvent_1_value
				row.m_nMission__Kind[1],			// seasonEvent_2_kind
				row.m_nMission__Value[1],			// seasonEvent_2_value
				row.m_nMission__Kind[2],			// seasonEvent_3_kind
				row.m_nMission__Value[2],			// seasonEvent_3_value
				row.m_nCompleteMission,
				GLOBAL::TIME_DB2UTC(row.m_tmRewardTime)
			);
		}
	}
	else
	{
		// 다른 타입이 들어올경우
		assert(false);
	}
}

void Util::SeasonEvent::SendToDB_Season_EVENT_USER_INFO_Update(const INT64 userID, INT32 curSeason, INT32 seasonEventKind, INT32 seasonMEventMission_1_Kind, INT32 seasonMEventMission_1_Value, INT32 seasonMEventMission_2_Kind, INT32 seasonMEventMission_2_Value, INT32 seasonMEventMission_3_Kind, INT32 seasonMEventMission_3_Value, INT32 completeMission, INT64 rewardTime, const QueryPackLinker::SharedPtr& load_balancer /*= nullptr*/)
{
	const auto query = std::make_shared<QP_T_SEASON_EVENT_UPDATE>();
	query->m_nServerID = GLOBAL::GS_INFO.SVID;
	query->m_nUserID = userID;
	query->m_nSeason = curSeason;
	query->m_nSeasonEventKind = seasonEventKind;
	query->m_nMission__Kind[0] = seasonMEventMission_1_Kind;
	query->m_nMission__Value[0] = seasonMEventMission_1_Value;
	query->m_nMission__Kind[1] = seasonMEventMission_2_Kind;
	query->m_nMission__Value[1] = seasonMEventMission_2_Value;
	query->m_nMission__Kind[2] = seasonMEventMission_3_Kind;
	query->m_nMission__Value[2] = seasonMEventMission_3_Value;
	query->m_nCompleteMission = completeMission;
	query->m_tmRewardTime = GLOBAL::TIME_UTC2DB(rewardTime); 

	QueryPacker packer(load_balancer);
	packer.Add_Server(query->m_nServerID, query);
	packer.Request();

}

void Util::SeasonEvent::Export_SeasonEvent(const UserSharedPtr& game_user, const EnumUserContents& content, const Int32& server_id_transfer, const bool& is_reset, Actin::ByteBuffer& byte_buffer)
{
	if (nullptr == game_user)
	{
		LOGGER_INFO(CONST_TRANSFER_LOG, "Export_SeasonEvent User is nullptr");
		return;
	}

	if (content == EnumUserContents::SeasonEvent)
	{
		auto query = std::make_shared<QP_T_SEASON_EVENT_FETCH_USER>();

		int64_t userID = game_user->UID();

		for (auto iter = BASE::SEASON_EVENT_DATA.begin(); iter != BASE::SEASON_EVENT_DATA.end(); ++iter)
		{
			int season = iter->second->CurSeason;
			int seasonEventKind = iter->second->SeasonEventKind;
			int completeMission = 0;
			std::vector<int> missionKind;
			std::vector<int> missionValue;

			INT64 rewardTime =0;
			bool findMission = false;

			//미션 진행 기록이 있는지 확인

			INT64 seasonEventValue_1 = CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_1);
			INT64 seasonEventValue_2 = CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_2);
			INT64 seasonEventValue_3 = CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_3);

			if (seasonEventValue_1 != 0)
				findMission = true;
			if (seasonEventValue_2 != 0)
				findMission = true;
			if (seasonEventValue_3 != 0)
				findMission = true;

			if (!findMission)continue;

			// 보상데이터가 있는지 확인

			completeMission = CSeasonEventManager::Instance()->GetSeasonEventRewardState(userID, iter->second->SeasonEventKind);
			rewardTime = CSeasonEventManager::Instance()->GetSeasonEventRewardTime(userID, iter->second->SeasonEventKind);;

			missionKind.emplace_back(iter->second->Mission_Kind_1);
			missionKind.emplace_back(iter->second->Mission_Kind_2);
			missionKind.emplace_back(iter->second->Mission_Kind_3);

			missionValue.emplace_back(seasonEventValue_1);
			missionValue.emplace_back(seasonEventValue_2);
			missionValue.emplace_back(seasonEventValue_3);

			query->AddData_SET_1(
				::GetServerID(),
				game_user->UID(),
				season,
				seasonEventKind,
				missionKind,
				missionValue,
				completeMission,
				rewardTime		
			);
		}
		byte_buffer << query;
	}
}

void Util::SeasonEvent::Import_SeasonEvent(const UserSharedPtr& game_user, const EnumUserContents& content, const bool& is_reset, const Int64& time_offset, Actin::ByteBuffer& byte_buffer, const QueryPackLinker::SharedPtr& load_balancer)
{
	if (nullptr == game_user)
	{
		LOGGER_INFO(CONST_TRANSFER_LOG, "Export_SeasonEvent User is nullptr");
		return;
	}

	QueryPacker packer(load_balancer);

	if (content == EnumUserContents::SeasonEvent)
	{
		auto query = std::make_shared<QP_T_SEASON_EVENT_FETCH_USER>();
		byte_buffer >> query;

		query->m_nServerID = ::GetServerID();
		query->m_nUserID = game_user->UID();

		SetData_Season_Event(query);

		int64_t userID = game_user->UID();

		for (auto iter = BASE::SEASON_EVENT_DATA.begin(); iter != BASE::SEASON_EVENT_DATA.end(); ++iter)
		{
			int season = iter->second->CurSeason;
			int seasonEventKind = iter->second->SeasonEventKind;

			INT64 seasonEventValue_1 = CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_1);
			INT64 seasonEventValue_2 = CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_2);
			INT64 seasonEventValue_3 = CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_3);

			INT64 rewardTime = 0;
			int completeMission = 0;

			//미션 진행 기록이 있는지 확인
			bool findMission = false;
			if (CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_1) != 0)
				findMission = true;
			if (CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_2) != 0)
				findMission = true;
			if (CSeasonEventManager::Instance()->GetSeasonEventValue(userID, iter->second->Mission_Kind_3) != 0)
				findMission = true;

			if (!findMission)continue;

			// 보상데이터가 있는지 확인
			completeMission = CSeasonEventManager::Instance()->GetSeasonEventRewardState(userID, iter->second->SeasonEventKind);
			rewardTime = CSeasonEventManager::Instance()->GetSeasonEventRewardTime(userID, iter->second->SeasonEventKind);;

			Util::SeasonEvent::SendToDB_Season_EVENT_USER_INFO_Update(
				userID,
				season,
				seasonEventKind,
				iter->second->Mission_Kind_1, seasonEventValue_1,
				iter->second->Mission_Kind_2, seasonEventValue_2,
				iter->second->Mission_Kind_3, seasonEventValue_3,
				completeMission,
				rewardTime,
				packer);
		}
	}

	packer.Request();
}

void Util::SeasonEvent::Delete_UserSeasonEventData(INT64 userID)
{
	for (auto iter = BASE::SEASON_EVENT_DATA.begin(); iter != BASE::SEASON_EVENT_DATA.end(); ++iter)
	{
		CSeasonEventManager::Instance()->DeleteSeasonEvent_Info(userID, iter->second->Mission_Kind_1);
		CSeasonEventManager::Instance()->DeleteSeasonEvent_Info(userID, iter->second->Mission_Kind_2);
		CSeasonEventManager::Instance()->DeleteSeasonEvent_Info(userID, iter->second->Mission_Kind_3);
		CSeasonEventManager::Instance()->DeleteSeasonEvent_Reward(userID);
	}
}


