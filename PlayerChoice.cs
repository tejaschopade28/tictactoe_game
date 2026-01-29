using UnityEngine;
using UnityEngine.UI;
using static GameEnums;

public class PlayerChoice : MonoBehaviour
{
    [SerializeField] UIMenuManager uIMenuManager;
    [SerializeField] Toggle xToggle;
    [SerializeField] Toggle oToggle;

    private void Awake()
    {
        // Auto-link UIManager if not assigned
        if (uIMenuManager == null)
            uIMenuManager = FindObjectOfType<UIMenuManager>();
    }

    public void OnXSelected(bool isOn)
    {
        if (!isOn) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL");
            return;
        }

        GameManager.Instance.SetPlayerType(PlayerType.X);
        uIMenuManager.OnSymbolSelected();
    }

    public void OnOSelected(bool isOn)
    {
        if (!isOn) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL");
            return;
        }

        GameManager.Instance.SetPlayerType(PlayerType.O);
        uIMenuManager.OnSymbolSelected();
    }

    public void Unlock()
    {
        xToggle.interactable = true;
        oToggle.interactable = true;

        xToggle.isOn = false;
        oToggle.isOn = false;
    }

    public void Lock()
    {
        xToggle.interactable = false;
        oToggle.interactable = false;
    }
}
