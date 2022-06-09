using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SlidingPuzzle
{
    public class PuzzleState : MonoBehaviour
    {
        [SerializeField] GridGenerator gridGenerator;
        [SerializeField] GameObject flashUnderMask;
        SlidingPuzzleGameManager _gameManager;
        public bool Locked { get; set; }
        public int Size { get; set; }

        [Inject]
        public void Construct(GameManager gameManager)
        {
            this._gameManager = gameManager as SlidingPuzzleGameManager;
        }

        public void Initialize(int size)
        {
            Size = size;
            SetFramesDisplay(false);
        }

        public IEnumerator DivideAndShuffle()
        {
            yield return new WaitForSeconds(0.35F);
            StartCoroutine(HighlightTides());
            yield return new WaitForSeconds(0.6F);
            SetFramesDisplay(true);
            Shuffle();
        }

        public void ActiveIdTexts(bool standardPuzzle)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Tile>().ActiveIdText(standardPuzzle);
            }
        }

        public void MakeTransparentTheLastTile()
        {
            ActiveIdTexts(_gameManager.IsStandardPuzzle);
            int max = Size * Size;
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Tile>().BecomeEmpty(max);
            }
        }

        public int GetAdjacentEmptyTileID(int pos)
        {
            int max = Size * Size;
            if (pos + Size < max)
            {
                if (transform.GetChild(pos + Size).GetComponent<Tile>().Empty)
                    return pos + Size;
            }
            if (pos - Size >= 0)
            {
                if (transform.GetChild(pos - Size).GetComponent<Tile>().Empty)
                    return pos - Size;
            }         
            if (pos + 1 < max && ((pos + 1) % Size != 0))
            {
                if (transform.GetChild(pos + 1).GetComponent<Tile>().Empty)
                    return pos + 1;
            }
            if (pos - 1 >= 0 && (pos % Size != 0))
            {
                if (transform.GetChild(pos - 1).GetComponent<Tile>().Empty)
                    return pos - 1;
            }
            return -1;
        }

        public void ExecuteMove(int firstPos, int secondPos)
        {
            LockPuzzle();
            _gameManager.Moves++;

            var tile1 = transform.GetChild(firstPos);
            var tile2 = transform.GetChild(secondPos);
            transform.GetChild(secondPos).SetSiblingIndex(firstPos);
            tile1.SetSiblingIndex(secondPos);

            SwapTiles(tile1, tile2);

            LeanTween.delayedCall(this.gameObject, 0.2F,() => UnlockPuzzle());

            if (CheckVictoryCondition())
            {
                HandleVictory();
            }      
        }

        public void SwapTiles(Transform tile1, Transform tile2)
        {
            
            LeanTween.move(tile1.gameObject, tile2.position, 0.2f).setEase(LeanTweenType.easeOutQuad);
            tile2.position = tile1.position;
        }

        public void LockPuzzle()
        {
            Locked = true;
            GetComponent<GridLayoutGroup>().enabled = false;
        }

        public void UnlockPuzzle()
        {
            Locked = false;
            GetComponent<GridLayoutGroup>().enabled = true;
        }

        public void HandleVictory()
        {
            ShowFullImage(Size * Size);
            LeanTween.delayedCall(this.gameObject, 0.7F, () => _gameManager.HandleVictory());
            LeanTween.delayedCall(gameObject, 3.5F, () => _gameManager.NotifyGameOver());
        }

        public void Shuffle()
        {
            GetComponent<GridLayoutGroup>().enabled = true;
            foreach (Transform child in transform)
            {
                child.SetSiblingIndex(Random.Range(0, Size*Size));
            }
        }

        public bool CheckVictoryCondition()
        {
            int num = 0;
            int max = Size * Size;
            foreach (Transform child in transform)
            {
                if (num == max - 1)
                    return true;
                if (num + 1 == child.GetComponent<Tile>().Id)
                    num++;
                else break;
            }
            return false;
        }

        public void ShowFullImage(int max)
        {
            LeanTween.color(transform.GetChild(max - 1).gameObject, new Color(1, 1, 1, 1), 0.5f).setDelay(0.3f);
            StartCoroutine(HighlightTides());
            SetFramesDisplay(false);
        }

        public IEnumerator HighlightTides()
        {
            LeanTween.color(flashUnderMask.gameObject, Color.white, 0.3f).setDelay(0.1f);
            yield return new WaitForSeconds(0.6F);
            LeanTween.color(flashUnderMask.gameObject, new Color(0, 0, 0, 0), 0.3f).setDelay(0.1f);
        }

        public void SetFramesDisplay(bool value)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Tile>().Locked = !value;
                if(child.gameObject.GetComponent<Tile>().Id != Size * Size)
                    child.gameObject.GetComponent<Tile>().ShowSmallNumber(value);
                child.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = value;
                
            }
        }
    }
}