using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameEnums;

public class UIGameManager : MonoBehaviour
{
    [Header("Managers")]
    public GameObject MainGame;        // GameScene
    public GameObject AfterGame;       // GameScene

    [Header("UI")]
    [SerializeField] private EndScreen end;
    [SerializeField] private Button rematchButton;

    public bool isChoiceSelected = false;
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += UpdateUI;

        //  Sync UI with current state immediately
        if (GameManager.Instance != null)
        {
            UpdateUI(GameManager.Instance.currentGameState);
        }
    }


    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= UpdateUI;
    }

    private void Start()
    {
        SetActiveSafe(MainGame, true);
        SetActiveSafe(AfterGame, false);
    }
    void SetActiveSafe(GameObject go, bool value)
    {
        if (go != null)
        {
            Debug.Log("Setting active: " + go.name + " = " + value);
            go.SetActive(value);
        }
    }


    void DisableAllScreens()
    {
        SetActiveSafe(MainGame, false);
        SetActiveSafe(AfterGame, false);
    }

    // ================= UI UPDATE =================

    public void UpdateUI(GameState state)
    {
        DisableAllScreens();
        Debug.Log("UIManager: Updating UI for state: " + state);

        switch (state)
        {
            case GameState.Playing:
               Debug.Log("Playing on board");
               SetActiveSafe(MainGame, true);
                break;

            case GameState.GameOver:
            case GameState.Draw:
                ShowEndGameUI(state);
                break;
        }
    }

    // ================= SYMBOL =================

    public void OnSymbolSelected()
    {
        isChoiceSelected = true;
    }
    void ShowEndGameUI(GameState state)
    {
       // SetActiveSafe(AfterGame, true);

        if (end == null) return;

        end.ResetUI();

        if (state == GameState.Draw)
        {
            end.ShowDraw();
            return;
        }

        if (GameManager.Instance.currentGameMode == GameMode.VsOnlinePlayer)
        {
            bool isMeWinner =
                GameManager.Instance.winnerIndex == OnlineGameManager.Instance.myPlayerIndex;
            end.ShowWinner(isMeWinner ? "YOU" : "Opponent");
        }
        else
        {
            string winner = GameManager.Instance.winnerIndex == 0 ? "X" : "O";
            end.ShowWinner(winner);
        } 
    }

    public void ShowOpponentLeftUI()
    {
        if (end != null)
            end.ShowOpponentLeft();
    }

    // ================= BACK / HOME =================

    public void OnBackDuringGameClicked()
    {
        Debug.Log("mainc scene loadeded");
        SceneManager.LoadScene("MenuScene");
        if (GameManager.Instance.currentGameMode == GameMode.VsOnlinePlayer)
        {
            OnlineGameManager.Instance.isLeaving = true;
            WSClients.Instance.LeaveRoomAndDisconnect();
        }
        GameManager.Instance.GameRestart();
        GameManager.Instance.SetGameState(GameState.OfflineMode);
    }

    public void OnClickedRematch()
    {
        if (GameManager.Instance.currentGameMode == GameMode.VsOnlinePlayer)
        {
            OnlineGameManager.Instance.RematchRequest();
        }
        else
        {
            GameManager.Instance.GameRestart();
            GameManager.Instance.SetGameState(GameState.Playing);
        }
    }

    public void OnClickedHome()
    {
        if (GameManager.Instance.currentGameMode == GameMode.VsOnlinePlayer)
        {
            OnlineGameManager.Instance.isLeaving = true;
            WSClients.Instance.LeaveRoomAndDisconnect();
        }

        GameManager.Instance.SetGameState(GameState.ModeSelection);
        SceneManager.LoadScene("MenuScene");
    }

    public void OnClickedExit()
    {
        Application.Quit();
    }
}
