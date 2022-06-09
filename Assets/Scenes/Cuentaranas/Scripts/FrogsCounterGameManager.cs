using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Cuentaranas
{
    public class FrogsCounterGameManager : ModeSystemGameManager, IQuestionable
    {
        public override string Name { get => "CuentaRanas"; }
        [SerializeField] private Countdown countdown;
        [SerializeField] private Bush bush;
        [SerializeField] private FrogsManager frogsManager;
        [SerializeField] private QuestionPanel questionPanel;
        public int Rounds { get; set; }
        public float WaitTime { get; set; }
        public int IterationNumber { get; set; }
        public float NormalSpeed { get; set; }
        public float SpeedVariance { get; set; }
        public int ActiveFrogsNumber { set => frogsManager.ActiveFrogsNumber = value; }

        public override void StartGame()
        {
            StartCountdown();
        }

        public override void IncreaseDifficulty()
        {
            _gameMode.IncreaseDifficulty(Rounds);
        }

        public void StartCountdown()
        {
            bush.ToOriginalPosition();
            frogsManager.ToRandomPosition();
            if (Rounds > 0)
            {
                frogsManager.ToRandomPosition();
                StartCoroutine(countdown.StartCountdown(3, 0.25F, () => StartJumping()));
            }
            else
                LeanTween.delayedCall(gameObject, 1F, () => NotifyGameOver());
        }

        public void StartJumping()
        {
            bush.Fall();
            StartCoroutine(frogsManager.StartJumping(IterationNumber, WaitTime, NormalSpeed, SpeedVariance));
            Rounds--;
        }

        public int CompareUserInput(int input)
        {
            return frogsManager.CompareUserInput(input);
        }

        public void ShowQuestionPanel()
        {
            questionPanel.gameObject.SetActive(true);
        }

        public override void EndGame()
        {
            RaiseEndGameEvent();
            StopAllCoroutines();
        }

        public void ReduceLife()
        {
            if (_gameMode.GetType() == typeof(IllimitedMode))
                (_gameMode as IllimitedMode).ReduceLife();
        }
    }
}
