using DG.Tweening;
using UnityEngine;

public class DOTweenInitializer : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init(useSafeMode: true, logBehaviour: LogBehaviour.Default);
        DOTween.SetTweensCapacity(500, 50);
    }
}