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

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            //paintCell = false;

            gameManager = GameObject.Find("GameManager").GetComponent<FlowFreeGameManager>();
        }


        private void OnMouseDown()
        {
            //print("OnMouseDown ");
            if (gameManager.CellsInputPathGroup[containedValueCell].Count > 0 && containedValueCell != 0 && gameManager.PaintCell == false)
            {
                gameManager.BreakCellImputPath(row, col, containedValueCell);
            }
            gameManager.ColorPaint = spriteRenderer.color;
            gameManager.PaintCell = true;
            gameManager.contenCellMouseDown = containedValueCell;

        }


        private void OnMouseDrag()
        {
            //print("OnMouseDrag ");
            gameManager.ColorPaint = spriteRenderer.color;
            gameManager.PaintCell = true;

        }

        private void OnMouseEnter()
        {
            //print("OnMpuseEnter()");
            if (gameManager.PaintCell && containedValueCell == 0 )
            {
                spriteRenderer.color = gameManager.ColorPaint;
                containedValueCell = gameManager.contenCellMouseDown;
                gameManager.CellsInputPathGroup[containedValueCell].Add(gameObject.name);

                //print("OnMouseEnter " + gameObject.name + " " + gameObject.GetComponent<Cell>().containedValueCell);
            }
        }

        private void OnMouseUp()
        {
            //print("OnMouseUp ");
            gameManager.PaintCell = false;
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

