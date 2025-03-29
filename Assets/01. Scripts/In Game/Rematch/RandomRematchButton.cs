using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class RandomRematchButton : MonoBehaviour
{
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private int requiredCoins = 100;

    public void OnClick()
    {
        if (CoinManager.Instance.CheckCoin(requiredCoins))
        {
            confirmPanel.SetActive(true);
        }
        else
        {
            CoinManager.Instance.CoinNotEnough();
        }
    }

    public void Confirm()
    {
        if (CoinManager.Instance.SpendCoin(requiredCoins))
        {
            // 타이틀 씬을 거쳐 인게임 씬으로 전환
            SceneManager.LoadScene("Title");
            SceneManager.LoadScene("InGame");
        }
        confirmPanel.SetActive(false);
    }

    public void Cancel()
    {
        confirmPanel.SetActive(false);
    }
}