
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static GameEnums;

public class Gameboard : MonoBehaviour
{

    [SerializeField] GameManager gameManager;
    bool IsOnline =>
    gameManager.currentGameMode == GameMode.VsOnlinePlayer;

    public GameState currentState;
    int counter;
    public int  BoardSize = 3;
    public PlayerType[,] Board;

    public PlayerType currentPlayer = PlayerType.X; 
    public PlayerType winner= PlayerType.empty;
    public void SetState(GameState state)
    {
        Debug.Log(state + " This is State");
        currentState= state;
    }

    public void GameReset()
    {
        Board = new PlayerType[BoardSize, BoardSize];
        for(int i = 0; i < BoardSize; i++)
        {
            for(int j=0;j< BoardSize; j++)
            {
                Board[i,j] = PlayerType.empty;
            }
             
        }
        if (!IsOnline){
            counter=0;
            currentPlayer= gameManager.playerSymbol;
            currentState= GameState.Playing;
        }
    }
    public bool WinCheck(PlayerType Player)
    {

        for(int row = 0; row < BoardSize ; row++)
        {
            bool MatchWin=true;

            for(int col=0; col< BoardSize; col++)
            {
                if(Board[row,col]!= Player)
                {
                    MatchWin= false;
                    break;
                }
            }
            if(MatchWin) return true;
        }

        for(int col = 0; col < BoardSize ; col++)
        {
            bool MatchWin=true;

            for(int row=0; row<BoardSize; row++)
            {
                if(Board[row,col]!= Player)
                {
                    MatchWin= false;
                    break;
                }
            }
            if(MatchWin) return true;
        }
        bool diagonal1=true;
        bool diagonal2=true;
        for(int i = 0; i < BoardSize ; i++)
        {
            if (Board[i, i] != Player)
            {
                diagonal1=false;
            }
            if (Board[i, BoardSize - i-1] != Player)
            {
                diagonal2= false;
            }
        }
        
        if(diagonal1 || diagonal2) return true;

        return false;
    }
    

    public bool PlayerMove(int index)
    {
        if (IsOnline) return false;
        if(currentState== GameState.GameOver || Board[index/BoardSize,index%BoardSize] != PlayerType.empty)
        {
            //Debug.Log("Ya game over ya Cell is not empty");
            return false ;
        }

        Board[index/BoardSize,index%BoardSize] = currentPlayer;
        counter++;
        GameOverCheck(currentPlayer);
        NextTurn();
        return true;
    }

    public int ComputerMove()
    {
       if (IsOnline) return -1;
        //Fills Centre first;   
        if(Board[BoardSize/2,BoardSize/2] == PlayerType.empty)
        {
            Board[BoardSize/2,BoardSize/2] = currentPlayer;
            counter++;
            GameOverCheck(currentPlayer);
            NextTurn();
            
            return BoardSize*BoardSize/2;
        }

        int [,] corner=
        {
            {0,0},
            {0,BoardSize-1},
            {BoardSize-1,0},
            {BoardSize-1,BoardSize-1}
        };

        for(int i = 0; i < 4; i++)
        {
            int r = corner[i,0];
            int c = corner[i,1];
            if(Board[r,c] == PlayerType.empty)
            {
                Board[r,c] = currentPlayer;
                counter++;
                GameOverCheck(currentPlayer);
                NextTurn();
                return r*BoardSize+c;
            }
        }
        for(int row = 0; row < BoardSize; row ++)
        {
            for(int col=0;col< BoardSize; col++)
            {
                if(Board[row,col] == PlayerType.empty)
                {
                    Debug.Log("random Selection");
                    Board[row,col] = currentPlayer;
                    counter++;
                    GameOverCheck(currentPlayer);
                    NextTurn();
                    return BoardSize*row + col;
                }
            }
        }

        return -1;
    }



    public string GetWinner()
    {   
        if(currentState == GameState.GameOver)
        {
            return winner.ToString();
        }
        return "";
    }
    public int GetWinnerIndex()
    {
        if (currentState != GameState.GameOver)
            return -1;

        if (winner == PlayerType.X) return 0;
        if (winner == PlayerType.O) return 1;

        return -1; // draw or no winner
    }



    public void GameOverCheck(PlayerType player)
    {
        //PlayerType winner;
        if (IsOnline) return;

        if (WinCheck(player))
        {
            Debug.Log( player + " Wins!");
            gameManager.SetGameState(GameEnums.GameState.GameOver);
            winner= player;
            return;
        }
        if(counter== BoardSize*BoardSize)
        {
            Debug.Log("Draw");
            gameManager.SetGameState(GameEnums.GameState.Draw);
            return;
        }
    }

    public void NextTurn()
    {
        if (IsOnline) return;
        currentPlayer = currentPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;

    }
}