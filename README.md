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
4. 해당 빈 칸이 3-3/4-4인지는 해당 위치로 부터 base-condition을 정의하고 **8 방향을 재귀적으로** 검사했습니다.
![Image](https://github.com/user-attachments/assets/41edabe8-64f4-465b-b67b-4f0b00b91526)


# AI 연산 비동기화
