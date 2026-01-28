using System;
using System.Collections;
using System.Net.WebSockets;
using TMPro;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static GameEnums;

public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;
    [SerializeField] UIManager uIManager;
    [SerializeField] Gameboard gameboard;
    [SerializeField] EndScreen endScreen;
    [SerializeField] TextMeshProUGUI currentPlayerText;

    [SerializeField]  Cell[] cell;
    [SerializeField] Sprite oSprite;
    [SerializeField] Sprite xSprite;

    TurnState  turnState = TurnState.playerTurn;
    [SerializeField] public GameMode currentGameMode;

    public GameEnums.PlayerType playerSymbol;   // chosen by player
    public GameEnums.PlayerType opponentSymbol; // computer or 2nd player



    public int winnerIndex = -1;
 
 /*
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    } */


    private void Start()
    {
        SetGameState(GameEnums.GameState.ModeSelection);
        currentPlayerText.text = gameboard.currentPlayer== GameEnums.PlayerType.O? "O is Playing" : "X is Playing";
    }
    public void StartGame(GameMode mode)
    {
        SetGameMode(mode);
        //turnState = TurnState.playerTurn;
        turnReset();    
        gameboard.GameReset();
        SetGameState(GameEnums.GameState.Playing);
        currentPlayerText.text = gameboard.currentPlayer== GameEnums.PlayerType.O? "O is Playing" : "X is Playing";
        gameboard.gameObject.SetActive(true);
    }
    
    public void GameRestart()
    {
        gameboard.GameReset();

        foreach (Cell c in cell)
        {
            c.ClearSprite();
        }
        turnState = TurnState.playerTurn;

        gameboard.currentPlayer = playerSymbol;
        currentPlayerText.text =
            gameboard.currentPlayer == GameEnums.PlayerType.O
            ? "O is Playing"
            : "X is Playing";
        SetGameState(GameEnums.GameState.Playing);
    }

    public void turnReset()
    {
        turnState = TurnState.playerTurn;
    }

    public void SetPlayerType(GameEnums.PlayerType playerType)
    {
        playerSymbol = playerType;
        opponentSymbol= (playerType== GameEnums.PlayerType.X)?GameEnums.PlayerType.O:GameEnums.PlayerType.X;
        gameboard.currentPlayer= playerSymbol;
        Debug.Log("Player chose: " + playerSymbol + ", Opponent: " + opponentSymbol);
    }
    public void SetGameState(GameEnums.GameState state)
    {
        gameboard.SetState(state);
        if (state == GameEnums.GameState.Playing)
        {
            gameboard.gameObject.SetActive(true); 
            endScreen.gameObject.SetActive(false);
        }
        uIManager.UpdateUI(state);
        
    }
    public void SetGameMode(GameMode mode)
    {
        Debug.Log(mode + " This is mode");
        currentGameMode= mode;
    }

    public void OnCellClicked(int index)
    {
        if (currentGameMode == GameMode.VsComputer && turnState == TurnState.computerTurn)
        {
            return;
        }
        if(turnState != TurnState.playerTurn)
        {
            return;
        }
        bool sucess=gameboard.PlayerMove(index);
        if (!sucess)
            return;
        currentPlayerText.text = gameboard.currentPlayer== GameEnums.PlayerType.O? "O is Playing" : "X is Playing";
       // gameboard.PlayerMove(index);
        Debug.Log("player move hua hai");
        cell[index].SetSprite(gameboard.currentPlayer== GameEnums.PlayerType.O? xSprite:oSprite);
        ShowResult();
        currentPlayerText.text = gameboard.currentPlayer== GameEnums.PlayerType.O? "O is Playing" : "X is Playing";
        if(currentGameMode== GameMode.VsComputer)
        {
            turnState  = TurnState.computerTurn;
            StartCoroutine(ComputerTurnRoutine()); 
        }
        
    }

    public void ShowResult()
    {
        switch (gameboard.currentState)
        {
            case GameEnums.GameState.GameOver:
                winnerIndex = gameboard.GetWinnerIndex(); // you already have logic
                SetGameState(GameEnums.GameState.GameOver);
                break;

            case GameEnums.GameState.Draw:
                winnerIndex = -1;
                SetGameState(GameEnums.GameState.Draw);
                break;
        }
    }

    public void SetWinner(int index)
    {
        winnerIndex = index;
    }

    IEnumerator ComputerTurnRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        int computerIndex = gameboard.ComputerMove();
        if(computerIndex==-1) yield break;
        cell[computerIndex].SetSprite(gameboard.currentPlayer== GameEnums.PlayerType.O? xSprite:oSprite);
        ShowResult();
        //turn state
        turnState = TurnState.playerTurn;
        currentPlayerText.text =gameboard.currentPlayer== GameEnums.PlayerType.O? "O is Playing" : "X is Playing";
    }
}
