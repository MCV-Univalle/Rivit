using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFreeV2
{

    public class GenerateBoard : MonoBehaviour
    {
        public static GenerateBoard _instance;

        [SerializeField] private GameObject cellPrefab;

        private float scale;
        private int n, flujosCount;
        public readonly List<GameObject> cells = new List<GameObject>();
        private List<Color> colores = new List<Color>();
        public int[,] boardData;

        public int N { get => n; set => n = value; }
        public int FlujosCount { get => flujosCount; set => flujosCount = value; }
        public int CantLines { get => cantLines; set => cantLines = value; }
        public int TamBoard { get => tamBoard; set => tamBoard = value; }

        //public List<TextAsset> levelsList = new List<TextAsset>();
        [SerializeField] private TextAsset levelVoid;
        private List<Color> coloresList = new List<Color>();
        private int tamBoard;
        private int cantLines;
        private int indexList = 1;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
                CrearListaColores();
                CreateBoardVoid();
            }
            else
            {
                Destroy(this);
            }
        }

        void Start()
        {

        }


        public void CreateBoardVoid()
        {
            CreateBoard(ReadTxt(levelVoid));
            FlowFreeV2UIManager._instance.ChangeLevelNameCurrent();
        }

        public void CreateBoard(List<string[]> level)
        {
            ClearChildren();
            FlowFreeV2UIManager._instance.ChangeLevelNameCurrent();

            TamBoard = int.Parse(level[0][0]);
            CantLines = int.Parse(level[0][1]);
            int size = tamBoard;

            LineManager._instance.ClearChildren();
            LineManager._instance.AddLineTolist();
            LineManager._instance.InstanciarPathLineList();
            InputMouse._instance.MouseEnterPointLisAdd();

            gameObject.GetComponent<GridLayoutGroup>().constraintCount = size;
            scale = 2.1f / (size + 1);
            transform.localScale = Vector3.one * (scale);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    GameObject cell = Instantiate(cellPrefab, transform);
                    cell.name = "[" + i + "," + j + "]";
                    GetChildWithName(cell, "circulo_blanco").name = cell.name;
                    cells.Add(cell);
                    GameObject cosa = cell.transform.GetChild(0).gameObject;

                    int content = int.Parse(level[i + 1][j]);
                    //print(cell.name + " = " + content);

                    cosa.GetComponent<Renderer>().material.SetColor("_Color", coloresList[content]);

                    cosa.GetComponent<Point>().content = content;

                    AssignStartAndEndPoint(content, i, j);
                }
            }
        }

        void AssignStartAndEndPoint(int content, int i, int j)
        {
            if (content != 0)
            {
                GameObject lineObject = LineManager._instance.lineList[content - 1];

                string pointInicial = lineObject.GetComponent<LineCreator>().posPointIni;
                string pointFinal = lineObject.GetComponent<LineCreator>().posPointFin;
                if (string.IsNullOrEmpty(pointInicial))
                {
                    lineObject.GetComponent<LineCreator>().posPointIni = "[" + i + "," + j + "]";
                }
                else if (string.IsNullOrEmpty(pointFinal))
                {
                    lineObject.GetComponent<LineCreator>().posPointFin = "[" + i + "," + j + "]";
                }
            }
        }

        public void ClearChildren()
        {
            int i = 0;
            GameObject[] allChildren = new GameObject[transform.childCount];
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        public static List<string[]> ReadTxt(TextAsset txt)
        {
            List<string[]> myList = new List<string[]>();
            try
            {
                List<string> lines = new List<string>(txt.text.Split('\n'));
                char letter = ' ';
                for (int i = 0; i < lines.Count; i++)
                {
                    myList.Add(lines[i].Split(letter));
                    letter = '\t';
                }
            }
            catch (System.Exception e)
            {
                print("The file could not be read:");
                print(e.Message);
            }
            return myList;
        }

        public void CrearListaColores()
        {
            Color color = Color.white;
            color.a = 0.0f;
            coloresList.Add(color);
            coloresList.Add(Color.red);
            coloresList.Add(Color.yellow);
            coloresList.Add(Color.blue);
            coloresList.Add(Color.green);
            coloresList.Add(Color.cyan);
            coloresList.Add(Color.magenta);


            ColorUtility.TryParseHtmlString("#F2533A", out color);
            coloresList[1] = color;
            ColorUtility.TryParseHtmlString("#F9D91E", out color);
            coloresList[2] = color;
            ColorUtility.TryParseHtmlString("#3A7FF2", out color);
            coloresList[3] = color;
            ColorUtility.TryParseHtmlString("#BF46F2", out color);
            coloresList[4] = color;
            ColorUtility.TryParseHtmlString("#23F997", out color);
            coloresList[5] = color;
            ColorUtility.TryParseHtmlString("#BAF222", out color);
            coloresList[6] = color;
        }

        private GameObject GetChildWithName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }

        public bool ExitsCell(string name)
        {
            if (GetChildWithName(gameObject, name) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
