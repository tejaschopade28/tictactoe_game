using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameEnums;

public class UIMenuManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private PlayerChoice playerChoice;

    [Header("Screens (assign only what exists in THIS scene)")]
    public GameObject SelectMode;     // MenuScene
    public GameObject SelectOffline;  // MenuScene
    public GameObject WaitingRoom;    // MenuScene

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
        SetActiveSafe(SelectMode, true);
        SetActiveSafe(SelectOffline, false);
        SetActiveSafe(WaitingRoom, false);
    }

    void SetActiveSafe(GameObject go, bool value)
    {
        Debug.Log("called set active safe");
        if (go != null)
        Debug.Log("go object is not null");
            go.SetActive(value);
    }

    void DisableAllScreens()
    {
        SetActiveSafe(SelectMode, false);
        SetActiveSafe(SelectOffline, false);
        SetActiveSafe(WaitingRoom, false);
    }

    public void UpdateUI(GameState state)
    {
        DisableAllScreens();
        Debug.Log("UIManager: Updating UI for state: " + state);


        switch (state)
        {
            case GameState.ModeSelection:
                SetActiveSafe(SelectMode, true);
                Debug.Log("SelectMode active: " + (SelectMode != null ? SelectMode.activeSelf.ToString() : "NULL"));
                break;

            case GameState.OfflineMode:
                SetActiveSafe(SelectOffline, true);
                break;

            case GameState.WaitingRoom:
                SetActiveSafe(WaitingRoom, true);
                break;
        }
    }

    // ================= SYMBOL =================

    public void OnSymbolSelected()
    {
        isChoiceSelected = true;
    }

    bool ValidateSymbolSelection()
    {
        if (!isChoiceSelected)
        {
            Debug.Log("Select X or O first");
            return false;
        }

        if (playerChoice != null)
            playerChoice.Lock();

        return true;
    }


    public void OfflineClicked()
    {
        GameManager.Instance.SetGameState(GameState.OfflineMode);
    }

    public void OnVsComputerClicked()
    {
        
        if (!ValidateSymbolSelection()) return;

        GameManager.Instance.SetGameMode(GameMode.VsComputer);
        SceneManager.LoadScene("GameScene");
        
    }

    public void OnVsOfflinePlayerClicked()
    {
        
        if (!ValidateSymbolSelection()) return;
        GameManager.Instance.SetGameMode(GameMode.VsOfflinePlayer);
        SceneManager.LoadScene("GameScene");
        //GameManager.Instance.SetGameState(GameState.Playing);

        
    }

    public void OnVsOnlinePlayerClicked()
    {
        
        if (!ValidateSymbolSelection()) return;
        GameManager.Instance.SetGameMode(GameMode.VsOnlinePlayer);

        WSClients.Instance.Connect();
        SceneManager.LoadScene("GameScene");
    }

    public void OnBackClicked()
    {
        isChoiceSelected = false;

        if (playerChoice != null)
            playerChoice.Unlock();

        GameManager.Instance.SetGameState(GameState.ModeSelection);
    }
    public void OnBackClickedDuringWaitingScreen()
    {
        WSClients.Instance.LeaveRoomAndDisconnect();
        GameManager.Instance.SetGameState(GameState.ModeSelection);
    }
    public void OnClickedExit()
    {
        Application.Quit();
    }
}
