using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{

    public class InputMouse : MonoBehaviour
    {

        public static InputMouse _instance;
        public List<GameObject> mouseEnterPoint;
        public List<GameObject>[] mouseEnterPointVector;
        public List<List<GameObject>> mouseEnterPointList;
        [SerializeField] private GameObject circularPointer;
        public Vector2 worldPositionMouse;
        [SerializeField] private bool isPressingClick;
        public bool IsPressingClick { get => isPressingClick; set => isPressingClick = value; }
        public int pointToush;

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
            isPressingClick = false;
            MouseEnterPointLisAdd();
        }

        public void MouseEnterPointLisAdd()
        {
            mouseEnterPointList = new List<List<GameObject>>();
            mouseEnterPoint.Clear();
            for (int i = 0; i < GenerateBoard._instance.CantLines; i++)
            {
                mouseEnterPointList.Add(new List<GameObject>());
            }
        }

        private void Update()
        {
            worldPositionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public void AddEnterPoint(GameObject gameObject, int content)
        {
            if (mouseEnterPointList[content - 1].Contains(gameObject))
            {
                int index = mouseEnterPointList[content - 1].IndexOf(gameObject);
                //print("En la lista ya se encuentra " + gameObject.name + " en la posicion " + index);

                SublistEnterpoints(content ,index);
            }
            else
            {
                mouseEnterPointList[content - 1].Add(gameObject);
            }
        }

        public void ActiveCircularPointer(bool value)
        {
            circularPointer.SetActive(value);
        }

        public void MoveCirularPointer()
        {
            circularPointer.transform.localPosition = worldPositionMouse;
        }

        public void SublistEnterpoints(int content, int index)
        {
            List<GameObject> sublist = mouseEnterPointList[content - 1].GetRange(0, index + 1);
            mouseEnterPointList[content - 1].Clear();
            mouseEnterPointList[content - 1] = sublist;
        }


        public void SetColorCirularPointer(Color color)
        {
            circularPointer.GetComponent<Renderer>().material.SetColor("_Color", color);
        }

        public void MostarLongitudEnterLists()
        {
            int cont = 0;
            foreach (var x in mouseEnterPointList)
            {
                //print(cont + " " + x.Count);
                cont++;
            }
        }

    }

}