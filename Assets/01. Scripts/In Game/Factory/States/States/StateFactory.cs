using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFactory : IConcreteFactory
{
    public Dictionary<Type, IState> CreateItems()
    {
        Dictionary<Type, IState> states = new Dictionary<Type, IState>();

        StateMachine stateMachine = GameManager.Instance.GetComponent<StateMachine>();
        
        states.Add(typeof(MatchingState), new MatchingState(stateMachine));
        states.Add(typeof(StartState), new StartState(stateMachine));
        states.Add(typeof(PlayerState), new PlayerState(stateMachine));
        states.Add(typeof(OpponentState), new OpponentState(stateMachine));
        states.Add(typeof(OnDropState), new OnDropState(stateMachine));
        states.Add(typeof(ChangeTurnState), new ChangeTurnState(stateMachine));
        states.Add(typeof(ConstraintCheckState), new ConstraintCheckState(stateMachine));
        states.Add(typeof(EndGameState), new EndGameState(stateMachine));
        
        return states;
    }
}
