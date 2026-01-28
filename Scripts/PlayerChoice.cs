using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class PlayerChoice : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] Toggle xToggle;
    [SerializeField] Toggle oToggle;

    public void OnXSelected(bool isOn)
    {
        if (!isOn) return;
        gameManager.SetPlayerType(GameEnums.PlayerType.X);
        uIManager.OnSymbolSelected();
    }

    public void OnOSelected(bool isOn)
    {
        if (!isOn) return;
        gameManager.SetPlayerType(GameEnums.PlayerType.O);
        uIManager.OnSymbolSelected();
    }
    
    public void Unlock()
    {
        Debug.Log("into unlock");
        xToggle.interactable = true;
        oToggle.interactable = true;
        xToggle.isOn = false;
        oToggle.isOn = false;
    }
}
