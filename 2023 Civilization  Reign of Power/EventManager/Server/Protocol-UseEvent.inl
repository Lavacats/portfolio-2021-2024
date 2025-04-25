#pragma once
#include "Contents/UseEvent/Util-UseEvent.h"

namespace Protocol::UseEvent
{
	inline void OnRecv_GS_USE_EVENT_INFO_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		auto [error_code, req] = VERIFY_PROTOCOL(GS_USE_EVENT_INFO_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		if (req == nullptr)
			return;

		int groupKind = req->EventGroupKind();

		ASE_INSTANCE(game_user, CUserUseEventManager)->SendUserUseEventInfoClientACK(game_user.get(), groupKind);
	}

	inline void OnRecv_GS_USE_EVENT_REWARD_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		auto [error_code, req] = VERIFY_PROTOCOL(GS_USE_EVENT_REWARD_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		if (req == nullptr)
			return;

		int eventKind	= req->EventKind();
 		int pointGrade	= req->PointGrade();
		int groupKind	= req->EventGroupKind();

		ASE_INSTANCE(game_user, CUserUseEventManager)->SendUserUseEventRewardClientACK(game_user.get(), eventKind, groupKind, pointGrade);
	}


	// 초기 등록 단계
	inline void InitMessage()
	{
		CCmdTarget::Instance()->Insert(PROTOCOL::ID::GS_USE_EVENT_INFO_REQ, OnRecv_GS_USE_EVENT_INFO_REQ);
		CCmdTarget::Instance()->Insert(PROTOCOL::ID::GS_USE_EVENT_REWARD_REQ, OnRecv_GS_USE_EVENT_REWARD_REQ);
	}
}
