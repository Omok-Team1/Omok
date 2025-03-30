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

    public void DoneTask()
    {
        _isProcessing = false;
        if (_taskQueue.Count <= 0) return;

        _isProcessing = true;
        
        _taskQueue.Dequeue()?.Invoke();
    }
}
