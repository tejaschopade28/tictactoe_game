using JetBrains.Annotations;
using TMPro;
using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class EndScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button rematchButton;
    [SerializeField] Button homeButton;
    [SerializeField] GameObject rematchStatusRoot;
    [SerializeField] TextMeshProUGUI rematchStatusText;

    void Awake()
    {
        ResetUI();
    }

    public void ShowWinner(string winner)
    {
        gameObject.SetActive(true);
        resultText.text = winner + " Wins the game";
        HideRematchStatus();
    }

    public void ShowDraw()
    {
        gameObject.SetActive(true);
        resultText.text = "Game Draw!";
        HideRematchStatus();
    }

    public void UpdateRematchUI(RematchState state)
    {
        rematchStatusRoot.SetActive(true);
        rematchStatusText.text = ""; // use this for rematch messages

        switch (state)
        {
            case RematchState.IAccepted:
                rematchStatusText.text = "Waiting for opponent...";
                rematchButton.interactable = false;
                break;
            case RematchState.OpponentAccepted:
                rematchStatusText.text = "Opponent wants rematch...";
                rematchButton.interactable = true;
                break;
            case RematchState.BothAccepted:
                rematchStatusText.text = "Match starting...";
                rematchButton.interactable = false;
                break;
            case RematchState.None:
            default:
                rematchStatusRoot.SetActive(false);
                rematchButton.interactable = true;
                break;
        }
    }

    public void ShowOpponentLeft()
    {
        Debug.Log("Showing opponent left");

        resultText.text = "Opponent left the game";

        rematchStatusRoot.SetActive(false);
        rematchButton.gameObject.SetActive(false);
        rematchButton.interactable = false;

        homeButton.gameObject.SetActive(true);
    }


    // only hides text, DOES NOT change button state
    private void HideRematchStatus()
    {
        rematchStatusRoot.SetActive(false);
        rematchStatusText.text = "";
    }

    // called ONLY when new match starts
    public void ResetUI()
    {
        resultText.text = "";
        rematchStatusText.text = "";
        rematchStatusRoot.SetActive(false);
        rematchButton.interactable = true;
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
