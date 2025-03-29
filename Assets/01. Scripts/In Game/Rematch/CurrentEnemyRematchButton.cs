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

    public void LockButton()
    {
        lockOverlay.SetActive(true);
        currentEnemyRematchButton.interactable = false;
    }
}