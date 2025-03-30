using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotEnoughCoinsPanel : MonoBehaviour
{
    [SerializeField] private Button returnToTitleButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string defaultMessage = "코인이 부족합니다.\n타이틀로 돌아가시겠습니까?";
    
    private RandomRematchButton rematchButton;
    
    private void Awake()
    {
        // RandomRematchButton 참조 찾기
        rematchButton = FindObjectOfType<RandomRematchButton>();
        
        if (rematchButton == null)
        {
            Debug.LogError("RandomRematchButton을 찾을 수 없습니다!");
        }
    }
    
    private void OnEnable()
    {
        // 초기화
        if (messageText != null)
        {
            messageText.text = defaultMessage;
        }
        
        // 버튼 이벤트 설정
        if (returnToTitleButton != null)
        {
            // 모든 이전 리스너 제거
            returnToTitleButton.onClick.RemoveAllListeners();
            
            // 리매치 버튼이 있는지 확인 후 타이틀로 돌아가는 메소드 연결
            if (rematchButton != null)
            {
                returnToTitleButton.onClick.AddListener(rematchButton.ReturnToTitle);
            }
        }
        
        if (closeButton != null)
        {
            // 모든 이전 리스너 제거
            closeButton.onClick.RemoveAllListeners();
            
            // 패널 닫기 리스너 추가
            closeButton.onClick.AddListener(() => {
                gameObject.SetActive(false);
                
                // 필요한 경우 리매치 버튼 다시 활성화
                if (rematchButton != null)
                {
                    rematchButton.EnableButton();
                }
            });
        }
    }
    
    private void OnDisable()
    {
        // 버튼 이벤트 해제 (중복 등록 방지)
        if (returnToTitleButton != null)
        {
            returnToTitleButton.onClick.RemoveAllListeners();
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}