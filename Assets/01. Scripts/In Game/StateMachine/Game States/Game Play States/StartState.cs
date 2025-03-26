using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 매칭이 잡히거나, ReGame하는 경우 게임을 초기화 하는 상태이다.
/// 1. 네트워크 대전 상대인지, AI인지에 따라 OpponentController의 Strategy 클래스를 할당한다.
/// 2. Board 클래스의 Cell들의 데이터를 Turn.NONE, emptySprite로 전부 초기화한다.
/// 3. 대국 복기를 위해, 이번 대국의 { 플레이어 타입, 몇 수, 좌표 } 정보를 저장할 컨테이너 클래스를 생성한다.
/// 4. 누가 선공을 할 지 정한 후 해당 플레이어의 상태로 전환한다.
/// </summary>
public class StartState : IState
{
    public StartState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    
    public void EnterState()
    {
        
    }

    public void UpdateState()
    {
        StateMachine.ChangeState<PlayerState>();
    }

    public void ExitState()
    {

    }

    private readonly Invoker _actions = new();
    
    public StateMachine StateMachine { get; set; }
}