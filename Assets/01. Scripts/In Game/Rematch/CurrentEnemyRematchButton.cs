using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CurrentEnemyRematchButton : MonoBehaviour
{
    [SerializeField] private Button currentEnemyRematchButton;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private float unlockAnimDuration = 0.3f;

    private void Awake()
    {
        // 초기 상태: 잠금
        LockButton();
    }

    public void UnlockButton()
    {
        // 애니메이션과 함께 잠금 해제
        lockOverlay.transform.DOScale(1.2f, unlockAnimDuration)
            .OnComplete(() => 
            {
                lockOverlay.SetActive(false);
                currentEnemyRematchButton.interactable = true;
                transform.DOScale(1f, unlockAnimDuration);
            });
    }


	public void OnRematchButtonClicked()
{
    // 이벤트 큐 초기화
    if (EventManager.Instance != null)
    {
        EventManager.Instance.ClearEventQueue();
    }
    
    // 씬 재로드 전에 이벤트 리스너 초기화
    if (GameEndEventDispatcher.Instance != null)
    {
        GameEndEventDispatcher.Instance.ClearEventListeners();
        GameEndEventDispatcher.Instance.ResetDispatchState();
    }
    
    // 리매치 확인 패널 표시
    EnemyRematchConfirmPanel confirmPanel = FindObjectOfType<EnemyRematchConfirmPanel>();
    if (confirmPanel != null)
    {
        confirmPanel.Show();
    }
}
    public void LockButton()
    {
        lockOverlay.SetActive(true);
        currentEnemyRematchButton.interactable = false;
    }
}