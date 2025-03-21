using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    // Singleton
    public static CoinManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI CurrentCoinText; // UI용 TMP 텍스트
    [SerializeField] private int initialCoins = 0; // 초기 코인 수
    [SerializeField] private GameObject notEnoughCoinsPanel; // 코인 부족 패널
    [SerializeField] private GameObject confirmStartGamePanel; // 게임 시작 확인 패널
    [SerializeField] private GameObject lowCoinPanel; // 코인 부족 시 상점 이동 안내 패널
    [SerializeField] private string gameSceneName = "GameScene"; // 게임 씬 이름
    
    public int coin { get; private set; } // 현재 코인 수
    private const int GAME_START_COST = 100; // 게임 시작 비용
    private const int RETRY_COST = 100; // 리트라이 비용
    
    // 저장 경로 설정
    private string saveFilePath;
    private const string saveFileName = "CoinData.json";
    
    [System.Serializable]
    private class SaveData
    {
        public int coins;
    }
    
    // 저장 경로 변경: Assets 폴더 내에 CoinData.json 저장
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 저장 경로: Unity의 Assets 폴더 내
            saveFilePath = Path.Combine(Application.dataPath, saveFileName);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadCoins(); // 코인 데이터 불러오기
        UpdateCoinDisplay(); // 즉시 UI 업데이트
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
                coin = data.coins; // 저장된 코인 불러오기
                Debug.Log($"코인 데이터를 불러왔습니다. 현재 코인: {coin}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"코인 데이터 로드 중 오류 발생: {e.Message}");
                coin = initialCoins; // 오류 발생 시 초기값 설정
            }
        }
        else
        {
            coin = initialCoins;
            Debug.Log($"저장된 코인 데이터가 없습니다. 초기 코인으로 설정: {initialCoins}");
        }

        UpdateCoinDisplay(); // 불러온 코인 값으로 UI 업데이트
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
    
    // 코인 감소 함수
    public bool SpendCoin(int amount)
    {
        if (coin >= amount)
        {
            coin -= amount;
            UpdateCoinDisplay();
            SaveCoins(); // 코인 변경 시 저장
            Debug.Log($"코인이 {amount}개 감소했습니다. 남은 코인: {coin}");
            return true;
        }
        return false;
    }
    
    // 게임 시작 전 코인량 체크
    public bool CheckCoin(int requiredAmount)
    {
        return coin >= requiredAmount;
    }
    
    // 게임 시작 확인 버튼 클릭 시 호출
    public void OnClickStartGame()
    {
        if (CheckCoin(GAME_START_COST))
        {
            // 코인이 충분하면 확인 패널 표시
            if (confirmStartGamePanel != null)
            {
                confirmStartGamePanel.SetActive(true);
            }
        }
        else
        {
            // 코인이 부족하면 상점 이동 안내 패널 표시
            if (lowCoinPanel != null)
            {
                lowCoinPanel.SetActive(true);
            }
        }
    }
    
    // 확인 패널에서 확인 버튼 클릭 시 호출
    public void ConfirmStartGame()
    {
        if (SpendCoin(GAME_START_COST))
        {
            // 게임 씬으로 이동
            SceneManager.LoadScene(gameSceneName);
        }
    }
    
    // 확인 패널에서 취소 버튼 클릭 시 호출
    public void CancelStartGame()
    {
        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }
    }
    
    // 상점 이동 확인 패널에서 확인 버튼 클릭 시 호출
    public void ConfirmGoToShop()
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
    
    // 상점 이동 확인 패널에서 취소 버튼 클릭 시 호출
    public void CancelGoToShop()
    {
        if (lowCoinPanel != null)
        {
            lowCoinPanel.SetActive(false);
        }
        
        if (confirmStartGamePanel != null)
        {
            confirmStartGamePanel.SetActive(false);
        }
    }
    
    // 게임 플레이 후 재시작 시 코인 체크
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
    
    // 코인이 부족할 경우 표시
    public void CoinNotEnough()
    {
        if (notEnoughCoinsPanel != null)
        {
            notEnoughCoinsPanel.SetActive(true);
        }
        Debug.Log("코인이 부족합니다! 게임을 시작하려면 최소 100코인이 필요합니다.");
    }
    
    // 상대방 재대결 시 코인 차감 없음
    public void OnEnemyRegame()
    {
        Debug.Log("상대방의 재대결 신청! 코인이 차감되지 않습니다.");
    }
    
    // 코인 표시 업데이트
    public void UpdateCoinDisplay()
    {
        if (CurrentCoinText != null)
        {
            CurrentCoinText.text = coin.ToString();
        }

        // 모든 CoinDisplay UI 업데이트
        CoinDisplay[] coinDisplays = FindObjectsOfType<CoinDisplay>();
        foreach (var display in coinDisplays)
        {
            display.UpdateCoinText();
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