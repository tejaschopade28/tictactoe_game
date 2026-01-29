using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int id;

    [SerializeField] private Image cellImage;
    [SerializeField] private Sprite defaultImage;

    public bool IsOccupied => cellImage.sprite != defaultImage;

    // Called from Button OnClick (Inspector)
    public void OnCellClicked()
    {
        Debug.Log("CELL BUTTON CLICKED: " + id);
        GameManager.Instance.OnCellClicked(id);
    }
    public void OnCellClickeddummy()
    {
        Debug.Log("CELL BUTTON CLICKED: " + id);
    }
    public void SetSprite(Sprite sprite)
    {
        cellImage.sprite = sprite;
    }

    public void ClearSprite()
    {
        cellImage.sprite = defaultImage;
    }
}
