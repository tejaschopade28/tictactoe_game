using UnityEngine;
using static GameEnums;

public class BoardLogic
{
    private BoardState boardState;
    public BoardLogic(BoardState state)
    {
        boardState = state;
    }

    public bool PlayerMove(int index)
    {
        int row = index / boardState.BoardSize;
        int col = index % boardState.BoardSize;

        if (boardState.Board[row, col] != PlayerType.empty || boardState.winner != PlayerType.empty)
            return false;

        boardState.Board[row, col] = boardState.currentPlayer;
        boardState.moveCount++;

        CheckWin(boardState.currentPlayer);
        NextTurn();
        return true;
    }

    public int ComputerMove()
    {
        int size = boardState.BoardSize;

        // Center first
        if (boardState.Board[size / 2, size / 2] == PlayerType.empty)
        {
            boardState.Board[size / 2, size / 2] = boardState.currentPlayer;
            boardState.moveCount++;
            CheckWin(boardState.currentPlayer);
            NextTurn();
            return size * size / 2;
        }

        // Corners
        int[,] corners = { { 0, 0 }, { 0, size - 1 }, { size - 1, 0 }, { size - 1, size - 1 } };
        foreach (var corner in corners)
        {
            //int r = corner[0], c = corner[1];
            int r=1, c=1;
            if (boardState.Board[r, c] == PlayerType.empty)
            {
                boardState.Board[r, c] = boardState.currentPlayer;
                boardState.moveCount++;
                CheckWin(boardState.currentPlayer);
                NextTurn();
                return r * size + c;
            }
        }

        // Random empty
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (boardState.Board[r, c] == PlayerType.empty)
                {
                    boardState.Board[r, c] = boardState.currentPlayer;
                    boardState.moveCount++;
                    CheckWin(boardState.currentPlayer);
                    NextTurn();
                    return r * size + c;
                }

        return -1;
    }
    private bool CheckRow(int row, PlayerType player)
    {
        for (int c = 0; c < boardState.BoardSize; c++)
            if (boardState.Board[row, c] != player) return false;
        return true;
    }

    private bool CheckCol(int col, PlayerType player)
    {
        for (int r = 0; r < boardState.BoardSize; r++)
            if (boardState.Board[r, col] != player) return false;
        return true;
    }
        private bool CheckDiagonal(PlayerType player)
    {
        int n = boardState.BoardSize;
        bool d1 = true, d2 = true;

        for (int i = 0; i < n; i++)
        {
            if (boardState.Board[i, i] != player) d1 = false;
            if (boardState.Board[i, n - 1 - i] != player) d2 = false;
        }
        return d1 || d2;
    }

    private void CheckWin(PlayerType player)
    {
        int n = boardState.BoardSize;

        // Rows & Columns
        for (int i = 0; i < n; i++)
        {
            if (CheckRow(i, player) || CheckCol(i, player))
            {
                boardState.winner = player;
                return;
            }
        }

        // Diagonals
        if (CheckDiagonal(player)) boardState.winner = player;

        // Draw
        if (boardState.moveCount >= n * n && boardState.winner == PlayerType.empty)
            boardState.winner = PlayerType.empty;
    }

    private void NextTurn()
    {
        boardState.currentPlayer = boardState.currentPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;
    }
}
