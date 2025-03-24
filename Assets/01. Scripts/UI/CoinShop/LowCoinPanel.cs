using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // 확인 버튼 클릭 시 (상점으로 이동)
    private void OnConfirmClicked()
    {
        if (mainStorePanel != null)
        {
            mainStorePanel.Show(); // 애니메이션 실행
            gameObject.SetActive(false); // 패널 닫기
        }
        else
        {
            Debug.LogError("mainStorePanel이 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
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