using System.Collections;
using UnityEngine;
using static GameEnums;

public class OfflineManager : MonoBehaviour
{
    public BoardState boardState;
    private BoardLogic boardLogic;
    
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    //[SerializeField] private GameManager gameManager;
    [SerializeField] public Cell[] cells;
    public bool vsComputer = true;
    private bool inputLocked = false;

    private void Awake()
    {
        if (boardState == null)
            boardState = new BoardState();
        boardState.Initialize();
        boardLogic = new BoardLogic(boardState);
    }
    private void Start()
    {
        GameManager.Instance.RegisterOfflineManager(this);
    }

    public void SetCells(Cell[] cellsArray)
        {
            cells = cellsArray;
            Debug.Log(cellsArray.Length);
        }
    public void StartOfflineGame(PlayerType startingPlayer)
    {
        if (cells == null || cells.Length == 0)
        {
            Debug.LogWarning("OfflineManager: Cells not ready yet, delaying start...");
            return;
        }
        boardState.Reset();
        Debug.Log("Cells length: " + cells.Length);

        boardState.currentPlayer = startingPlayer;
        inputLocked = false;

        foreach (var c in cells)
            c.ClearSprite();
    }

    public void OnCellClickedOffline(int index)
    {
        if (inputLocked) return;
        if (boardState.winner != PlayerType.empty) return;

        bool success = boardLogic.PlayerMove(index);
        if (!success) return;

        UpdateCellVisual(index);
        Debug.Log(index);

        if (CheckGameEnd())
            return;

        if (vsComputer)
        {
            inputLocked = true;
            StartCoroutine(ComputerTurn());
        }
    }

    IEnumerator ComputerTurn()
    {
        yield return new WaitForSeconds(0.8f);

        int aiIndex = boardLogic.ComputerMove();
        if (aiIndex < 0)
        {
            CheckGameEnd();
            yield break;
        }

        UpdateCellVisual(aiIndex);
        inputLocked = false;

        CheckGameEnd();
    }

    private bool CheckGameEnd()
    {
        if (boardState.winner != PlayerType.empty)
        {
            GameManager.Instance.OnOfflineGameOver(boardState.winner);
            return true;
        }

        if (boardState.moveCount >= boardState.BoardSize * boardState.BoardSize)
        {
            GameManager.Instance.SetGameState(GameState.Draw);
            return true;
        }

        return false;
    }

private void UpdateCellVisual(int index)
{
    if (cells == null)
    {
        Debug.LogError("cells array is NULL");
        return;
    }

    if (index < 0 || index >= cells.Length)
    {
        Debug.LogError($"Invalid index {index}, cells.Length = {cells.Length}");
        return;
    }

    var value = boardState.Board[
        index / boardState.BoardSize,
        index % boardState.BoardSize
    ];

    if (value == PlayerType.X)
        cells[index].SetSprite(xSprite);
    else if (value == PlayerType.O)
        cells[index].SetSprite(oSprite);
}



    public void ResetGame()
    {
        boardState.Reset();
        inputLocked = false;

        if (cells == null) return;
        foreach (var c in cells)
            c.ClearSprite();
    }

}
