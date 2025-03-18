using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BoardGrid : MonoBehaviour
{
    void Awake()
    {
        _gameData = Addressables.LoadAssetAsync<GameData>("Assets/02. Prefabs/GameData/GameData.asset").WaitForCompletion();
        
        _cameraOffsetY = gameObject.transform.position.y;
        
        Init(_gameData);
    }
    
    //operator '[]' overloading
    public Cell this[int row, int col]
    {
        get
        {
            // 격자 사이즈가 바뀌면 범위도 여기도 수정해줘야함!
            if (row < -7 || row >= GridSize - 7 || col < -7 || col >= GridSize - 7)
                return null;
            else
                return _grid[(row, col)];
        }
    }
    
    public bool TryMarkingOnCell((int, int) coordi, Sprite sprite = null)
    {
        if (_grid[coordi].Marker is not null)
        {
            if (sprite is not null)
            {
                _grid[coordi].Marker = sprite;
            }
            else if (_gameData.currentTurn == Turn.PLAYER1)
            {
                _grid[coordi].Marker = _gameData.player1Marker;
                _grid[coordi].CellOnwer = Turn.PLAYER1;
            }
            else
            {
                _grid[coordi].Marker = _gameData.player2Marker;
                _grid[coordi].CellOnwer = Turn.PLAYER2;
            }

            _grid[coordi].gameObject.layer = LayerMask.NameToLayer("Selected");
            _remainCells--;
            
            return true;
        }
        
        return false;
    }

    public bool TryUnmarkingOnCell((int, int) coordi)
    {
        if (_grid[coordi].Marker != _gameData.emptySprite)
        {
            _grid[coordi].Marker = _gameData.emptySprite;
            _remainCells++;
            return true;
        }

        return false;
    }

    public void Init(GameData gameData)
    {
        _gameData = gameData;
        _remainCells = GridSize * GridSize;
        
        _grid = new Dictionary<(int, int), Cell>();

        for (int row = -7; row < GridSize - 7; row++)
        {
            for (int col = -7; col < GridSize - 7; col++)
            {
                Vector3 pos = new Vector3(col, row + _cameraOffsetY, 0);
                
                GameObject obj = Instantiate(gameData.cellPrefab, pos, Quaternion.identity);
                obj.transform.SetParent(transform);
                
                Cell cell = obj.AddComponent<Cell>();
                cell.Init((row, col), gameData.emptySprite);
                _grid.Add((row, col), cell);
            }
        }
    }

    public void Clear()
    {
        for (int row = -7; row < GridSize - 7; row++)
        {
            for (int col = -7; col < GridSize - 7; col++)
            {
                _grid[(row, col)].EraseMarker(_gameData.emptySprite);
            }
        }
    }
    
    /// <summary>
    /// Min Max 알고리즘을 위한 데이터로만 표현하여 렌더링 되지 않게 마킹하는 함수
    /// </summary>
    /// <param name="coordi">마킹할 좌표</param>
    /// <param name="player">좌표에 마킹할 플레이어</param>
    /// <returns></returns>
    public void MarkingTurnOnCell((int, int) coordi, Turn player)
    {
        _grid[coordi].CellOnwer = player;
    }

    private int _remainCells = 0;
    public bool RemainCells => _remainCells <= 0;

    private const int GridSize = 15;
    
    private float _cameraOffsetY;
    private GameData _gameData;
    private IDictionary<(int, int), Cell> _grid;
}