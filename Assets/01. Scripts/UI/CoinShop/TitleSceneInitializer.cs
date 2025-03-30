using UnityEngine;
using UnityEngine.UI;

public class TitleSceneInitializer : MonoBehaviour
{
    [SerializeField] private Button startButton; // 시작 버튼 참조
    [SerializeField] private GameStartManager gameStartManager; // 게임 시작 매니저 참조

    private void Awake()
    {
        // 시작 시 초기화
        InitializeComponents();
    }

    private void OnEnable()
    {
        // 타이틀 씬이 활성화될 때마다 컴포넌트 초기화
        InitializeComponents();
    }

    private void Start()
    {
        // PlayerPrefs를 통해 게임에서 돌아왔는지 확인
        if (PlayerPrefs.GetInt("ReturnedFromGame", 0) == 1)
        {
            // 게임에서 돌아온 경우 추가 초기화 수행
            Debug.Log("TitleSceneInitializer: 게임에서 타이틀로 돌아옴, 추가 초기화 수행");
            
            // 초기화 완료 후 플래그 리셋
            PlayerPrefs.SetInt("ReturnedFromGame", 0);
            PlayerPrefs.Save();
            
            // UI 컴포넌트 강제 리프레시
            ForceRefreshUI();
        }
    }

    private void InitializeComponents()
    {
        // 게임 시작 매니저가 Inspector에서 할당되지 않았다면 찾기
        if (gameStartManager == null)
        {
            gameStartManager = FindObjectOfType<GameStartManager>();
        }

        // 시작 버튼이 Inspector에서 할당되지 않았다면 찾기
        if (startButton == null)
        {
            // 버튼 찾기 (태그나 이름으로 검색 가능)
            startButton = GameObject.FindGameObjectWithTag("StartButton")?.GetComponent<Button>();
            
            // 또는 일반적인 방법으로 찾기
            if (startButton == null)
            {
                startButton = GameObject.Find("StartButton")?.GetComponent<Button>();
            }
        }

        // 버튼 이벤트 초기화
        if (startButton != null && gameStartManager != null)
        {
            // 기존 이벤트 제거 후 새로 등록
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartButtonClicked);
            Debug.Log("TitleSceneInitializer: 시작 버튼 이벤트 등록 완료");
        }
        else
        {
            if (startButton == null)
                Debug.LogError("TitleSceneInitializer: 시작 버튼을 찾을 수 없습니다!");
            
            if (gameStartManager == null)
                Debug.LogError("TitleSceneInitializer: GameStartManager를 찾을 수 없습니다!");
        }
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("TitleSceneInitializer: 시작 버튼 클릭됨");
        
        // 게임 시작 매니저의 메서드 호출
        if (gameStartManager != null)
        {
            // GameStartManager의 private 메서드를 호출할 수 없으므로
            // 이 클래스에 public 메서드를 추가해야 합니다
            // 아래는 GameStartManager에 추가할 public 메서드를 직접 호출하는 예시입니다
            gameStartManager.TriggerStartButton();
        }
    }

    private void ForceRefreshUI()
    {
        // UI 요소들을 재활성화하여 강제 리프레시
        
        // 1. 게임 시작 매니저 비활성화 후 다시 활성화
        if (gameStartManager != null)
        {
            gameStartManager.gameObject.SetActive(false);
            gameStartManager.gameObject.SetActive(true);
        }
        
        // 2. 시작 버튼 비활성화 후 다시 활성화
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
        
        // 3. 레이아웃 리프레시 (Canvas에 LayoutGroup이 있는 경우)
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            // 캔버스 재활성화로 레이아웃 강제 갱신
            canvas.enabled = false;
            canvas.enabled = true;
        }
    }
}