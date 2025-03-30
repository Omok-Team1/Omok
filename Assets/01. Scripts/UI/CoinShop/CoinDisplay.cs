using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI coinText;

    private void Awake()
    {
        coinText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        UpdateCoinText(); // 게임 시작 시 즉시 반영
    }

    public void UpdateCoinText()
    {
        if (coinText != null && CoinManager.Instance != null)
        {
            coinText.text = CoinManager.Instance.coin.ToString();
        }
    }
}
