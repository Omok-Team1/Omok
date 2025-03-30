using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReturnToPreviousButton : UserFriendlyComponent
{
    public override void Init()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(EventMethod);
        }
    }

    // override 키워드 추가
    public override void EventMethod()
    {
        // UIManager가 초기화되었는지 확인
        if (UIManager.Instance != null)
        {
            Debug.Log("ReturnToPreviousButton: 이전 화면으로 돌아가기 시도");
            
            // 현재 활성화된 StorePanel 찾기
            StorePanel storePanel = GetComponentInParent<StorePanel>();
            if (storePanel != null)
            {
                // StorePanel이 있으면 애니메이션 완료 후 UI 닫기
                storePanel.HideAndCloseUI();
            }
            else
            {
                // StorePanel이 없으면 기존처럼 바로 UI 닫기
                UIManager.Instance.CloseChildrenCanvas();
            }
        }
        else
        {
            Debug.LogError("UIManager 인스턴스가 없습니다.");
        }
    }
}