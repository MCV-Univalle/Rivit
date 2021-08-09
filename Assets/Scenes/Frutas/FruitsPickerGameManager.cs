using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class FruitsPickerGameManager : ModeSystemGameManager
    {
        public override string Name => "FruitsPicker";
        public float MinWaitTime { get; set; }
        public float MaxWaitTime { get; set; }

        public float SelfDestructionTime { get; set; }

        public int MaxActiveSpawners { get; set; }
        public int MaxActiveFruits { get; set; }

        public float Time { get => timer.CurrentTime; set => timer.CurrentTime = value; }


        [SerializeField] private FrutalTree tree;
        [SerializeField] private Timer timer;
        [SerializeField] private GameObject timeOverNotification;

        public float TotalTime { get; set; }
        public FruitsPickerAdditionalData AdditionalData { get => additionalData; set => additionalData = value; }

        private FruitsPickerAdditionalData additionalData;

        private void Start()
        {
            Frog.onScore += IncrementScore;
        }
        private void OnDestroy()
        {
            Frog.onScore -= IncrementScore;
        }

        private void NotifyTimeIsOver()
        {
            RaiseEndGameEvent();
            timeOverNotification.SetActive(true);
            LeanTween.scale(timeOverNotification, new Vector3(1F, 1F, 1F), 0.25F).setFrom(new Vector3(0, 0, 0));
        }

        private void Update()
        {
            if(timer.CurrentTime <= 0 && timer.Started)
            {
                timer.Started = false;
                NotifyTimeIsOver();
                LeanTween.delayedCall(this.gameObject, 2F, () => NotifyGameOver());
            }
        }

        private void IncrementScore(int num)
        {
            Score += num;
        }

        public override void EndGame()
        {
            tree.IsActive = false;
            timer.Started = false;
            additionalData.AverageTime = TotalTime / (additionalData.RigthAnswers + additionalData.WrongAnswers);
        }

        public override void StartGame()
        {
            timeOverNotification.SetActive(false);
            tree.IsActive = true;
            timer.IsIncrementing = false;
            timer.Started = true;

            additionalData = new FruitsPickerAdditionalData();
            TotalTime = 0;

            tree.ActiveSpawners(MaxActiveFruits);
        }
        public override string RegisterAdditionalData()
        {
            return JsonConvert.SerializeObject(additionalData);
        }
    }
}