using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    [SerializeField] private StorePanel targetStorePanel; // 타겟 StorePanel 지정

    private void Start()
    {
        // 버튼 컴포넌트 가져오기
        Button button = GetComponent<Button>();

        // 클릭 이벤트 등록
        button.onClick.AddListener(OnButtonClick);

        // 타겟 StorePanel이 없으면 자동으로 찾기 시도
        if (targetStorePanel == null)
        {
            targetStorePanel = FindObjectOfType<StorePanel>();
            if (targetStorePanel == null)
            {
                Debug.LogError("StorePanel을 찾을 수 없습니다. Inspector에서 할당해주세요.");
            }
        }
    }

    private void OnButtonClick()
    {
        if (targetStorePanel != null)
        {
            targetStorePanel.Hide(); // DoTween 애니메이션 실행
        }
        else
        {
            Debug.LogError("StorePanel이 할당되지 않았습니다.");
        }
    }
}