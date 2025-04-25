#include "GameAfx.h"
#include "SeasonEventManager.h"

#include "Item/Util-Item.h"
#include "OBJECTS/GrowthEventManager.h"
#include "Server/Util-ServerList.h"
#include "_DatabaseThreadManager/DBQUERY/MASTERDB/P_SEASONNO_UPDATE.h"
#include "SeasonEvent\LifeCycle_SeasonEvent.h"
#include "Systems/ServerInitializer/ServerInitializer.h"
#include "Util-SeasonEvent.h"
#include "Season/SeasonManager.h"

struct CSeasonEventManager::SeasonEventMissionData
{
public:
	INT64 UserId = 0;
	INT32 SeasonEventMissionKind = 0;
	INT32 MissionValue = 0;						// �̼� ��밪
public:
	SeasonEventMissionData()
	{
		UserId = 0;
		SeasonEventMissionKind = 0;
		MissionValue = 0;
	}
};

struct CSeasonEventManager::SeasonEventRewardData
{
public:
	INT64 UserId = 0;
	INT32 SeasonEventKind = 0;
	INT32 completeMissionValue = 0;						// �̼� ��밪
	INT32 itemKind = 0;
	INT32 itemCount = 0;
	INT64 rewardTime = 0;
public:
	SeasonEventRewardData()
	{
		UserId = 0;
		SeasonEventKind = 0;

	}
};

void CSeasonEventManager::SendUserSeasonEventInfoACK(int64_t userID, int showUI, int curSeason, int curPeriod, std::vector < std::shared_ptr<SeasonEventRewardData >> rewardData, std::vector < std::shared_ptr<SeasonEventMissionData >>  missionData)
{
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return;

	NEW_FLATBUFFER(GS_SEASON_EVENT_INFO_ACK, packet);
	packet.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_REWARD_INFO>> flatSeasonEventRewardList;
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_MISSION_INFO>> flatSeasonEventInfoList;

		if (rewardData.size() > 0)
		{
			for (int i = 0; i < rewardData.size(); i++)
			{
				if (rewardData[i]->SeasonEventKind != 0)
				{
					if (repo_SeasonEventReward.find(userID) == repo_SeasonEventReward.end())
						continue;

					std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::ITEM>>  flatItems;

					for (auto rewardItem : repo_SeasonEventReward[userID])
					{
						flatItems.push_back(PROTOCOL::FLATBUFFERS::CreateITEM(fbb, rewardItem->itemKind, rewardItem->itemCount, 0, 0, 0, 0));
					}
					
					flatSeasonEventRewardList.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_REWARD_INFO
					(
						fbb,
						rewardData[i]->UserId,
						rewardData[i]->completeMissionValue,
						rewardData[i]->SeasonEventKind,
						fbb.CreateVector(flatItems)
					));
				}
			}
		}

		if (missionData.size() > 0)
		{
			for (int i = 0; i < missionData.size(); i++)
			{
				if (missionData[i]->SeasonEventMissionKind != 0)
				{
					flatSeasonEventInfoList.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_MISSION_INFO
					(
						fbb,
						missionData[i]->UserId,
						missionData[i]->SeasonEventMissionKind,
						missionData[i]->MissionValue
					));
				}
			}
		}

		return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_INFO_ACK(fbb,
			showUI,
			curSeason,
			curPeriod,
			fbb.CreateVector(flatSeasonEventRewardList),
			fbb.CreateVector(flatSeasonEventInfoList));
	});
	SEND_ACTIVE_USER(pUser, packet);
}

void CSeasonEventManager::SendUserSeasonEventInfoNFY(int64_t userID, int curSeason, int curPeriod, std::vector < std::shared_ptr<SeasonEventRewardData >>rewardData, std::vector < std::shared_ptr<SeasonEventMissionData >>  missionData)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;

	NEW_FLATBUFFER(GS_SEASON_EVENT_INFO_NFY, packet);
	packet.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_REWARD_INFO>> flatSeasonEventRewardList;
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_MISSION_INFO>> flatSeasonEventInfoList;

		if (rewardData.size() > 0)
		{
			for (int i = 0; i < rewardData.size(); i++)
			{
				if (rewardData[i]->SeasonEventKind != 0)
				{
					if (repo_SeasonEventReward.find(userID) == repo_SeasonEventReward.end())
						continue;

					std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::ITEM>>  flatItems;
					
					for (auto rewarditem : repo_SeasonEventReward[userID])
					{
						flatItems.push_back(PROTOCOL::FLATBUFFERS::CreateITEM(fbb, rewarditem->itemKind, rewarditem->itemCount, 0, 0, 0, 0));
					}
					flatSeasonEventRewardList.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_REWARD_INFO
					(
						fbb,
						rewardData[i]->UserId,
						rewardData[i]->completeMissionValue,
						rewardData[i]->SeasonEventKind,
						fbb.CreateVector(flatItems)
					));
				}
			}
		}

		if (missionData.size() > 0)
		{
			for (int i = 0; i < missionData.size(); i++)
			{
				if (missionData[i]->SeasonEventMissionKind != 0)
				{
					flatSeasonEventInfoList.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_MISSION_INFO
					(
						fbb,
						missionData[i]->UserId,
						missionData[i]->SeasonEventMissionKind,
						missionData[i]->MissionValue
					));
				}
			}
		}

		return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_INFO_NFY(fbb,
			curSeason,
			curPeriod,
			fbb.CreateVector(flatSeasonEventRewardList),
			fbb.CreateVector(flatSeasonEventInfoList));
	});
	SEND_ACTIVE_USER(pUser, packet);
}

int CSeasonEventManager::GetCurPeriod()
{
	// ���� ������
	const INT64 serverBirthTime = CGlobalGameServerListManager::Instance()->GetMyServer()->GetBirthDay();

	// ���� ������ ���� UTC 00 ��
	const INT64 seasonStartTime = (serverBirthTime + SECOND_PER_DAY) - ((serverBirthTime + SECOND_PER_DAY) % SECOND_PER_DAY);

	const INT64 onePeriodPerDay = PERIOD_DURATION * SECOND_PER_DAY;

	const INT64 curTime = GetDueDay_UTC(0);

	const INT64 firstPeriodEndTime = seasonStartTime + onePeriodPerDay;
	const INT64 secondPeriodEndTime = firstPeriodEndTime + onePeriodPerDay;

	if (curTime < firstPeriodEndTime)
		return SeasonEventPeriodState::Period_First;
	if (firstPeriodEndTime <= curTime && curTime < secondPeriodEndTime)
		return SeasonEventPeriodState::Period_second;
	if(secondPeriodEndTime<=curTime)
		return SeasonEventPeriodState::End_Season;

	return -1;
}

int CSeasonEventManager::GetPreviosPeriod(int period)
{
	switch ((SeasonEventPeriodState)period)
	{
	case SeasonEventPeriodState::Period_second:
		return SeasonEventPeriodState::Period_First;
	case SeasonEventPeriodState::End_Season:
		return SeasonEventPeriodState::Period_second;
	}

	return SeasonEventPeriodState::Not_Open_Season;
}

void CSeasonEventManager::Update()
{
	ProvideMissedReward();
}

INT64 CSeasonEventManager::GetSeasonEventValue(int64_t userID, int seasonEventMissionKind)
{
	int eventValue = 0;
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return eventValue;

	auto keyVlaue = make_pair(userID, seasonEventMissionKind);

	if (repo_SeasonEvent.find(keyVlaue) != repo_SeasonEvent.end())
		eventValue = repo_SeasonEvent[keyVlaue]->MissionValue;

	return eventValue;
}

INT32 CSeasonEventManager::GetSeasonEventRewardState(int64_t userID, int seasonEventKind)
{
	int rewardValue = 0;
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return rewardValue;

	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardItem : repo_SeasonEventReward[userID])
		{
			if (rewardItem->SeasonEventKind == seasonEventKind)
			{
				rewardValue = rewardItem->completeMissionValue;
				break;
			}
		}
	}
	return rewardValue;
}

INT64 CSeasonEventManager::GetSeasonEventRewardTime(int64_t userID, int seasonEventKind)
{
	INT64 rewardTime = 0;
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return rewardTime;

	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardItem : repo_SeasonEventReward[userID])
		{
			if (rewardItem->SeasonEventKind == seasonEventKind)
			{
				rewardTime = rewardItem->rewardTime;
				break;
			}
		}
	}
	return rewardTime;
}

void CSeasonEventManager::ProcessMission(INT64 i64UserID, int iConditionKind, int iConditionValue, int iTargetValue, Quest::SETTYPE setType)
{
	// i64UserID			: �������̵�
	// iConditionKind		: �̼� Kind
	// iConditionValue		: ���� �̼� �� 
	// iTargetValue			: ��ǥ �� 
#pragma region CheckProcessMission
	if (false == GLOBAL::GAMEWORKER_LOADER.IsCompleted())
	{
		return;
	}

	auto pUser = CUserManager::Instance()->Seek(i64UserID);
	if (IS_NULL(pUser))
		return;

	if (false == Util::GOT::IsActive(EnumContentState::PROGRESS_EVENT_SEASON))
		return;

	if (setType == Quest::SETTYPE::SET)
	{
		// SET (������) �� �ô������� �����Ѵ�
	}
	else
	{
		if (pUser->GetLord()->GetEra() < (INT16)EEra::MIDDLE_ERA)	// �߼�  ���� �ô�� ������ �� ����.
		{
			return;
		}
	}
	// ���̰��� ���°�� 
	if (iTargetValue <= 0)
		return;

	// ��Ʋ�ξ� ���� ������ ���
	if (GLOBAL::IsBattleRoyalServer())
		return;

#pragma endregion

	int iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	int iCurPeriod = GetCurPeriod();

	int userSeasonEventKind = 0;
	int userSeasonMissionKind = 0;
	int missionConditionValue = 0;

	int missionSeason = 0;
	int missionPeriod = 0;

	int allMissionCompleteKind = 0;
	int allMissionCompleteRewardItemKind = 0;
	int allMissionCompleteRewardItemCount = 0;
	int itemKind = 0;
	int itemCount = 0;

	std::vector<int> curPeriodSeasonMissionKind;

	int MissionValue_1 = 0;
	int MissionValue_2 = 0;
	int MissionValue_3 = 0;
	int CanReceiveRewardCount = 0;
	int eventRewardValue = 0;
	int totalMissioNCompleCount = 0;

#pragma region CheckAndFindProcessMission

	for (auto iter = BASE::SEASON_EVENT_MISSION_INFO_DATA.begin(); iter != BASE::SEASON_EVENT_MISSION_INFO_DATA.end(); ++iter)
	{
		if (iter->second->conditionKind == iConditionKind)
		{
			if (BASE::QUEST_CONDITION_INFO::CanCompleteByHigherValue(iConditionKind))	// Target�� �̻��϶��� ������ �ʿ��� Mission���� üũ���ִ� �Լ�
			{
				if (iter->second->conditionValue <= iConditionValue)
				{
					userSeasonMissionKind = iter->second->missionKind;
					missionConditionValue = iter->second->conditionValue;

					if (iTargetValue > iter->second->targetValue)
					{
						iTargetValue = iter->second->targetValue;
					}
				}
			}
			else
			{
				if (iter->second->conditionValue == iConditionValue)
				{
					userSeasonMissionKind = iter->second->missionKind;
					missionConditionValue = iter->second->conditionValue;

					if (iTargetValue > iter->second->targetValue)
					{
						iTargetValue = iter->second->targetValue;
					}
				}
			}
		}
	}
	
	if (userSeasonMissionKind == 0)
		return;

	for (auto iter = BASE::SEASON_EVENT_DATA.begin(); iter != BASE::SEASON_EVENT_DATA.end(); ++iter)
	{
		if (iter->second->CurSeason == iCurSeason)
		{
			if ((iter->second->SeasonPeriod == iCurPeriod) || (setType == Quest::SETTYPE::SET))
			{
				if (iter->second->MissionCard == 0)
				{
					if (userSeasonEventKind == 0)
					{
						allMissionCompleteKind = iter->second->SeasonEventKind;
						allMissionCompleteRewardItemKind = iter->second->SeasonRewards[0].Kind;
						allMissionCompleteRewardItemCount = iter->second->SeasonRewards[0].Count;
					}
				}
				else
				{
					curPeriodSeasonMissionKind.emplace_back(iter->second->SeasonEventKind);
				}

				if (iter->second->Mission_Kind_1 == userSeasonMissionKind)
				{
					userSeasonEventKind = iter->second->SeasonEventKind;
					missionSeason = iter->second->CurSeason;
					missionPeriod = iter->second->SeasonPeriod;

					if (iter->second->SeasonRewards.size() > 0)
					{
						itemKind = iter->second->SeasonRewards[0].Kind;
						itemCount = iter->second->SeasonRewards[0].Count;
					}
				}
				else if (iter->second->Mission_Kind_2 == userSeasonMissionKind)
				{
					userSeasonEventKind = iter->second->SeasonEventKind;
					missionSeason = iter->second->CurSeason;
					missionPeriod = iter->second->SeasonPeriod;

					if (iter->second->SeasonRewards.size() > 0)
					{
						itemKind = iter->second->SeasonRewards[0].Kind;
						itemCount = iter->second->SeasonRewards[0].Count;
					}
				}
				else if (iter->second->Mission_Kind_3 == userSeasonMissionKind)
				{
					userSeasonEventKind = iter->second->SeasonEventKind;
					missionSeason = iter->second->CurSeason;
					missionPeriod = iter->second->SeasonPeriod;

					if (iter->second->SeasonRewards.size() > 0)
					{
						itemKind = iter->second->SeasonRewards[0].Kind;
						itemCount = iter->second->SeasonRewards[0].Count;
					}
				}
			}
		}
	}

	if (userSeasonEventKind == 0)
		return;

	auto DBtoMissionData = BASE::GetSeasonEventInfo(userSeasonEventKind);

	if (IS_NULL(DBtoMissionData))
		return;

	auto seasonEventMissionInfo_1 = BASE::GetSeasonEventMissionInfo(DBtoMissionData->Mission_Kind_1);
	auto seasonEventMissionInfo_2 = BASE::GetSeasonEventMissionInfo(DBtoMissionData->Mission_Kind_2);
	auto seasonEventMissionInfo_3 = BASE::GetSeasonEventMissionInfo(DBtoMissionData->Mission_Kind_3);

	if (IS_NULL(seasonEventMissionInfo_1))
		return;
	if (IS_NULL(seasonEventMissionInfo_2))
		return;
	if (IS_NULL(seasonEventMissionInfo_3))
		return;

#pragma endregion

	auto missionKey = std::make_pair(i64UserID, userSeasonMissionKind);

	if (setType == Quest::SETTYPE::SET)
	{
		// ���� �����ϴ� Ÿ��
		if (auto info = repo_SeasonEvent.find(missionKey); info != repo_SeasonEvent.end())
		{
			// �̼� targetValue �޼��ϸ� ���̻� ��������ʴ´�.
			if (info->second->MissionValue >= iTargetValue)
				return;
		}
	}

	std::shared_ptr < SeasonEventMissionData> UserSeasonEventInfo = std::make_shared<SeasonEventMissionData>();
	UserSeasonEventInfo->UserId = i64UserID;
	UserSeasonEventInfo->SeasonEventMissionKind = userSeasonMissionKind;
	UserSeasonEventInfo->MissionValue = 0;

	// repo_seasonEvent ���� [ ���� �̼� ���� ]
	if (repo_SeasonEvent.find(missionKey) != repo_SeasonEvent.end())
	{
		auto seasonEventKeyData = repo_SeasonEvent[missionKey];
		if (seasonEventKeyData)
		{
			auto seasonEventMissionInfoData = BASE::GetSeasonEventMissionInfo(userSeasonMissionKind);

			if (IS_NULL(seasonEventMissionInfoData))
				return;

			if (seasonEventMissionInfoData->targetValue <= seasonEventKeyData->MissionValue)
				return;

			if (seasonEventKeyData->MissionValue < 0)
			{
				seasonEventKeyData->MissionValue = 0;
			}
			// UserSeasonEventInfo ���� ����ϴ� Value���� ���� ����ϴ� ���� �ҷ��ɴϴ�.
			UserSeasonEventInfo->MissionValue = seasonEventKeyData->MissionValue;
		}
		else
		{
			// UserSeasonEventInfo �� null�� ��� UserSeasonEventInfo�� �����մϴ�.
			seasonEventKeyData = UserSeasonEventInfo;
		}
	}
	else
	{
		// repo�� ���ٸ� ���� �߰��մϴ� [ �ű� ���� �̼� ]
		repo_SeasonEvent.emplace(missionKey, UserSeasonEventInfo);
	}

	auto eventMission = repo_SeasonEvent[missionKey];

	// �̼� setType�� ���� ���� �������ݴϴ�.
	if (setType == Quest::SETTYPE::INCREASE)
	{
		// ���� �߰��ϴ� Ÿ��
		repo_SeasonEvent[missionKey]->MissionValue += iTargetValue;
		UserSeasonEventInfo->MissionValue = eventMission->MissionValue;

	}
	else if (setType == Quest::SETTYPE::SET)
	{
		// ���� �����ϴ� Ÿ��
		repo_SeasonEvent[missionKey]->MissionValue = iTargetValue;
		UserSeasonEventInfo->MissionValue = eventMission->MissionValue;
	}

	// �ش� SeasonMissionKind ���� ����ϴ� 3�� �̼��� ���� �ҷ��´�
	if (repo_SeasonEvent.find(make_pair(i64UserID, DBtoMissionData->Mission_Kind_1)) != repo_SeasonEvent.end())
	{
		MissionValue_1 = repo_SeasonEvent[make_pair(i64UserID, DBtoMissionData->Mission_Kind_1)]->MissionValue;
	}
	if (repo_SeasonEvent.find(make_pair(i64UserID, DBtoMissionData->Mission_Kind_2)) != repo_SeasonEvent.end())
	{
		MissionValue_2 = repo_SeasonEvent[make_pair(i64UserID, DBtoMissionData->Mission_Kind_2)]->MissionValue;
	}
	if (repo_SeasonEvent.find(make_pair(i64UserID, DBtoMissionData->Mission_Kind_3)) != repo_SeasonEvent.end())
	{
		MissionValue_3 = repo_SeasonEvent[make_pair(i64UserID, DBtoMissionData->Mission_Kind_3)]->MissionValue;
	}

	// ���� 3�� �̼��߿��� Taget������ �����ϴ� �̼ǵ��� ������ Ȯ���Ѵ�.

	if (seasonEventMissionInfo_1->targetValue <= MissionValue_1)
		CanReceiveRewardCount++;
	if (seasonEventMissionInfo_2->targetValue <= MissionValue_2)
		CanReceiveRewardCount++;
	if (seasonEventMissionInfo_3->targetValue <= MissionValue_3)
		CanReceiveRewardCount++;

	if (CanReceiveRewardCount > 0)
	{
		if (seasonEventMissionInfo_1->targetValue <= MissionValue_1)
		{
			if (DBtoMissionData->Mission_Kind_1 == userSeasonMissionKind)
			{
				GLOBAL::SendLog(i64UserID, 0, DB_LOG::REASON_SEASONEVENT_MISSION, 0, 0, {
				CSeasonManager::Instance()->GetSeasonNo(),
				allMissionCompleteKind,
				userSeasonEventKind,
				userSeasonMissionKind,
				CanReceiveRewardCount,
				CARD_MISSION_COUNT,
					}
				, { });
			}
		}

		if (seasonEventMissionInfo_2->targetValue <= MissionValue_2)
		{
			if (DBtoMissionData->Mission_Kind_2 == userSeasonMissionKind)
			{
				GLOBAL::SendLog(i64UserID, 0, DB_LOG::REASON_SEASONEVENT_MISSION, 0, 0, {
				CSeasonManager::Instance()->GetSeasonNo(),
				allMissionCompleteKind,
				userSeasonEventKind,
				userSeasonMissionKind,
				CanReceiveRewardCount,
				CARD_MISSION_COUNT,
					}
				, { });
			}
		}

		if (seasonEventMissionInfo_3->targetValue <= MissionValue_3)
		{
			if (DBtoMissionData->Mission_Kind_3 == userSeasonMissionKind)
			{
				GLOBAL::SendLog(i64UserID, 0, DB_LOG::REASON_SEASONEVENT_MISSION, 0, 0, {
				CSeasonManager::Instance()->GetSeasonNo(),
				allMissionCompleteKind,
				userSeasonEventKind,
				userSeasonMissionKind,
				CanReceiveRewardCount,
				CARD_MISSION_COUNT,
					}
				, { });
			}
		}
	}

	// 3���� �̼��� ��� �Ϸ����� ��
	if (CanReceiveRewardCount == CARD_MISSION_COUNT)
	{
		std::shared_ptr < SeasonEventRewardData> UserSeasonEventRewardInfo = std::make_shared<SeasonEventRewardData>();
		UserSeasonEventRewardInfo->UserId = i64UserID;
		UserSeasonEventRewardInfo->SeasonEventKind = userSeasonEventKind;
		UserSeasonEventRewardInfo->completeMissionValue = (INT16)SeasonEventRewardState::CanReceiveReward;
		UserSeasonEventRewardInfo->itemKind = itemKind;
		UserSeasonEventRewardInfo->itemCount = itemCount;

		// repo_SeasonEventReward�� �̹� ������� ������ �ִ��� Ȯ�� , ����
		if (repo_SeasonEventReward.find(i64UserID) == repo_SeasonEventReward.end())
		{
			// ���� ��� ���� �߰�
			std::vector<std::shared_ptr<SeasonEventRewardData>> vecRewardData;
			
			vecRewardData.push_back(UserSeasonEventRewardInfo);

			repo_SeasonEventReward.emplace(i64UserID, vecRewardData);

			eventRewardValue = (INT16)SeasonEventRewardState::CanReceiveReward;
		}
		else
		{
			int compleValue = 0;
			for (auto rewardInfo : repo_SeasonEventReward[i64UserID])
			{
				// �̹� ���� �ִ� ��� ���� �����´�
				if (rewardInfo->SeasonEventKind == userSeasonEventKind)
				{
					compleValue = rewardInfo->completeMissionValue;
				}
			}
			// ���� ���� ���
			if (compleValue == 0)
			{
				// �߰��Ѵ�
				repo_SeasonEventReward[i64UserID].push_back(UserSeasonEventRewardInfo);
				compleValue = (INT16)SeasonEventRewardState::CanReceiveReward;
			}
			eventRewardValue = compleValue;
		}

		// ��� �̺�Ʈ �̼� ���� ���� ���� < �ش� season�� period ���� Ȱ��ȭ�� ��� �̼��� �Ϸ�Ǿ����� üũ�Ѵ� >
		for (int i = 0; i < curPeriodSeasonMissionKind.size(); i++)
		{
			if (repo_SeasonEventReward.find(i64UserID) != repo_SeasonEventReward.end())
			{
				for (auto rewardInfo : repo_SeasonEventReward[i64UserID])
				{
					if (rewardInfo->SeasonEventKind == curPeriodSeasonMissionKind[i])
					{
						if (rewardInfo->completeMissionValue > (INT16)SeasonEventRewardState::None)
						{
							totalMissioNCompleCount++;
						}
					}
				}
			}
		}
		// ��� �̼��� �Ϸ�� ���
		if (totalMissioNCompleCount == CARD_NUM)
		{
			int completeValue = 0;
			if (!FindSeasonEventReward(i64UserID, allMissionCompleteKind))
			{
				std::shared_ptr < SeasonEventRewardData> UserSeasonEventRewardInfo = std::make_shared<SeasonEventRewardData>();
				UserSeasonEventRewardInfo->UserId = i64UserID;
				UserSeasonEventRewardInfo->SeasonEventKind = allMissionCompleteKind;
				UserSeasonEventRewardInfo->completeMissionValue = (INT16)SeasonEventRewardState::CanReceiveReward;
				UserSeasonEventRewardInfo->itemKind = allMissionCompleteRewardItemKind;
				UserSeasonEventRewardInfo->itemCount = allMissionCompleteRewardItemCount;

				repo_SeasonEventReward[i64UserID].push_back(UserSeasonEventRewardInfo);
				completeValue = (INT16)SeasonEventRewardState::CanReceiveReward;
				// ���� ���� ���� ���¸� reoo_SeasonEventReward�� ����
			}
			else
			{
				completeValue = GetSeasonEventRewardState(i64UserID, allMissionCompleteKind);
			}

			// �ش� ������ DB���� < �̼� ���Ű��� �ٸ��� ! >
			QueryPacker packer;
			Util::SeasonEvent::SendToDB_Season_EVENT_USER_INFO_Update(i64UserID, iCurSeason, allMissionCompleteKind,
				0, 0, 0, 0, 0, 0, completeValue, GetDueDay_UTC(0), packer);
			packer.Request();
			
		}

		GLOBAL::SendLog(i64UserID, 0, DB_LOG::REASON_SEASONEVENT_MISSION, 0, 0, {
			CSeasonManager::Instance()->GetSeasonNo(),
			allMissionCompleteKind,
			userSeasonEventKind,
			0,
			totalMissioNCompleCount,
			CARD_NUM,
			}
		, { });
		SendUserSeasonEventInfoClinetNFY(i64UserID);
	}

	// DB�� ������ �۽�
	QueryPacker packer;
	Util::SeasonEvent::SendToDB_Season_EVENT_USER_INFO_Update(i64UserID, DBtoMissionData->CurSeason, DBtoMissionData->SeasonEventKind,
		DBtoMissionData->Mission_Kind_1, MissionValue_1, DBtoMissionData->Mission_Kind_2, MissionValue_2, DBtoMissionData->Mission_Kind_3, MissionValue_3,
		eventRewardValue, GetDueDay_UTC(0), packer);
	packer.Request();
}

void CSeasonEventManager::UpdateUserSeasonEventInfo_Reward(int64_t userID, INT16 userSeasonEventKind, INT16 completeEventValue, const QueryPackLinker::SharedPtr& load_balancer /*= nullptr*/)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;

	if (repo_SeasonEventReward.find(userID) == repo_SeasonEventReward.end())
		return;

	for (auto rewardInfo : repo_SeasonEventReward[userID])
	{
		if (rewardInfo->SeasonEventKind == userSeasonEventKind)
		{
			if (rewardInfo->completeMissionValue == (INT16)SeasonEventRewardState::CanReceiveReward)
			{
				// ���� �κ��丮 ó��
				if (rewardInfo->itemKind != 0 && rewardInfo->itemCount != 0)
				{
					int preItemCount = pUser->GetInventory()->GetItemNum(rewardInfo->itemKind);


					// ReceiveRewad ( ���� ���ɿ�û ) ���� ���� �κ��丮�� �߰�
					Util::ItemManager::AddItem(pUser->GetSharedPtr(), rewardInfo->itemKind, rewardInfo->itemCount, GLOBAL::EnumContentsGroup::Reward_SeasonEvent, 0, load_balancer);

					// LOG �߰� 
					GLOBAL::SendLog(userID, 0, DB_LOG::REASON_SEASONEVENT_REWARD, 0, 0, {
						CSeasonManager::Instance()->GetSeasonNo(),
						userSeasonEventKind,
						userSeasonEventKind,
						rewardInfo->itemKind,
						preItemCount,
						rewardInfo->itemCount,
						 pUser->GetInventory()->GetItemNum(rewardInfo->itemKind)
						}
					, { });

					// ���� �̺�Ʈ ���� ���� ( ��� �̼� �Ϸ� ) ����
					if (completeEventValue == (INT16)SeasonEventRewardState::ShowAllCompletedMissionReward)
					{
						if (BASE::SEASON_EVENT_DATA[userSeasonEventKind]->MissionCard == 0)
						{
							rewardInfo->completeMissionValue = (INT16)SeasonEventRewardState::ShowAllCompletedMissionReward;
						}
					}
					else
					{
						// ī�� �̼� ���� ���� ���� ����
						rewardInfo->completeMissionValue = (INT16)SeasonEventRewardState::ReceivedRewad;
					}
					rewardInfo->rewardTime= GetDueDay_UTC(0);
				}
			}
		}
	}
}

void CSeasonEventManager::SendUserSeasonEventInfoClinetACK(int64_t userID, int showUI)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;

	int iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	int iCurPeriod = GetCurPeriod();

	std::vector < std::shared_ptr<SeasonEventMissionData >> missionList;
	std::vector < std::shared_ptr<SeasonEventRewardData >> rewardList;

	for (auto eventMissionInfo : BASE::SEASON_EVENT_MISSION_INFO_DATA)
	{
		auto userSeasonMissionKey = make_pair(userID, eventMissionInfo.first);
		if (repo_SeasonEvent.find(userSeasonMissionKey) != repo_SeasonEvent.end())
		{
			if (repo_SeasonEvent[userSeasonMissionKey]->SeasonEventMissionKind != 0)
			{
				missionList.emplace_back(repo_SeasonEvent[userSeasonMissionKey]);
			}
		}
	}
	
	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->SeasonEventKind != 0)
			{
				rewardList.emplace_back(rewardInfo);
			}
		}
	}

	SendUserSeasonEventInfoACK(userID, showUI, iCurSeason, iCurPeriod, rewardList, missionList);
}

void CSeasonEventManager::SendUserSeasonEventInfoClinetNFY(int64_t userID)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;

	int iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	int iCurPeriod = GetCurPeriod();

	std::vector < std::shared_ptr<SeasonEventMissionData >> missionList;
	std::vector < std::shared_ptr<SeasonEventRewardData >> rewardList;

	for (auto eventMissionInfo : BASE::SEASON_EVENT_MISSION_INFO_DATA )
	{
		auto userSeasonMissionKey = make_pair(userID, eventMissionInfo.first);
		if (repo_SeasonEvent.find(userSeasonMissionKey) != repo_SeasonEvent.end())
		{
			if (repo_SeasonEvent[userSeasonMissionKey]->SeasonEventMissionKind != 0)
			{
				missionList.emplace_back(repo_SeasonEvent[userSeasonMissionKey]);
			}
		}
	}		
	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->SeasonEventKind != 0)
			{
				rewardList.emplace_back(rewardInfo);
			}
		}
	}

	SendUserSeasonEventInfoNFY(userID, iCurSeason, iCurPeriod, rewardList, missionList);

}

void CSeasonEventManager::UpdateToDBuserSeasonEventReward(INT64 userID, INT16 userSeasonEventKind, const QueryPackLinker::SharedPtr& load_balancer /*= nullptr*/)
{
	auto pUser = CUserManager::Instance()->Seek(userID);
	auto SeasonEventInfoData = BASE::GetSeasonEventInfo(userSeasonEventKind);

	if (IS_NULL(pUser))
		return;

	if (IS_NULL(SeasonEventInfoData))
		return;

	// �̼� �� value�� �߰�
	int mission_1_Value = 0;
	int mission_2_Value = 0;
	int mission_3_Value = 0;

	if (repo_SeasonEvent.find(make_pair(userID, SeasonEventInfoData->Mission_Kind_1)) != repo_SeasonEvent.end())
		mission_1_Value = repo_SeasonEvent[make_pair(userID, SeasonEventInfoData->Mission_Kind_1)]->MissionValue;

	if (repo_SeasonEvent.find(make_pair(userID, SeasonEventInfoData->Mission_Kind_2)) != repo_SeasonEvent.end())
		mission_2_Value = repo_SeasonEvent[make_pair(userID, SeasonEventInfoData->Mission_Kind_2)]->MissionValue;

	if (repo_SeasonEvent.find(make_pair(userID, SeasonEventInfoData->Mission_Kind_3)) != repo_SeasonEvent.end())
		mission_3_Value = repo_SeasonEvent[make_pair(userID, SeasonEventInfoData->Mission_Kind_3)]->MissionValue;

	// ���� ����
	int rewardState = 0;
	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->SeasonEventKind == userSeasonEventKind)
			{
				rewardState = rewardInfo->completeMissionValue;
			}
		}
	}

	QueryPacker packer;
	Util::SeasonEvent::SendToDB_Season_EVENT_USER_INFO_Update(userID, SeasonEventInfoData->CurSeason, userSeasonEventKind,
		SeasonEventInfoData->Mission_Kind_1, mission_1_Value, SeasonEventInfoData->Mission_Kind_2, mission_2_Value, SeasonEventInfoData->Mission_Kind_3, mission_3_Value,
		rewardState, GetDueDay_UTC(0), packer);
	packer.Request();
}

void CSeasonEventManager::UserLoginData(INT64 userID)
{
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return;

	SendUserSeasonEventInfoClinetNFY(userID);

	int iCurPeriod = CSeasonEventManager::Instance()->GetCurPeriod();
	int prePeriod = CSeasonEventManager::Instance()->GetPreviosPeriod(iCurPeriod);

	CSeasonEventManager::Instance()->PushMissedRewardUser(prePeriod, userID);
}

void CSeasonEventManager::OnDBComplete()
{
	RegisterScheduler();
}

void CSeasonEventManager::DeleteSeasonEvent_Info(INT64 userID, int seasonEventMissonKind)
{
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return;

	auto keyVlaue = make_pair(userID, seasonEventMissonKind);

	if (repo_SeasonEvent.find(keyVlaue) != repo_SeasonEvent.end())
	{
		repo_SeasonEvent.erase(keyVlaue);
	}

}

void CSeasonEventManager::DeleteSeasonEvent_Reward(INT64 userID)
{
	auto pUser = CUserManager::Instance()->Seek(userID);
	if (IS_NULL(pUser))
		return;

	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		repo_SeasonEventReward.erase(userID);
	}
}

void CSeasonEventManager::SendServerUserSeasonEventReward(INT16 season, INT16 prevPeriod)
{
	for (auto seasonEventRewardInfo : repo_SeasonEventReward)
	{
		auto userID = seasonEventRewardInfo.first;
		auto pUser = CUserManager::Instance()->Seek(userID);

		if (IS_NULL(pUser))
			continue;

		PushMissedRewardUser(prevPeriod, userID);
	}
}

void CSeasonEventManager::SendToClientUserSeasonEventReward(INT64 userID, INT16 userSeasonEventKind)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;


	// ����, ���� ī�ε忡 ���� ���� ������ �����ϴ� ���!
	NEW_FLATBUFFER(GS_SEASON_EVENT_REWARD_ACK, packet);
	packet.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::SEASON_REWARD_INFO>> flatSeasonEventInfoList;

		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::ITEM>>  flatItems;

		if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
		{
			for (auto rewardInfo : repo_SeasonEventReward[userID])
			{
				if (rewardInfo->SeasonEventKind == userSeasonEventKind)
				{
					flatItems.push_back(PROTOCOL::FLATBUFFERS::CreateITEM(fbb, rewardInfo->itemKind, rewardInfo->itemCount, 0, 0, 0, 0));
				}
			}
			
			flatSeasonEventInfoList.emplace_back(PROTOCOL::FLATBUFFERS::CreateSEASON_REWARD_INFO
			(
				fbb,
				pUser->UID(),
				GetSeasonEventRewardState(userID,userSeasonEventKind),
				userSeasonEventKind,
				fbb.CreateVector(flatItems)
			)
			);
		}

		return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_REWARD_ACK(fbb, 0,
			fbb.CreateVector(flatSeasonEventInfoList));
	});
	SEND_ACTIVE_USER(pUser, packet);

}

void CSeasonEventManager::LoadToDB_Season_Event(int64_t userID, int32_t SeasonKind, int SeasonMissionKind, INT16 Mission_1_Kind, INT64 Mission_1_value, INT16 Mission_2_Kind, INT64 Mission_2_value, INT16 Mission_3_Kind, INT64 Mission_3_value, INT16 completeMission, int64_t RewardTime)
{
	int	countReward = 0;
	int itemKind = 0;
	int itemCount = 0;

	if(SeasonKind != CSeasonManager::Instance()->GetSeasonNo())
		return;
	
	auto pUser = CUserManager::Instance()->Seek(userID);
	
	if (IS_NULL(pUser))
		return;

	// Value_1 ����
	if (Mission_1_value != 0)
	{
		auto missionKey_1 = std::make_pair(userID, Mission_1_Kind);
		if (repo_SeasonEvent.find(missionKey_1) != repo_SeasonEvent.end())
		{
			repo_SeasonEvent[missionKey_1]->MissionValue = Mission_1_value;
		}
		else
		{
			std::shared_ptr < SeasonEventMissionData> UserSeasonEventInfo = std::make_shared<SeasonEventMissionData>();
			UserSeasonEventInfo->UserId = userID;
			UserSeasonEventInfo->SeasonEventMissionKind = Mission_1_Kind;
			UserSeasonEventInfo->MissionValue = Mission_1_value;

			repo_SeasonEvent.emplace(missionKey_1, UserSeasonEventInfo);
		}

		if (Mission_1_Kind > 0)
		{
			if (BASE::SEASON_EVENT_MISSION_INFO_DATA.find(Mission_1_Kind) != BASE::SEASON_EVENT_MISSION_INFO_DATA.end())
			{
				if (Mission_1_value >= BASE::SEASON_EVENT_MISSION_INFO_DATA[Mission_1_Kind]->targetValue)
				{
					countReward++;
				}
			}
		}
	}

	// Value_2 ����
	if (Mission_2_value != 0)
	{
		auto missionKey_2 = std::make_pair(userID, Mission_2_Kind);
		if (repo_SeasonEvent.find(missionKey_2) != repo_SeasonEvent.end())
		{
			repo_SeasonEvent[missionKey_2]->MissionValue = Mission_2_value;
		}
		else
		{
			std::shared_ptr < SeasonEventMissionData> UserSeasonEventInfo = std::make_shared<SeasonEventMissionData>();
			UserSeasonEventInfo->UserId = userID;
			UserSeasonEventInfo->SeasonEventMissionKind = Mission_2_Kind;
			UserSeasonEventInfo->MissionValue = Mission_2_value;

			repo_SeasonEvent.emplace(missionKey_2, UserSeasonEventInfo);
		}

		if (Mission_2_Kind > 0)
		{
			if (BASE::SEASON_EVENT_MISSION_INFO_DATA.find(Mission_2_Kind) != BASE::SEASON_EVENT_MISSION_INFO_DATA.end())
			{
				if (Mission_2_value >= BASE::SEASON_EVENT_MISSION_INFO_DATA[Mission_2_Kind]->targetValue)
				{
					countReward++;
				}
			}
		}
	}

	// Value_3 ����
	if (Mission_3_value != 0)
	{
		auto missionKey_3 = std::make_pair(userID, Mission_3_Kind);
		if (repo_SeasonEvent.find(missionKey_3) != repo_SeasonEvent.end())
		{
			repo_SeasonEvent[missionKey_3]->MissionValue = Mission_3_value;
		}
		else
		{
			std::shared_ptr < SeasonEventMissionData> UserSeasonEventInfo = std::make_shared<SeasonEventMissionData>();
			UserSeasonEventInfo->UserId = userID;
			UserSeasonEventInfo->SeasonEventMissionKind = Mission_3_Kind;
			UserSeasonEventInfo->MissionValue = Mission_3_value;

			repo_SeasonEvent.emplace(missionKey_3, UserSeasonEventInfo);
		}

		if (Mission_3_Kind > 0)
		{
			if (BASE::SEASON_EVENT_MISSION_INFO_DATA.find(Mission_3_Kind) != BASE::SEASON_EVENT_MISSION_INFO_DATA.end())
			{
				if (Mission_3_value >= BASE::SEASON_EVENT_MISSION_INFO_DATA[Mission_3_Kind]->targetValue)
				{
					countReward++;
				}
			}
		}
	}
	
	if (BASE::SEASON_EVENT_DATA.find(SeasonMissionKind) != BASE::SEASON_EVENT_DATA.end())
	{
		if (BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards.size() > 0)
		{
			itemKind = BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards[0].Kind;
			itemCount = BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards[0].Count;
		}
	}

	if (countReward == CARD_MISSION_COUNT)
	{
		std::shared_ptr < SeasonEventRewardData> UserSeasonEventRewardInfo = std::make_shared<SeasonEventRewardData>();

		UserSeasonEventRewardInfo->UserId = userID;
		UserSeasonEventRewardInfo->SeasonEventKind = SeasonMissionKind;
		UserSeasonEventRewardInfo->completeMissionValue = (int)SeasonEventRewardState::CanReceiveReward;

		UserSeasonEventRewardInfo->itemKind = itemKind;
		UserSeasonEventRewardInfo->itemCount = itemCount;

		if (repo_SeasonEventReward.find(userID) == repo_SeasonEventReward.end())
		{
			std::vector<std::shared_ptr<SeasonEventRewardData>> vecRewardData;

			if (BASE::SEASON_EVENT_DATA.find(SeasonMissionKind) != BASE::SEASON_EVENT_DATA.end())
			{
				if (BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards.size() > 0)
				{
					UserSeasonEventRewardInfo->itemKind = BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards[0].Kind;
					UserSeasonEventRewardInfo->itemCount = BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards[0].Count;
				}
			}

			vecRewardData.push_back(UserSeasonEventRewardInfo);

			repo_SeasonEventReward.emplace(userID, vecRewardData);
		}
		else
		{
			repo_SeasonEventReward[userID].push_back(UserSeasonEventRewardInfo);
		}
	}

	if (completeMission != 0)
	{
		std::shared_ptr < SeasonEventRewardData> UserSeasonEventRewardInfo = std::make_shared<SeasonEventRewardData>();

		UserSeasonEventRewardInfo->UserId = userID;
		UserSeasonEventRewardInfo->SeasonEventKind = SeasonMissionKind;
		UserSeasonEventRewardInfo->completeMissionValue = completeMission;

		if (BASE::SEASON_EVENT_DATA.find(SeasonMissionKind) != BASE::SEASON_EVENT_DATA.end())
		{
			if (BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards.size() > 0)
			{
				UserSeasonEventRewardInfo->itemKind = BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards[0].Kind;
				UserSeasonEventRewardInfo->itemCount = BASE::SEASON_EVENT_DATA[SeasonMissionKind]->SeasonRewards[0].Count;
			}
		}


		if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
		{
			bool IsComplete = false;
			for (auto rewardInfo : repo_SeasonEventReward[userID])
			{
				if (rewardInfo->SeasonEventKind == SeasonMissionKind)
				{
					rewardInfo->completeMissionValue = completeMission;
					IsComplete = true;
				}
			}
			if (!IsComplete)
			{
				repo_SeasonEventReward[userID].push_back(UserSeasonEventRewardInfo);
			}
		}
		else
		{
			std::vector<std::shared_ptr<SeasonEventRewardData>> vecRewardData;


			vecRewardData.push_back(UserSeasonEventRewardInfo);
			repo_SeasonEventReward.emplace(userID, vecRewardData);
		}
	}
}

void CSeasonEventManager::PushMissedRewardUser(INT32 period, INT64 userID)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;

	bool isMissedRewardUser = false;

	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->completeMissionValue == (INT16)SeasonEventRewardState::CanReceiveReward)	// ���� ���� ���°� ���ɰ����� ����
			{
				int seasonEventKind = rewardInfo->SeasonEventKind;
				auto seasonEventData = BASE::GetSeasonEventInfo(seasonEventKind);

				if (IS_NULL(seasonEventData))
					continue;

				// ���ݱ�/�Ϲݱ� �˻�
				if (seasonEventData->SeasonPeriod != period)
					continue;

				isMissedRewardUser = true;
			}
		}
	}
	if (!isMissedRewardUser)
		return;

	auto iterF = m_missedRewardUserList.find(period);
	if (iterF == m_missedRewardUserList.end()) 
		iterF = m_missedRewardUserList.emplace(period, std::queue<INT64>()).first;

	auto& userList = iterF->second;
	userList.emplace(userID);
}

void CSeasonEventManager::ProvideMissedReward()
{
	std::map<INT64, std::vector<PROTOCOL::ITEM_INFO>> rewardUserReward;

	for (auto& [period, UserQueue] : m_missedRewardUserList)
	{
		INT32 count = 0;

		while (count < PROVIDE_REWARD_ONE_CYCLE && UserQueue.size() > 0)
		{
			auto userId = UserQueue.front();

			UserQueue.pop();

			const auto& game_user = CUserManager::Instance()->Seek(userId);
			if (IS_NULL(game_user))
				continue;

			SendUserMissedReward(userId);

			++count;
		}
	}
}

void CSeasonEventManager::SendUserMissedReward(INT64 userID)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return;

	INT16 curPeriod = GetCurPeriod();
	INT16 prePeriod = GetPreviosPeriod(curPeriod);
	QueryPacker packer;
	std::vector<PROTOCOL::ITEM_INFO> rewardUserReward;

	//  repo�� �ִ� ����� �߿��� ����ID�� ������ �� �ִ� ������ ��� Ȯ���մϴ�
	if(repo_SeasonEventReward.find(userID)!=repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->completeMissionValue == (INT16)SeasonEventRewardState::CanReceiveReward)	// ���� ���� ���°� ���ɰ����� ����
			{
				int seasonEventKind = rewardInfo->SeasonEventKind;
				auto seasonEventData = BASE::GetSeasonEventInfo(seasonEventKind);

				if (IS_NULL(seasonEventData))
					continue;

				// ���ݱ�/�Ϲݱ� �˻�
				if (seasonEventData->SeasonPeriod != prePeriod)
					continue;

				auto rewardItems = FindSeasonKindReward(seasonEventKind);
				if (rewardItems.size() > 0)
				{
					for (auto rewadrItem : rewardItems)
					{
						rewardUserReward.emplace_back(rewadrItem);
					}
				}
				rewardInfo->completeMissionValue = (INT16)SeasonEventRewardState::ReceivedRewad;
				rewardInfo->rewardTime = GetDueDay_UTC(0);
				// DB���� ���� ���� ����
	
				CSeasonEventManager::Instance()->UpdateToDBuserSeasonEventReward(userID, seasonEventKind, packer);
		
			}
		}
	}
	packer.Request();
	//������� ���� �Ÿ���
	if (rewardUserReward.size() == 0)
		return;

	CMailManager::Instance()->SendImportantMail(GAME::eMAIL_TYPE::MAIL_TYPE_EVENT_SEASON_REWARD_NOT_RECEIVE
		, userID
		, _T("")
		, pUser->GetLordName()
		, _T("MAIL_SEASONEVENTEND_TITLE")
		, _T("MAIL_SEASONEVENTEND_DESC")
		, _T("-")
		, GLOBAL::EnumContentsGroup::Reward_SeasonEvent
		, 0
		, rewardUserReward
	);
}

void CSeasonEventManager::Cheat_SetSeasonEvent(INT64 userID, INT32 cardNum ,INT32 MissionNum, int missionValue)
{
	int iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	int iCurPeriod = GetCurPeriod();
	int seasonEventKind = 0;
	int seasonEventMissionKind = 0;

	for (auto iter = BASE::SEASON_EVENT_DATA.begin(); iter != BASE::SEASON_EVENT_DATA.end(); ++iter)
	{
		if (iter->second->CurSeason == iCurSeason)
		{
			if (iter->second->SeasonPeriod == iCurPeriod)
			{
				if (iter->second->MissionCard == cardNum)
				{
					seasonEventKind = iter->second->SeasonEventKind;

					switch (MissionNum)
					{
					case 1:
						seasonEventMissionKind = iter->second->Mission_Kind_1;
						break;
					case 2:
						seasonEventMissionKind = iter->second->Mission_Kind_2;
						break;
					case 3:
						seasonEventMissionKind = iter->second->Mission_Kind_3;
						break;
					default:
						seasonEventMissionKind = 0;
						break;
					}

					break;
				}
			}
		}
	}
	if (seasonEventMissionKind == 0)return;

	auto missionKey = std::make_pair(userID, seasonEventMissionKind);

	if (repo_SeasonEvent.find(missionKey) != repo_SeasonEvent.end())
	{
		repo_SeasonEvent[missionKey]->MissionValue = missionValue;
	}
	else
	{
		std::shared_ptr < SeasonEventMissionData> UserSeasonEventInfo = std::make_shared<SeasonEventMissionData>();
		UserSeasonEventInfo->UserId = userID;
		UserSeasonEventInfo->SeasonEventMissionKind = seasonEventMissionKind;
		UserSeasonEventInfo->MissionValue = missionValue;
		repo_SeasonEvent.emplace(missionKey, UserSeasonEventInfo);
	}

	UpdateDBdata(userID, seasonEventKind);


}

void CSeasonEventManager::UpdateDBdata(INT64 userID, INT16 userSeasonEventKind)
{
	int iCurSeason = CSeasonManager::Instance()->GetSeasonNo();
	int iCurPeriod = GetCurPeriod();

	int mission_1_kind = 0;
	int mission_2_kind = 0;
	int mission_3_kind = 0;

	int mission_1_value = 0;
	int mission_2_value = 0;
	int mission_3_value = 0;

	int rewardValue = 0;

	for (auto iter = BASE::SEASON_EVENT_DATA.begin(); iter != BASE::SEASON_EVENT_DATA.end(); ++iter)
	{
		if (iter->second->CurSeason == iCurSeason)
		{
			if (iter->second->SeasonPeriod == iCurPeriod)
			{
				if (iter->second->SeasonEventKind == userSeasonEventKind)
				{
					mission_1_kind = iter->second->Mission_Kind_1;
					mission_2_kind = iter->second->Mission_Kind_2;
					mission_3_kind = iter->second->Mission_Kind_3;
					break;
				}
			}
		}
	}

	if (repo_SeasonEvent.find(std::make_pair(userID, mission_1_kind)) != repo_SeasonEvent.end())
		mission_1_value = repo_SeasonEvent[std::make_pair(userID, mission_1_kind)]->MissionValue;
	
	if (repo_SeasonEvent.find(std::make_pair(userID, mission_2_kind)) != repo_SeasonEvent.end())
		mission_2_value = repo_SeasonEvent[std::make_pair(userID, mission_2_kind)]->MissionValue;

	if (repo_SeasonEvent.find(std::make_pair(userID, mission_3_kind)) != repo_SeasonEvent.end())
		mission_3_value = repo_SeasonEvent[std::make_pair(userID, mission_3_kind)]->MissionValue;

	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->SeasonEventKind == userSeasonEventKind)
			{
				rewardValue = rewardInfo->completeMissionValue;
			}
		}
	}

	// DB�� ������ �۽�
	QueryPacker packer;
	Util::SeasonEvent::SendToDB_Season_EVENT_USER_INFO_Update(userID, iCurSeason, userSeasonEventKind,
		mission_1_kind, mission_1_value, mission_2_kind, mission_2_value, mission_3_kind, mission_3_value,
		rewardValue, GetDueDay_UTC(0), packer);
	packer.Request();
}

void CSeasonEventManager::RegisterScheduler()
{
	// ����ð�
	INT64 curTimeUTC = GetDueDay_UTC(0);

	// ���µǴ� �ð� (UTC)
	const INT64 openTimeFirst = CGlobalGameServerListManager::Instance()->GetMyServer()->GetBirthDay();

	// ������ �ð� (UTC)
	const INT64 endTimeFirst = MakeEndTimeByType(openTimeFirst, SeasonEventPeriodState::Period_First);
	const INT64 endTimeSecond = MakeEndTimeByType(openTimeFirst, SeasonEventPeriodState::Period_second);

	// ���ݱ� cycle
	const auto& First_lifeCycle = MakeSeasonEventScheduler(SeasonEventPeriodState::Period_First, endTimeFirst);

	// �Ĺݱ� cycle
	const auto& Second_lifeCycle = MakeSeasonEventScheduler(SeasonEventPeriodState::Period_second, endTimeSecond);

	// ������ ������Ʈ �ð�
	INT64 lastUpdateTimeUTC = CLifeCycleManager::Instance()->CheckUpdateTime(ETimer_Type::Utc);

	// ����Ŭ ������ ���� �ð�
	// �̺�Ʈ ���� ���� ���� (��/�Ĺ�) endtimefirst/seconde
	// ���� ���� �ð� cur

	// 1. ����ð� ���� ���½ð��� ���� ��� 
	if (curTimeUTC < endTimeFirst)
	{
		CLifeCycleManager::Instance()->Insert(First_lifeCycle);
	}
	else
	{
		// �̺�Ʈ ���� ���������� üũ
		if (lastUpdateTimeUTC < endTimeFirst)
		{
			First_lifeCycle->OnCompleted(false);
		}
	}

	if (curTimeUTC < endTimeSecond)
	{
		CLifeCycleManager::Instance()->Insert(Second_lifeCycle);

	}
	else
	{
		if (lastUpdateTimeUTC < endTimeSecond)
		{
			Second_lifeCycle->OnCompleted(false);
		}
	}
}

CLifeCycle::SharedPtr CSeasonEventManager::MakeSeasonEventScheduler(SeasonEventPeriodState type, INT64 completeTime)
{
	switch (type)
	{
	case SeasonEventPeriodState::Period_First:
		return make_shared<CLifeCycle_SeasonEventPeriod_First>(ELifeCycleContentsType::SeasonEventPeriod_First, static_cast<INT32>(type), completeTime, ETimer_Type::Utc);
	case SeasonEventPeriodState::Period_second:
		return make_shared<CLifeCycle_SeasonEventPeriod_Second>(ELifeCycleContentsType::SeasonEventPeriod_Second, static_cast<INT32>(type), completeTime, ETimer_Type::Utc);
	}

	return nullptr;
}

INT64 CSeasonEventManager::MakeEndTimeByType(const INT64 startTime, SeasonEventPeriodState type)
{
	switch (type)
	{
		case SeasonEventPeriodState::Period_First:
			return  startTime + SEC_ONE_WEEK * 4;
		case SeasonEventPeriodState::Period_second:
			return startTime + SEC_ONE_WEEK * 8;
	}

	return startTime;
}

std::vector<PROTOCOL::ITEM_INFO> CSeasonEventManager::FindSeasonKindReward(int seasonKind)
{
	std::vector<PROTOCOL::ITEM_INFO> vecRewardItem;

	auto seasonEventData = BASE::GetSeasonEventInfo(seasonKind);

	if (IS_NULL(seasonEventData))
		return vecRewardItem;

	if(seasonEventData->SeasonRewards.size()>0)
	{
		for (auto rewardItem : seasonEventData->SeasonRewards)
		{
			vecRewardItem.emplace_back(PROTOCOL::ITEM_INFO(rewardItem.Kind, rewardItem.Count));
		}
	}
	return vecRewardItem;
}

bool CSeasonEventManager::FindSeasonEventReward(int64_t userID, int seasonEventKind)
{
	auto pUser = CUserManager::Instance()->Seek(userID);

	if (IS_NULL(pUser))
		return false;

	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->SeasonEventKind == seasonEventKind)
			{
				return true;
			}
		}
	}
	return false;
}
