using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeTurnEvent", menuName = "IOnEventSO/ChangeTurnEvent")]
public class ChangeTurnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();

        var currentAnim = listenerObj.transform.GetChild(0).gameObject;

        if (currentAnim is null)
            throw new Exception("Invalid Child");
        
        if (listenerObj.TryGetComponent(out _rectTransform) is false)
        {
            throw new Exception("RectTransform Component is not found");
        }
        else
        {
            currentAnim.SetActive(true);
            StaticCoroutine.StartStaticCoroutine(PlayAnimation());
        }
    }
    
    IEnumerator PlayAnimation()
    {
        while (true)
        {
            float newY = Mathf.Sin(Time.time * frequency * Mathf.Deg2Rad) * amplitude;
            _rectTransform.anchoredPosition = new Vector2(_startPosition.x, _startPosition.y - newY);
            yield return null;
        }
    }
    
    private const float amplitude = 10f;
    private const float frequency = 50f;
    
    private RectTransform _rectTransform;
    private Vector2 _startPosition;
}