using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class TaskQueue
{
    private Queue<Action> _taskQueue = new();
    private bool _isProcessing = false;

    public void BeginTask(Action task)
    {
        if (_isProcessing is true)
        {
            _taskQueue.Enqueue(task);
        }
        else
        {
            _isProcessing = true;
            task?.Invoke();
        }
    }

    public UniTask<T> BeginTask<T>(Func<CancellationToken, UniTask<T>> task, CancellationTokenSource token = default(CancellationTokenSource))
    {
        //비동기 작업의 결과를 수동으로 제어하고 알리는데 필요한 클래스
        //비동기 작업의 결과를 UniTask로 변환하는데 쓸 수 있다.
        var tcs = new UniTaskCompletionSource<T>();
        
        if (_isProcessing is true)
        {
            _taskQueue.Enqueue(async () =>
            {
                try
                {
                    token?.Token.ThrowIfCancellationRequested();
                    tcs.TrySetResult(await task(token.Token));
                }
                catch (OperationCanceledException)
                {
                    Debug.LogError("작업이 취소 되었습니다.");
                    tcs.TrySetCanceled();
                }
            });
        }
        else
        {
            _isProcessing = true;
            
            UniTask.Void(async () =>
            {
                try
                { 
                    token?.Token.ThrowIfCancellationRequested();
                    
                    tcs.TrySetResult(await task(token.Token));
                }
                catch (OperationCanceledException)
                {
                    tcs.TrySetCanceled();
                }
                finally
                {
                    token.Dispose();
                    token = null;
                }
            });
        }
        
        return tcs.Task;
    }

    public void DoneTask()
    {
        _isProcessing = false;
        if (_taskQueue.Count <= 0) return;

        _isProcessing = true;
        
        _taskQueue.Dequeue()?.Invoke();
    }
}
