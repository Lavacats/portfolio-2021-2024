# 크로스파이어: WarZone - 북마크 시스템 구조 (Client/Server)

이 코드는 크로스파이어: WarZone 프로젝트에서 구현한 **길드 북마크 시스템**의 구조입니다.
클라이언트와 서버가 각각 북마크 UI, 데이터 처리, 통신 프로토콜 및 보관 로직을 분리하여 구현한 사례입니다.

---

## 🖥️ 클라이언트 (Unity 기반, C#)

### 주요 파일
- `GUI_BookmarkPopup.cs`: 북마크 팝업 UI 초기화 및 열기 처리
- `GUI_BookmarkPopup_RegisterPage.cs`: 북마크 등록 화면 UI 및 데이터 전달
- `GUI_BookmarkPopup_BookmarkPage.cs`: 북마크 조회 화면 구성
- `GUI_BookmarkModifiedPopup.cs`: 북마크 수정 팝업 처리
- `GuildBookmarkContainer.cs`: 클라이언트에서 북마크 데이터를 관리하는 컨테이너
- `NetGame_Guild.cs`: 서버와의 북마크 관련 패킷 송신 처리

---

## 🛠️ 서버 (C++ 기반)

### 주요 파일
- `GuildBookmarkManager.h`: 길드 북마크 생성, 수정, 삭제, 조회 로직 선언
- `_Guild.inl`: 길드 객체 내 북마크 데이터 처리 구현

---

## 📦 통신 프로토콜

- `GS_GUILD_BOOKMARK_CREATE_REQ.fbs`  
  Flatbuffers 기반으로 구현된 북마크 생성 요청 구조로, 클라-서버 간 명확한 구조 정의

---

## 🎯 구조 설계 특징

- UI, 데이터 처리, 서버 통신이 모두 모듈화되어 있어 유지보수가 뛰어남
- 클라이언트는 Flatbuffers 기반 프로토콜을 통해 서버에 요청 전송
- 서버는 북마크 데이터 저장/갱신을 GuildBookmarkManager를 통해 일관되게 처리
- 북마크 수정/등록/조회 UI가 분리되어 사용자 편의성과 관리 효율을 높임

---

본 코드는 북마크 기능 하나를 중심으로 **클라이언트-서버 양측의 구조 설계와 통신 흐름을 설득력 있게 보여주는 사례**입니다.
