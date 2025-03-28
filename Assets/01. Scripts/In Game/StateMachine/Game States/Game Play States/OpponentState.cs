using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class OpponentState : IState
{
    public OpponentState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;

        _eventSO = ScriptableObject.CreateInstance<TimeOutOnEvent>();
        
        EventManager.Instance.AddListener("Player2TimeOver", _eventSO, StateMachine.gameObject);
    }
    
    public async void EnterState()
    {
        //타임 아웃으로 인해 강제로 턴이 변경될 때, 이 비동기 함수를 기다리면서 에러가 발생함
        //그래서 토큰으로 타임 아웃시 강제로 종료하게 만들었음
        if (cts.IsCancellationRequested)
        {
            //기존 토큰 정리
            cts.Dispose();
            cts = new CancellationTokenSource();
        }
        //얘가 한 프레임 동안 모든 연산을 끝내고 값을 던지기 때문에,
        //누구의 턴인지 표시해주는 애니메이션 코루틴이 블락(정확히는 yield return null으로 인해 다음 프레임에 호출되어야 하는데 못함)되어서 비동기로 실행함
        try
        {
            bestCoordi = await Task.Run(() => OmokAIController.GetBestMove(cts.Token), cts.Token);
        }
        catch (OperationCanceledException e)
        {
            Debug.Log("AI 연산이 정상적으로 종료되었습니다.");
        }
    }

    public void UpdateState()
    {
        if (bestCoordi.HasValue is true)
        {
            GameManager.Instance.BoardManager.RecordDrop(bestCoordi);
        
            StateMachine.ChangeState<OnDropState>();
        
            GameManager.Instance.TimerController.EndPlayerTurn();

            bestCoordi = null;
        }
    }

    public void ExitState()
    {
        cts.Cancel();
    }

    private (int, int)? bestCoordi;
    private CancellationTokenSource cts = new CancellationTokenSource();
    private readonly IOnEventSO _eventSO;

    private readonly Invoker _actions;
    public StateMachine StateMachine { get; set; }
}
