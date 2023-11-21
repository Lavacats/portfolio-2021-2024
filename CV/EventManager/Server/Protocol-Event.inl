inline void OnRecv_GS_DAILY_EVENT_DATA_GET_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		auto [error_code, req] = VERIFY_PROTOCOL(GS_DAILY_EVENT_DATA_GET_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		if (GLOBAL::IsBattleRoyalServer() == true)
			return;

		GLOBAL::EVENT_MANAGER.SendClientDailyEventData(game_user.get(), req->ShowUI());
	}

	inline void OnRecv_GS_SEASON_EVENT_INFO_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		// ≈¨∂Ûø°º≠ º≠πˆ∑Œ seasonEvent¡§∫∏∏¶ ø‰√ª«— ∞ÊøÏ
		auto [error_code, req] = VERIFY_PROTOCOL(GS_SEASON_EVENT_INFO_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		// º≠πˆø°º≠ ≈¨∂Û∑Œ ¿Ø¿˙∞° ∞°¡¯ πÃº«µ•¿Ã≈Õ º€Ω≈ 
		CSeasonEventManager::Instance()->SendUserSeasonEventInfoClinetACK(game_user->UID(),req->ShowUI());
	}


	inline void OnRecv_GS_SEASON_EVENT_REWARD_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		auto [error_code, req] = VERIFY_PROTOCOL(GS_SEASON_EVENT_REWARD_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		auto sendError = [&](RESULT::eRESULT err) -> void
		{
			NEW_FLATBUFFER(GS_SEASON_EVENT_REWARD_ACK, pFAILPACKET);
			pFAILPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
			{
				return PROTOCOL::FLATBUFFERS::CreateGS_SEASON_EVENT_REWARD_ACK(fbb, static_cast<int>(err), 0);
			});
			SEND_ACTIVE_USER(game_user, pFAILPACKET);
		};

		if (false == Util::GOT::IsActive(EnumContentState::PROGRESS_EVENT_SEASON))
		{
			sendError(RESULT::R_FAIL_NOT_OPEN_CONTENT);
			return;
		}

		// ∞À¡ı ∞˙¡§
		if (BASE::SEASON_EVENT_DATA.find(req->SeasonEventKind())== BASE::SEASON_EVENT_DATA.end())
		{
			return;
		}
		if (CSeasonEventManager::Instance()->FindSeasonEventReward(game_user->UID(),req->SeasonEventKind()))	//∫∏ªÛ∞¸∏Æ repo_seasonEventReewarø° «ÿ¥Á seasonKind ∫∏ªÛ¿Ã ¡∏¿Á«œ¥¬¡ˆ »Æ¿Œ
		{
			QueryPacker packer;

			// ∫∏ªÛ ºˆ∑… ø©∫Œ √≥∏Æ
			CSeasonEventManager::Instance()->UpdateUserSeasonEventInfo_Reward(game_user->UID(), req->SeasonEventKind(), req->Result(), packer);
			// ≈¨∂Û¿Ãæ∆Æ ACK
			CSeasonEventManager::Instance()->SendToClientUserSeasonEventReward(game_user->UID(), req->SeasonEventKind());
			// DBø°º≠ ∫∏ªÛ ªÛ≈¬ ∫Ø∞Ê
			CSeasonEventManager::Instance()->UpdateToDBuserSeasonEventReward(game_user->UID(), req->SeasonEventKind(),  packer);

			packer.Request();
		}
	}


	inline void OnRecv_GS_DAILY_EVENT_POINT_REWARD_SET_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		auto [error_code, req] = VERIFY_PROTOCOL(GS_DAILY_EVENT_POINT_REWARD_SET_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		auto sendError = [&](RESULT::eRESULT err) -> void
		{
			NEW_FLATBUFFER(GS_DAILY_EVENT_POINT_REWARD_SET_ACK, pFAILPACKET);
			pFAILPACKET.Build([&](flatbuffers::FlatBufferBuilder& fbb)->auto
			{
				return PROTOCOL::FLATBUFFERS::CreateGS_DAILY_EVENT_POINT_REWARD_SET_ACK(fbb, static_cast<int>(err), 0, 0, 0, 0, 0);
			});
			SEND_ACTIVE_USER(game_user, pFAILPACKET);
		};

		if (false == Util::GOT::IsActive(EnumContentState::PROGRESS_EVENT_DAILY))
		{
			sendError(RESULT::R_FAIL_NOT_OPEN_CONTENT);
			return;
		}

		//GLOBAL::EVENT_MANAGER.SendDBDailyEventRewardSet(game_user.get(), req);

		auto pUser = game_user.get();
		INT32 weekDay = GLOBAL::GetDailyEventWeekDay(GetDueDay_UTC(0));
		INT32 curWeekNum = GLOBAL::GetDailyEventWeeklyID(GetDueDay_UTC(0));

		//«ˆ¿Á UTC Ω√∞£∞˙ ∏∂¡ˆ∏∑¿∏∑Œ æ˜µ•¿Ã∆Æµ«æ˙¥¯ Ω√∞£¿Ã ∏¬¡ˆ æ ¿∏∏È ¿Ã∫•∆Æ ∆˜¿Œ∆Æ & ∫∏ªÛ¡§∫∏ √ ±‚»≠.
		if (pUser->CheckDailyPointRefreshData() == false)
		{
			pUser->RefreshDailyEventDatas();
		}

		INT32 rewardStep = req->RewardStep();
		auto funcCompare = [&](INT32 iValue)->bool { return iValue == rewardStep; };

		// ¡ﬂ∫π∞™¿Ã ¿÷¿∏∏È ¿ﬂ∏¯µ» ø‰√ª¿‘¥œ¥Ÿ.
		// πÃ∏Æ ∏ﬁ∏∏Æ º±√≥∏Æ «œ¡ˆ æ ¥¬ ¿Ã¿Ø¥¬ DB ∞™ ∞ªΩ≈ «œ¥¬ ∫Œ∫–ø°º≠ √ﬂ∞°∑Œ øπø‹√≥∏Æ ¡¯«‡«œ±‚ ∂ßπÆ¿‘¥œ¥Ÿ.
		auto vecReward = pUser->GetDailyEventRewardStep(curWeekNum, weekDay);
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

		//√º≈©!! (≥Œ√º≈©, ∆˜¿Œ∆Æ √º≈©, ∑π∫ß√º≈©)
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

		//// value ºˆƒ°ø° ∏¬√Á 1¡æ¿« æ∆¿Ã≈€ ¡ˆ±ﬁ
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
			case GAME::ITEM_NATIVE_RESOURCE: //¡˜¡¢ ¡ˆ±ﬁ«—¥Ÿ!
			{
				if (itemInfo->i32ITEM_KIND == 1) oil_add += reward.Count;
				else if (itemInfo->i32ITEM_KIND == 2) iron_add += reward.Count;
				else if (itemInfo->i32ITEM_KIND == 3) silver_add += reward.Count;
				else if (itemInfo->i32ITEM_KIND == 4) gold_add += reward.Count;
			}
			break;

			default:  //æ∆¿Ã≈€¿∏∑Œ ¡ˆ±ﬁ«—¥Ÿ.
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

	inline void OnRecv_GS_EVENT_MISSION_LIST_GET_REQ(const UserSharedPtr& game_user, TLNET::HEADER* packet)
	{
		auto [error_code, req] = VERIFY_PROTOCOL(GS_EVENT_MISSION_LIST_GET_REQ, packet);
		if (error_code != Error::E_SUCCEED)
			return;

		game_user->GetEventMissionList()->SendProcessingEventList();
	}

 
