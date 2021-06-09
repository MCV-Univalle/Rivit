using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FlowFreeV2
{
    public class LineCreator : MonoBehaviour
    {
        private const float widthLine = 0.4f;
        private LineRenderer lineRenderer;
        [SerializeField] private int indexBreak;
        public Material lineMaterial;
        [SerializeField] private Color colorLine;
        public int numeroLinea;


        public List<Vector3> points = new List<Vector3>();
        public List<Vector3> sublistPoints;

        public int IndexBreak { get => indexBreak; set => indexBreak = value; }
        public Color ColorLine { get => colorLine; set => colorLine = value; }

        public string posPointIni, posPointFin;

        [System.Obsolete]
        private void Start()
        {
            lineRenderer =  GetComponent<LineRenderer>();
            lineRenderer.SetVertexCount(10);
            lineRenderer.startWidth = widthLine;
            lineRenderer.endWidth = widthLine;
        }

        public GameObject CrearLine(GameObject padre)
        {
            GameObject obj = new GameObject();
            obj.name = "Prueba";
            obj.transform.SetParent(padre.transform);
            obj.AddComponent<LineRenderer>();
            obj.GetComponent<LineRenderer>().startWidth = 0.1f;
            obj.GetComponent<LineRenderer>().endWidth = 0.1f;
            obj.GetComponent<LineRenderer>().useWorldSpace = true;
            obj.GetComponent<LineRenderer>().numCapVertices = 10;
            obj.GetComponent<LineRenderer>().numCornerVertices = 10;

            return obj;
        }

        public void ClearChildren()
        {
            //Debug.Log("Children count ini = "+transform.childCount);
            int i = 0;

            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[transform.childCount];

            //Find all child obj and store to that array
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }

            //Debug.Log("Children count fin = " + transform.childCount);
        }

        public void CreateLines(List<Vector3> list)
        {
            ClearChildren();
            Vector3 inicio, fin;
            for (int i = 0; i < list.Count - 1; i++)
            {
                inicio = list[i];
                fin = list[i + 1];

                GameObject obj = CrearLine(gameObject);

                obj.name = "LineChild" + i;
                obj.GetComponent<LineRenderer>().SetPosition(0, inicio);
                obj.GetComponent<LineRenderer>().SetPosition(1, fin);
                obj.GetComponent<LineRenderer>().material.SetColor("_Color", colorLine);
                obj.GetComponent<LineRenderer>().material.shader = Shader.Find("Sprites/Default");
            }
        }

        public void AddListPositionsLine(List<GameObject> gameObjectsList, LineRenderer lr)
        {
            lineRenderer = lr;
            points.Clear();
            int cont = 0;
            foreach (GameObject element in gameObjectsList)
            {
                points.Add(element.transform.position);
                cont++;
            }
            CreateLines(points);
        }

        public void SetColorLine()
        {
            lineMaterial.SetColor("_Color", colorLine);
            lineRenderer.material = lineMaterial;
        }
    }
}
