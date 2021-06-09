using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class GenerateLevel : MonoBehaviour, ILevelGenerator
    {
        //-----------------------------------------
        public bool IsGameStarted { get => _isGameStarted; set => _isGameStarted = value; }
        public bool PaintCell { get => paintCell; set => paintCell = value; }
        public Color ColorPaint { get => colorPaint; set => colorPaint = value; }
        public int LevelIndexList { get => levelIndexList; set => levelIndexList = value; }
        public List<Object> LevelsList { get => levelsList; set => levelsList = value; }
        public List<List<string>> CellsInputPathGroup { get => cellsInputPathGroup; set => cellsInputPathGroup = value; }

        private bool _isGameStarted = false;
        private bool _gameOver = false;
        public int n = 5;
        public List<Object> levelsList = new List<Object>();
        public List<TextAsset> levelsList2;
        public int levelIndexList;
        public GameObject board;
        private bool paintCell = false;
        private Color colorPaint = new Color();
        public int contenCellMouseDown;

        private List<List<string>> cellsInputPathGroup = new List<List<string>>();

        public List<string> cellsInputPath1 = new List<string>();


        public GameObject panelLevelComplete;
        public GameObject cellPrefab;
        //---------------------------------------



        void ILevelGenerator.GenerateLevel(List<string[]> level)
        {
            LoadLevel(level);
            Debug.Log("Se genero el nivel correctamente!");
        }

        private void Update()
        {
            GameOver();
        }

        public void LoadLevel(List<string[]> level)
        {
            PaintCell = false;
            TableroLayout.instance.CreateLevel(level);
            FlowFreeUIManager.instance.SetLevelTmpLabel();

            CellsInputPathGroup.Clear();
            for (int i = 0; i <= 5; i++)
            {
                CellsInputPathGroup.Add(new List<string>());
            }
        }

        public void BreakCellImputPath(int row, int col, int containedValueCell)
        {
            string name = "[" + row + "," + col + "]";
            int indexName = CellsInputPathGroup[containedValueCell].IndexOf(name);
            List<string> pathCut = new List<string>();
            pathCut.Clear();
            pathCut = CellsInputPathGroup[containedValueCell].GetRange(indexName + 1, CellsInputPathGroup[containedValueCell].Count - indexName - 1);

            print("pathCut" + pathCut[0]);

            for (int i = 0; i < pathCut.Count; i++)
            {
                for (int j = 0; j < TableroLayout.instance.cells.Count; j++)
                {
                    if (TableroLayout.instance.cells[j].name == pathCut[i])
                    {
                        TableroLayout.instance.cells[j].GetComponent<SpriteRenderer>().color = Color.gray;
                        TableroLayout.instance.cells[j].GetComponent<Cell>().containedValueCell = 0;
                    }
                }
            }
            CellsInputPathGroup[containedValueCell] = CellsInputPathGroup[containedValueCell].GetRange(0, indexName + 1);
        }

        private void GameOver()
        {
            int tamTotal = 0;
            for (int t = 0; t < CellsInputPathGroup.Count; t++)
            {
                tamTotal += CellsInputPathGroup[t].Count;
            }

            int n = TableroLayout.instance.N;
            int CellsFree = (n * n) - (2 * TableroLayout.instance.FlujosCount);
            int[,] boardDataFinish = new int[n, n];
            int[,] boardDataStart = TableroLayout.instance.boardData;

            int complete = 0;
            if (tamTotal == CellsFree)
            {
                for (int i = 1; i < CellsInputPathGroup.Count - 1; i++)
                {
                    int row, col, focus = 0, up = 0, down = 0, left = 0, right = 0;

                    string name = CellsInputPathGroup[i][0];
                    row = int.Parse(name.Substring(1, 1));
                    col = int.Parse(name.Substring(3, 1));

                    focus = boardDataFinish[row, col];
                    if (row > 0) up = boardDataFinish[row - 1, col];
                    if (row < n - 1) down = boardDataFinish[row + 1, col];
                    if (col > 0) left = boardDataFinish[row, col - 1];
                    if (col < n - 1) right = boardDataFinish[row, col + 1];

                    int cont = 0;
                    if (up == focus) cont++;
                    if (down == focus) cont++;
                    if (left == focus) cont++;
                    if (right == focus) cont++;

                    string name2 = CellsInputPathGroup[i][CellsInputPathGroup[i].Count - 1];
                    row = int.Parse(name2.Substring(1, 1));
                    col = int.Parse(name2.Substring(3, 1));

                    focus = boardDataFinish[row, col];
                    if (row > 0) up = boardDataFinish[row - 1, col];
                    if (row < n - 1) down = boardDataFinish[row + 1, col];
                    if (col > 0) left = boardDataFinish[row, col - 1];
                    if (col < n - 1) right = boardDataFinish[row, col + 1];

                    int cont2 = 0;
                    if (up == focus) cont2++;
                    if (down == focus) cont2++;
                    if (left == focus) cont2++;
                    if (right == focus) cont2++;

                    if (cont >= 2 && cont2 >= 2) complete++;
                }
            }


            if (tamTotal == CellsFree && complete == CellsInputPathGroup.Count - 2)
            {
                print("Game Over: Gano");
                board.SetActive(false);
                panelLevelComplete.SetActive(true);
                CellsInputPathGroup.Clear();
            }

        }
    }

}
