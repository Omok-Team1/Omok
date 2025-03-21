using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    [SerializeField] private StorePanel targetStorePanel;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        // 타겟이 할당되지 않은 경우 이름으로 찾기
        if (targetStorePanel == null)
        {
            // 특정 이름의 StorePanel 찾기 (예: "ShopTotalPanel")
            GameObject shopPanel = GameObject.Find("ShopTotalPanel");
            if (shopPanel != null)
            {
                targetStorePanel = shopPanel.GetComponent<StorePanel>();
            }
            
            // 찾지 못한 경우 로그 출력
            if (targetStorePanel == null)
            {
                Debug.LogError("ShopTotalPanel을 찾을 수 없습니다.");
            }
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
            // 타겟이 없으면 기존처럼 바로 UI 닫기
            UIManager.Instance.CloseChildrenCanvas();
        }
    }
}