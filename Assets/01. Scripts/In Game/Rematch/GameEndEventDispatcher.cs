using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEndEventDispatcher : MonoBehaviour
{
    public static GameEndEventDispatcher Instance { get; private set; }
    public UnityEvent OnGameEnded = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}