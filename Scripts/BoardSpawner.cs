using UnityEngine;
using static GameEnums;

public class BoardSpawner : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private int boardSize = 3; // 3x3 board by default

    [SerializeField] private Cell[] cells;

    void Start()
    {
        SpawnBoard();
    }

    void SpawnBoard()
    {
        int totalCells = boardSize * boardSize;
        cells = new Cell[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            Cell cell = Instantiate(cellPrefab, transform);
            cell.id = i;
            cells[i] = cell;
        }

        // Assign generated cells to managers
        if (GameManager.Instance != null)
        {
            Debug.Log("settig cells in online and offline managers");
            GameManager.Instance.SetCells(cells);
        }
        else
        {
            Debug.LogWarning("GameManager instance not found! Cells not assigned to managers.");
        }
    }
}

