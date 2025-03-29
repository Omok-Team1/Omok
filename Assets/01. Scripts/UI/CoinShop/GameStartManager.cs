using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private Button startButton; // 시작 버튼 레퍼런스
    [SerializeField] private GameObject confirmStartGamePanel; // 게임 시작 확인 패널
    [SerializeField] private GameObject lowCoinPanel; // 코인 부족 시 상점 이동 안내 패널
    [SerializeField] private int gameCost = 100; // 게임 시작에 필요한 코인
    [SerializeField] private SceneId targetScene; // 로드할 게임 씬 ID
    
    
    public void TriggerStartButton()
    {
        // 기존 OnStartButtonClicked() 메서드 호출
        OnStartButtonClicked();
    }
    
    private void Awake()
    {
        // 확인 패널과 코인 부족 패널은 시작시 비활성화
        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }
        
        if (lowCoinPanel != null)
        {
            lowCoinPanel.SetActive(false);
        }
        
        // 시작 버튼에 새로운 onClick 이벤트 등록
        if (startButton != null)
        {
            // 기존 이벤트 제거
            startButton.onClick.RemoveAllListeners();
            
            // 새로운 onClick 이벤트 등록
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }
    
    private void OnStartButtonClicked()
    {
        // 코인 확인
        if (CoinManager.Instance != null && CoinManager.Instance.CheckCoin(gameCost))
        {
            // 코인이 충분하면 확인 패널 활성화
            if (confirmStartGamePanel != null)
            {
                confirmStartGamePanel.SetActive(true);
            }
        }
        else
        {
            // 코인이 부족하면 상점 이동 안내 패널 활성화
            if (lowCoinPanel != null)
            {
                lowCoinPanel.SetActive(true);
            }
        }
    }
    
    // 확인 패널에서 확인 버튼 클릭 시 호출
    public void OnConfirmStartGame()
    {
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoin(gameCost))
        {
            // 코인 차감 성공 시 직접 씬 로드
            if (targetScene != null)
            {
                targetScene.Load();
            }
            else
            {
                Debug.LogError("대상 씬(targetScene)이 설정되지 않았습니다.");
            }
        }
        
        // 패널 닫기
        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }
    }
    
    // 확인 패널에서 취소 버튼 클릭 시 호출
    public void OnCancelStartGame()
    {
        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }
    }
    
    // 상점 이동 패널에서 확인 버튼 클릭 시 호출
    public void OnConfirmGoToShop()
    {
        // 모든 패널 닫기
        if (lowCoinPanel != null)
        {
            lowCoinPanel.SetActive(false);
        }

        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }

        // 상점 패널 열기
        StorePanel storePanel = FindObjectOfType<StorePanel>();
        if (storePanel != null)
        {
            storePanel.Show();
        }
        else
        {
            Debug.LogError("상점 패널을 찾을 수 없습니다!");
        }
    }
    
    // 상점 이동 패널에서 취소 버튼 클릭 시 호출
    public void OnCancelGoToShop()
    {
        if (lowCoinPanel != null)
        {
            lowCoinPanel.SetActive(false);
        }
    }
    
    public void ResetState()
    {
        // 확인 패널과 코인 부족 패널 비활성화
        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }
    
        if (lowCoinPanel != null)
        {
            lowCoinPanel.SetActive(false);
        }
    
        // 시작 버튼 이벤트 재등록
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartButtonClicked);
            Debug.Log("GameStartManager: 버튼 이벤트 재등록 완료");
        }
    }
}