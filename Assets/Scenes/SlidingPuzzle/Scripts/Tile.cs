using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SlidingPuzzle
{
    public class Tile : MonoBehaviour
    {
        PuzzleState puzzleState;
        public int Id { get; set; }
        public bool Empty { get; set; }
        public bool Locked { get; set; }

        private void Start()
        {
            Locked = false;
            puzzleState = (PuzzleState) GameObject.FindObjectOfType(typeof(PuzzleState));    
        }

        public void SetNumberId()
        {
            var temp = this.gameObject.transform.GetChild(2).transform;
            temp.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = Id + "";
            temp.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Id + "";
        }

        public void ShowSmallNumber()
        {
            var temp = this.gameObject.transform.GetChild(2).transform;
            temp.GetChild(0).gameObject.SetActive(true);
        }

        public void ShowBigNumber()
        {
            var temp = this.gameObject.transform.GetChild(2).transform;
            temp.GetChild(1).gameObject.SetActive(true);
        }

        public void BecomeTransparent()
        {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                transform.GetChild(1).gameObject.SetActive(false);
        }

        public void ActiveIdText(bool bigNumber)
        {
            var temp = this.gameObject.transform.GetChild(2).transform;
            if (!bigNumber)
            {
                temp.GetChild(0).gameObject.SetActive(true);
                temp.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = Id + "";
            }

        }

        public void BecomeEmpty(int max)
        {
            var temp = this.gameObject.transform.GetChild(2).transform;
            if (Id == max)
            {
                Empty = true;
                BecomeTransparent();
                temp.GetChild(0).gameObject.SetActive(false);
            }
        }

        private void OnMouseDown()
        {
            if (!Empty && !Locked && !puzzleState.Locked)
            {
               int newPos = puzzleState.GetAdjacentEmptyTileID(transform.GetSiblingIndex());
                if (newPos != -1)
                {
                    this.gameObject.GetComponent<AudioSource>().Play();
                    puzzleState.ExecuteMove(transform.GetSiblingIndex(), newPos);
                }           
            }
        }
    }
}