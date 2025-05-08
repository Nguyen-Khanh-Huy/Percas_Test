using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int width;
    public int height;
    public int[,] mapData;
    public GameObject cellPrefab;
    public float cellSize = 6f;
    public Transform gridParent;
    public Sprite wallSprite;
    public Sprite pathSprite;
    public Sprite goalSprite;
    public Sprite visitedSprite;

    private Cell[,] cells;
    private Vector2Int startPos;
    private Vector2Int goalPos;

    private void Start()
    {
        mapData = new int[,]
        {
            {0, 0, 0, 1, 0, 0, 0, 1, 0, 3},
            {0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 1, 0, 0},
            {1, 0, 0, 0, 0, 1, 0, 0, 0, 1},
            {0, 0, 1, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
            {0, 1, 0, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 1, 0, 1, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 0},
            {2, 0, 0, 0, 0, 1, 0, 0, 0, 0}
        };
        height = mapData.GetLength(0);
        width = mapData.GetLength(1);

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        cells = new Cell[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 position = new(j * cellSize, -i * cellSize, 0);
                GameObject cellGo = Instantiate(cellPrefab, position, Quaternion.identity, gridParent);
                Cell cell = cellGo.GetComponent<Cell>();
                cell.UpdateCell(j, i, mapData[i, j], wallSprite, pathSprite, goalSprite, visitedSprite);
                cells[i, j] = cell;

                if (mapData[i, j] == 2) startPos = new Vector2Int(j, i);
                if (mapData[i, j] == 3) goalPos = new Vector2Int(j, i);
            }
        }
    }

    public Vector2Int GetStartPos() => startPos;
    public Vector2Int GetGoalPos() => goalPos;

    public Cell GetCellAtPos(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return cells[y, x];
        
        return null;
    }

    public bool IsWall(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height && mapData[y, x] != 1;
    }
}