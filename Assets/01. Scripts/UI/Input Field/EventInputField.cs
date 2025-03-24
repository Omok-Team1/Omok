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
public class EventInputField : DataFriendlyComponent
{
    public override void Init()
    {
        InputField = GetComponentsInChildren<TextMeshProUGUI>().
            Where(tmpro => tmpro.gameObject.name != "Placeholder").ToList();
        
        if (InputField is not null)
        {
            EventMethod();
        }
        else
        {
            throw new Exception("Input field is missing.");
        }
    }
    
    public List<TextMeshProUGUI> InputField { get; private set; }
}