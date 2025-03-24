using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class LowCoinPanel : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private int requiredCoins = 100;
    [SerializeField] private StorePanel mainStorePanel; // 인스펙터에서 할당할 스토어 패널
    [Tooltip("메인 상점 패널을 여기에 직접 연결하세요")]

    private GameStartManager gameStartManager;

    private void Awake()
    {
        // 기본적으로 패널은 비활성화 상태로 시작
        gameObject.SetActive(false);

        // GameStartManager 찾기
        gameStartManager = FindObjectOfType<GameStartManager>();

        // 버튼 이벤트 설정
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        // 상점 패널이 설정되지 않았다면 경고 로그 출력
        if (mainStorePanel == null)
        {
            Debug.LogWarning("LowCoinPanel의 mainStorePanel이 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
        }
    }

    private void OnEnable()
    {
        // 패널이 활성화될 때마다 메시지 업데이트
        if (messageText != null)
        {
            int currentCoins = CoinManager.Instance ? CoinManager.Instance.coin : 0;

            messageText.text = $"코인이 부족해요...\n(현재: {currentCoins}, 필요: {requiredCoins})\n상점으로 이동할까요?";
        }
    }

    private void OnConfirmClicked()
    {
        if (mainStorePanel != null)
        {
            // 참조 저장
            StorePanel storePanel = mainStorePanel;
        
            // 상점 패널 활성화 후 현재 패널 비활성화
            storePanel.Show();
        
            // 현재 패널 비활성화
            gameObject.SetActive(false);
        
            // 디버그 로그
            Debug.Log("LowCoinPanel에서 StorePanel 활성화됨");
        }
        else
        {
            Debug.LogError("mainStorePanel이 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
        }
    }

    private IEnumerator ShowStorePanelDelayed(StorePanel storePanel)
    {
        // 한 프레임 대기
        yield return null;
        
        // 스토어 패널이 아직 유효한지 확인
        if (storePanel != null)
        {
            // 패널이 이미 활성화되어 있는지 확인
            if (!storePanel.gameObject.activeSelf)
            {
                // 단순히 패널 표시 (UIManager는 직접 사용하지 않음)
                storePanel.Show();
                
                Debug.Log("LowCoinPanel에서 StorePanel 활성화됨");
            }
        }
    }

    // 취소 버튼 클릭 시
    private void OnCancelClicked()
    {
        if (gameStartManager != null)
        {
            gameStartManager.OnCancelGoToShop();
        }
    }
}