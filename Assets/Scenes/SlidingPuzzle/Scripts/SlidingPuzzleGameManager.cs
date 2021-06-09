using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlidingPuzzle
{
    public class SlidingPuzzleGameManager : ModeSystemGameManager
    {
        [SerializeField] PuzzleState puzzleState;
        [SerializeField] GridGenerator gridGenerator;
        [SerializeField] ImageSelector imageSelector;
        [SerializeField] Timer timer;
        [SerializeField] GameObject imageSelectScreen;
        [SerializeField] GameObject inGamePanel;
        [SerializeField] PuzzleBackground puzzleBackground;

        private int _size;

        public bool IsStandardPuzzle { get; set; }

        public int Moves { get; set; }
        

        public override string Name => "SliddingPuzzle";

        public override void NotifyGameOver()
        {
            Score = (int) timer.CurrentTime * -1 + 100000;
            base.NotifyGameOver();
        }

        public override void EndGame()
        {
            DestroyTides();
            timer.Started = false;
            puzzleBackground.gameObject.SetActive(false);
        }

        public override void StartGame()
        {
            DestroyTides();
            Moves = 0;
            inGamePanel.SetActive(false);
            imageSelectScreen.SetActive(true);
            _size = (_gameMode as SlidingPuzzleGameMode).Size;
            gridGenerator.AdjustGridScale(_size);
            imageSelector.Initialize();
            puzzleBackground.ClearBackground();
            puzzleBackground.gameObject.transform.localScale = gridGenerator.transform.localScale * _size;
        }

        public void SelectImage()
        {
            inGamePanel.SetActive(true);
            imageSelectScreen.SetActive(false);
            timer.CurrentTime = 0;
            timer.Started = true;
            if(!IsStandardPuzzle)
                GenerateBoard(_size);
            puzzleBackground.DarkenBackground();
            LeanTween.delayedCall(this.gameObject, 0.7F, () => puzzleState.MakeTransparentTheLastTile());
            StartCoroutine(puzzleState.DivideAndShuffle());

        }

        public void GenerateBoard(int size)
        {
            var image = imageSelector.GetCurrentImage();
            gridGenerator.GenerateGrid(size, image);
            puzzleState.Initialize(size);
        }

        internal void HandleVictory()
        {
            puzzleBackground.ClearBackground();
        }

        public void GenerateStandardPuzzle()
        {
            var image = imageSelector.GetCurrentImage();
            gridGenerator.GenerateGrid(_size, image);
            puzzleState.Initialize(_size);
            puzzleState.ActiveIdTexts(true);
        }

        public void DestroyTides()
        {
            foreach (Transform element in gridGenerator.transform)
            {
                Destroy(element.gameObject);
            }
        }
    }
}