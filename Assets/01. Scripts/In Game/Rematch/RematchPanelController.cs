using UnityEngine;
using DG.Tweening;

public class RematchPanelController : MonoBehaviour
{
    [SerializeField] private GameObject waitPanel;
    [SerializeField] private GameObject RematchOKPanel;
    [SerializeField] private float panelSwitchDuration = 0.3f;

    private void OnEnable()
    {
        // 패널 초기화: 첫 번째 페이지만 활성화
        waitPanel.SetActive(true);
        RematchOKPanel.SetActive(false);
    }

    // "다음" 버튼 클릭 시 호출
    public void OnNextButtonClicked()
    {
        // 첫 번째 패널 닫고 두 번째 패널 열기
        waitPanel.SetActive(false);
        RematchOKPanel.transform.localScale = Vector3.zero;
        RematchOKPanel.SetActive(true);
        RematchOKPanel.transform.DOScale(1f, panelSwitchDuration);
    }

    // "확인" 버튼 클릭 시 호출
    public void OnConfirmButtonClicked()
    {
        // 전체 패널 닫기
        gameObject.SetActive(false);
        // 추가 로직 (예: 재대국 버튼 잠금 해제)
        FindObjectOfType<CurrentEnemyRematchButton>().UnlockButton();
    }
}