
# 🛡️ Project T - 전투 시스템 하이라이트

본 레포지토리는 Unity 기반 프로젝트 *"Project T"* 에서 구현한 **전투 시스템의 핵심 컴포넌트**를 선별하여 정리한 포트폴리오 하이라이트입니다.  
RTS 스타일의 유닛 제어, 실시간 경로 탐색, 스킬 기반 전투 로직을 중심으로 구성되어 있습니다.

---

## 📁 주요 코드 구성

| 파일 | 역할 설명 |
|------|------------|
| `Army_ComBatController.cs` | 근접 유닛 간 난전 처리용 A* 경로 탐색 컨트롤러 |
| `Battle_MapDirector.cs` | 유닛 이동, 스킬 범위 판정에 필요한 픽셀/타일 위치 정보 제공 |
| `BattleArmy.cs` | 군단 단위의 유닛 상태, 이동, 전투 상태 전반을 제어 |
| `BattleArmyCell.cs` | 군단의 위치와 목표 이동 좌표를 기준으로 회전 및 이동 처리 |
| `BattleBaseUnit.cs` | 개별 유닛의 이동, 상태 전환, 애니메이션 연동 처리 |
| `BattleBaseUnit_Status.cs` | 유닛의 스탯, 이동 속도, 타겟 정보 및 회전 처리 관리 |
| `BattleData.cs` | 전투 유닛의 상태, 스킬 타입, 타겟 정보 등을 포함한 데이터 구조 |
| `BattleEngine.cs` | 전투 시스템 전체 실행 루프 및 상태 갱신 제어 |
| `BattleEngine_Action.cs` | 마우스 입력 기반의 유닛 선택, 이동 및 공격 명령 처리 |
| `BattleEngine_Skill.cs` | 스킬 발동, 범위 판정, 효과 적용 (투사체, 버프, 데미지) 등 처리 |

---

## 🧠 주요 구현 포인트

- 컴포넌트 분리 기반의 유닛 설계 (`BattleBaseUnit`, `Status` 등)
- **난전 상황 대응 A\* 경로 탐색 알고리즘** 구현
- **비동기 스킬 연출 및 투사체 풀링 적용**
- UI 선택/입력과 실제 유닛 반응 로직의 분리 구조
- BattleData와 Enum 기반의 **데이터 주도 설계 방식**

---

## 🕹️ 사용 목적

이 레포지토리는 실제 프로젝트의 전체 코드가 아닌,  
**전투 시스템 구현 역량을 보여주기 위한 하이라이트 코드 모음**입니다.

- 실시간 의사결정 및 명령 체계
- 구조적 사고 기반의 설계 능력
- 게임 플레이 중심 시스템 설계 역량 강조

---

## 📎 관련 자료

- 📄 [기술 문서 (한글)](https://docs.google.com/document/d/1fjIJAN8CZI7dHBuPFz5Cqr_Aoy1z5vLcnoMCQkYuOXc/edit?usp=sharing)
- 🎬 [게임플레이 시연 영상](https://www.youtube.com/watch?v=nYjViIJPWa0)

