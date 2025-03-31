using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WavyText : MonoBehaviour
{
    void OnEnable()
    {
        if (_coroutine is not null)
            StopCoroutine(_coroutine);
        
        _coroutine = StartCoroutine(AnimateText());
    }

    private void OnDisable()
    {
        StopCoroutine(_coroutine);
    }

    IEnumerator AnimateText()
    {
        string text = textMeshPro.text;
        textMeshPro.text = ""; // 초기화

        // 개별 문자 처리
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            GameObject letterObj = new GameObject("Letter_" + i);
            letterObj.transform.SetParent(textMeshPro.transform);
            TextMeshProUGUI letterTMP = letterObj.AddComponent<TextMeshProUGUI>();

            letterTMP.text = c.ToString();
            letterTMP.font = textMeshPro.font;
            letterTMP.fontSize = textMeshPro.fontSize;
            letterTMP.alignment = TextAlignmentOptions.Center;

            RectTransform rect = letterObj.GetComponent<RectTransform>();
            rect.anchoredPosition = startPosition + new Vector2(i * letterSpacing, 0);

            // DoTween으로 Sin 애니메이션 적용
            float delay = i * 0.1f; // 각 문자에 딜레이 추가
            rect.DOAnchorPosY(amplitude, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(delay);
        }

        yield return null;
    }
    
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private float amplitude = 10f; // 진폭
    [SerializeField] private float frequency = 2f;  // 주파수
    [SerializeField] private float duration = 1f;   // 한 사이클 시간
    [SerializeField] private float letterSpacing = 50;
    [SerializeField] private Vector2 startPosition = Vector2.zero;

    private Coroutine _coroutine = null;
}