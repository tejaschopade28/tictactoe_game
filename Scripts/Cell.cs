using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] public int id;
    //public int val;
    [SerializeField] public Button button;

    [SerializeField] Image cellImage;
    [SerializeField] Sprite defaultIMage;
    [SerializeField] GameManager gameManager;
   // public bool isIntaractive= true;
   public bool IsOccupied => cellImage.sprite != defaultIMage;


    void Start()
    {
       // gameManager = GameManager.Instance;
        button.onClick.AddListener(OnclickButton);
    }

    public void OnclickButton()
    {
        if (gameManager.currentGameMode == GameEnums.GameMode.VsOnlinePlayer)
        {
            
            OnlineGameManager.Instance.OnCellClickedOnline(id);
        }
        else{
            gameManager.OnCellClicked(id);
        }
    }

    public void SetSprite(Sprite sprite, bool disableButton = true)
    {
        Debug.Log("SetSprite called on cell " + id);
        Debug.Log("Sprite received: " + sprite);
        cellImage.sprite = sprite;
        cellImage.enabled = true;
        button.interactable = !disableButton;
    }
    public void ClearSprite()
    {
        cellImage.sprite = defaultIMage;
       // cellImage.enabled= false;
        button.interactable=true;
    }
}
