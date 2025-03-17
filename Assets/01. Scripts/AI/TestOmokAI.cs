using UnityEngine;
using UnityEngine.UI;

public class TestOmokAI : MonoBehaviour
{
    [SerializeField] private Constants.PlayerType[,] board = new Constants.PlayerType[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
    [SerializeField] private Button aiMoveButton; // UI 버튼 연결
    private Constants.PlayerType currentPlayer = Constants.PlayerType.PlayerA; // 현재 플레이어 추적

    void Start()
    {
        aiMoveButton.onClick.AddListener(MakeMove);
    }

    void MakeMove()
    {
        var (row, col) = OmokAIController.GetBestMove(board);
        Debug.Log($"🤖 AI 추천 착수 위치: ({row}, {col})");

        board[row, col] = currentPlayer; // 현재 플레이어의 돌 놓기

        PrintBoard();
        SwitchPlayer(); // 플레이어 전환
    }

    void SwitchPlayer()
    {
        if (currentPlayer == Constants.PlayerType.PlayerA)
            currentPlayer = Constants.PlayerType.PlayerB;
        else
            currentPlayer = Constants.PlayerType.PlayerA;
    }

    void PrintBoard()
    {
        string boardString = "";
        for (int i = 0; i < Constants.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Constants.BOARD_SIZE; j++)
            {
                if (board[i, j] == Constants.PlayerType.None) boardString += "・";
                else if (board[i, j] == Constants.PlayerType.PlayerA) boardString += "A";
                else if (board[i, j] == Constants.PlayerType.PlayerB) boardString += "B";
            }
            boardString += "\n";
        }
        Debug.Log("\n" + boardString);
    }
}
