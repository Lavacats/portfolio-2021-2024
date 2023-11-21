#include "GameAfx.h"
#include "ContentsEventManager.h"

#include "_DatabaseThreadManager/DBAGlobal.h"
#include "_DatabaseThreadManager/DBQUERY/GameDB.h"
#include "_DatabaseThreadManager/DBQUERY/GameDBRef.h"
#include "OBJECTS/VipManager.h"
#include "OBJECTS/Territory/TerritoryBuilding.h"
#include "GrowthEventManager.h"
#include "ContentsEvent/Util-ContentsEvent.h"

//using namespace ContentsEvent;

ContentsEventManager::ContentsEventManager()
{
	
}

ContentsEventManager::~ContentsEventManager()
{
	for (auto iter = ContentsEventUsers.begin(); iter != ContentsEventUsers.end(); ++iter)
	{
		SAFE_DELETE(iter->second);
	}
	ContentsEventUsers.clear();

	Clear();
}

void ContentsEventManager::Clear()
{
	for (auto iter = ContentsEventRewards.begin(); iter != ContentsEventRewards.end(); ++iter)
	{
		SAFE_DELETE(iter->second);
	}
	ContentsEventRewards.clear();

	ItemKinds.clear();
}

int ContentsEventManager::GetItemKind(int packageKind)
{
	const auto packageIter = ItemKinds.find(packageKind);
	if (packageIter != ItemKinds.end())
		return packageIter->second;

	return -1;
}

//ContentsEventUser* ContentsEventManager::UpdateContentsEventUser(INT64 userId, const std::vector<INT32>& normalRewards, const std::vector<INT32>& premiumRewards, const std::unordered_map<int, int> killCounts)
//{
//	auto contentsEventUser = GetContentsEventUser(userId);
//
//	if (IS_NULL(contentsEventUser))
//	{
//		contentsEventUser = new ContentsEventUser();
//		contentsEventUser->UserId = userId;
//
//		ContentsEventUsers.emplace(userId, contentsEventUser);
//	}
//	else
//	{
//		contentsEventUser->UserId = userId;
//		contentsEventUser->normalRewardKinds.clear();
//		contentsEventUser->premiumRewardKinds.clear();
//		contentsEventUser->killCounts.clear();
//	}
//
//	// 일반보상 획득 정보.
//	for (flatbuffers::uoffset_t i = 0; i < normalRewards.size(); ++i)
//		contentsEventUser->normalRewardKinds.emplace(normalRewards[i], normalRewards[i]);
//
//	// 프리미엄보상 획득 정보.
//	for (flatbuffers::uoffset_t i = 0; i < premiumRewards.size(); ++i)
//		contentsEventUser->premiumRewardKinds.emplace(premiumRewards[i], premiumRewards[i]);
//
//	// 야만인 처치 카운트.
//	for (auto iter = killCounts.begin(); iter != killCounts.end(); ++iter)
//	{
//		auto contentsEventKind = iter->first;
//		auto find = contentsEventUser->killCounts.find(contentsEventKind);
//		if (find == contentsEventUser->killCounts.end())
//		{
//			auto killCount = iter->second;
//			contentsEventUser->killCounts.emplace(contentsEventKind, killCount);
//		}
//	}
//
//	return contentsEventUser;
//}

void ContentsEventManager::CreateContentsEventUser(INT64 userId)
{
	auto contentsEventUser = GetContentsEventUser(userId);
	if (IS_NULL(contentsEventUser))
	{
		contentsEventUser = new ContentsEventUser();
		contentsEventUser->UserId = userId;
		ContentsEventUsers.emplace(userId, contentsEventUser);
	}
}

ContentsEventUser* ContentsEventManager::UpdateChloeEventUser(INT64 userId, const std::vector<INT32>& chloeRewards)
{
	auto contentsEventUser = GetContentsEventUser(userId);

	if (IS_NULL(contentsEventUser))
	{
		contentsEventUser = new ContentsEventUser();
		contentsEventUser->UserId = userId;

		ContentsEventUsers.emplace(userId, contentsEventUser);
	}
	//else
	//{
	//	contentsEventUser->UserId = userId;
	//	contentsEventUser->normalRewardKinds.clear();
	//	contentsEventUser->premiumRewardKinds.clear();
	//	contentsEventUser->killCounts.clear();
	//}

	// 일반보상 획득 정보.
	for (flatbuffers::uoffset_t i = 0; i < chloeRewards.size(); ++i)
		contentsEventUser->normalRewardKinds.emplace(chloeRewards[i], chloeRewards[i]);

	return contentsEventUser;
}

ContentsEventUser* ContentsEventManager::GetContentsEventUser(INT64 userId)
{
	const auto find = ContentsEventUsers.find(userId);

	if (find != ContentsEventUsers.end())
		return find->second;

	return nullptr;
}

ContentsEventReward* ContentsEventManager::GetRewardInfo(int contentsEventKind)
{
	const auto find = ContentsEventRewards.find(contentsEventKind);

	if (find == ContentsEventRewards.end())
		return nullptr;

	return find->second;
}


SRESULT ContentsEventManager::LoadContentsEvent(NDataReader & data, INT32& i32DataCount)
{
	Clear();

	i32DataCount = 0;
	NDT_LOOP2(data, _T("Table"))
	{
		int index = 0;
		
		int contentsEventKind = 0;
		const NDataReader::Row& row = data.GetCurrentRow();
		ContentsEventReward* contentsEventReward = nullptr;

		row.GetColumn(index++, contentsEventKind);

		auto rewardIter = ContentsEventRewards.find(contentsEventKind);
		if (rewardIter == ContentsEventRewards.end())
		{
			contentsEventReward = new ContentsEventReward();
			if (IS_NULL(contentsEventReward))
				return SERVER_FAIL;

			contentsEventReward->ContentsEventKind = contentsEventKind;
			ContentsEventRewards.emplace(contentsEventKind, contentsEventReward);
			++i32DataCount;
		}
		else
		{
			return SERVER_FAIL;
		}

		row.GetColumn(index++, contentsEventReward->ContentsEventGroup);
		row.GetColumn(index++, contentsEventReward->PackageKind);
		row.GetColumn(index++, contentsEventReward->ItemKind);

		auto packageIter = ItemKinds.find(contentsEventReward->PackageKind);
		if (packageIter == ItemKinds.end())
		{
			ItemKinds.emplace(contentsEventReward->PackageKind, contentsEventReward->ItemKind);
		}

		row.GetColumn(index++, contentsEventReward->ConditionType);
		row.GetColumn(index++, contentsEventReward->TargetValue_1);
		row.GetColumn(index++, contentsEventReward->TargetValue_2);
		row.GetColumn(index++, contentsEventReward->TargetValue_3);

		// reward item
		std::wstring strRewardItem;
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 0);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 0);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 0);

		// premium reward item
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);
		row.GetColumn(index++, strRewardItem);
		InnerLoadContentsEvent(contentsEventReward, strRewardItem, 1);

		row.GetColumn(index++, contentsEventReward->TextMailTitle); //index++; // REWARD_MAIL_TITLE
		row.GetColumn(index++, contentsEventReward->TextMailDesc); //index++; // REWARD_MAIL_DESC
		index++; // CORE_REWARD_ENABLE
		index++; // CORE_REWARD_TITLE
		index++; // CORE_REWARD_DESC
		index++; // EVENT_DESC

		// 야만인 처치하기 카인드 목록.
		//if (contentsEventReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::MONSTER_KILL)
		//	MonsterKillKinds.push_back(contentsEventReward->ContentsEventKind);
	}
	NDT_LOOP_END;

	return SERVER_OK;
}

// rewardFlag: 0(normal reward), 1(premium reward)
void ContentsEventManager::InnerLoadContentsEvent(ContentsEventReward* contentsEventReward, wstring& strItems, INT32 rewardFlag)
{
	std::vector<wstring> strDest;
	GLOBAL::Tokenize(strItems, strDest, L"(:)");
	if (strDest.size() == 2)
	{
		if (rewardFlag == 0)
			contentsEventReward->NormalRewardItems.emplace_back(BASE::REWARDITEM(_ttoi(strDest[0].c_str()), _ttoi(strDest[1].c_str())));
		else
			contentsEventReward->PremiumRewardItems.emplace_back(BASE::REWARDITEM(_ttoi(strDest[0].c_str()), _ttoi(strDest[1].c_str())));
	}
}

void ContentsEventManager::Purchase(const INT64 userId, const int packageKind)
{
	const auto itemKind = GetItemKind(packageKind);
	if (0 > itemKind)
	{
		TLNET_LOG(boost::log::trivial::severity_level::warning,
			"[ContentsEventManager::Purchase()] Fail! UserID = %d, PackageKind = %d, Result = %d", userId, packageKind, ContentsEventResult::RESULT_INVALID_PACKAGE);
		return;
	}

	auto user = CUserManager::Instance()->FindByUID(userId);
	if (IS_NOT_NULL(user))
	{
		GLOBAL::SendLog(userId, 0, DB_LOG::REASON_GROWTH_FUND_PURCHASE, 
			0, 0, { packageKind, user->GetCastleLevel(), user->Get_PayingLv(), user->VipManager()->GetVipLevel() }, {});

		const auto svrNum = user->GetInventory()->GetItemNum(itemKind);
		
		user->GiveItems(userId, itemKind, 1, svrNum, DB_LOG::REASON_PURCHASE_ITEM_GET);
	}
}



//void ContentsEventManager::OnUserLogin(INT64 userId)
//{
//	//if (false == GLOBAL::IsContentStateOn(GLOBAL::eCONTENT_STATE_ID::CONTENT_GROWTH_FUND))
//	//	return;
//	auto game_user = CUserManager::Instance()->Seek(userId);
//	if (game_user != nullptr)
//	{	
//		// ack를 보냅니다.
//		NEW_FLATBUFFER(GS_CONTENTS_EVENT_LIST_GET_ACK, pPacket);
//		pPacket.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
//		{
//			std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::CONTENTS_EVENT_LIST>> flatContentsEventList;
//
//			auto contents_event = ContentsEventManager::Instance()->GetContentsEventUser(userId);
//			if (contents_event != nullptr)
//			{
//				for (auto& chloe : contents_event->normalRewardKinds)
//				{
//					flatContentsEventList.emplace_back(PROTOCOL::FLATBUFFERS::CreateCONTENTS_EVENT_LIST(fbb
//						, chloe.first
//						, contents_event->IsRewarded_Normal(chloe.first)
//						, 0
//						, 0));
//				}
//			}
//
//			auto growh_event = GrowthEventManager::Instance()->GetGrowthEventUser(userId);
//			if (growh_event != nullptr)
//			{
//				for (auto& normal : growh_event->m_normalRewardKinds)
//				{
//					flatContentsEventList.emplace_back(PROTOCOL::FLATBUFFERS::CreateCONTENTS_EVENT_LIST(fbb
//						, normal.first
//						, growh_event->IsRewarded_Normal(normal.first)
//						, growh_event->IsRewarded_Premium(normal.first)
//						, growh_event->GetStateValue(normal.first)));
//				}
//			}
//
//			return PROTOCOL::FLATBUFFERS::CreateGS_CONTENTS_EVENT_LIST_GET_ACK(fbb,
//				0,
//				userId,
//				fbb.CreateVector(flatContentsEventList));
//		});
//
//		SEND_ACTIVE_USER(game_user, pPacket);
//	}
//}

void ContentsEventManager::OnPurchase(INT64 userId, int packageKind)
{
	//if (false == GLOBAL::IsContentStateOn(GLOBAL::eCONTENT_STATE_ID::CONTENT_GROWTH_FUND))
	//	return;

	auto contentsEventUser = GetContentsEventUser(userId);
	if (contentsEventUser == nullptr)
		return;

	const auto contentsEventResult = contentsEventUser->IsPurchasable(packageKind);
	if (ContentsEventResult::RESULT_INVALID_PACKAGE == contentsEventResult)
		return;

	if (ContentsEventResult::RESULT_OK != contentsEventResult)
	{
		TLNET_LOG(boost::log::trivial::severity_level::warning,
			"[ContentsEventManager::Purchase()] Fail! UserID = %d, PackageKind = %d, Result = %d", userId, packageKind, contentsEventResult);
		return;
	}

	Purchase(userId, packageKind);
}

// rewardFlag: 0(normal reward), 1(premium reward)
void ContentsEventManager::SendMailContentsEventReward(int32_t serverId, INT64 userId, const int contentsEventKind, INT32 rewardFlag)
{
	ContentsEventReward* pReward = GetRewardInfo(contentsEventKind);

	if (IS_NULL(pReward))
	{
		TLNET_LOG(boost::log::trivial::severity_level::warning, "[ContentsEventManager::SendMail_ContentsEventReward()] Fail! pReward is Null. UserID = %d, contentsEventKind = %d", userId, contentsEventKind);
		return;
	}

	std::vector<BASE::REWARDITEM> listReward;
	GAME::eMAIL_CATEGORY eMailCategory = GAME::eMAIL_CATEGORY::NOTICE;

	if (rewardFlag == 0)
	{
		for (const auto& rewardItem : pReward->NormalRewardItems)
		{
			const auto itemInfo = BASE::GET_ITEM_DATA(rewardItem.Kind);
			if (itemInfo != nullptr)
				listReward.push_back(rewardItem);
		}
		eMailCategory = GAME::eMAIL_CATEGORY::NOTICE;
	}
	else
	{
		for (const auto& rewardItem : pReward->PremiumRewardItems)
		{
			const auto itemInfo = BASE::GET_ITEM_DATA(rewardItem.Kind);
			if (itemInfo != nullptr)
				listReward.push_back(rewardItem);
		}
		eMailCategory = GAME::eMAIL_CATEGORY::IMPORTANT;
	}

	if (listReward.size() > 0)
	{
		auto param_value = 0;
		if (pReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::MASTERPIECE_CREATE)
			param_value = pReward->TargetValue_1;
		else if (pReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::BUILDING_LEVELUP)
			param_value = pReward->TargetValue_2;
		else if (pReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::MONSTER_KILL)
			param_value = pReward->TargetValue_1;
		else
			param_value = pReward->TargetValue_1;

		CMailManager::Instance()->SendContentsEventRewardMail(
			userId,
			_T(""),
			GAME::eMAIL_TYPE::MAIL_TYPE_CONTENTS_EVENT_REWARD,
			eMailCategory,
			pReward->TextMailTitle,
			pReward->TextMailDesc,
			param_value,
			listReward);
	}
}

// 보상 정보 갱신 및 보상지급.
bool ContentsEventManager::SetReward(int32_t serverId, INT64 userId, int contentsEventKind, INT32 rewardFlag)
{
	const auto contentsEventUser = GetContentsEventUser(userId);
	const auto contentsEventReward = GetRewardInfo(contentsEventKind);
	if (contentsEventUser != nullptr && contentsEventReward != nullptr)
	{
		return contentsEventUser->SetReward(serverId, userId, contentsEventReward, rewardFlag);
	}

	return false;
}

bool ContentsEventManager::IsRewardable(INT64 userId, int contentsEventKind, INT32 rewardFlag)
{
	const auto contentsEventUser = GetContentsEventUser(userId);
	const auto contentsEventReward = GetRewardInfo(contentsEventKind);
	if (contentsEventUser != nullptr && contentsEventReward != nullptr)
	{
		return (contentsEventUser->IsRewardable(contentsEventReward, rewardFlag) == ContentsEventResult::RESULT_OK);
	}

	return false;
}

// 야만인 처치수 증가.
//void ContentsEventManager::IncreaseMonsterKillCount(INT64 userId, int monsterLevel)
//{
//	const auto contentsEventUser = GetContentsEventUser(userId);
//	if (contentsEventUser == nullptr)
//		return;
//
//	// 처치수 증가 가능한것만 증가시킵니다.
//	// 증가시 목표수치보다 넘지 않게 체크하여 처리됩니다.
//	for (flatbuffers::uoffset_t i = 0; i < MonsterKillKinds.size(); ++i)
//	{
//		const auto contentsEventInfo = GetRewardInfo(MonsterKillKinds[i]);
//		if (contentsEventInfo == nullptr)
//			continue;
//
//		// 보상을 모두 완료했으면 스킵.
//		if (contentsEventUser->IsRewarded_All(contentsEventInfo->ContentsEventKind) == true)
//			continue;
//
//		// 진행중인 이벤트중 목표 야만인 레벨을 체크하여 타겟 레벨과 같은 경우에만 카운팅 됩니다.
//		if (monsterLevel == contentsEventInfo->TargetValue_1)
//		{
//			auto bIncrease = contentsEventUser->IncreaseMonsterKillCount(contentsEventInfo);
//
//			if (bIncrease)
//			{
//				// DB 갱신.
//				auto pUser = CUserManager::Instance()->FindByUID(userId);
//				if (pUser != nullptr)
//				{
//					Util::ContentsEvent::SetRewardGrowthContentsEvent(pUser->GetSharedPtr(),
//						contentsEventInfo->ContentsEventKind,
//						false,
//						false,
//						contentsEventUser->GetMonsterkillCount(contentsEventInfo->ContentsEventKind));
//				}
//			}
//		}
//	}
//}

// {{ 클로이의 비밀 스토리 이벤트.
bool ContentsEventManager::IsRewardable_ChloeStoryEvent(INT64 userId, int seasonKind)
{
	const auto contentsEventUser = GetContentsEventUser(userId);
	if (contentsEventUser != nullptr)
	{
		return (contentsEventUser->IsRewardable_ChloeStoryEvent(seasonKind) == ContentsEventResult::RESULT_OK);
	}

	return false;
}

bool ContentsEventManager::SetReward_ChloeStoryEvent(int32_t serverId, INT64 userId, int seasonKind)
{
	const auto contentsEventUser = GetContentsEventUser(userId);
	if (contentsEventUser != nullptr)
	{
		return contentsEventUser->SetReward_ChloeStoryEvent(serverId, userId, seasonKind);
	}

	return false;
}
// }} 클로이의 비밀 스토리 이벤트.

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ContentsEventUser

// 보상 정보 갱신 및 보상지급.
// rewardFlag: 0(normal reward), 1(premium reward)
// retrue value: ture(보상 지급됨), false(보상 지급되지 않음)
bool ContentsEventUser::SetReward(int32_t serverId, INT64 userId, ContentsEventReward* contentsEventReward, INT32 rewardFlag)
{
	if (IsRewardable(contentsEventReward, rewardFlag) != ContentsEventResult::RESULT_OK)
		return false;

	if (rewardFlag == 0)
		normalRewardKinds.emplace(contentsEventReward->ContentsEventKind, contentsEventReward->ContentsEventKind);
	else
		premiumRewardKinds.emplace(contentsEventReward->ContentsEventKind, contentsEventReward->ContentsEventKind);

	// 보상 지급.
	CUser* user = CUserManager::Instance()->FindByUID(userId);
	if (user == nullptr)
		return false;

	if (contentsEventReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::VEILED_LADY)
	{
		// 인밴토리로 지급.
		if (rewardFlag == 0)
		{
			for (const auto reward : contentsEventReward->NormalRewardItems)
				user->GiveItems(userId, reward.Kind, reward.Count, user->GetInventory()->GetItemNum(reward.Kind), 0);
		}
		else
		{
			for (const auto reward : contentsEventReward->PremiumRewardItems)
				user->GiveItems(userId, reward.Kind, reward.Count, user->GetInventory()->GetItemNum(reward.Kind), 0);
		}
		//user->GetInventory()->UpdateRows();
	}
	else
	{
		// 메일로 지급.
		ContentsEventManager::Instance()->SendMailContentsEventReward(serverId, userId, contentsEventReward->ContentsEventKind, rewardFlag);
	}

	return true;
}

// rewardFlag: 0(normal reward), 1(premium reward)
ContentsEventResult ContentsEventUser::IsRewardable(ContentsEventReward* contentsEventReward, INT32 rewardFlag)
{
	//컨텐츠 온오프
	//if (false == GLOBAL::IsContentStateOn(GLOBAL::eCONTENT_STATE_ID::CONTENT_GROWTH_FUND))
	//	return ContentsEventResult::RESULT_CONTENTS_OFF;

	if (nullptr == contentsEventReward)
		return ContentsEventResult::RESULT_INVALID_REWARD;

	//보상 중복 체크
	if (rewardFlag == 0)
	{
		auto find_normalReward = normalRewardKinds.find(contentsEventReward->ContentsEventKind);
		if (find_normalReward != normalRewardKinds.end())
			return ContentsEventResult::RESULT_ALREADY_REWARD;

		//보상 내역 체크
		if (0 == contentsEventReward->NormalRewardItems.size())
			return ContentsEventResult::RESULT_INVALID_REWARD;
	}
	else
	{
		auto find_premiumReward = premiumRewardKinds.find(contentsEventReward->ContentsEventKind);
		if (find_premiumReward != premiumRewardKinds.end())
			return ContentsEventResult::RESULT_ALREADY_REWARD;

		//보상 내역 체크
		if (0 == contentsEventReward->PremiumRewardItems.size())
			return ContentsEventResult::RESULT_INVALID_REWARD;
	}

	return IsContentsEventCompleted(contentsEventReward);
}


ContentsEventResult ContentsEventUser::IsPurchasable(int packageKind)
{
	//if (false == GLOBAL::IsContentStateOn(GLOBAL::eCONTENT_STATE_ID::CONTENT_GROWTH_FUND))
	//	return ContentsEventResult::RESULT_CONTENTS_OFF;

	CUser* pUser = CUserManager::Instance()->FindByUID(UserId);

	if (IS_NULL(pUser))
		return ContentsEventResult::RESULT_NULL_USER;

	int itemKind = ContentsEventManager::Instance()->GetItemKind(packageKind);

	if (0 > itemKind)
		return ContentsEventResult::RESULT_INVALID_PACKAGE;

	if (0 != pUser->GetInventory()->GetItemNum(itemKind))
		return ContentsEventResult::RESULT_ALREADY_PURCHASE;

	return ContentsEventResult::RESULT_OK;
}

// 컨텐츠 이밴트를 완료하였는가?
ContentsEventResult ContentsEventUser::IsContentsEventCompleted(ContentsEventReward* contentsEventReward)
{
	auto user = CUserManager::Instance()->GetByUID(UserId);
	if (IS_NULL(user))
		return ContentsEventResult::RESULT_NULL_USER;

	// 컨텐츠 이밴트 컨디션 체크.
	// 1:걸작보유하기, 2:건물업그래이드, 3:야만인처치하기.
	if (contentsEventReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::MASTERPIECE_CREATE)
	{
		auto targetCount = contentsEventReward->TargetValue_1;
		auto haveCount = user->Territory().GetMasterpieces().size();
		if (haveCount >= targetCount)
			return ContentsEventResult::RESULT_OK;
	}
	else if (contentsEventReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::BUILDING_LEVELUP)
	{
		//auto castle = user->Territory().GetBuildFromKind(GAME::KIND_BUILD_COMMAND);
		auto building = user->Territory().GetBuildFromKind(contentsEventReward->TargetValue_1);
		if (nullptr == building)
			return ContentsEventResult::RESULT_BUILDING_INFO_NOT;

		auto targetLevel = contentsEventReward->TargetValue_2;
		auto curLevel = building->GetBuildLevel();
		if (curLevel >= targetLevel)
			return ContentsEventResult::RESULT_OK;
	}
	else if (contentsEventReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::MONSTER_KILL)
	{
		auto find = killCounts.find(contentsEventReward->ContentsEventKind);
		if (find != killCounts.end())
		{
			auto targetCount = contentsEventReward->TargetValue_2;
			auto curCount = find->second;
			if (curCount >= targetCount)
				return ContentsEventResult::RESULT_OK;
		}
	}
	else if (contentsEventReward->ConditionType == GAME::eCONTENTS_EVENT_CONDITION_TYPE::VEILED_LADY)
	{
		if (1 == contentsEventReward->TargetValue_1)
		{
			// 걸작 체크.
			auto masterpieceKind = contentsEventReward->TargetValue_2;
			if (nullptr != user->Territory().GetHaveMasterpiece(masterpieceKind))
				return ContentsEventResult::RESULT_OK;
		}
		else if (2 == contentsEventReward->TargetValue_1)
		{
			// 연구 체크.
			auto researchKind = contentsEventReward->TargetValue_2;
			if (true == user->GetLaboratory()->ContainsSkill(researchKind, 1))
				return ContentsEventResult::RESULT_OK;
		}
		else
		{
			return ContentsEventResult::RESULT_EVENT_FAILED;
		}
	}
	else
	{
		return ContentsEventResult::RESULT_EVENT_FAILED;
	}

	return ContentsEventResult::RESULT_EVENT_FAILED;
}

// 야만인 처치수 갱신.
//bool ContentsEventUser::IncreaseMonsterKillCount(ContentsEventReward* contentsEventReward)
//{
//	auto increaseCount = GetMonsterkillCount(contentsEventReward->ContentsEventKind) + 1;
//	auto targetCount = contentsEventReward->TargetValue_2;
//
//	auto bIncrease = (increaseCount <= targetCount);
//
//	if (bIncrease)
//		SetMonsterkillCount(contentsEventReward->ContentsEventKind, increaseCount);
//
//	return bIncrease;
//}
//
//int ContentsEventUser::GetMonsterkillCount(int contentsEventKind)
//{
//	auto curCount = 0;
//	auto find = killCounts.find(contentsEventKind);
//	if (find != killCounts.end())
//		curCount = find->second;
//
//	return curCount;
//}
//
//void ContentsEventUser::SetMonsterkillCount(int contentsEventKind, int monsterKillCount)
//{
//	auto find = killCounts.find(contentsEventKind);
//	if (find != killCounts.end())
//		find->second = monsterKillCount;
//	else
//		killCounts.emplace(contentsEventKind, monsterKillCount);
//}

// 보상 획득 유/무.
bool ContentsEventUser::IsRewarded_Normal(int contentsEventKind)
{
	auto find = normalRewardKinds.find(contentsEventKind);
	if (find != normalRewardKinds.end())
		return true;
	return false;
}

// 보상 획득 유/무.
bool ContentsEventUser::IsRewarded_Premium(int contentsEventKind)
{
	auto find = premiumRewardKinds.find(contentsEventKind);
	if (find != premiumRewardKinds.end())
		return true;
	return false;
}

// 보상 획득 유/무.
bool ContentsEventUser::IsRewarded_All(int contentsEventKind)
{
	return IsRewarded_Normal(contentsEventKind) && IsRewarded_Premium(contentsEventKind);
}

// {{ 클로이의 비밀 스토리 이벤트.
// 클로이의 비밀 스토리 이벤트를 완료하였는가?
ContentsEventResult ContentsEventUser::IsChleoStoryEventCompleted(int seasonKind)
{
	auto user = CUserManager::Instance()->GetByUID(UserId);
// 	if (IS_NULL(user))
// 		return ContentsEventResult::RESULT_NULL_USER;
// 
// 	auto kind = seasonKind > 10000 ? seasonKind - 10000 : seasonKind;
// 	auto pSeasonInfo = BASE::CHLOE_EVENT_INFO_DATA.find(kind);
// 	if (pSeasonInfo == BASE::CHLOE_EVENT_INFO_DATA.end())
// 		return ContentsEventResult::RESULT_INVALID_REWARD;
// 
// 	for (int i = 0; i < pSeasonInfo->second->size(); ++i)
// 	{
// // 		if (nullptr == pSeasonInfo->second->at(i))
// // 			continue;
// // 
// // 		const auto& info = pSeasonInfo->second->at(i).get();
// // 
// // 		if (info == nullptr || info->MissionKind <= 0)
// // 			continue;
// 
// // 		if (1 == info->MissionTargetValue_1)
// // 		{
// // 			// 걸작 체크.
// // 			auto masterpieceKind = info->MissionTargetValue_2;
// // 			if (nullptr == user->Territory().GetHaveMasterpiece(masterpieceKind))
// // 				return ContentsEventResult::RESULT_EVENT_FAILED;
// // 		}
// // 		else if (2 == info->MissionTargetValue_1)
// // 		{
// // 			// 연구 체크.
// // 			auto researchKind = info->MissionTargetValue_2;
// // 			if (false == user->GetLaboratory()->ContainsSkill(researchKind, 1))
// // 				return ContentsEventResult::RESULT_EVENT_FAILED;
// // 		}
// // 		else
// // 		{
// // 		}
// 	}

	return ContentsEventResult::RESULT_OK;
}

// rewardFlag: 0(normal reward), 1(premium reward)
ContentsEventResult ContentsEventUser::IsRewardable_ChloeStoryEvent(int seasonKind)
{
	//컨텐츠 온오프
	//if (false == GLOBAL::IsContentStateOn(GLOBAL::eCONTENT_STATE_ID::CONTENT_GROWTH_FUND))
	//	return ContentsEventResult::RESULT_CONTENTS_OFF;

	//보상 중복 체크
	auto find_normalReward = normalRewardKinds.find(seasonKind);
	if (find_normalReward != normalRewardKinds.end())
		return ContentsEventResult::RESULT_ALREADY_REWARD;

	return IsChleoStoryEventCompleted(seasonKind);
}

bool ContentsEventUser::SetReward_ChloeStoryEvent(int32_t serverId, INT64 userId, int seasonKind)
{

	return false;
}
// }} 클로이의 비밀 스토리 이벤트.
