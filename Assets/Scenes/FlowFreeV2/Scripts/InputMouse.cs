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
            mouseEnterPointList[content-1].Add(gameObject);
        }

        public void ActiveCircularPointer(bool value)
        {
            circularPointer.SetActive(value);
        }

        public void MoveCirularPointer()
        {
            circularPointer.transform.localPosition = worldPositionMouse;
        }

        public void SublistEnterpoints(int index)
        {
            List<GameObject> sublist =  mouseEnterPoint.GetRange(0, index + 1);
            mouseEnterPoint.Clear();
            mouseEnterPoint = sublist;
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