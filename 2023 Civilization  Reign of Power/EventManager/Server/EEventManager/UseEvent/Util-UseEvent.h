#pragma once

namespace Util::UseEvent
{
	// DB �ε�
	void SetData_UseEventUserEventInfo(const TLDB::CQuery::SharedPtr& query);
	void SetData_UseEventUserEventReward(const TLDB::CQuery::SharedPtr& query);

	/// <summary>
	/// Fetch_Server, Fetch_User�� ȣȯ�Ѵ�.
	/// ��, ���� ������ �����Ϳ�����.
	/// QP_T_SEASONPASS_USER_FETCH_USER, QP_T_SEASONPASS_USER_FETCH_SERVER
	/// </summary>

	void SendToDB_UseEventInfo_Fetch_User(const UserSharedPtr& game_user, const QueryPackLinker::SharedPtr& load_balancer = nullptr);
	void SendToDB_UseEventInfo_Update(const UserSharedPtr& game_user, const INT64 eventKind, const INT64 eventPoint, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // ������Ʈ
	void SendToDB_UseEventInfo_InitDelete(const UserSharedPtr& game_user, const INT64 eventKind, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // ����

	void SendToDB_UseEventReward_Fetch_User(const UserSharedPtr& game_user, const QueryPackLinker::SharedPtr& load_balancer = nullptr);
	void SendToDB_UseEventReward_Update(const UserSharedPtr& game_user, const INT64 eventKind, const INT64 eventStep, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // ������Ʈ
	void SendToDB_UseEventReward_InitDelete(const UserSharedPtr& game_user, const INT64 eventKind, const QueryPackLinker::SharedPtr& load_balancer = nullptr); // ����


	void SendUserUseEventInfoClientLoginNFY(const UserSharedPtr& game_User);

}