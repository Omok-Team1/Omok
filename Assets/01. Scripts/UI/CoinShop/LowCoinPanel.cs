using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LowCoinPanel : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private int requiredCoins = 100;
    
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

        // 메시지 설정
        if (messageText != null)
        {
            messageText.text = $"코인이 부족해서 게임을 할 수 없어요. 상점으로 이동할까요?";
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
        if (gameStartManager != null)
        {
            gameStartManager.OnConfirmGoToShop();
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