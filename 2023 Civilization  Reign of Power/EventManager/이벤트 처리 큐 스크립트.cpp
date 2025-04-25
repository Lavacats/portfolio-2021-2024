// 1. 유저가 로그인할 떄 마지막으로 로그인한 시즌과 다를 경우 보상수령 대상자로 처리
void CSeasonEventManager::UserLoginData(INT64 userID)
{
  // ... 로그인시 처리
	CSeasonEventManager::Instance()->PushMissedRewardUser(prePeriod, userID);
}


// 2. 이벤트 매니저에서 추가된 유저들 대상으로 검증 후 보상 지급 인원으로 분류
void CSeasonEventManager::PushMissedRewardUser(INT32 period, INT64 userID)
{
	bool isMissedRewardUser = false;

  // 검증과정
	if (repo_SeasonEventReward.find(userID) != repo_SeasonEventReward.end())
	{
		for (auto rewardInfo : repo_SeasonEventReward[userID])
		{
			if (rewardInfo->completeMissionValue == (INT16)SeasonEventRewardState::CanReceiveReward)	// «ˆ¿Á ∫∏ªÛ ªÛ≈¬∞° ºˆ∑…∞°¥…¿Œ ªÛ≈¬
			{
				int seasonEventKind = rewardInfo->SeasonEventKind;
				auto seasonEventData = BASE::GetSeasonEventInfo(seasonEventKind);

				if (IS_NULL(seasonEventData))
					continue;

				if (seasonEventData->SeasonPeriod != period)
					continue;

				isMissedRewardUser = true;
			}
		}
	}
	if (!isMissedRewardUser)
		return;

// 시즌별 유저 보상데이터를 큐로 관리
	auto iterF = m_missedRewardUserList.find(period);
	if (iterF == m_missedRewardUserList.end()) 
		iterF = m_missedRewardUserList.emplace(period, std::queue<INT64>()).first;

	auto& userList = iterF->second;
	userList.emplace(userID);
}

// 3. 한 업데이트에 보상을 보낼 수 있는 유저 수를 PROVIDE_REWARD_ONE_CYCLE 로 지정
// 라이브 당시 사이클은 5로 한번의 업데이트 동안 5명의 유저들에게 보상지급
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