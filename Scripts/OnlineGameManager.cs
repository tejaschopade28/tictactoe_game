using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameEnums;
public class OnlineGameManager : MonoBehaviour
{
    public static OnlineGameManager Instance;
    [SerializeField] GameManager gameManager;
    [SerializeField] Cell[] cells;
    [SerializeField] Sprite xSprite;
    [SerializeField] Sprite oSprite;
    [SerializeField] TextMeshProUGUI currentPlayerText;
    [SerializeField] EndScreen endScreen;
    [SerializeField] UIManager uIManager;
    public RematchState rematchState = RematchState.None;

    public int myPlayerIndex=-1;
    public int currentPlayerIndex = -1;
    bool gameOver= false;
    bool inputLocked = false;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void StartOnlineGame(int myIndex)
    {
        Debug.Log("On server Start");
        myPlayerIndex = myIndex; 
        gameOver= false;
        isLeaving = false;

        ClearBoard();
        gameManager.SetGameMode(GameEnums.GameMode.VsOnlinePlayer);
        gameManager.SetGameState(GameEnums.GameState.Playing);
        endScreen.gameObject.SetActive(false);

        currentPlayerIndex = 0;
        //gameboard.GameReset();
        currentPlayerText.text = myIndex==0? "Your turn": "Opponent turn";
    }

    public void ServerBoardApply( int[] board, int turn)
    {
        Debug.Log("turn" + turn);
       
        Debug.Log("Server board applying ### ");

        inputLocked = turn != myPlayerIndex;
        currentPlayerIndex= turn;
        for(int i = 0; i < board.Length; i++)
        {
            if (board[i] == 1)
            {
                cells[i].SetSprite(xSprite,false);
            }
            else if (board[i] == 2)
            {
                cells[i].SetSprite(oSprite, false);
            }
            else
            {
                cells[i].ClearSprite();
            }
        }
        if(turn !=-1)
            currentPlayerText.text= (turn == myPlayerIndex)?
                                 "Your turn": "Opponent turn";
    }
    public void OnCellClickedOnline(int index)
    {
        Debug.Log($"CLICK index={index} my={myPlayerIndex} turn={currentPlayerIndex}");
        if (gameOver) return;
        if (inputLocked) return;
        if (currentPlayerIndex != myPlayerIndex) return;
        inputLocked = true;
        if (cells[index].IsOccupied) return; 
        WSClients.Instance.SendMove(index);
    }
    public void OnGameOver(int winnerIndex)
    {
        gameManager.winnerIndex = winnerIndex;

        if (winnerIndex == -1)
            gameManager.SetGameState(GameEnums.GameState.Draw);
        else
            gameManager.SetGameState(GameEnums.GameState.GameOver);
    }
    public bool isLeaving = false;

    public void OnOpponentLeft()
    {
        if (isLeaving)
        {
            return;
        }
        Debug.Log("Opponent left");
        gameOver = true;
        inputLocked = true;
        uIManager.ShowOpponentLeftUI();
    }

    public void RematchRequest()
    {
        if (rematchState == RematchState.IAccepted)
        return;
        Debug.Log("REMATCH BUTTON CLICKED");
        rematchState = RematchState.IAccepted;
        endScreen.UpdateRematchUI(rematchState);
        WSClients.Instance.SendRematch();
    }

public void OnRematchUpdate(bool[] accepted)
{
    if (accepted == null || accepted.Length < 2)
    {
        Debug.LogWarning("Invalid rematch update");
        return;
    }
    Debug.Log("updateed rematch");
    bool iAccepted = accepted[myPlayerIndex];
    bool opponentAccepted = accepted[1 - myPlayerIndex];

    if (iAccepted && opponentAccepted)
    {
        rematchState = RematchState.BothAccepted;
    }
    else if (opponentAccepted)
    {
        rematchState = RematchState.OpponentAccepted;
    }
    else if (iAccepted)
    {
        rematchState = RematchState.IAccepted;
    }
    else
    {
        rematchState = RematchState.None;
    }

    endScreen.UpdateRematchUI(rematchState);
}


    public void StartRematch()
    {  
        Debug.Log("Rematch started");
        rematchState = RematchState.None;
        gameOver = false;
        isLeaving = false;

        inputLocked = myPlayerIndex != 0;

        ClearBoard();
        currentPlayerIndex = 0;
        endScreen.ResetUI();
        endScreen.Hide();
        gameManager.SetGameState(GameEnums.GameState.Playing);

        currentPlayerText.text =
            myPlayerIndex == 0 ? "Your turn" : "Opponent turn";
    }


    void ClearBoard()
    {
        foreach (var c in cells)
            c.ClearSprite();
    }
}
