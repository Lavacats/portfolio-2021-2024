# 세컨드 웨이브 - 소셜/채팅 시스템 구조

이 코드는 프로젝트 '세컨드 웨이브'에서 구현된 **채팅 및 소셜 UI 시스템 구조**입니다.  
유저 메뉴, 친구 목록, 채팅 UI, 블록 기능, 콘솔 친구 연동 등 다양한 소셜 기능이 포함되어 있습니다.

---

## 💬 채팅 시스템

### 주요 파일
- `ChattingManager.cs`  
  채팅 데이터 수신 및 송신, 이벤트 분배, 채널 관리 기능 담당
- `ChattingManager.EventHandler.cs`  
  채팅 메시지 및 시스템 이벤트 핸들링 분리 처리
- `UI_ChattingController.cs`  
  채팅 UI 및 입력창 동기화, 텍스트 스크롤, 사용자 입력 처리
- `UI_Popup_Chatting.cs`  
  채팅 팝업창 전용 UI 관리

---

## 👥 유저 메뉴 / 친구 기능

### 주요 파일
- `UI_Popup_UserMenu.cs`, `UI_UserMenuController.cs`  
  유저 우클릭 메뉴 팝업 처리 및 블록/친구 초대 기능 연결
- `UI_Popup_AddFriends.cs`, `UI_Popup_Block.cs`, `UI_Popup_KickUser.cs`  
  친구 추가, 유저 차단, 강퇴 UI 처리
- `UI_UserMenuItem_Renewal.cs`  
  각 유저에 대한 메뉴 옵션 목록 구성

---

## 🤝 소셜 탭 / 친구 / 스쿼드

### 주요 파일
- `UI_SocialFriendController.cs`, `UI_SocialSquadController.cs`  
  친구/스쿼드 탭에서의 유저 리스트 관리 및 필터링
- `UI_SocialTabController_Renewal.cs`, `UI_SocialTabItem_Renewal.cs`  
  탭 전환 UI 및 선택 처리
- `UI_SocialUserBannerContainer.cs`, `UISocialUserBannerAccordian.cs`  
  유저 배너 렌더링 구조

---

## 🧠 설계 특징

- 채팅 기능은 **Manager + Controller + Popup UI**로 명확하게 분리되어 있음
- 친구/차단/초대 관련 기능은 **UserMenu 영역에서 모듈화되어 재활용 가능**
- 콘솔 플랫폼 연동 (`UI_SocialConsoleFriendController.cs`) 등 확장성 고려
- UI 구조는 Unity 기반의 **컴포넌트/컨테이너 중심 패턴**으로 구성됨

---

본 코드는 실제 라이브 서비스 게임에서 필요한 소셜 기능과 UI 흐름을 구조적으로 설계한 예시로,  
**다양한 유저 인터랙션을 분리된 책임 기반으로 처리**하는 구조 감각을 보여줍니다.
