using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        private static Board _instance;

        public static Board Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Board();
                }
                return _instance;
            }
        }
        void Awake()
        {
            _instance = this;
        }


        public void CreateBoard(int row, int col)
        {
            int centerRow = (int)Math.Ceiling(Convert.ToDouble(row / 2));
            int centerCol = (int)Math.Ceiling(Convert.ToDouble(col / 2));
            GameObject cellCenter = new GameObject();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    GameObject cell = Instantiate(cellPrefab, new Vector3(i, j, 0), Quaternion.identity);
                    cell.transform.parent = this.transform;

                    if (i == centerRow && j == centerCol) cellCenter = cell;
                }
            }

            float width = Screen.width;
            this.transform.localScale = Vector3.one * (0.378f);
            this.transform.position -= cellCenter.transform.position;
            print(transform.lossyScale);
        }


    }
}