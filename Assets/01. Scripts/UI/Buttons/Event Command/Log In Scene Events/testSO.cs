using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testSO", menuName = "IOnEventSO/testSO")]
public class testSO : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
    }
}
