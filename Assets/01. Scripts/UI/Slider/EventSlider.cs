using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Input Field들의 부모로 빈 게임 오브젝트를 만들어서 해당 컴포넌트를 추가해서 사용합니다.
/// </summary>
public class EventSlider : DataFriendlyComponent
{
    public override void Init()
    {
        EventMethod();
    }

    public List<Slider> GetSlider()
    {
        return new List<Slider>() { negativeSlider, positiveSlider };
    }
    
    [Header("음의 방향")]
    [SerializeField] private Slider negativeSlider;
    
    [Header("양의 방향")]
    [SerializeField] private Slider positiveSlider;
}