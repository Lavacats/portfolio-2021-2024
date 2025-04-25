#include "GameAfx.h"
#include "UserUseEventManager.h"
#include "Util-UseEvent.h"
#include "Item/Util-Item.h"
#include "Season/SeasonManager.h"

#include "UseEventManager.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_USE_EVENT_INFO_UPDATE.h"
#include "_DatabaseThreadManager/DBQUERY/GDB/P_T_USE_EVENT_REWARD_UPDATE.h"

CUserUseEventManager::CUserUseEventManager()
{

}

CUserUseEventManager::~CUserUseEventManager()
{
	m_repo_UseEvent_Info.clear();
	m_repo_UseEvent_Reward.clear();
}

auto CUserUseEventManager::Seek_Info(const INT64 groupKind) ->CUserUseEventInfo::SharedPtr
{
	const auto iter_s = m_repo_UseEvent_Info.find(groupKind);
	const auto iter_e = m_repo_UseEvent_Info.end();

	if (iter_s == iter_e)
		return nullptr;

	return iter_s->second->useEvent_Info;
}

auto CUserUseEventManager::Seek_Reward(const INT64 eventKind, const INT64 groupKind) ->CUserUseEventReward::SharedPtr
{
	const auto iter_s = m_repo_UseEvent_Reward.find(make_pair(eventKind,groupKind));
	const auto iter_e = m_repo_UseEvent_Reward.end();

	if (iter_s == iter_e)
		return nullptr;

	return iter_s->second->useEvent_Reward;
}

void CUserUseEventManager::Insert_Info( const Int64 groupKind, const Int64 eventPoint)
{
	const auto iter_s = m_repo_UseEvent_Info.find(groupKind);
	const auto iter_e = m_repo_UseEvent_Info.end();

	if (iter_s != iter_e)
		return;

	UseEventInfo::SharedPtr eventInfo =  std::make_shared<UseEventInfo>();
	const auto useEventInfo = std::make_shared<CUserUseEventInfo>();
	const auto useEventInfoData = BASE::GET_USE_EVENT_INFO_DATA(groupKind);
	if (useEventInfoData == nullptr)
		return;
	// 데이터 확인
	useEventInfo->SetGroupKind(groupKind);
	useEventInfo->SetEventPoint(eventPoint);
	useEventInfo->SetMinLv(useEventInfoData->LvMin);
	useEventInfo->SetMaxLV(useEventInfoData->LvMax);
	useEventInfo->SetStartTime(useEventInfoData->useEventStartTime);
	useEventInfo->SetEndTime(useEventInfoData->useEventEndTime);

	eventInfo->useEvent_Info = useEventInfo;

	// 이터레이터 설정
	eventInfo->iter_1 = m_repo_UseEvent_Info.emplace(groupKind, eventInfo).first;
}

void CUserUseEventManager::Insert_Reward(const Int64 eventKind, const Int64 pointStep)
{
	const auto iter_s = BASE::USE_EVENT_TOTAL_INFO_DATA.find(eventKind);
	const auto iter_e = BASE::USE_EVENT_TOTAL_INFO_DATA.end();

	if (iter_s == iter_e)
		return;

	int eGroupKind = BASE::USE_EVENT_TOTAL_INFO_DATA[eventKind]->groupKind;

	const auto m_iter_s = m_repo_UseEvent_Reward.find(make_pair(eventKind, eGroupKind));
	const auto m_iter_e = m_repo_UseEvent_Reward.end();

	if (m_iter_s != m_iter_e)
		return;

	const auto useEventReward = std::make_shared<CUserUseEventReward>();
	UseEventReward::SharedPtr eventReward = std::make_shared<UseEventReward>();

	useEventReward->SetEventKind(eventKind);
	useEventReward->SetEventRewardStep(pointStep);
	useEventReward->SetGroupKind(eGroupKind);

	eventReward->useEvent_Reward = useEventReward;

	// 이터레이터 설정
	eventReward->iter_1 = m_repo_UseEvent_Reward.emplace(make_pair(eventKind, eGroupKind), eventReward).first;
}

void CUserUseEventManager::OnQuestEvent(int iConditionKind, int iConditionValue, int iTargetValue, Quest::SETTYPE setType)
{
	if (iConditionValue < 0 || iConditionKind <= 0)
		return;

	if (iConditionKind == GAME::eEVENTCONDITION_TYPE::USE_ITEM_GEM && iConditionValue == 1)
	{
		if (Util::GOT::IsBlockedContentsByUserCountry(GetUser()->UID(), EnumContentBlock::BLOCK_EVENT_USE))
			return;
	}
	// 현재시간에 해당 미션 condition 의 group이 있는지 확인
	std::vector<INT64> vec_GroupKind = GetGroupKindEvent_Condition(iConditionKind, iConditionValue);

	for (auto iGroupKind : vec_GroupKind)
	{
		if (iGroupKind == 0)
			return;

		if(!CUseEventManager::Instance()->IsDurationUseEvent(iGroupKind))
			continue;

		// 이벤트가 있는 경우 미션 처리 진행
		auto mEventInfo = Seek_Info(iGroupKind);

		if (mEventInfo == nullptr)
			return;

	
		
		// 포인트 계산
		INT64 prePoint = mEventInfo->GetEventPoint();
		INT64 eventPoint = prePoint + iTargetValue;
		mEventInfo->SetEventPoint(eventPoint);

		// DB정보 갱신
		Util::UseEvent::SendToDB_UseEventInfo_Update(GetUser(), iGroupKind, eventPoint);

		// 클라이언트 포인트 정보 갱신
		SendUserUseEventClientNFY(iGroupKind);

		// 로그 추가
		Int64 iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
		GLOBAL::SendLog(GetUser()->UID(), 0, DB_LOG::REASON_USE_EVENT_POINT, 0, 0, {
			iCurSeason,
			iConditionKind,
			prePoint,
			iTargetValue,
			eventPoint
			}
		, { });

	}
}

void CUserUseEventManager::SetEmpty()
{
	/// <summary>
	/// 데이터 초기화 함수
	/// </summary>
	m_repo_UseEvent_Info.clear();
	m_repo_UseEvent_Reward.clear();
}

void CUserUseEventManager::SetUpUseEventUser()
{
	for (auto iter = BASE::USE_EVENT_INFO_DATA.begin(); iter != BASE::USE_EVENT_INFO_DATA.end(); ++iter)
	{
		int groupKind = iter->second->groupKind;

		if (Seek_Info(groupKind) == nullptr)
		{
			Insert_Info(groupKind, 0);
			// DB 세팅
			Util::UseEvent::SendToDB_UseEventInfo_Update(GetUser(), groupKind, 0);
		}
	}
}

void CUserUseEventManager::SendUserUseEventInfoClientACK(CUser* pUser, int groupKind)
{
	auto sendError = [&](RESULT::eRESULT err) -> void
	{
		NEW_FLATBUFFER(GS_USE_EVENT_INFO_ACK, pFAILPACKET);
		pFAILPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
		{
			return  PROTOCOL::FLATBUFFERS::CreateGS_USE_EVENT_INFO_ACK(fbb, static_cast<int>(err), 0);
		});
		SEND_ACTIVE_USER(pUser, pFAILPACKET);
	};

	if (IS_NULL(pUser))
		sendError(RESULT::R_FAIL);

	if (m_repo_UseEvent_Info.find(groupKind) == m_repo_UseEvent_Info.end())
		sendError(RESULT::R_FAIL);

	if (!CUseEventManager::Instance()->IsDurationUseEvent(groupKind))
		sendError(RESULT::R_FAIL);

	const auto useEventInfo = BASE::GET_USE_EVENT_INFO_DATA(groupKind);
	if (useEventInfo == nullptr)
		sendError(RESULT::R_FAIL);

	NEW_FLATBUFFER(GS_USE_EVENT_INFO_ACK, packet);
	packet.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::USE_EVENT_INFO>> useEventList;

		auto eventInfo = Seek_Info(groupKind);
		int m_chronicle = 0;
		long m_startTime = 0;
		long m_endTime = 0;

		
		//보상 정보 확인
		std::vector<int> rewardPointGradeList;

		for (auto rewardInfo : useEventInfo->eventRewardInfoList)
		{
			int evetnKind = rewardInfo->eventKind;

			auto rewardInfo = Seek_Reward(evetnKind, groupKind);

			if (rewardInfo != nullptr)
			{
				m_chronicle = useEventInfo->eventChronicle;
				m_startTime = useEventInfo->useEventStartTime;
				m_endTime = useEventInfo->useEventEndTime;

				rewardPointGradeList.emplace_back(rewardInfo->GetEventRewardStep());
			}
		}


		int m_chronicleTime = (0 == m_chronicle ? 0 : GLOBAL::CHRONICLE_MANAGER.GetChronicleOpenTime((EChronicleType)m_chronicle));
		long eventStartTime = m_chronicleTime + m_startTime;
		long eventEndTime = m_chronicleTime + m_endTime;

		useEventList.emplace_back(PROTOCOL::FLATBUFFERS::CreateUSE_EVENT_INFO(
			fbb,
			eventInfo->GetGroupKind(),
			eventInfo->GetEventPoint(),
			fbb.CreateVector(rewardPointGradeList),
			eventStartTime,
			eventEndTime
			)
		);

		return PROTOCOL::FLATBUFFERS::CreateGS_USE_EVENT_INFO_ACK(fbb, RESULT::R_OK,
			fbb.CreateVector(useEventList));
	});
	SEND_ACTIVE_USER(pUser, packet);
}

void CUserUseEventManager::SendUserUseEventRewardClientACK(CUser* pUser, int eventKind, int groupKind, int poingGrade)
{
	auto sendError = [&](RESULT::eRESULT err) -> void
	{
		NEW_FLATBUFFER(GS_USE_EVENT_REWARD_ACK, pFAILPACKET);
		pFAILPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
		{
			return  PROTOCOL::FLATBUFFERS::CreateGS_USE_EVENT_REWARD_ACK(fbb, static_cast<int>(err), 0, 0, 0);
		});
		SEND_ACTIVE_USER(pUser, pFAILPACKET);
	};

	auto userUserEvent = Seek_Info(groupKind);

	if (IS_NULL(pUser))
		sendError(RESULT::R_FAIL);

	if (userUserEvent == nullptr)
		sendError(RESULT::R_FAIL);

	// 유저 포인트 검증 과정 
	int userEventpoint = userUserEvent->GetEventPoint();
	if (userEventpoint < poingGrade)
		sendError(RESULT::R_FAIL);

	// kind/group 이 이미 존재하는 값인지 검사 < repo에는 이미 수령한 값만 담긴다 >
	if (m_repo_UseEvent_Reward.find(make_pair(eventKind,groupKind)) != m_repo_UseEvent_Reward.end())
		sendError(RESULT::R_FAIL);

	// 현재 진행중인 이벤트 인지 확인
	if (!CUseEventManager::Instance()->IsDurationUseEvent( groupKind))
		sendError(RESULT::R_FAIL);


	int m_groupIndex = 0;
	Int64 iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	const auto useEventInfo = BASE::GET_USE_EVENT_INFO_DATA(groupKind);
	if (useEventInfo == nullptr)
		return;
	for (auto rewardInfo : useEventInfo->eventRewardInfoList)
	{
		m_groupIndex++;

		if (eventKind != rewardInfo->eventKind)
			continue;

		if (!rewardInfo->rewardItem_1.IsEmpty())
		{
			int preItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->rewardItem_1.Kind);

			Util::ItemManager::AddItem(pUser->GetSharedPtr(), rewardInfo->rewardItem_1.Kind, rewardInfo->rewardItem_1.Count, GLOBAL::EnumContentsGroup::Reward_UseEvent, useEventInfo->eventCondition);

			int curItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->rewardItem_1.Kind);

			// 로그 추가
			GLOBAL::SendLog(GetUser()->UID(), 0, DB_LOG::REASON_USE_EVENT_REWARD, 0, 0, {
				iCurSeason,
				useEventInfo->eventCondition,
				m_groupIndex,
				rewardInfo->rewardItem_1.Kind,
				preItemCount,
				rewardInfo->rewardItem_1.Count,
				curItemCount
				}
			, { });
		}
		if (!rewardInfo->rewardItem_2.IsEmpty())
		{
			int preItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->rewardItem_2.Kind);

			Util::ItemManager::AddItem(pUser->GetSharedPtr(), rewardInfo->rewardItem_2.Kind, rewardInfo->rewardItem_2.Count, GLOBAL::EnumContentsGroup::Reward_UseEvent, useEventInfo->eventCondition);

			int curItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->rewardItem_2.Kind);

			// 로그 추가
			GLOBAL::SendLog(GetUser()->UID(), 0, DB_LOG::REASON_USE_EVENT_REWARD, 0, 0, {
				iCurSeason,
				useEventInfo->eventCondition,
				m_groupIndex,
				rewardInfo->rewardItem_2.Kind,
				preItemCount,
				rewardInfo->rewardItem_2.Count,
				curItemCount
				}
			, { });
		}
		if (!rewardInfo->rewardItem_3.IsEmpty())
		{
			int preItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->rewardItem_3.Kind);

			Util::ItemManager::AddItem(pUser->GetSharedPtr(), rewardInfo->rewardItem_3.Kind, rewardInfo->rewardItem_3.Count, GLOBAL::EnumContentsGroup::Reward_UseEvent, useEventInfo ->eventCondition);

			int curItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->rewardItem_3.Kind);

			// 로그 추가
			GLOBAL::SendLog(GetUser()->UID(), 0, DB_LOG::REASON_USE_EVENT_REWARD, 0, 0, {
				iCurSeason,
				useEventInfo->eventCondition,
				m_groupIndex,
				rewardInfo->rewardItem_3.Kind,
				preItemCount,
				rewardInfo->rewardItem_3.Count,
				curItemCount
				}
			, { });
		}
		Insert_Reward(eventKind, poingGrade);
		Util::UseEvent::SendToDB_UseEventReward_Update(GetUser(), eventKind, poingGrade);
	}

	NEW_FLATBUFFER(GS_USE_EVENT_REWARD_ACK, packet);

	packet.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
	{
		return PROTOCOL::FLATBUFFERS::CreateGS_USE_EVENT_REWARD_ACK(fbb,
			RESULT::R_OK,
			eventKind,
			poingGrade,
			groupKind
		);
	});
	SEND_ACTIVE_USER(pUser, packet);
}

void CUserUseEventManager::SendUserUseEventClientNFY( int eventGroupKind)
{
	NEW_FLATBUFFER(GS_USE_EVENT_NFY, packet);
	packet.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
	{
		int eventPoint = 0;
		auto eventInfo = Seek_Info(eventGroupKind);

		if (eventInfo != nullptr)
			eventPoint = eventInfo->GetEventPoint();

		return PROTOCOL::FLATBUFFERS::CreateGS_USE_EVENT_NFY(fbb,
			eventPoint,
			eventGroupKind
		);
	});
	SEND_ACTIVE_USER(GetUser(), packet);
}

void CUserUseEventManager::SendUserUseFinishEventReward(int eventGroupKind)
{
	auto seekUseEventInfo = Seek_Info(eventGroupKind);
	if (seekUseEventInfo == nullptr)return;

	const auto useEventInfo = BASE::GET_USE_EVENT_INFO_DATA(eventGroupKind);
	if (useEventInfo == nullptr)return;

	std::vector<PROTOCOL::ITEM_INFO> rewardItemList;

	for (auto rewardInfo : useEventInfo->eventRewardInfoList)
	{
		if (rewardInfo->i64PointGrade > seekUseEventInfo->GetEventPoint())
			continue;

		if (Seek_Reward(rewardInfo->eventKind, eventGroupKind) != nullptr)
			continue;

		// 해당 포인트 보상 수령 내용 서버/DB 추가
		Insert_Reward(rewardInfo->eventKind, rewardInfo->i64PointGrade);
		Util::UseEvent::SendToDB_UseEventReward_Update(GetUser(), rewardInfo->eventKind, rewardInfo->i64PointGrade);

		// 보상 리스트 
		if (!rewardInfo->rewardItem_1.IsEmpty())
			rewardItemList.emplace_back(PROTOCOL::ITEM_INFO(rewardInfo->rewardItem_1.Kind, rewardInfo->rewardItem_1.Count));

		if (!rewardInfo->rewardItem_2.IsEmpty())
			rewardItemList.emplace_back(PROTOCOL::ITEM_INFO(rewardInfo->rewardItem_2.Kind, rewardInfo->rewardItem_2.Count));

		if (!rewardInfo->rewardItem_3.IsEmpty())
			rewardItemList.emplace_back(PROTOCOL::ITEM_INFO(rewardInfo->rewardItem_3.Kind, rewardInfo->rewardItem_3.Count));
	}

	if (rewardItemList.size() > 0)
	{
		CMailManager::Instance()->SendImportantMail(GAME::eMAIL_TYPE::MAIL_TYPE_EVENT_USE_NOT_RECEIVE
			, GetUser()->UID()
			, _T("")
			, GetUser()->GetLordName()
			, _T("COMMON_USE_REWARD_MAIL_TITLE")
			, _T("COMMON_USE_REWARD_MAIL_DESC")
			, _T("-")
			, GLOBAL::EnumContentsGroup::Reward_UseEvent
			, useEventInfo->eventCondition
			, rewardItemList
		);
	}
}

std::vector<INT64> CUserUseEventManager::GetGroupKindEvent_Condition(int iConditionKind, int iConditionValue)
{
	std::vector<INT64> groupEventList;
	Int64 iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	INT64 curTimeUTC = GetDueDay_UTC(0);

	for (auto iter = BASE::USE_EVENT_INFO_DATA.begin(); iter != BASE::USE_EVENT_INFO_DATA.end(); ++iter)
	{
		int m_chronicle			= iter->second->eventChronicle;
		INT64 m_chronicleTime	= (0 == m_chronicle ? 0 : GLOBAL::CHRONICLE_MANAGER.GetChronicleOpenTime((EChronicleType)m_chronicle));
		INT64 m_startTime		= m_chronicleTime + iter->second->useEventStartTime;
		INT64 m_endTime			= m_chronicleTime + iter->second->useEventEndTime;

		// Info 검사시 연대기 검사하려면 여기서 추가
		if (iCurSeason != iter->second->eventSeason)
		{
			if(iter->second->eventSeason !=0)
				continue;
		}
		if (iConditionKind != iter->second->eventCondition)
			continue;

		if(iConditionValue != iter->second->eventConditionValue)
			continue;

		if (curTimeUTC < m_startTime)
			continue;

		if (curTimeUTC > m_endTime)
			continue;

		groupEventList.emplace_back(iter->second->groupKind);
	}
	return groupEventList;
}

bool CUserUseEventManager::IsMissedEventReward(int groupKind)
{
	bool isLeftReward = false;
	auto userUseEvent = Seek_Info(groupKind);
	const auto useEventInfo = BASE::GET_USE_EVENT_INFO_DATA(groupKind);

	if (userUseEvent == nullptr)
		return false;

	if (useEventInfo == nullptr)
		return false;

	for (auto rewardInfo : useEventInfo->eventRewardInfoList)
	{
		if (rewardInfo->i64PointGrade > userUseEvent->GetEventPoint())
			continue;

		if (Seek_Reward(rewardInfo->eventKind, groupKind) != nullptr)
			continue;

		isLeftReward = true;
	}
	return isLeftReward;
}