using TMPro;
using UnityEngine;
using static GameEnums;

public class OnlineGameManager : MonoBehaviour
{
    public static OnlineGameManager Instance;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] public Cell[] cells;
    //[SerializeField] private UIGameManager uIGameManager;
    [SerializeField] private EndScreen endScreen;

    [Header("Sprites")]
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    [SerializeField] private TextMeshProUGUI currentPlayerText;

    [Header("State")]
    public RematchState rematchState = RematchState.None;
    public int myPlayerIndex = -1;
    public int currentPlayerIndex = -1;

    private bool inputLocked = false;
    private bool gameOver = false;
    public bool isLeaving = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.RegisterOnlineManager(this);
    }
    public void SetCells(Cell[] cellsArray)
    {
        cells = cellsArray;
    }

    public void StartOnlineGame(int myIndex)
    {
        myPlayerIndex = myIndex;
        currentPlayerIndex = 0;
        gameOver = false;
        isLeaving = false;
        inputLocked = myPlayerIndex != 0;

        ClearBoard();
        endScreen.ResetUI();
        endScreen.Hide();

        gameManager.SetGameState(GameState.Playing);

        UpdateCurrentPlayerText();
    }

    public void ServerBoardApply(int[] board, int turn)
    {
        inputLocked = turn != myPlayerIndex;
        currentPlayerIndex = turn;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 1) cells[i].SetSprite(xSprite);
            else if (board[i] == 2) cells[i].SetSprite(oSprite);
            else cells[i].ClearSprite();
        }

        if (turn != -1)
            UpdateCurrentPlayerText();
    }


    public void OnCellClickedOnline(int index)
    {
        if (gameOver || inputLocked || currentPlayerIndex != myPlayerIndex) return;
        if (cells[index].IsOccupied) return;

        inputLocked = true;
        WSClients.Instance.SendMove(index);
    }


    public void OnGameOver(int winnerIndex)
    {
        gameOver = true;
        gameManager.OnOnlineGameOver(winnerIndex);
    }

    public void OnOpponentLeft()
    {
        if (isLeaving) return;

        gameOver = true;
        inputLocked = true;

        gameManager.OnOpponentLeft();
    }


    public void RematchRequest()
    {
        if (rematchState == RematchState.IAccepted) return;

        rematchState = RematchState.IAccepted;
        endScreen.UpdateRematchUI(rematchState);
        WSClients.Instance.SendRematch();
    }

    public void OnRematchUpdate(bool[] accepted)
    {
        if (accepted == null || accepted.Length < 2) return;

        bool iAccepted = accepted[myPlayerIndex];
        bool opponentAccepted = accepted[1 - myPlayerIndex];

        if (iAccepted && opponentAccepted) rematchState = RematchState.BothAccepted;
        else if (opponentAccepted) rematchState = RematchState.OpponentAccepted;
        else if (iAccepted) rematchState = RematchState.IAccepted;
        else rematchState = RematchState.None;

        endScreen.UpdateRematchUI(rematchState);
    }

    public void StartRematch()
    {
        rematchState = RematchState.None;
        gameOver = false;
        isLeaving = false;
        inputLocked = myPlayerIndex != 0;
        currentPlayerIndex = 0;

        ClearBoard();
        endScreen.ResetUI();
        endScreen.Hide();

        gameManager.SetGameState(GameState.Playing);
        UpdateCurrentPlayerText();
    }
    private void ClearBoard()
    {
        foreach (var c in cells)
            c.ClearSprite();
    }

    private void UpdateCurrentPlayerText()
    {
        currentPlayerText.text = currentPlayerIndex == myPlayerIndex ? "Your turn" : "Opponent turn";
    }
}
