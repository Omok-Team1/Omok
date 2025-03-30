using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class OpponentState : IState
{
    public OpponentState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;

        _eventSO = ScriptableObject.CreateInstance<OpponentTimeOutOnEvent>();
        
        EventManager.Instance.AddListener("OpponentTimeOut", _eventSO, StateMachine.gameObject);
    }

    public async void EnterState()
    {
        //얘가 한 프레임 동안 모든 연산을 끝내고 값을 던지기 때문에,
        //누구의 턴인지 표시해주는 애니메이션 코루틴이 블락(정확히는 yield return null으로 인해 다음 프레임에 호출되어야 하는데 못함)되어서 비동기로 실행함
        OmokAIController._board = GameManager.Instance.BoardManager.Grid.CloneInvisibleObj();
        
        var cts = new CancellationTokenSource();
        
        var message = new EventMessage("OpponentTimeOut");
        message.AddParameter<CancellationTokenSource>(cts);
        
        EventManager.Instance.PushEventMessageEvent(message);
        
        GameManager.Instance.TimerController.StartTurn(Turn.PLAYER2);
        
        coordi = ((int)1e9, (int)1e9);
        
        coordi = await GameManager.Instance.OpponentController.BeginOpponentTask(cts);

        //await UniTask.WaitUntil(() => task.Status is UniTaskStatus.Succeeded || task.Status is UniTaskStatus.Canceled);
        
        //coordi = await GameManager.Instance.OpponentController.BeginOpponentTask(cts);

        // await UniTask.WaitUntil(() =>
        // {
        //     //양의 무한대인 경우 : 아직 AI 연산 중, 연산이 끝나거나 Time-out인 경우 bestMove 좌표거나 음의 무한대를 던져준다.
        //     // if (coordi.Item1 != (int)1e9 || coordi.Item2 != (int)1e9)
        //     // {
        //     //     return true;
        //     // }
        //     // return false;
        //     return task.Status is UniTaskStatus.Succeeded || task.Status is UniTaskStatus.Canceled;
        // });
        
        // //값이 전달 되거나, token에 의해 취소 될 때까지 대기
        // GameManager.Instance.BoardManager.RecordDrop(coordi);
        //
        // if (cts.IsCancellationRequested)
        // {
        //     Debug.Log("Opponent timed out.");
        // }
        // else
        // {
        //     Debug.Log("Opponent OnDrop!!!!!!!.");
        //     EventManager.Instance.PopLastEventMessageEvent();
        //     GameManager.Instance.TimerController.EndPlayerTurn();
        // }
        //
        // StateMachine.ChangeState<OnDropState>();
    }

    public void UpdateState()
    {
        //Do nothing at Unity Update Loop...
        //값이 양의 무한대가 아니라면, 시간 초과이거나, 연산을 완료해서 정상적인 좌표를 가지고 있다.
        //Timer의 코루틴이 null이면 Timer의 세팅이 끝났다.
        if (coordi.Item1 != INF || coordi.Item2 != INF)
        {
            GameManager.Instance.BoardManager.RecordDrop(coordi);
        
            //Time-out case
            if (coordi.Item1 == -INF || coordi.Item2 == -INF)
            {
                Debug.Log("Opponent timed out.");
            }
            else
            {
                Debug.Log("Opponent OnDrop!!!!!!!.");
                EventManager.Instance.PopLastEventMessageEvent();
                GameManager.Instance.TimerController.EndTurn();
            }
        
            StateMachine.ChangeState<OnDropState>();
        }
    }

    public void ExitState()
    {

    }

    // private IEnumerator WaitTimerSettingAndRun()
    // {
    //     GameManager.Instance.BoardManager.RecordDrop(coordi);
    //
    //     //Time-out case
    //     if (coordi.Item1 == -INF || coordi.Item2 == -INF)
    //     {
    //         Debug.Log("Opponent timed out.");
    //     }
    //     else
    //     {
    //         Debug.Log("Opponent OnDrop!!!!!!!.");
    //         EventManager.Instance.PopLastEventMessageEvent();
    //         GameManager.Instance.TimerController.EndPlayerTurn();
    //     }
    //
    //     StateMachine.ChangeState<OnDropState>();
    //
    //     yield return null;
    // }

    private readonly int INF = (int)1e9;
    
    private (int, int) coordi;
    private readonly IOnEventSO _eventSO;
    private readonly Invoker _actions;
    public StateMachine StateMachine { get; set; }
}
