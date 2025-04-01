# 오목 프로젝트
+ 렌주룰이 적용된 오목을 온라인 대국과 인공지능 대국으로 즐길 수 있는 오목 프로젝트입니다.
+ 

# 개발 기간
+ 25/03/11 ~ 25/03/31

# 팀원
+ 박종한, 이채은, 유승종, 조재현, 서보람, 김동욱

# 담당 업무
1. 인 게임 구조 설계 및 구현
  State Pattern을 활용한 게임 상태 관리, Event Queue를 이용해 협업, Custom Scene Loader

2. 승리 체크, 렌주룰 구현(3-3, 4-4 탐지 알고리즘)
  Back-Tracking을 활용한 완전 탐색 알고리즘

3. AI 연산 비동기화
  UniTask 라이브러리를 활용해 UI를 위한 메인 스레드에서 작업 분리

# 인 게임 구조 설계 및 구현
### 게임 상태 정의, 관리
### 코드 위치 : [In Game State](https://github.com/Omok-Team1/Omok/tree/ParkJongHan/Assets/01.%20Scripts/In%20Game/StateMachine/Game%20States/Game%20Play%20States)

![Image](https://github.com/user-attachments/assets/96348d68-e5d4-45b2-a540-83149de26e20)
1. 턴제 게임인 오목을 게임 상태를 정의해 각 State에 기능에 대한 책임을 부여해 FSM으로 관리하면 Requirements에서 도출된 기능과 클래스들을 효과적으로 인 게임 담당 팀원들과 협업해 개발할 수 있을 것이라 생각했습니다.
2. State들을 정의하고 클래스들에게 각각의 State들에서 동작해야 하는 기능들을 부여한 후 팀원들에게 역할(AIController, TimerController 개발)을 분배했습니다.

### Cell, BoardGrid, BoardManager 정의
### 코드 위치 : [Cell, BoardGrid 클래스](https://github.com/Omok-Team1/Omok/tree/ParkJongHan/Assets/01.%20Scripts/In%20Game/Grid)
![Image](https://github.com/user-attachments/assets/90710652-8e10-4b1a-81fd-6e9978b9cf4c)
3. Board판과 Board판의 원소 Cell을 정의하고 Baord에 대한 연산을 지원하는 BoardManager를 정의하여 관리하였습니다.

### Event Queue를 이용해 협업
### 코드 위치 : [EventManager, Message](https://github.com/Omok-Team1/Omok/tree/ParkJongHan/Assets/01.%20Scripts/Utill/EventQueue)
1. 다른 팀원에게 데이터를 넘겨주거나(대국 정보, 로그인시 ID, PW 등등) 어떤 클래스의 동작 혹은 이벤트를 기다려야 하는 의존적인 상황을 독립적으로 분리하기 위해 사용했습니다.
![Image](https://github.com/user-attachments/assets/186cd4fe-063e-4892-a4c0-e20c176fc1e8)
2. Singleton으로 구현하여 어디서든 접근 할 수 있게 하였고 AddListener를 통해 이벤트를 수신받고 해당 이벤트를 발신한 객체가 보낸 데이터를 전달 받을 수 있습니다.
![Image](https://github.com/user-attachments/assets/8596f824-680a-4e9d-b557-18ade3206d57)
3. 이벤트를 발신하려는 객체는 EventMessage 객체를 만들어 AddParameter<T>(object)를 통해 메세지에 전달한 데이터를 추가할 수 있고 EventManager의 PushEventMessage()와 PublishEventMessageQueue()를 통해 발신 할 수 있습니다.
![Image](https://github.com/user-attachments/assets/7cc5c1f0-c58c-4eea-802a-d459bc4ffbba)
4. 추가적으로 Block-Coding 느낌으로 기능을 ScriptableObject로 만들어 인스펙터에서만 작업할 수 있도록 구현해봤습니다.


### Custom Scene Loader

# 렌주룰 구현(3-3, 4-4 탐지 알고리즘)

# AI 연산 비동기화

## 급수 시스템

-급수는 18급에서 1급까지 총 18단계

-플레이어는 현재 급수에서 연속으로 3번을 이기면 급수가 올라가고, 연승 중 패를 하면 승급 포인트가 1회 차감됩니다.

처음에는 승급 포인트가 0이었다가 1번 승리하면 승급 포인트가 1

2번 연속 승리하면 승급 포인트가 2 

그리고 또 한 번 승리하면 승급 포인트가 3이되어 다음 급수로 승급합니다.

1급이 되었을 땐 더 이상 승급이 되지 않습니다.

연승 횟수는 -1, -2로도 떨어질 수 있는데, -3까지 떨어지면 한 단계 낮은 급수로 강등됩니다.


## AI 플레이

-AI 플레이는 난이도를 구분하여 쉬운 AI에서 부터 어려운 AI까지 나누어 집니다.

-낮은 급수를 가진 플레이어와 매칭되는 AI는 쉬운 난이도의 AI이고 높은 급수를 가진 플레이어와는 어려운 난이도의 AI와 매칭합니다.

## 메인 게임플레이

-일반 렌즈룰:
흑이 유리한 입장을 보완하기 위해 3x3, 혹은 4x4가 불가능하게 하는 것입니다.

-게임은 한 턴에 30초의 시간이 주어집니다. 30초를 초과하면 패하게 됩니다.

기물을 배치하기 위해서는 터치 후 “착수” 버튼을 클릭해야 합니다.

렌즈룰에 의해 착수할 수 없는 곳은 “x” 마크가 표시 되어야 합니다.

게임 중에는 누구의 턴인지를 UI를 통해 표시합니다.


## 랭킹 시스템

-급수를 기반으로 랭킹을 표시

-동일한 급수끼리는 승률을 바탕으로 랭킹

## 기보 시스템

-자신이 플레이한 기록을 다시 돌려볼 수 있는 기능을 제공

-한 턴 씩 게임을 재생할 수 있습니다.


## 코인 시스템

-오목 게임에서는 코인이라는 재화가 사용됩니다

-상대방과 플레이 후 상대방이 재대국을 신청하면 신청한 유저는 코인이 차감되지만 자신의 코인은 차감되지 않습니다.

-게임을 플레이하기 위해서는 100 코인이 필요(상점에서 구매)

## 상점

-코인을 구매

-광고 시청 후 500 코인을 획득할 수 있는 항목이 존재

## 유저 관리

-멀티플레이와 싱글플레이 모두 최초 게임 진입 시 회원가입 화면을 통해 유저 정보를 입력해야 함

-이 때 입력하는 정보

:아이디(이메일 주소를 사용하고, 중복된 사용자 생성은 막습니다)

:비번(암호화 되어 저장되어야 합니다)

:프로필 이미지(디폴트 제공, 하나를 선택)
