using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameEnums;
using System.Threading.Tasks;
public class UIManager : MonoBehaviour
{   
    [SerializeField] GameManager gameManager;
    [SerializeField] PlayerChoice playerChoice;
    [SerializeField] private Button rematchButton;

    [SerializeField] EndScreen end;

    public GameObject SelectMode;
    public GameObject WaitingRoom;
    public GameObject SelectOffline;
    public GameObject MainGame;
    public GameObject AfterGame;
    public bool isChoiceSelected= false;

    void Start()
    {
        SelectOffline.SetActive(false);
        MainGame.SetActive(false);
        AfterGame.SetActive(false);
    }
    public void OnSymbolSelected()
    {
        isChoiceSelected = true;
    }
    public void UpdateUI(GameEnums.GameState state)
    {
        // Disable all first
        SelectMode.SetActive(false);
        SelectOffline.SetActive(false);
        WaitingRoom.SetActive(false);
        MainGame.SetActive(false);
        AfterGame.SetActive(false);

        switch (state)
        {
            case GameEnums.GameState.ModeSelection:
                SelectMode.SetActive(true);
                break;

            case GameEnums.GameState.WaitingRoom:
                WaitingRoom.SetActive(true);
                break;

            case GameEnums.GameState.OfflineMode:
                SelectOffline.SetActive(true);
                break;

            case GameEnums.GameState.Playing:
                MainGame.SetActive(true);
               // rematchButton.interactable = true;
                break;

            case GameEnums.GameState.GameOver:
            case GameEnums.GameState.Draw:
                AfterGame.SetActive(true);
                end.ResetUI();
                if (state == GameEnums.GameState.Draw)
                {
                    end.ShowDraw();
                }
                else
                {
                    if (gameManager.currentGameMode == GameEnums.GameMode.VsOnlinePlayer)
                    {
                        // ONLINE
                        if (gameManager.winnerIndex == OnlineGameManager.Instance.myPlayerIndex)
                            end.ShowWinner("YOU");
                        else
                            end.ShowWinner("Opponent");
                    }
                    else
                    {
                        // OFFLINE
                        string winner = gameManager.winnerIndex == 0 ? "X" : "O";
                        end.ShowWinner(winner);
                    }
                }
                break;

        }
    }
    public void ShowOpponentLeftUI()
    {
        //AfterGame.SetActive(true);
        gameManager.SetGameState(GameEnums.GameState.GameOver);
        EndScreen end = AfterGame.GetComponent<EndScreen>();
        end.ShowOpponentLeft();
    }

    public void OnVsComputerClicked()
    {
        Debug.Log("select vs computer");

        if (!isChoiceSelected)
        {
            Debug.Log("Select X or O first");
            return;
        }
        //playerChoice.Lock();
        gameManager.SetGameMode(GameEnums.GameMode.VsComputer);
        gameManager.StartGame(GameEnums.GameMode.VsComputer);
        gameManager.SetGameState(GameEnums.GameState.Playing);
    }
    public void OnVsOffilinePlayerClicked()
    {
        Debug.Log("select  vs offline player");
       // gameManager.SetGameMode(GameManager.GameMode.VsOfflinePlayer);
        if (!isChoiceSelected)
        {
            Debug.Log("Select X or O first");
            return;
        }
       // playerChoice.Lock();
        gameManager.SetGameMode(GameEnums.GameMode.VsOfflinePlayer);
        gameManager.StartGame(GameEnums.GameMode.VsOfflinePlayer);
        gameManager.SetGameState(GameEnums.GameState.Playing);
        //gameManagerSetGameState(Gameboard.GameState.Playing);
    }
    public void OnVsOnlinePlayerClicked()
    {
        if (!isChoiceSelected)
        {
            Debug.Log("Select X or O first");
            return;
        }

        gameManager.SetGameMode(GameEnums.GameMode.VsOnlinePlayer);
        gameManager.SetGameState(GameEnums.GameState.WaitingRoom);
        WSClients.Instance.Connect();

        if (WSClients.Instance == null)
        {
            Debug.LogError("WSClient not found!");
            return;
        }
        Debug.Log("Connecting to online match...");

    }

    public void OfflineClickced()
    {
        Debug.Log("select offline mode");
        gameManager.SetGameState(GameEnums.GameState.OfflineMode);
    }
    public void OnBackClicked()
    {
        isChoiceSelected = false;
        playerChoice.Unlock();
        gameManager.SetGameState(GameEnums.GameState.ModeSelection);
    }
    public void OnBackDuringGameClicked()
    {
        if (gameManager.currentGameMode == GameEnums.GameMode.VsOnlinePlayer)
        {
            Debug.Log("click home while on online play");
            gameManager.SetGameState(GameEnums.GameState.ModeSelection);
            OnlineGameManager.Instance.isLeaving = true;
            WSClients.Instance.LeaveRoomAndDisconnect();
            
         return;
        }
        if (gameManager.currentGameMode == GameEnums.GameMode.VsComputer ||
            gameManager.currentGameMode == GameEnums.GameMode.VsOfflinePlayer)
        {
            Debug.Log("click home while on offline play");
            gameManager.GameRestart();
            gameManager.SetGameState(GameEnums.GameState.OfflineMode);
          return;
        }
        
    }
    public void OnClickedHome()
    {
        RequestLeaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnClickedRematch()
    {
        Debug.Log("Rematch clicked");

        if (gameManager.currentGameMode == GameEnums.GameMode.VsOnlinePlayer)
        {
            OnlineGameManager.Instance.RematchRequest();
            return;
        }

        if (gameManager.currentGameMode == GameEnums.GameMode.VsComputer)
        {
            Debug.Log("Vs Computer rematch");

            gameManager.GameRestart();
            gameManager.SetGameState(GameEnums.GameState.Playing);
            return;
        }

        if (gameManager.currentGameMode == GameEnums.GameMode.VsOfflinePlayer)
        {
            Debug.Log("Vs Offline rematch");

            gameManager.GameRestart();
            gameManager.SetGameState(GameEnums.GameState.Playing);
        }
    }
    public async void OnBackClickedDuringWaitingScreen()
    {
        if (gameManager.currentGameMode == GameEnums.GameMode.VsOnlinePlayer)
        {
            Debug.Log("Click back while on waiting screen");
            // Await the proper leave function
            WSClients.Instance.LeaveRoomAndDisconnect();
            gameManager.SetGameState(GameEnums.GameState.ModeSelection);

            return;
        }
    }

    public  void RequestLeaveGame()
    {
        if (gameManager.currentGameMode == GameEnums.GameMode.VsOnlinePlayer)
        {
            OnlineGameManager.Instance.isLeaving = true;
            WSClients.Instance.LeaveRoomAndDisconnect();
        }

        gameManager.SetGameState(GameEnums.GameState.ModeSelection);
    }

    public void OnClickedExit()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }
}

