using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartPanel : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private int gameCost = 100;
    
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

        
    }

   

    // 확인 버튼 클릭 시
    private void OnConfirmClicked()
    {
        if (gameStartManager != null)
        {
            gameStartManager.OnConfirmStartGame();
        }
    }

    // 취소 버튼 클릭 시
    private void OnCancelClicked()
    {
        if (gameStartManager != null)
        {
            gameStartManager.OnCancelStartGame();
        }
    }
}