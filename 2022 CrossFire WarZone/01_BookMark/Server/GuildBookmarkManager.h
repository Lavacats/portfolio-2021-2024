#pragma once

enum class EnumGuildBookmarkKind
{
	None = 0,
	Attack,
	Deffence,
	Rally,
	Target
};

class CGuildBookmark : public std::enable_shared_from_this<CGuildBookmark>
{
	core::ObjID				m_cGameObjID;
	Int32					m_nServID = 0;

	Int32					m_nKind = 0;
	std::wstring			m_szName;		//	이름
	core::Point				m_cPoint;		//	위치

	core::ObjID				m_cTargetObjID;	//  북마크 생성시 타겟 오브젝트

public:
	typedef std::shared_ptr<CGuildBookmark>	SharedPtr;
	typedef std::weak_ptr<CGuildBookmark>	WeakPtr;

	//	새로 생성
	CGuildBookmark();

	//	DB 생성
	CGuildBookmark(const core::ObjID& cGameObjID) { m_cGameObjID = cGameObjID; }
	virtual ~CGuildBookmark() = default;

public:
	auto	GetGameObjID() -> decltype(m_cGameObjID)& { return m_cGameObjID; }
	void	SetGameObjID(const decltype(m_cGameObjID)& o) { m_cGameObjID = o; }
	auto	GetServID() const { return m_nServID; }
	void	SetServID(const decltype(m_nServID)& o) { m_nServID = o; }

	auto	GetKIND() { return static_cast<EnumGuildBookmarkKind>(m_nKind); }
	void	SetKIND(Int32 o) { m_nKind = o; }

	auto	GetName() { return m_szName; }
	void	SetName(const decltype(m_szName)& o) { m_szName = o; }
	auto	GetPoint()->core::Point& { return m_cPoint; }
	void	SetPoint(Int32 x, Int32 y) { m_cPoint.x = x; m_cPoint.y = y; }
	auto	GetX() { return GetPoint().x; }
	auto	GetY() { return GetPoint().y; }

	auto	GetTargetObjID() -> decltype(m_cTargetObjID)& { return m_cTargetObjID; }
	void	SetTargetObjID(const decltype(m_cTargetObjID)& o) { m_cTargetObjID = o; }

public:
	virtual std::wstring	GetProperties();
	virtual void			SetProperties(const std::wstring& s);

	static auto CreateINFO_GUILD_BOOKMARK(flatbuffers::FlatBufferBuilder& builder, CGuildBookmark::SharedPtr pBookmark, bool RemoveFlag = false)
	{
		if (!pBookmark)
			return PROTOCOL::FLATBUFFERS::INFO_GUILD_BOOKMARKBuilder(builder).Finish();

		return PROTOCOL::FLATBUFFERS::CreateINFO_GUILD_BOOKMARK(builder,
			CBaseObject::CreateINFO_OBJID(builder, pBookmark->GetGameObjID()),
			core::EnumToInt(pBookmark->GetKIND()),
			::to_flatbuffer(builder, pBookmark->GetName()),
			pBookmark->GetX(),
			pBookmark->GetY(),
			CBaseObject::CreateINFO_OBJID(builder, pBookmark->GetTargetObjID()),
			RemoveFlag ? 1 : 0
		);
	}
};

class CGuildBookmarkManager : public core::Container<core::ObjID, CGuildBookmark::SharedPtr>
{
public:
	auto	GetFlatBufferVector(flatbuffers::FlatBufferBuilder& builder)
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::INFO_GUILD_BOOKMARK>>	result;

		for (const auto& [key, pBookmark] : GetRepository())
		{
			if (pBookmark)
				result.emplace_back(pBookmark->CreateINFO_GUILD_BOOKMARK(builder, pBookmark));
		}

		return result;
	}

	static auto	GetFlatBufferVector(flatbuffers::FlatBufferBuilder& builder, const std::vector<CGuildBookmark::SharedPtr>& lstBookmarks)
	{
		std::vector<flatbuffers::Offset<PROTOCOL::FLATBUFFERS::INFO_GUILD_BOOKMARK>>	result;

		for (auto pBookmark : lstBookmarks)
		{
			if (pBookmark)
				result.emplace_back(pBookmark->CreateINFO_GUILD_BOOKMARK(builder, pBookmark));
		}

		return result;
	}

	void	DeleteAll(GuildSharedPtr pGulid, QueryPackHelperSharedPtr pQueryPack);

	CGuildBookmark::SharedPtr SeekByPos(const core::Point& cPoint)
	{
		auto lockguard = GetRepositoryLockGuard();
		for (const auto& [key, pData] : GetRepository())
		{
			if (pData && pData->GetPoint() == cPoint)
				return pData;
		}

		return nullptr;
	}

	CGuildBookmark::SharedPtr SeekByKind(const Int32& nKind)
	{
		auto lockguard = GetRepositoryLockGuard();
		for (const auto& [key, pData] : GetRepository())
		{
			if (pData && static_cast<Int32>(pData->GetKIND()) == nKind)
				return pData;
		}

		return nullptr;
	}

	static void	SetData(VOID* pData);
	static Query::SharedPtr CreateQuery(GuildSharedPtr pGuild, CGuildBookmark::SharedPtr pBookmark);
	static Query::SharedPtr DeleteQuery(GuildSharedPtr pGuild, const core::ObjID& cObjID);

	static void BroadCast(GuildSharedPtr pGuild, const std::vector<CGuildBookmark::SharedPtr>& repo, bool RemoveFlag, CUser::SharedPtr except = nullptr);
};

//	새로이 생성
CGuildBookmark::SharedPtr MakeNewGuildBookmark();
//	DB에서 생성
CGuildBookmark::SharedPtr MakeNewGuildBookmark(const core::ObjID& cGameObjID);
