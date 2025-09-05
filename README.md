# 스파르타 내일배움캠프 유니티 11기 9주차 17조 - Tartaros

<!-- 
<p align="center">
<br>
  <img src="./Images/Playing.gif">
  <br>
</p> 
 -->

## 프로젝트 소개
### CrossAin:Tartaros
미궁을 탐사해 얻은 아이템으로 미궁을 공략하자!!
- 다양한 전략전술과 RPG적 성장 시스템을 결합한 몰입형 미궁 공략
- 미궁을 공략하면 그 경험과 전술이 다음 미궁공략에 도움이 된다.
- 핵심 재미요소 : 공략, 성장, 전술전략
    - 스테이지를 클리어하면서 캐릭터를 성장시키는 재미
    - 미궁을 탐사하며 얻은 아이템으로 다음 스테이지를 공략
    - 위 두가지 요소를 결합해 다양한 전략전술, 성장을 통해 클리어시 성취감을 부여

## 기술 스택

| C# | .Net | Unity |
| :--------: | :--------: | :--------: |
|   ![csharp]    |   ![dotnet]    |   ![unity]    |

<br>

## 구현 기능

### 필수 기능
- 주인공 캐릭터의 이동 및 기본 동작
  - 키보드 입력: 기본 이동 WASD, 점프 SPACE, 달리기 Shift + A, D
  - 마우스 입력: 좌클릭 - 공격/패링
- 레벨 디자인 및 적절한 게임 오브젝트 배치
  - 2개 이상의 스테이지 배치: 튜토리얼 스테이지, 보스 스테이지
- 충돌 처리 및 피해량 계산
  - 플레이어/몬스터 공격 상호작용
  - 피해량 계산식
      - 플레이어가 받는 피해량 = 몬스터의 공격력 - 플레이어의 방어력
      - 몬스터가 받는 피해량 = 플레이어의 공격력
  - 경직/사망 애니메이션
- UI/UX 요소
   - 게임 시작 메뉴 구현
   - 생명력 게이지, 오토 쉴드 잔량, 스테이지 달성도, 코인 획득량 구현
### 도전 기능
- 다양한 적 캐릭터와 그들의 행동 패턴 추가
    - 원거리 타입 몬스터 추가
    - 엘리트/보스 타입 몬스터 추가
        - 근거리 타입 + 원거리 타입 조합형
    - 근거리 타입 몬스터 순찰/추격 패턴 추가
- 다양한 무기나 아이템 추가
    - 룬 아이템(플레이어 기본 스탯에 영향)추가
    - 슬롯 아이템(오토쉴드 기능 추가. 정해진 횟수만큼 몬스터의 공격을 방어.)
- 다양한 환경과 배경 설정
    - 함정 기믹 설치
- 
<br>

## 콘텐츠
- 던전 스테이지 공략(스테이지의 모든 몬스터 토벌)
  - 패링(몬스터의 공격과 플레이어의 공격범위가 충돌할 때 발동)
- 슬롯 아이템 : 스테이지 클리어와 동시에 획득하는 추가 아이템으로서 플레이어의 스탯에는 직접적으로 영향을 주지 않고 플레이에 도움을 준다. 플레이어 사망시 슬롯 아이템이 들어가는 슬롯창은 초기화가 되며, 다음 회복의 샘과 상호작용시에 해당 항목이 복구가 된다.
  - 예시)오토 쉴드 : 총 세칸의 게이지를 가지며, 한번 피격될 때 마다 게이지가 한칸씩 소실이 되고, 3번 피격되면 게이지가 모두 소실되어 더이상 사용할 수 없다. 만약 게이지가 한칸 이상 3개 미만일 때 다음 스테이지 클리어 보상으로 오토 쉴드를 획득하면 오토쉴드의 게이지가 3칸으로 회복된다. 해당 아이템의 발동은 플레이어가 직접 발동하는것이 아닌 자동으로 발동한다.
- 룬 아이템 : 던전 입장전, 상점에서 구입 가능한 아이템으로 플레이어의 스탯에 직접적인 영향을 주는 아이템이다. 슬롯 아이템과는 차별적으로 플레이어 사망시에도 초기화 되지 않는다.

## 조작법
- W : 사다리 오르기
- A, D: 좌, 우 이동
- S : 웅크리기, 사다리 내려가기
- E : 상호작용 키(키오스크, 회복의 샘)
- 마우스 좌클릭 : 공격, 패링

## 상호작용 오브젝트 목록
- 키오스크 : 상점, 던전 입장
- 회복의 샘 : 체력 회복, 소실된 스탯과 아이템 목록 복구
- 포탈 : 다른 스테이지로 이동.
  - 파란색 : 이전 스테이지로 이동
  - 핑크색 : 다음 스테이지로 이동
  - 상호작용 방법 : 해당 포탈의 위치로 이동하면 자동으로 상호작용이 실행된다.
## 아이템 획득과 보상
- 아이템 획득 : 스테이지 클리어 후 다음 스테이지로 넘어간 직후 포탈 앞에서 아이템 선택 리스트 출력, 아이템 선택시 해당 리스트 비활성화와 함께 아이템 효과 적용이 된다.
- 보상 : 몬스터를 처치시 코인이 드랍되며, 해당 코인은 메인씬의 상점에서 룬 아이템을 구매하는 용도로 사용을 할 수 있다.

## 몬스터 리스트
- 워리어 타입 :
  - 공격 패턴 : 근거리 공격.
  - 특수 패턴 : 순찰, 추격. 리스폰 지점을 중심점으로 좌우 X좌표를 2씩 번갈아 움직인다. 이 범위에 플레이어의 좌표가 위치할 때, 추격 패턴으로 변경이 되며, 플레이어를 끝까지 추격하며 공격한다. 플레이어에게 공격을 받으면 넉백으로 진행 반대 방향으로 X좌표가 0.5 이동한다. 
- 런쳐 타입 :
  - 공격 패턴 : 원거리 공격
  - 특수 패턴 : 자리 고정. 런쳐 타입은 리스폰 지점에 좌표가 고정되며, 플레이어에게 피해를 받아도 넉백이 일어나지 않는다.
- 엘리트/보스 타입 :
    - 공격 패턴 : 근거리, 원거리 공격. 원거리 사거리 이내이면 원거리 공격을 실행하며, 근거리 공격 범위 안으로 플레이어의 좌표가 이동할 시 근거리 공격을 실행한다.
    - 특수 패턴 : 자리 고정, 근거리 원거리 공격 스위칭. 해당 몬스터도 런쳐 몬스터처럼 리스폰 위치에 고정이며 플레이어의 위치에 따라 원거리 공격과 근거리 공격을 스위칭 해서 사용한다.
## 소요 기간 : 5일

##사용 기술
- Unity 2022.3 LTS
- C# (게임 로직 및 시스템 구현)
- Git & GitHub (버전 관리 및 협업)
- Figma, Aseprite (UI 디자인 및 도트 리소스 제작)
- TextMeshPro, DoTween (UI 시스템 및 애니메이션)
- ScriptableObject (데이터 관리: 맵, 스킬, 몬스터 등)
- FSM (상태 전이 기반 AI)
- Generic Singleton (전역 매니저 관리)

## 주요 구현 기능
## 맵 & 탐험
- 랜덤 방/맵 로드 및 전환 (Stage1, Boss, Town)
- 샘물(회복 구조물) 상태 저장 → PlayerManager.waterUsed로 사용 여부 관리

## 몬스터 & 보스 AI
- 근거리/원거리 몬스터 FSM 구현 (Idle, Move, Attack, Hit, Death 등)
- 보스는 상태 전이 기반 FSM + 다양한 공격 패턴

## 플레이어 시스템
- 이동, 조준, 공격, 피격 처리
- 쉴드 아이템: 다단계 방어, 사용 시 VFX/사운드 출력
- 룬 시스템: 공격/방어 룬 구매 → 스탯 즉시 반영 + UI 동기화

## 상점 시스템
- Shop, ShopUIController 통해 상호작용 시 상점 열림
- 룬 구매 시 PlayerManager에서 즉시 적용, UI 자동 업데이트

## UI 시스템
- UIManager로 모든 UI 프리팹 관리 (동적 로드 & 캐싱)
- 상황별 UI (HealthBar, Progress, RuneHUD, ItemSelect, Setting, StartUI 등)
- 이벤트 기반 HUD 반영 (OnRuneOwnedChanged)

## 사운드 시스템
- SoundManager + SoundSource 기반
- 배경음, 효과음, 보스 등장음 등 상황별로 제어
- PlayerPrefs로 볼륨 저장 & 슬라이더 연동

## 데이터 관리
- PlayerData 직렬화 후 JSON 저장/로드
- 체력, 코인, 쉴드 수량, 룬 보유 상태 동기화
- Application.persistentDataPath/players.json에 저장

## 최적화 & 유틸
- Generic Singleton 패턴 (GameManager, PlayerManager, UIManager 등)
- 오브젝트 파괴/씬 전환 시 메모리 누수 방지 처리
- UI 메모리 정리 (UIManager.CleanAllUIs)

## 프로젝트 폴더 구조 (일부)
📦 Scripts/
├── 📂Manager/
│   ├── GameManager.cs
│   ├── PlayerManager.cs
│   ├── MapManager.cs
│   ├── SceneLoadManager.cs
│   ├── SoundManager.cs
│   └── UIManager.cs
├── 📂Monster/
│   ├── MonsterAnimationData.cs
│   ├── MonsterType.cs
│   └── ...
├── 📂Player/
│   ├── Player.cs
│   ├── PlayerStat.cs
│   ├── Shield.cs
│   └── Coin.cs
├── 📂UI/
│   ├── RuneHUD.cs
│   ├── HealthBar.cs
│   ├── ProgressUI.cs
│   ├── UICoin.cs
│   ├── UIShield.cs
│   ├── ItemSelectPanel.cs
│   ├── SettingPanel.cs
│   ├── SoundPanel.cs
│   ├── ScreenFader.cs
│   └── StartUI.cs
├── 📂System/
│   ├── Singleton.cs
│   ├── SceneBase.cs
│   └── ResourceManager.cs
└── 📂Etc/
    ├── MapType.cs
    ├── SceneType.cs
    └── Path.cs


## 개발자 역할 분담
개발
김세웅님 → 매니저 관리(게임/씬/UI), 아이템(쉴드), 상호작용, 맵 구조물(샘물/함정)
이혜림님 → 몬스터 FSM, 전투 로직, 저장 기능, 사운드 시스템
김유경님 → 플레이어 구현, 전투 시스템, 상점, 룬 시스템

기획
강인구님 → 게임 기획, UI 디자인, 사운드 리소스 제공
심낙형님 → 게임 기획, UI 디자인, 사운드 리소스 제공

## 사용 에셋 목록
https://aamatniekss.itch.io/fantasy-knight-free-pixelart-animated-character

https://xzany.itch.io/samurai-2d-pixel-art

https://bdragon1727.itch.io/basic-pixel-health-bar-and-scroll-bar

https://darkpixel-kronovi.itch.io/mecha-golem-free

https://penusbmic.itch.io/sci-fi-add-on-rust-town-planet-one

https://soundeffect-lab.info/

https://textcraft.net/?utm_source=chatgpt.com

https://drive.google.com/drive/folders/1Vvpd6x9P0Sk4UtnPJqR9rWpJiWeHnkja?usp=sharing

https://drive.google.com/drive/folders/1AOUWH4Um4oBdmLNiSZ9w2apgljtwvKlE?usp=sharing

https://immortal-burrito.itch.io/sickle-warrior

https://nyknck.itch.io/wood-set

https://beyonderboy.itch.io/top-down-lava-tileset-16x16

https://clash-monias.itch.io/free-16x16-shields-15-pack

https://kr.pinterest.com/pin/144889313006526812/

https://kr.pinterest.com/pin/158189005657929985/

https://opengameart.org/content/sideview-sci-fi-patreon-collection

https://drive.google.com/drive/folders/1wRwWl6yOh3u80p0YY_FFEbe7FVXonTnQ?usp=sharing

https://drive.google.com/drive/folders/1bKWKNvGRgyeKNjnLxWDS138BwQ38vaPu?usp=sharing

https://opengameart.org/content/shield-sprite

https://opengameart.org/content/shield-effect

## 라이센스

MIT &copy; CrossAin-Tartaros

<!-- Stack Icon Refernces -->

[csharp]: /Images/Csharp.png
[dotnet]: /Images/Dotnet.png
[unity]: /Images/Unity.png
