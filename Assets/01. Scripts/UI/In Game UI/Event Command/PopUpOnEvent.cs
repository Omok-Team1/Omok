using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PopUpOnEvent", menuName = "IOnEventSO/PopUpOnEvent")]
public class PopUpOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        // 처음 크기를 0으로 설정
        targetImage.transform.localScale = Vector3.zero;

        // DoTween을 이용해 점점 커지면서 튀어나오는 효과 적용
        targetImage.transform.DOScale(Vector3.one * overshoot, duration)
            .SetEase(Ease.OutBack); // 자연스럽게 튀어나오는 애니메이션 적용
    }
    
    [SerializeField] private Image targetImage;
    [SerializeField] private float duration = 0.3f;
    
    // 튀어 나오는 정도
    [SerializeField] private float overshoot = 1.3f;
}