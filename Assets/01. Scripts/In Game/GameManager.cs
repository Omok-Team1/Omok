using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[
    RequireComponent(typeof(StateMachine))
]
public class GameManager : Singleton<GameManager>
{
    new void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();

        _stateMachine.Run();
    }
    
    void OnDestroy()
    {
        var keys = _isDirty.Keys.ToArray();

        foreach (var isDirtyKey in keys)
        {
            _isDirty[isDirtyKey] = true;
        }
    }

    private StateMachine _stateMachine;
    private BoardManager _boardManager;
    private OpponentController _opponentController;

    // Scene을 변경하고 다시 InGame으로 돌아왔을 때 DataManager에 대한 참조가 변경되어
    // MissingReferenceException가 발생했던 문제를 그냥 BoardManager가 필요할 때마다 FindObjectOfType으로 찾아 반환하는 방식으로 해결
    // 매번 이렇게 찾아서 반환하면 비효율적이라 판단되어
    // dirty 비트를 만들어 Scene 전환을 여러번 하다 다시 InGame에 돌아와 BoardManager가 필요할 때 한 번만 찾도록 수정하였음
    private static IDictionary<string, bool> _isDirty = new Dictionary<string, bool>() 
        { {nameof(BoardManager), true}, {nameof(OpponentController), true}};
    
    public BoardManager BoardManager
    {
        get
        {
            if (_isDirty[nameof(BoardManager)] is true)
            {
                _boardManager = FindObjectOfType<BoardManager>();
                _isDirty[nameof(BoardManager)] = false;
            }
            
            return _boardManager;
        }
    }
    public OpponentController OpponentController
    {
        get
        {
            if (_isDirty[nameof(OpponentController)] is true)
            {
                _opponentController = FindObjectOfType<OpponentController>();
                _isDirty[nameof(OpponentController)] = false;
            }
            
            return _opponentController;
        }
    }
}
