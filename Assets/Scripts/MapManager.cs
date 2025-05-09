using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
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

    public Dropdown dropdown;
    public Button btnStart;
    public PathfinderAStar npc;

    public bool isStarted;

    private int[,] mapData10x10 = new int[,]
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

    private int[,] mapData20x20 = new int[,]
    {
        {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 3},
        {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},
        {0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
        {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1},
        {0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0},
        {1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0},
        {1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {2, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0}
    };

    private void Start()
    {
        btnStart.onClick.AddListener(OnBtnStart);
        dropdown.onValueChanged.AddListener(OnDropdown);

        OnDropdown(0);
        GenerateGrid();
    }

    public void OnBtnStart()
    {
        if (!isStarted)
        {
            isStarted = true;
            npc.StartFindingPath();
            btnStart.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(false);
        }
    }

    private void OnDropdown(int value)
    {
        if (value == 0)
        {
            mapData = mapData10x10;
            width = 10;
            height = 10;
        }
        else if (value == 1)
        {
            mapData = mapData20x20;
            width = 20;
            height = 20;
        }
        GenerateGrid();
        npc.transform.position = new Vector2(startPos.x * cellSize, -startPos.y * cellSize);
    }

    private void GenerateGrid()
    {
        if (cells != null)
        {
            foreach (Cell cell in cells)
            {
                Destroy(cell.gameObject);
            }
        }

        cells = new Cell[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 position = new(j * cellSize, -i * cellSize, 0);
                GameObject cellGo = Instantiate(cellPrefab, position, Quaternion.identity, gridParent);
                Cell cell = cellGo.GetComponent<Cell>();
                cell.x = j;
                cell.y = i;
                cell.type = mapData[i, j];
                cell.sp.sprite = GetSpriteByType(cell.type);
                cells[i, j] = cell;

                if (cell.type == 2) startPos = new Vector2Int(j, i);
                if (cell.type == 3) goalPos = new Vector2Int(j, i);
            }
        }
    }

    private Sprite GetSpriteByType(int type)
    {
        switch (type)
        {
            case 0: return pathSprite;
            case 1: return wallSprite;
            case 2: return pathSprite;
            case 3: return goalSprite;
            case 4: return visitedSprite;
            default: return pathSprite;
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

    public bool IsWall(int x, int y) => x >= 0 && x < width && y >= 0 && y < height && mapData[y, x] == 1;
}

