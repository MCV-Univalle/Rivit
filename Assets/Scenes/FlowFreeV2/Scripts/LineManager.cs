using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{
    public class LineManager : MonoBehaviour
    {
        public static LineManager _instance;
        public List<GameObject> lineList;
        [SerializeField] private Material lineMaterialPrefab;
        [SerializeField] private List<Material> lineMaterialList;

        List<Vector3> pointPrueba = new List<Vector3>();
        bool complete = false;

        [SerializeField] private List<int> puntosOcupados = new List<int>();
        [SerializeField] public List<List<string>> pathLineList = new List<List<string>>();
        [SerializeField] public List<bool> flowCompleted = new List<bool>();

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            AddLineTolist();
            InstanciarPathLineList();
        }

        public void AddLineTolist()
        {
            lineList.Clear();
            for (int i = 0; i < GenerateBoard._instance.TamBoard; i++)
            {
                lineList.Add(InstanciarLinea("Linea" + i, i));
            }
        }

        public void InstanciarPathLineList()
        {
            pathLineList.Clear();
            for (int i = 0; i < GenerateBoard._instance.TamBoard; i++)
            {
                pathLineList.Add(new List<string>());
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

        public GameObject InstanciarLinea(string name, int index)
        {
            GameObject obj = new GameObject();
            obj.name = name;
            obj.transform.SetParent(gameObject.transform);
            obj.AddComponent<LineRenderer>();
            obj.AddComponent<LineCreator>();
            obj.GetComponent<LineCreator>().lineMaterial = lineMaterialList[index];

            return obj;
        }


        public void AddListPositionsLine(int index, List<GameObject> gameObjectsList)
        {
            //print(lineList[index - 1].name);
            LineRenderer lrGameObject = lineList[index - 1].GetComponent<LineRenderer>();
            LineCreator lcGameobject = lineList[index - 1].GetComponent<LineCreator>();
            lcGameobject.AddListPositionsLine(gameObjectsList, lrGameObject);
        }


        public GameObject CrearLine(GameObject padre)
        {
            GameObject obj = new GameObject();
            obj.name = "LinePrefab";
            obj.transform.SetParent(padre.transform);
            obj.AddComponent<LineRenderer>();
            obj.GetComponent<LineRenderer>().startWidth = 0.2f;
            obj.GetComponent<LineRenderer>().endWidth = 0.2f;
            obj.GetComponent<LineRenderer>().useWorldSpace = true;
            obj.GetComponent<LineRenderer>().numCapVertices = 10;
            obj.GetComponent<LineRenderer>().numCornerVertices = 10;

            return obj;
        }

        public void CountLinesComplete()
        {
            puntosOcupados.Clear();
            for (int i=0; i< lineList.Count; i++)
            {
                puntosOcupados.Add(lineList[i].transform.childCount);
            }
        }

        public bool ContainsStartEndPoints( int i,string poinIni, string pointFin)
        {
            return (pathLineList[i].Contains(poinIni) && pathLineList[i].Contains(pointFin));
        }

        public void CompletedFlowValidation()
        {
            flowCompleted.Clear();
            string poinIni;
            string pointFin;
            for (int indexLine = 0; indexLine < lineList.Count; indexLine++)
            {
                poinIni = lineList[indexLine].GetComponent<LineCreator>().posPointIni;
                pointFin = lineList[indexLine].GetComponent<LineCreator>().posPointFin;

                flowCompleted.Add(ContainsStartEndPoints(indexLine, poinIni, pointFin));
            }
        }

        public int CountFowCompleted()
        {
            int cont = 0;
            CompletedFlowValidation();
            foreach (bool element in flowCompleted)
            {
                if (element == true) cont++;
            }
            return cont;
        }



    }
}
