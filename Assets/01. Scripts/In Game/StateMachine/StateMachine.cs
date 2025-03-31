using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public void Run()
    {
        _states = Factory.CreateItems<StateFactory>();
        
        _currentState = _states[typeof(InitState)];
        
        ChangeState<InitState>();
    }
    
    public void ChangeState<T>()
    {
        _currentState?.ExitState();
        
        _currentState = _states[typeof(T)];
        
        _currentState?.EnterState();
    }
    
    void Update()
    {
        _currentState?.UpdateState();
    }
    
    private IState _currentState;

    private IDictionary<Type, IState> _states;
}
