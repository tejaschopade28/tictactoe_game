using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEnums;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<GameState> OnGameStateChanged;

    [Header("Managers")]
    private OfflineManager offlineManager;
    private OnlineGameManager onlineGameManager;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentPlayerText; // Assign in GameScene if you have UI

    [Header("Game State")]
    public GameMode currentGameMode;
    public GameState currentGameState;
    public int winnerIndex = -1;

    [Header("Player Choice")]
    public PlayerType playerSymbol = PlayerType.X;
    public PlayerType opponentSymbol = PlayerType.O;
    private Cell[] cachedCells;


    // Internal flags for safe startup
    private bool cellsReady = false;
    private bool offlineReady = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentGameState = GameState.ModeSelection;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // -------------------- Registration --------------------
    public void RegisterOfflineManager(OfflineManager manager)
    {
        offlineManager = manager;
        offlineReady = true;
        Debug.Log("regstered offline");
    }

    public void RegisterOnlineManager(OnlineGameManager manager)
    {
        onlineGameManager = manager;
    }

    // -------------------- Game Mode --------------------
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;

        switch (mode)
        {
            case GameMode.VsComputer:
            case GameMode.VsOfflinePlayer:
                SetGameState(GameState.Playing); // intent only
                TryStartOfflineGame();
                break;

            case GameMode.VsOnlinePlayer:
                SetGameState(GameState.WaitingRoom);
                break;
        }
    }

    public void SetPlayerType(PlayerType type)
    {
        playerSymbol = type;
        opponentSymbol = (type == PlayerType.X) ? PlayerType.O : PlayerType.X;

        Debug.Log($"Player symbol: {playerSymbol} | Opponent: {opponentSymbol}");
    }

    // -------------------- Cells --------------------
    public void SetCells(Cell[] cellsArray)
    {
        Debug.Log("Setting cells in managers");
        cellsReady = true;
        cachedCells = cellsArray;
        offlineManager?.SetCells(cellsArray);
        onlineGameManager?.SetCells(cellsArray);
        
        // Only start offline game if the mode requires it
       /* if (currentGameMode == GameMode.VsComputer || currentGameMode == GameMode.VsOfflinePlayer)
        {
            offlineManager?.StartOfflineGame(playerSymbol);
        }*/
        TryStartOfflineGame();
    }
    private void TryStartOfflineGame()
    {
        if (!offlineReady || !cellsReady)
        {
            Debug.Log("Waiting... offlineReady=" + offlineReady + " cellsReady=" + cellsReady);
            return;
        }

        if (currentGameMode != GameMode.VsComputer &&
            currentGameMode != GameMode.VsOfflinePlayer)
            return;

        Debug.Log(" Starting offline game!");
        offlineManager.StartOfflineGame(playerSymbol);
        UpdateCurrentPlayerText();
    }


    // -------------------- Scene Loaded --------------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene") return;

        Debug.Log("GameScene loaded");
        // Offline game will auto-start via TryStartOfflineGame()
        // Online game will wait for server callback OnOnlineGameStarted()
    }

    // -------------------- Game State --------------------
    public void SetGameState(GameState state)
    {
        currentGameState = state;
        Debug.Log("GameState changed to: " + state);
        OnGameStateChanged?.Invoke(state);
    }

    // -------------------- Gameplay --------------------
    public void OnCellClicked(int index)
    {
        Debug.Log("id of the cell is "+ index);

        if (currentGameState != GameState.Playing) return;

        if (currentGameMode == GameMode.VsOnlinePlayer)
        {
            onlineGameManager.OnCellClickedOnline(index);
        }
        else
        {
            Debug.Log("Clinking offline");
            offlineManager.OnCellClickedOffline(index);
            UpdateCurrentPlayerText();
        }
    }

    public void UpdateCurrentPlayerText()
    {
        if (currentGameMode == GameMode.VsOnlinePlayer) return;

        if (currentPlayerText != null)
            currentPlayerText.text = offlineManager.boardState.currentPlayer.ToString() + " is Playing";
    }
    public void OnOfflineGameOver(PlayerType winner)
    {
        winnerIndex = (winner == PlayerType.X) ? 0 :
                      (winner == PlayerType.O) ? 1 : -1;

        SetGameState(winner == PlayerType.empty ? GameState.Draw : GameState.GameOver);
    }

    public void OnOnlineGameOver(int serverWinnerIndex)
    {
        winnerIndex = serverWinnerIndex;
        SetGameState(serverWinnerIndex == -1 ? GameState.Draw : GameState.GameOver);
    }

    public void OnOnlineGameStarted()
    {
        SetGameState(GameState.Playing);
    }

    public void OnOpponentLeft()
    {
        SetGameState(GameState.GameOver);
        // Show UI message
    }

    public void GameRestart()
    {
        winnerIndex = -1;

        if (currentGameMode == GameMode.VsComputer || currentGameMode == GameMode.VsOfflinePlayer)
        {
            offlineManager.ResetGame();
            TryStartOfflineGame();
        }
        else if (currentGameMode == GameMode.VsOnlinePlayer)
        {
            onlineGameManager.StartRematch();
        }

        Debug.Log("Game restarted");
    }
}
