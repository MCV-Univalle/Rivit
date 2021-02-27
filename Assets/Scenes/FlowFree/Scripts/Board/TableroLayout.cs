using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FlowFree;

public class TableroLayout : MonoBehaviour
{
    public static TableroLayout instance;

    private float scale;
    private int n, flujosCount;
    public GameObject cellPrefab;
    public readonly List<GameObject> cells = new List<GameObject>();
    private List<Color> colores = new List<Color>();
    public int[,] boardData;

    public int N { get => n; set => n = value; }
    public int FlujosCount { get => flujosCount; set => flujosCount = value; }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        colores.Add(Color.grey);
        colores.Add(Color.green);
        colores.Add(Color.cyan);
        colores.Add(Color.red);
        colores.Add(Color.yellow);
        colores.Add(Color.magenta);
        colores.Add(Color.blue);
    }

    public void CreateLevel(List<string[]> level)
    {
        DestroyCellList();
        N = int.Parse(level[0][0]);
        FlujosCount = int.Parse(level[0][1]);
        print(flujosCount);
        GameObject cellListValue;
        int contIndexCellList = 0;
        CreateBoard(N);
        boardData = new int[N,N];
        for (int i=0;i<N;i++)
        {
            for (int j = 0; j < N; j++)
            {
                cellListValue = cells[contIndexCellList];
                contIndexCellList++;
                int containedValueCell = int.Parse(level[i + 1][j + 1]);
                cellListValue.GetComponent<Cell>().row = i;
                cellListValue.GetComponent<Cell>().col = j;
                cellListValue.GetComponent<Cell>().containedValueCell = containedValueCell;
                cellListValue.GetComponent<SpriteRenderer>().color = colores[containedValueCell];
                boardData[i,j] = containedValueCell; 
            }
        }
    }

    public void CreateBoard(int size)
    {
        GetComponent<GridLayoutGroup>().constraintCount = size;
        scale = 2.1f / (size + 1);
        transform.localScale = Vector3.one * (scale);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);
                cell.name = "[" + i + "," + j + "]";
                cells.Add(cell);
            }
        }
    }

    public void DestroyCellList()
    {
        foreach (GameObject element in cells)
        {
            Destroy(element);
        }
        cells.Clear();
    }
}
