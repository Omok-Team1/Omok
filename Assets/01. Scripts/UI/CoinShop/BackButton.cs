using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    [SerializeField] private StorePanel targetStorePanel;
    
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    
        // 타겟 StorePanel 찾기
        if (targetStorePanel == null)
        {
            targetStorePanel = GetComponentInParent<StorePanel>();
        
            if (targetStorePanel == null)
            {
                targetStorePanel = FindObjectOfType<StorePanel>();
            }
        }
        
        if (targetStorePanel != null)
        {
            targetStorePanel.RegisterAlwaysActiveObject(gameObject);
        }
    }

    private void Start()
    {
        // 타겟 StorePanel이 설정되지 않았다면 경고 로그 출력
        if (targetStorePanel == null)
        {
            Debug.LogWarning("BackButton의 targetStorePanel이 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
            
            // 부모 계층에서 StorePanel 찾기 시도
            targetStorePanel = GetComponentInParent<StorePanel>();
            
            if (targetStorePanel == null)
            {
                // 마지막 시도로 씬에서 찾기
                targetStorePanel = FindObjectOfType<StorePanel>();
            }
        }
        
        // 이 버튼을 항상 활성화 상태로 유지하도록 StorePanel에 등록
        if (targetStorePanel != null)
        {
            targetStorePanel.RegisterAlwaysActiveObject(gameObject);
        }
    }

    private void OnButtonClick()
    {
        // 디버그 로그 추가
        Debug.Log("BackButton 클릭됨");

        // 타겟 StorePanel이 있으면 애니메이션 완료 후 UI 닫기
        if (targetStorePanel != null)
        {
            targetStorePanel.HideAndCloseUI();
        }
        else
        {
            // 안전장치: StorePanel이 없을 경우 다시 찾기 시도
            targetStorePanel = GetComponentInParent<StorePanel>();
            
            if (targetStorePanel == null)
            {
                targetStorePanel = FindObjectOfType<StorePanel>();
            }
            
            if (targetStorePanel != null)
            {
                targetStorePanel.HideAndCloseUI();
            }
            else
            {
                // 타겟이 여전히 없으면 기존처럼 바로 UI 닫기
                UIManager.Instance.CloseChildrenCanvas();
                Debug.LogWarning("targetStorePanel이 설정되지 않아 기본 닫기 동작을 실행합니다.");
            }
        }
    }
}