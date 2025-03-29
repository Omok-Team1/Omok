using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class EnemyRematchConfirmPanel : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "In Game";
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void ConfirmRematch()
    {
        // 코인 소모 없이 씬 재시작
        SceneManager.LoadScene(gameSceneName);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}