using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace FlowFree
{
    public class FlowFreeGameManager : LevelSystemGameManager
    {
        public override string Name => "FlowFree";
        [InjectOptional(Id = "SFXManager")] private AudioManager _SFXManager;
        

        public override void EndGame()
        {
        }

        public override void StartGame()
        {
        }

        private void Update()
        {
        }

        //private void GameOver()
        //{
        //    int tamTotal = 0;
        //    for (int t = 0; t < CellsInputPathGroup.Count; t++)
        //    {
        //        tamTotal += CellsInputPathGroup[t].Count;
        //    }

        //    int n = TableroLayout.instance.N;
        //    int CellsFree = (n * n) - (2 * TableroLayout.instance.FlujosCount);
        //    int[,] boardDataFinish = new int[n, n];
        //    int[,] boardDataStart = TableroLayout.instance.boardData;

        //    int complete = 0;
        //    if (tamTotal == CellsFree)
        //    {
        //        for (int i = 1; i < CellsInputPathGroup.Count - 1; i++)
        //        {
        //            int row, col, focus = 0, up = 0, down = 0, left = 0, right = 0;

        //            string name = CellsInputPathGroup[i][0];
        //            row = int.Parse(name.Substring(1, 1));
        //            col = int.Parse(name.Substring(3, 1));

        //            focus = boardDataFinish[row, col];
        //            if (row > 0) up = boardDataFinish[row - 1, col];
        //            if (row < n - 1) down = boardDataFinish[row + 1, col];
        //            if (col > 0) left = boardDataFinish[row, col - 1];
        //            if (col < n - 1) right = boardDataFinish[row, col + 1];

        //            int cont = 0;
        //            if (up == focus) cont++;
        //            if (down == focus) cont++;
        //            if (left == focus) cont++;
        //            if (right == focus) cont++;

        //            string name2 = CellsInputPathGroup[i][CellsInputPathGroup[i].Count - 1];
        //            row = int.Parse(name2.Substring(1, 1));
        //            col = int.Parse(name2.Substring(3, 1));

        //            focus = boardDataFinish[row, col];
        //            if (row > 0) up = boardDataFinish[row - 1, col];
        //            if (row < n - 1) down = boardDataFinish[row + 1, col];
        //            if (col > 0) left = boardDataFinish[row, col - 1];
        //            if (col < n - 1) right = boardDataFinish[row, col + 1];

        //            int cont2 = 0;
        //            if (up == focus) cont2++;
        //            if (down == focus) cont2++;
        //            if (left == focus) cont2++;
        //            if (right == focus) cont2++;

        //            if (cont >= 2 && cont2 >= 2) complete++;
        //        }
        //    }


        //    if (tamTotal == CellsFree && complete == CellsInputPathGroup.Count - 2)
        //    {
        //        print("Game Over: Gano");
        //        board.SetActive(false);
        //        panelLevelComplete.SetActive(true);
        //        CellsInputPathGroup.Clear();
        //    }

        //}

    }
}

