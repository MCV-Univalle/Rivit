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
            if (inputM.pointToush > 0 && inputM.mouseEnterPointList[inputM.pointToush - 1].Count != 0 && inputM.IsPressingClick)
            {
                int index = InputMouse._instance.mouseEnterPointList[inputM.pointToush - 1].Count;
                GameObject previousCell = InputMouse._instance.mouseEnterPointList[inputM.pointToush - 1][index - 1];

                if (ValidateCellAvailability(previousCell))
                {
                    inputM.AddEnterPoint(gameObject, inputM.pointToush);
                    lineManager.AddListPositionsLine(inputM.pointToush, InputMouse._instance.mouseEnterPointList[inputM.pointToush - 1]);
                    LineManager._instance.pathLineList[inputM.pointToush - 1].Add(gameObject.transform.parent.name);
                }
            }

            if (inputM.pointToush > 0)
            {
                inputM.mouseEnterPoint = inputM.mouseEnterPointList[inputM.pointToush - 1];
            }
        }

        private void OnMouseDown()
        {
            gameObjectColor = gameObject.GetComponent<Renderer>().material.GetColor("_Color");

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

        private bool ValidateCellAvailability(GameObject previousCell)
        {
            string up, down, left, right;
            List<string> cellsAvailable = new List<string>();
            char[] myChars = previousCell.name.ToCharArray();
            string newCell = gameObject.name;
            int i = int.Parse(myChars[1].ToString());
            int j = int.Parse(myChars[3].ToString());

            up = "[" + (i - 1) + "," + j + "]";
            down = "[" + (i + 1) + "," + j + "]";
            left = "[" + i + "," + (j - 1) + "]";
            right = "[" + i + "," + (j + 1) + "]";

            if (GenerateBoard._instance.ExitsCell(up)) cellsAvailable.Add(up);
            if (GenerateBoard._instance.ExitsCell(down)) cellsAvailable.Add(down);
            if (GenerateBoard._instance.ExitsCell(left)) cellsAvailable.Add(left);
            if (GenerateBoard._instance.ExitsCell(right)) cellsAvailable.Add(right);

            //print("cell enter = " + gameObject.name);
            //foreach (string s in cellsAvailable)
            //{
            //    print(s);
            //}

            return cellsAvailable.Contains(newCell);
        }
    }
}