using UnityEngine;
using TMPro;
using System.IO;

public class CoinManager : MonoBehaviour
{
    // Singleton
    public static CoinManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI CurrentCoinText; // UI용 TMP 텍스트
    [SerializeField] private int initialCoins = 0; // 초기 코인 수
    [SerializeField] private GameObject notEnoughCoinsPanel; // 코인 부족 패널
    
    public int coin { get; private set; } // 현재 코인 수
    private const int RETRY_COST = 100; // 리트라이 비용
    
    // 저장 경로 설정
    private string saveFilePath;
    private const string saveFileName = "CoinData.json";
    
    [System.Serializable]
    private class SaveData
    {
        public int coins;
    }
    
    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 저장 경로 설정 - 에셋 폴더 내 저장
            saveFilePath = Path.Combine(Application.dataPath, saveFileName);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadCoins();
        UpdateCoinDisplay();
    }
    
    // 코인 로드 함수
    private void LoadCoins()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
                coin = data.coins;
                Debug.Log($"코인 데이터를 불러왔습니다. 현재 코인: {coin}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"코인 데이터 로드 중 오류 발생: {e.Message}");
                coin = initialCoins;
            }
        }
        else
        {
            coin = initialCoins;
            Debug.Log($"저장된 코인 데이터가 없습니다. 초기 코인으로 설정: {initialCoins}");
        }
    }
    
    // 코인 저장 함수
    private void SaveCoins()
    {
        try
        {
            SaveData data = new SaveData
            {
                coins = coin
            };
            
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFilePath, jsonData);
            Debug.Log($"코인 데이터를 저장했습니다. 저장 경로: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"코인 데이터 저장 중 오류 발생: {e.Message}");
        }
    }
    
    // 코인 증가 함수 - ShopManager와 연동
    public void AddCoin(int amount)
    {
        coin += amount;
        UpdateCoinDisplay();
        SaveCoins(); // 코인 변경 시 저장
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
            SaveCoins(); // 코인 변경 시 저장
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
    
    // 애플리케이션 종료 시 저장
    private void OnApplicationQuit()
    {
        SaveCoins();
    }
    
    // 씬 변경 시 저장
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SaveCoins();
        }
    }
}