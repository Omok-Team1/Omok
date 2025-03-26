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
        if (targetStorePanel != null)
        {
            targetStorePanel.Show(); // DoTween 애니메이션 실행
        }
        else
        {
            Debug.LogError("StorePanel이 할당되지 않았습니다.");
        }
    }
}
