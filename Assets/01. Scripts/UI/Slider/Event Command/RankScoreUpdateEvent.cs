using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RankScoreUpdateEvent", menuName = "IOnEventSO/RankScoreUpdateEvent")]
public class RankScoreUpdateEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listener = msg.GetParameter<GameObject>().GetComponent<EventSlider>();

        _sliders = listener.GetSlider();

        StaticCoroutine.StartStaticCoroutine(SliderAnimation(-60, 120));
    }

    //currentScore = matchScore = -2^31 ~ 2^31-1
    private IEnumerator SliderAnimation(int currentScore, int matchScore)
    {
        float elapsedTime = 0f;
        float startValue = currentScore / 100f;
        float targetValue = (currentScore + matchScore) / 100f;
        
        bool isSignChanged = false;
        bool isPositive = currentScore >= 0;
        
        //true = 양수 방향, false = 음수 방향
        bool dir = currentScore + matchScore >= 0;
        
        Slider activeSlider = _sliders[isPositive ? 1 : 0];

        if (isPositive is false)
        {
            startValue = Mathf.Abs(startValue);
            _sliders[0].gameObject.SetActive(true);
            _sliders[1].gameObject.SetActive(false); 
        }
        else
        {
            _sliders[1].gameObject.SetActive(true); 
            _sliders[0].gameObject.SetActive(false); 
        }
        
        activeSlider.value = startValue;

        /*
         * 현재 점수에서 [증가], [감소] 하는 두 가지 경우만 존재한다.
         * [증가], [감소] 후에는 /음수/, /0/, /양수/ 세 가지 경우만 존재한다.
         * [증가]는 matchScore가 양수로 들어올 것이며, [감소]는 matchScore가 음수로 들어올 것이다.
         */
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            activeSlider.value = Mathf.Lerp(startValue, isPositive ? Mathf.Max(0, targetValue) : Mathf.Abs(Mathf.Min(0, targetValue)), t);
            
            // 양수 슬라이더가 0이 되면 음수 슬라이더로 전환
            if (isSignChanged is not true && isPositive is true && activeSlider.value <= 0)
            {
                isPositive = false;
                isSignChanged = true;
                activeSlider.gameObject.SetActive(false);
                _sliders[0].gameObject.SetActive(true);
                
                // 음수 슬라이더로 전환
                activeSlider = _sliders[0];

                // 시간 초기화
                elapsedTime = 0f;
                startValue = 0;
            }
            
            // 음수 슬라이더가 0이 되면 양수 슬라이더로 전환
            if (isSignChanged is not true && isPositive is false && activeSlider.value <= 0)
            {
                isPositive = true;
                isSignChanged = true;
                activeSlider.gameObject.SetActive(false);
                _sliders[1].gameObject.SetActive(true);
                
                // 양수 슬라이더로 전환
                activeSlider = _sliders[1];

                // 시간 초기화
                elapsedTime = 0f;
                startValue = 0;
            }

            yield return null;
        }
    }

    private List<Slider> _sliders;
    
    // 증가하는 데 걸리는 시간
    public float duration = 2f;
}
