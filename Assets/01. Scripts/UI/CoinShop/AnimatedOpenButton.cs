using UnityEngine;

public class AnimatedOpenButton : OpenSubCanvasButton
{
    [SerializeField] private StorePanel targetStorePanel; // 타겟 StorePanel 지정
    
    public override void Init()
    {
        base.Init();
        
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
    
    public override void EventMethod()
    {
        // 이 부분을 수정하여 기본 기능(UIManager.Instance.OpenChildrenCanvas)을 호출하지 않고
        // 직접 Panel을 보여주는 방식으로 변경
        
        // 타겟 StorePanel이 설정되어 있다면 Show 메서드 호출
        if (targetStorePanel != null)
        {
            targetStorePanel.gameObject.SetActive(true);
            targetStorePanel.Show();
        }
        else
        {
            Debug.LogError("StorePanel이 할당되지 않았습니다.");
        }
    }
}