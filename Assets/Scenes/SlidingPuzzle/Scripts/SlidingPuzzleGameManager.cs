using Newtonsoft.Json;
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
        [SerializeField] Timer timer;
        [SerializeField] GameObject imageSelectScreen;
        [SerializeField] GameObject inGamePanel;
        [SerializeField] PuzzleBackground puzzleBackground;

        private int _size;

        private SliddingPuzzleAdditionalData additionalData;

        public bool IsStandardPuzzle { get; set; }

        public int Moves { get; set; }
        
        public override string Name => "SliddingPuzzle";

        public int Size { get => _size; set => _size = value; }

        public override void NotifyGameOver()
        {
            double movesCalification = Math.Log10(Moves);
            double timeCalification = Math.Log10(timer.CurrentTime / 10);
            Score = ((int)((int)1000 * (1 / (movesCalification + timeCalification))));
            base.NotifyGameOver();
        }

        public override void EndGame()
        {
            DestroyTides();
            timer.Started = false;
            puzzleBackground.gameObject.SetActive(false);
        }

        public override string RegisterAdditionalData()
        {
            additionalData.Moves = this.Moves;
            additionalData.TotalTime = timer.CurrentTime;
            return JsonConvert.SerializeObject(additionalData);
        }

        public override void StartGame()
        {
            additionalData = new SliddingPuzzleAdditionalData();
            timer.IsIncrementing = true;
            DestroyTides();
            Moves = 0;
            inGamePanel.SetActive(false);
            imageSelectScreen.GetComponent<CanvasGroup>().alpha = 1;
            imageSelectScreen.SetActive(true);
            _size = (_gameMode as SlidingPuzzleGameMode).Size;
            imageSelectScreen.transform.GetChild(0).GetComponent<ImageSelectionScreen>().GenerateButtons();
            gridGenerator.AdjustGridScale(_size);
        }

        public void SelectImage(Texture2D image)
        {
            inGamePanel.SetActive(true);
            timer.CurrentTime = 0;
            timer.Started = true;
            puzzleBackground.ClearBackground();
            puzzleBackground.gameObject.transform.localScale = gridGenerator.transform.localScale * _size;
            GenerateBoard(image, _size);
            puzzleBackground.DarkenBackground();
            LeanTween.delayedCall(this.gameObject, 0.9F, () => puzzleState.MakeTransparentTheLastTile());
            StartCoroutine(puzzleState.DivideAndShuffle());

        }

        public void GenerateBoard(Texture2D image, int size)
        {
            gridGenerator.GenerateGrid(size, image);
            puzzleState.Initialize(size);
        }

        internal void HandleVictory()
        {
            puzzleBackground.ClearBackground();
            LeanTween.delayedCall(this.gameObject, 0.25F, () => puzzleBackground.DarkenBackground());
        }

        public void GenerateStandardPuzzle()
        {
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