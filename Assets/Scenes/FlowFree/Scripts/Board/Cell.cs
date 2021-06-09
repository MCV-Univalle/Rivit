using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FlowFree
{
    public class Cell : MonoBehaviour
    {
        public int containedValueCell, row, col;
        private SpriteRenderer spriteRenderer = new SpriteRenderer();
        private Color colorDrag = new Color();
        private FlowFreeGameManager gameManager = new FlowFreeGameManager();
        private TableroLayout tableroLayout = new TableroLayout();
        private GenerateLevel generateLevel = new GenerateLevel();

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            //paintCell = false;

            generateLevel = GameObject.Find("GenerateLevel").GetComponent<GenerateLevel>();
        }


        private void OnMouseDown()
        {
            //print("OnMouseDown ");
            if (generateLevel.CellsInputPathGroup[containedValueCell].Count > 0 && containedValueCell != 0 && generateLevel.PaintCell == false)
            {
                generateLevel.BreakCellImputPath(row, col, containedValueCell);
            }
            generateLevel.ColorPaint = spriteRenderer.color;
            generateLevel.PaintCell = true;
            generateLevel.contenCellMouseDown = containedValueCell;

        }


        private void OnMouseDrag()
        {
            //print("OnMouseDrag ");
            generateLevel.ColorPaint = spriteRenderer.color;
            generateLevel.PaintCell = true;

        }

        private void OnMouseEnter()
        {
            //print("OnMpuseEnter()");
            if (generateLevel.PaintCell && containedValueCell == 0 )
            {
                spriteRenderer.color = generateLevel.ColorPaint;
                containedValueCell = generateLevel.contenCellMouseDown;
                generateLevel.CellsInputPathGroup[containedValueCell].Add(gameObject.name);

                //print("OnMouseEnter " + gameObject.name + " " + gameObject.GetComponent<Cell>().containedValueCell);
            }
        }

        private void OnMouseUp()
        {
            //print("OnMouseUp ");
            generateLevel.PaintCell = false;
        }

        private void OnMouseOver()
        {
            //gameManager.PaintCell = false;
        }

        private void OnMouseExit()
        {
            
        }
    }
}

