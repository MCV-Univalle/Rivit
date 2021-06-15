using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{

    public class Point : MonoBehaviour
    {
        public LineManager lineManager;
        public Vector3 pos;
        public int content;
        Color gameObjectColor;

        InputMouse inputM;

        private void Start()
        {
            inputM = InputMouse._instance;
            lineManager = LineManager._instance;
        }


        private void OnMouseEnter()
        {
            if (inputM.pointToush > 0 && inputM.mouseEnterPointList[inputM.pointToush -1].Count != 0 && inputM.IsPressingClick)
            {
                
                inputM.AddEnterPoint(gameObject, inputM.pointToush);
                lineManager.AddListPositionsLine(inputM.pointToush, InputMouse._instance.mouseEnterPointList[inputM.pointToush-1]);

                //print("Esta entrando: " + gameObject.transform.parent.name);
                //print(LineManager._instance.pathLineList.Count);
                LineManager._instance.pathLineList[inputM.pointToush - 1].Add(gameObject.transform.parent.name);
            }


            if (inputM.pointToush > 0)
            {
                inputM.mouseEnterPoint = inputM.mouseEnterPointList[inputM.pointToush - 1];
            }
        }

        private void OnMouseDown()
        {
            gameObjectColor =  gameObject.GetComponent<Renderer>().material.GetColor("_Color");

            string lineToName = "Color" + content;

            inputM.IsPressingClick = true;
            inputM.ActiveCircularPointer(true);
            inputM.SetColorCirularPointer(gameObjectColor);

            if (content > 0) inputM.pointToush = content;
            else inputM.pointToush = -1;

            if (inputM.pointToush > 0 && inputM.mouseEnterPointList[inputM.pointToush - 1].Count == 0)
            {
                //print("se asañadio el primero en la lista "+ inputM.pointToush);
                
                inputM.AddEnterPoint(gameObject, inputM.pointToush);

                lineManager.lineList[inputM.pointToush - 1].GetComponent<LineCreator>().ColorLine = gameObjectColor;
                lineManager.lineList[inputM.pointToush - 1].GetComponent<LineCreator>().SetColorLine();

                //print("Esta entrando: " + gameObject.transform.parent.name);
                LineManager._instance.pathLineList[content - 1].Add(gameObject.transform.parent.name);
            }

            inputM.MostarLongitudEnterLists();
        }

        private void OnMouseDrag()
        {
            inputM.MoveCirularPointer();
        }

        private void OnMouseUp()
        {
            inputM.IsPressingClick = false;
            inputM.ActiveCircularPointer(false);
            LineManager._instance.CountLinesComplete();
        }

    }

}