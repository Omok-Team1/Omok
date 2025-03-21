using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testSO", menuName = "IOnEventSO/testSO")]
public class testSO : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var inputDatas = msg.GetParameter<List<string>>();

        foreach (string inputData in inputDatas)
        {
            Debug.Log(inputData);
        }
    }
}
