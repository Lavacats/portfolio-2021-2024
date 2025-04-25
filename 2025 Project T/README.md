# Project T (2025) - 전투 콘텐츠 시스템 구조

본 구조는 Unity 기반의 전략형 전투 콘텐츠에서 사용된 시스템 설계를 포함합니다.  
전투 흐름, 유닛, 맵, 툴, 카메라, 경로 탐색, 스크립터블 데이터 등을 **모듈화된 구조**로 관리하며  
디자이너/QA/프로그래머 간 효율적인 협업을 고려한 구조입니다.

---

## 🎮 전투 시스템 핵심 구성

### ⚙️ 엔진/흐름
- `BattleEngine.cs`, `BattleEngine_Action.cs`: 전투 흐름 제어, 상태 전이 및 명령 분기
- `Battle_Manager.cs`, `BattleTestScene.cs`: 씬 내 전투 테스트 및 관리 흐름 처리

### 👤 유닛
- `BattleBaseUnit.cs`: 유닛의 이동, 목표 위치, 상태 갱신
- `UnitDataManager.cs`: 선택 유닛 목록 관리 및 명령 일괄 전달

---

## 🌍 맵 시스템

### 🧩 맵 구성
- `Battle_MapDirector.cs`: Tile / Cell / Pixel 구조 생성
- `Battle_TestMapController.cs`: 맵 생성 및 초기화 흐름

### 🔍 탐색 / 경로
- `Battle_Pathfinder_Controller.cs`, `Battle_Pathfinder_Block.cs`: 경로 탐색 분기 및 검증
- `PathFinder_Astar_Region.cs` 외: A* 기반 경로 탐색 알고리즘 구현
- `PriorityQueue.cs`: 탐색 최적화를 위한 우선순위 큐 구조

---

## 🎥 카메라 시스템

- `BattleCameraController.cs`: 입력 분배 및 시점 제어
- `BattleCamera_UnitController.cs`: 드래그, 유닛 선택 기능
- `BattleCamera_CameraMove.cs`: 키보드 이동 처리

---

## 🧰 에디터 툴 + 맵 설정

- `BattleTestCustomWindow.cs`: 맵 데이터 확인용 툴
- `CustomWindow_MapValue.cs`: ScriptableObject 정보 GUI 시각화
- `MapDataScriptable.cs`: 맵 데이터를 저장하는 SO 구조
- `Battle_MapDataManager.cs`: 런타임 중 맵 데이터 접근 매니저

---

## 🧠 설계 특징

- `Tile → Cell → Pixel` 기반의 계층적 맵 설계
- 상태 전이 기반 유닛 처리, 단일 명령 → 다중 유닛 처리 구조
- 커스텀 툴 기반 ScriptableObject 시각화 및 맵 값 관리
- A* 경로 탐색 알고리즘 내장 및 블록 기반 필터링 적용
- 전투 흐름과 시점 제어, 유닛 상태 분리가 모듈화되어 확장성과 테스트 용이성 확보

---

본 구조는 실무에서 "복합 전투 콘텐츠 설계", "툴 연동", "구조 분리"를 한 프로젝트 내에 모두 반영한 사례입니다.
