void OnRecv_GS_GUILD_BOOKMARK_CREATE_REQ(Session::SharedPtr pSession, Stream::SharedPtr pStream)
	{
		/////////////////////////////////////////////////////////////////////////////////
		//	0. 에러: 패킷 검증 (플랫버퍼 유효성/ 패킷 일치/ 발신 유저 검색)
		/////////////////////////////////////////////////////////////////////////////////
		auto [errCode, req, pUser] = VERIFY_PROTOCOL_BY_USER(GS_GUILD_BOOKMARK_CREATE_REQ, pStream);
		if (errCode != Error::E_SUCCCEED)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, errCode);

		/////////////////////////////////////////////////////////////////////////////////
		//	0. 에러: 중복호출 검사 (0.25초)
		/////////////////////////////////////////////////////////////////////////////////
		VERIFY_AND_RETURN_PROTOCOL_INVERTAL(GS_GUILD_BOOKMARK_CREATE_ACK, pSession, pUser)

		auto pGuild = pUser->GetGuild();
		if (!pGuild)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_NOT_FOUND);

		// 조건체크 구간
		auto pMember = pGuild->Seek(pUser->GetGameObjID());
		if (!pMember)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_MEMBER_NOT_FOUND);

		// R4이상만 가능
		if (pMember->GetMemberRank() < (INT32)EnumGuildMemberRank::R4)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_BOOKMARK_NOT_ENOUGH_RANK);

		auto	nServID = pGuild->GetServID();
		auto	nKIND = req->Kind();
		auto	nPosX = req->PosX();
		auto	nPosY = req->PosY();
		auto	szName = to_wstring(req->Name());
		auto	cTargetObjID = CBaseObject::FlatBufferToObjID(req->TargetObjID());

		//	좌표 유효성 검사
		if (nPosX < 0 || nPosY < 0)
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_INVALID_PARAMETER);

		if (nPosX > 1024 || nPosY > 1024)
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_INVALID_PARAMETER);

		// 이름 크기 검사
		if (szName.empty())
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_BOOKMARK_LETTER_EMPTY);

		if (szName.size() > static_cast<UInt64>(COMMON_CONST_INFO::BOOKMARK_NAME_LENGTH_MAX))
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_BOOKMARK_LETTER_LIMIT_OVER);

		// 아이콘 검사
		auto pGuildBookMark = COMMON_GUILD_INFO::Instance()->GetBOOKMARKICON(nKIND);
		if (!pGuildBookMark)
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_INVALID_PARAMETER);

		// 데이터 개수 검사
		if (ASE_INSTANCE(pGuild, CGuildBookmarkManager)->size() + 1 > static_cast<UInt64>(COMMON_CONST_INFO::BOOKMARK_GUILD_COUNT_MAX))
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_BOOKMARK_COUNT_LIMIT_OVER);

		// 데이터 중복 검사 - 좌표 중복
		if (auto pData = ASE_INSTANCE(pGuild, CGuildBookmarkManager)->SeekByPos(core::Point(nPosX, nPosY)))
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_BOOKMARK_ALREADY_EXIST_POS);

		// 데이터 중복 검사 - 타입 중복
		if (auto pData = ASE_INSTANCE(pGuild, CGuildBookmarkManager)->SeekByKind(nKIND))
			return Error::Send(pSession, GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_BOOKMARK_ALREADY_EXIST_TYPE);

		auto pBookmark = ::MakeNewGuildBookmark(); // 이 시점에서 ContentID, DatabaseID 는 채워져 있다.
		if (!pBookmark)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_INVALID_DATA);

		// ...
		// 이하부터는 중간 반환 없음


		// 북마크 데이터 입력 구간
		pBookmark->SetServID(nServID);
		pBookmark->SetKIND(nKIND);
		pBookmark->SetName(szName);
		pBookmark->SetPoint(nPosX, nPosY);
		pBookmark->SetTargetObjID(cTargetObjID);

		// 북마크 데이터 컨테이너 삽입
		ASE_INSTANCE(pGuild, CGuildBookmarkManager)->Insert(pBookmark->GetGameObjID(), pBookmark);

		// 쿼리팩 생성
		auto pQueryPack = ::MakeRemoteQueryPack();

		// 쿼리팩에 북마크 갱신 쿼리 삽입
		pQueryPack->emplace_back(CGuildBookmarkManager::CreateQuery(pGuild, pBookmark));

		// 클라 전송
		NEW_FLATBUFFER(GS_GUILD_BOOKMARK_CREATE_ACK, pPACKET);
		pPACKET->Build([&](flatbuffers::FlatBufferBuilder& fbb) -> auto
		{
			return PROTOCOL::FLATBUFFERS::CreateGS_GUILD_BOOKMARK_CREATE_ACK(fbb,
				fbb.CreateVector(CGuildBookmarkManager::GetFlatBufferVector(fbb, { pBookmark })
				));
		});
		pUser->Send(pPACKET);

		// 연맹원 브로드캐스트
		CGuildBookmarkManager::BroadCast(pGuild, { pBookmark }, false, pUser);

		// DB 전송
		::ExecuteQuery(pQueryPack);

		//게임로그
		::GameLog_GuildBookMarkSet(pGuild, pUser, static_cast<Int64>(pBookmark->GetKIND()), nPosX, nPosY, ASE_INSTANCE(pGuild, CGuildBookmarkManager)->size(), pGuild->GetGuildNick(), pGuild->GetGuildName(),  pUser->GetName());
		
		// 연맹전체 우편
		CMailhelper::SendGuildBookMarkToMembers(pGuild, szName, nKIND, nPosX, nPosY);

		// 채팅
		::SendSystemMessage_GuildBookmarkCreate(pUser->GetDatabaseID(), pGuild->GetDatabaseID(), pBookmark);
	}

	void OnRecv_GS_GUILD_BOOKMARK_DELETE_REQ(Session::SharedPtr pSession, Stream::SharedPtr pStream)
	{
		/////////////////////////////////////////////////////////////////////////////////
		//	0. 에러: 패킷 검증 (플랫버퍼 유효성/ 패킷 일치/ 발신 유저 검색)
		/////////////////////////////////////////////////////////////////////////////////
		auto [errCode, req, pUser] = VERIFY_PROTOCOL_BY_USER(GS_GUILD_BOOKMARK_DELETE_REQ, pStream);
		if (errCode != Error::E_SUCCCEED)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_DELETE_ACK, errCode);

		/////////////////////////////////////////////////////////////////////////////////
		//	0. 에러: 중복호출 검사 (0.25초)
		/////////////////////////////////////////////////////////////////////////////////
		VERIFY_AND_RETURN_PROTOCOL_INVERTAL(GS_GUILD_BOOKMARK_DELETE_ACK, pSession, pUser)

		auto pGuild = pUser->GetGuild();
		if (!pGuild)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_DELETE_ACK, Error::E_GUILD_NOT_FOUND);

		auto cBookmarkID = CBaseObject::FlatBufferToObjID(req->BookmarkID());
		if (!cBookmarkID)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_DELETE_ACK, Error::E_GUILD_BOOKMARK_NOT_FOUND);
		
		// 조건체크 구간
		auto pMember = pGuild->Seek(pUser->GetGameObjID());
		if (!pMember)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_MEMBER_NOT_FOUND);

		// R4이상만 가능
		if (pMember->GetMemberRank() < (INT32)EnumGuildMemberRank::R4)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_BOOKMARK_NOT_ENOUGH_RANK);

		// 북마크 확인
		auto pBookmark = ASE_INSTANCE(pGuild, CGuildBookmarkManager)->Seek(cBookmarkID);
		if (!pBookmark)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_GUILD_BOOKMARK_NOT_FOUND);

		// ...
		// 이하부터는 중간 반환 없음

		// 북마크 데이터 삭제 구간
		ASE_INSTANCE(pGuild, CGuildBookmarkManager)->Delete(cBookmarkID);

		// 쿼리팩 생성
		auto pQueryPack = ::MakeRemoteQueryPack();

		// 쿼리팩에 북마크 갱신 쿼리 삽입
		pQueryPack->emplace_back(CGuildBookmarkManager::DeleteQuery(pGuild, cBookmarkID));

		NEW_FLATBUFFER(GS_GUILD_BOOKMARK_DELETE_ACK, pPACKET);
		pPACKET->Build([&](flatbuffers::FlatBufferBuilder& fbb) -> auto
		{
			return PROTOCOL::FLATBUFFERS::CreateGS_GUILD_BOOKMARK_DELETE_ACK(fbb, CBaseObject::CreateINFO_OBJID(fbb, cBookmarkID));
		});

		pUser->Send(pPACKET);

		// 연맹원 브로드캐스트
		CGuildBookmarkManager::BroadCast(pGuild, { pBookmark }, true, pUser);

		// DB 전송
		::ExecuteQuery(pQueryPack);

		// 게임 로그
		::GameLog_GuildBookMarkDelete(pGuild, pUser, static_cast<Int64>(pBookmark->GetKIND()), (Int64)pBookmark->GetX(), (Int64)pBookmark->GetY(), ASE_INSTANCE(pGuild, CGuildBookmarkManager)->size(), pGuild->GetGuildNick(), pGuild->GetGuildName(),  pUser->GetName());
	}

	void OnRecv_GS_GUILD_BOOKMARK_MODIFY_REQ(Session::SharedPtr pSession, Stream::SharedPtr pStream)
	{
		/////////////////////////////////////////////////////////////////////////////////
		//	0. 에러: 패킷 검증 (플랫버퍼 유효성/ 패킷 일치/ 발신 유저 검색)
		/////////////////////////////////////////////////////////////////////////////////
		auto [errCode, req, pUser] = VERIFY_PROTOCOL_BY_USER(GS_GUILD_BOOKMARK_MODIFY_REQ, pStream);
		if (errCode != Error::E_SUCCCEED)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_MODIFY_ACK, errCode);

		/////////////////////////////////////////////////////////////////////////////////
		//	0. 에러: 중복호출 검사 (0.25초)
		/////////////////////////////////////////////////////////////////////////////////
		VERIFY_AND_RETURN_PROTOCOL_INVERTAL(GS_GUILD_BOOKMARK_MODIFY_ACK, pSession, pUser)

		auto pGuild = pUser->GetGuild();
		if (!pGuild)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_GUILD_NOT_FOUND);

		auto cBookmarkID = CBaseObject::FlatBufferToObjID(req->BookmarkID());
		if (!cBookmarkID)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_GUILD_BOOKMARK_NOT_FOUND);

		// 조건체크 구간
		auto pMember = pGuild->Seek(pUser->GetGameObjID());
		if (!pMember)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_GUILD_MEMBER_NOT_FOUND);

		// R4이상만 가능
		if (pMember->GetMemberRank() < (INT32)EnumGuildMemberRank::R4)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_CREATE_ACK, Error::E_BOOKMARK_NOT_ENOUGH_RANK);

		// 북마크 확인
		auto pBookmark = ASE_INSTANCE(pGuild, CGuildBookmarkManager)->Seek(cBookmarkID);
		if (!pBookmark)
			return Error::Send(pSession, PROTOCOL::GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_GUILD_BOOKMARK_NOT_FOUND);

		auto	nPosX = req->PosX();
		auto	nPosY = req->PosY();
		auto	szName = to_wstring(req->Name());
		//	좌표 유효성 검사
		if (nPosX < 0 || nPosY < 0)
			return Error::Send(pSession, GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_INVALID_PARAMETER);

		if (nPosX > 1024 || nPosY > 1024)
			return Error::Send(pSession, GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_INVALID_PARAMETER);

		// 이름 크기 검사
		if (szName.empty())
			return Error::Send(pSession, GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_BOOKMARK_LETTER_EMPTY);

		if (szName.size() > static_cast<UInt64>(COMMON_CONST_INFO::BOOKMARK_NAME_LENGTH_MAX))
			return Error::Send(pSession, GS_GUILD_BOOKMARK_MODIFY_ACK, Error::E_BOOKMARK_LETTER_LIMIT_OVER);

		// ...
		// 이하부터는 중간 반환 없음

		// 북마크 데이터 입력 구간
		pBookmark->SetName(szName);
		pBookmark->SetPoint(nPosX, nPosY);

		// 북마크 데이터 컨테이너 삽입 => 없음. 있던것의 내용 변경이므로.
		//ASE_INSTANCE(pGuild, CGuildBookmarkManager)->Insert(pBookmark->GetGameObjID(), pBookmark);

		// 쿼리팩 생성
		auto pQueryPack = ::MakeRemoteQueryPack();

		// 쿼리팩에 북마크 갱신 쿼리 삽입
		pQueryPack->emplace_back(CGuildBookmarkManager::CreateQuery(pGuild, pBookmark));

		// 클라 전송
		NEW_FLATBUFFER(GS_GUILD_BOOKMARK_MODIFY_ACK, pPACKET);
		pPACKET->Build([&](flatbuffers::FlatBufferBuilder& fbb) -> auto
		{
			return PROTOCOL::FLATBUFFERS::CreateGS_GUILD_BOOKMARK_MODIFY_ACK(fbb,
				fbb.CreateVector(CGuildBookmarkManager::GetFlatBufferVector(fbb, { pBookmark })
				));
		});
		pUser->Send(pPACKET);

		// 연맹원 브로드캐스트
		CGuildBookmarkManager::BroadCast(pGuild, { pBookmark }, false, pUser);

		// DB 전송
		::ExecuteQuery(pQueryPack);
	}
