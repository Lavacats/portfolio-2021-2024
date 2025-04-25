# 문명: Reign of Power - 시즌/일일 이벤트 시스템 구조

이 코드는 Unity 클라이언트와 C++ 서버 양측에서 구현한 이벤트 시스템으로,
이벤트 등록, 수신, 보상 처리까지 전반을 다루는 구조입니다.

## 📂 주요 구조

### 클라이언트 (Unity C#)
- `EventManager.cs`: 게임 내 이벤트 타입 관리 및 이벤트 패킷 전송 핸들링
- `SeasonEventMission.cs`: 특정 시즌 이벤트 관련 미션 관리 및 UI 갱신
- `NrReciveGame_Common.cs`: 게임 서버로부터 수신한 이벤트 관련 패킷 처리
- `EventSeasonDlg.cs`: 시즌 이벤트 UI 팝업 구성

### 서버 (C++ 기반)
- `SeasonEventManager.cpp/h`: 시즌 이벤트 데이터 초기화, 상태 저장, 보상 처리
- `LifeCycle_SeasonEvent.cpp`: 시즌 이벤트의 라이프사이클 처리 흐름
- `UseEventManager.cpp`: 유저 단위 이벤트 조건 검출 및 보상 분기 처리

## 🎯 구조 설계 특징
- 클라이언트-서버 간 명확한 책임 분리
- Flatbuffers 기반 이벤트 패킷 송수신 구조
- 이벤트 로직과 보상 트리거가 독립적으로 구성되어 확장 용이

본 코드는 시즌 이벤트와 일일 이벤트의 전체 흐름을 구조적으로 관리하기 위한 실전 구현 예시입니다.
