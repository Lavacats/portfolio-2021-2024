#pragma once

namespace Util::SeasonEvent
{
	// 시즌이벤트 DB 로드
	void SetData_Season_Event(const TLDB::CQuery::SharedPtr& query);  

	// 시즌이벤트 DB 업데이트
	void SendToDB_Season_EVENT_USER_INFO_Update(const INT64 userID, INT32 curSeason, INT32	seasonEventKind,
		INT32 seasonMEventMission_1_Kind, INT32 seasonMEventMission_1_Value, 
		INT32 seasonMEventMission_2_Kind, INT32 seasonMEventMission_2_Value, 
		INT32 seasonMEventMission_3_Kind, INT32 seasonMEventMission_3_Value,
		INT32 completeMission, INT64 rewardTime, const QueryPackLinker::SharedPtr& load_balancer = nullptr);

	// 시즌이벤트 서버이동 대응
	void Export_SeasonEvent(const UserSharedPtr& game_user, const EnumUserContents& content, const Int32& server_id_transfer, const bool& is_reset, Actin::ByteBuffer& byte_buffer);
	void Import_SeasonEvent(const UserSharedPtr& game_user, const EnumUserContents& content, const bool& is_reset, const Int64& time_offset, Actin::ByteBuffer& byte_buffer, const QueryPackLinker::SharedPtr& load_balancer);

	// 시즌이벤트 서버이동 삭제 대응
	void Delete_UserSeasonEventData(INT64 userID);
}

