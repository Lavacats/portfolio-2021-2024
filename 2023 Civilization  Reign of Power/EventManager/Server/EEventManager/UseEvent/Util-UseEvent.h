#pragma once

namespace Util::UseEvent
{
	// DB 로드
	void SetData_UseEventUserEventInfo(const TLDB::CQuery::SharedPtr& query);
	void SetData_UseEventUserEventReward(const TLDB::CQuery::SharedPtr& query);

	/// <summary>
	/// Fetch_Server, Fetch_User를 호환한다.
	/// 단, 같은 형식의 데이터여야함.
	/// QP_T_SEASONPASS_USER_FETCH_USER, QP_T_SEASONPASS_USER_FETCH_SERVER
	/// </summary>

	void SendToDB_UseEventInfo_Fetch_User(const UserSharedPtr& game_user, const QueryPackLinker::SharedPtr& load_balancer = nullptr);
	void SendToDB_UseEventInfo_Update(const UserSharedPtr& game_user, const INT64 eventKind, const INT64 eventPoint, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // 업데이트
	void SendToDB_UseEventInfo_InitDelete(const UserSharedPtr& game_user, const INT64 eventKind, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // 삭제

	void SendToDB_UseEventReward_Fetch_User(const UserSharedPtr& game_user, const QueryPackLinker::SharedPtr& load_balancer = nullptr);
	void SendToDB_UseEventReward_Update(const UserSharedPtr& game_user, const INT64 eventKind, const INT64 eventStep, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // 업데이트
	void SendToDB_UseEventReward_InitDelete(const UserSharedPtr& game_user, const INT64 eventKind, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // 삭제


	void SendUserUseEventInfoClientLoginNFY(const UserSharedPtr& game_User);

}