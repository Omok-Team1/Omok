using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Data/Game Data", order = 1)]
public class GameData : ScriptableObject
{
    //player Info
    public string player1Name;
    public Sprite player1Sprite;
    public int player1Rank;
    
    public string player2Name;
    public Sprite player2Sprite;
    public int player2Rank;
    
    //Marker Sprite & Cell Prefab Data
    public Sprite player1Marker;
    public Sprite player2Marker;
    public Sprite constraintMarker;
    public Sprite emptySprite;
    
    public GameObject cellPrefab;

    //Play Data
    public Turn currentTurn;
    public Turn winner;
    
    public int playerWinCount;
    public int playerLoseCount;

    public delegate void OnChangedPlayersPoint(GameData gameData);
    
    //네트워크 매니저는 여기에 이벤트를 등록해서, 게임이 끝날 때 데이터를 전달 받는다.
    public event OnChangedPlayersPoint onChangedPlayersPoint;

    private void ChangePlayersPoint()
    {
        _ = (winner == Turn.PLAYER1) ? playerWinCount++ : playerLoseCount++;
        onChangedPlayersPoint?.Invoke(this);
    }

    public void ChangeTurn()
    {
        currentTurn = currentTurn != Turn.PLAYER1 ? Turn.PLAYER1 : Turn.PLAYER2;
    }

    public void SetWinner()
    {
        //현재 플레이어가 돌을 둔 후 턴이 바뀌기 전에 승리 조건을 검사하니, 이 함수가 호출될 때는
        //현재 플레이어가 승리자가 된다.
        winner = currentTurn;
        ChangePlayersPoint();
    }
}