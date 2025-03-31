using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IOpponentStrategy
{
    UniTask<(int, int)> GetCoordi(CancellationToken token);
}

public class AIOpponent : IOpponentStrategy
{
    public UniTask<(int, int)> GetCoordi(CancellationToken token)
    {
        return OmokAIController.GetBestMove(token);
    }
}

public class OpponentController : MonoBehaviour
{
    void Awake()
    {
        SetOpponentStrategy(new AIOpponent());
    }
    
    public void SetOpponentStrategy(IOpponentStrategy opponentStrategy)
    {
        _strategy = opponentStrategy;
    }

    public async UniTask<(int, int)> BeginOpponentTask(CancellationTokenSource cts)
    {
        try
        {
            OmokAIController._board = GameManager.Instance.BoardManager.Grid.CloneInvisibleObj();
            
            _coordi = await UniTask.RunOnThreadPool(() =>
            {
                cts.Token.ThrowIfCancellationRequested();

                return _strategy.GetCoordi(cts.Token);

            }, cancellationToken: cts.Token);

            Debug.Log("시간 안에 AI 연산을 완료했습니다.");
        }
        catch (OperationCanceledException)
        {
            Debug.LogError("시간 초과로 AI 연산 중 취소합니다.");
            
            _coordi = (-INF, -INF);
        }
        finally
        {
            await UniTask.SwitchToMainThread();
        }
        
        return _coordi;
    }
    
    private (int, int) _coordi;
    private IOpponentStrategy _strategy;

    private readonly int INF = (int)1e9;
}
