using System;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

        
    }

    public void UpdateState()
    {
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

    private readonly int INF = (int)1e9;
    
    private (int, int) coordi;
    private readonly IOnEventSO _eventSO;
    private readonly Invoker _actions;
    public StateMachine StateMachine { get; set; }
}
