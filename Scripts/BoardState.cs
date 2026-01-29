using UnityEngine;
using static GameEnums;

[System.Serializable]
public class BoardState
{
    public int BoardSize = 3;
    public PlayerType[,] Board;
    public PlayerType currentPlayer = PlayerType.X;
    public PlayerType winner = PlayerType.empty;
    public int moveCount = 0;

    public void Initialize()
    {
        Board = new PlayerType[BoardSize, BoardSize];
        for (int i = 0; i < BoardSize; i++)
            for (int j = 0; j < BoardSize; j++)
                Board[i, j] = PlayerType.empty;

        currentPlayer = PlayerType.X;
        winner = PlayerType.empty;
        moveCount = 0;
    }

    public void Reset()
    {
        Initialize();
    }
}
