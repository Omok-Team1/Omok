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
            return await UniTask.RunOnThreadPool(() => _strategy.GetCoordi(cts.Token), cancellationToken: cts.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private (int, int) _coordi;
    private IOpponentStrategy _strategy;
    
    private readonly TaskQueue _taskQueue = new();

    private readonly int INF = (int)1e9;
}
