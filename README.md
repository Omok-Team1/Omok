# 오목 프로젝트
+ 렌주룰이 적용된 오목을 온라인 대국과 인공지능 대국으로 즐길 수 있는 오목 프로젝트입니다.
![Image](https://github.com/user-attachments/assets/783aa0ef-fde4-4d08-bf9f-f75fe660b436)

# 개발 기간
+ 25/03/11 ~ 25/03/31

# 팀원
+ 박종한, 이채은, 유승종, 조재현, 서보람, 김동욱

# 담당 업무
1. 인 게임 구조 설계 및 구현   
  State Pattern을 활용한 게임 상태 관리, Event Queue를 이용해 협업, Custom Scene Loader

2. 렌주룰 구현(3-3, 4-4 탐지 알고리즘)   
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
### 코드 위치 : [147 ~ 392번 라인](https://github.com/Omok-Team1/Omok/blob/ParkJongHan/Assets/01.%20Scripts/In%20Game/ManagerController/BoardManager.cs#L147)
1. Requirements는 아래와 같았습니다.   
> 일반 렌주룰:   
> i. 흑이 유리한 입장을 보완하기 위해 3x3, 혹은 4x4가 불가능하게 하는 것입니다.   
> ii. 렌즈룰에 의해 착수할 수 없는 곳은 “x” 마크가 표시 되어야 합니다.
2. 렌주룰에 대해서 찾아보고 문제에 대해서 먼저 분석했습니다.   
![Image](https://github.com/user-attachments/assets/5bd9c02b-776f-4e8e-b46b-2beceef0a626)
3. 3-3/4-4는 한 수를 추가해서 3개, 4개의 돌들이 직선 형태로 나란히 연결되어 있는 형태를 **동시에 2개 만들 수 있는 경우**입니다. (이 때 한 칸 정도 띄어져 있는 형태까지는 허용합니다.)

### 해결
1. 먼저 15x15의 보드판을 이중 for-loop로 순회하며 해당 칸이 3-3/4-4의 가능성이 있는지 체크하는 것은 매우 비효율적이라 판단해 먼저 **문제의 범위를 줄일 수 있는지 생각했습니다.**
2. 3-3/4-4는 현재 보드판 위에 자신의 돌들 중에서 **자신의 8 방향 중 한 곳이라도 빈 칸이 존재하는 곳**이어야 합니다.
![Image](https://github.com/user-attachments/assets/0ad15912-66b2-4d90-b84c-70fad6161eeb)
3. 또한, 해당 방향의 빈 칸이 3-3/4-4의 자리가 아니더라도 "띈 3", "띈 4"도 3-3/4-4를 성립시키므로 현재 검사한 빈 칸으로부터 빈 칸이 검출된 방향으로 한 칸 더 위치한 곳도 3-3/4-4를 검사해야합니다.
![Image](https://github.com/user-attachments/assets/50da4458-5451-45b6-ad43-90c6177f326c)
4. 해당 빈 칸이 3-3/4-4인지는 base-condition을 정의하고 해당 위치로부터 **8 방향을 재귀적으로** 검사했습니다. 재귀적으로 자신이 지금 보는 칸이 "빈칸", "자신의 돌", "상대 돌", "보드 밖"인지를 판단하면서 "자신의 돌"이 해당 방향으로 몇 개가 있는지 카운트해서 반환합니다.
![Image](https://github.com/user-attachments/assets/41edabe8-64f4-465b-b67b-4f0b00b91526)

# AI 연산 비동기화
### 코드 위치 : [33 ~ 61번 라인](https://github.com/Omok-Team1/Omok/blob/ebbf28985b4fc2f2e9fbead82d168464d80e6e2b/Assets/01.%20Scripts/In%20Game/ManagerController/OpponentController.cs#L33)
### 문제 발견   
1. AI를 담당 했던 팀원분이 작업한 GetBestMove() 메소드를 호출 했을 때 Timer를 비롯한 모든 UI의 애니메이션이 정지되는 문제가 있었습니다.
2. (1)을 해결하기 위해 UniTask로 백그라운드 스레드에서 연산 시켜 해결했을 때 Unity Event Update()안에서 구현된 Timer 기능이 의도하지 않게 작동하는 문제가 있었습니다.
  (Time-out 되었을 때, EventMessage를 보내, CancelationTokenSource를 이용해 스레드를 강제로 종료하는 구조에서, Update()와 동기화가 잘 되지 않아, 이미 cancel()을 호출한 Token을 다시 cancel()을 시도하는 등의 문제가 생겼습니다.)
3. AI 연산이 BoardGrid의 데이터를 임시로 수정 후 다시 복구하며 연산하는데, cancel()될 때 원본 데이터 무결성을 위해 Clone Method를 구현해 복사본 위에서 연산을 진행하게 만들었습니다.
   (이 때 Timer의 Update()와 스레드가 동기화 되지 않았기에, Clone()되는 순서도 보장이 되지 않아 AI 연산이 계속 같은 위치만 반환하는 문제도 생겼습니다.) 

### 해결
1. 최대한 다른 분이 작업한 Timer를 수정하고 싶지 않았기에, 먼저 Update()와 동기화를 시도했습니다.
2. 이 과정에서 Task Queue를 활용해 실행 순서를 제어하려 시도했지만, 스레드 작업 미숙으로 인해 실패했습니다.
3. Update()로 작업하신 것을 제어할 수 있게 코루틴으로 수정하고 Token 또한 cancel()을 호출 한 message에서 자원을 정리하게 만들었습니다.
![Image](https://github.com/user-attachments/assets/ecd7df82-ff07-40b4-8b22-d40474ccf39e)
4. 스레드 작업 전 Timer의 기능이 완전 끝날 때까지 기다린 후 스레드 작업을 시작하게 만들고 cancel()시 좌표값을 "음의 무한대"를 반환하게 하여 예외 처리를 진행하여 해결하였습니다.
5. 유니티의 Transform, GetComponent와 같은 메소드들이 메인 스레드에서만 호출 가능하다는 것을 알았습니다.
6. 또한 async / await의 활용에 대해 조금 더 익숙해 질 수 있었습니다.
