using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    // Singleton
    public static CoinManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI CurrentCoinText; // UI용 TMP 텍스트
    [SerializeField] private int initialCoins = 0; // 초기 코인 수
    [SerializeField] private GameObject notEnoughCoinsPanel; // 코인 부족 패널
    
    public int coin { get; private set; } // 현재 코인 수
    private const int RETRY_COST = 100; // 리트라이 비용
    
    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        coin = initialCoins;
        UpdateCoinDisplay();
    }
    
    // 코인 증가 함수 - ShopManager와 연동
    public void AddCoin(int amount)
    {
        coin += amount;
        UpdateCoinDisplay();
        Debug.Log($"코인이 {amount}개 증가했습니다. 현재 코인: {coin}");
    }
    
    // 게임 시작 전 코인량 체크
    public bool CheckCoin(int requiredAmount)
    {
        return coin >= requiredAmount;
    }
    
    // 코인이 100 미만일 경우
    public void CoinNotEnough()
    {
        notEnoughCoinsPanel.SetActive(true);
        Debug.Log("코인이 부족합니다! 게임을 시작하려면 최소 100코인이 필요합니다.");
    }
    
    // 코인이 충분할 경우 리트라이 가능
    public bool CoinEnough()
    {
        if (coin >= RETRY_COST)
        {
            coin -= RETRY_COST;
            UpdateCoinDisplay();
            Debug.Log($"게임 재시작! {RETRY_COST} 코인이 차감되었습니다. 남은 코인: {coin}");
            return true;
        }
        else
        {
            CoinNotEnough();
            return false;
        }
    }
    
    // 상대방 재대결 시 코인 차감 없음
    public void OnEnemyRegame()
    {
        Debug.Log("상대방의 재대결 신청! 코인이 차감되지 않습니다.");
    }
    
    // 코인 표시 업데이트
    private void UpdateCoinDisplay()
    {
        if (CurrentCoinText != null)
        {
            CurrentCoinText.text = coin.ToString();
        }
    }
}
