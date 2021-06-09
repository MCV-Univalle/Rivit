using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class FruitsPickerGameManager : ModeSystemGameManager
    {
        public override string Name => "FruitsPicker";
        public float TimeSpawnGap { get; set; }
        public int SpawnRate { get; set; }
        public float SelfDestructionTime { get; set; }
        public Timer Timer { get => timer; set => timer = value; }

        private int time = 0;

        [SerializeField] private FrutalTree tree;
        [SerializeField] private Timer timer;

        private void Start()
        {
            Frog.onScore += IncrementScore;
        }
        private void OnDestroy()
        {
            Frog.onScore -= IncrementScore;
        }

        private void Update()
        {
            if(timer.CurrentTime <= 0 && timer.Started)
            {
                NotifyGameOver();
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
        }

        public override void StartGame()
        {
            tree.IsActive = true;
            timer.IsIncrementing = false;
            timer.Started = true;
        }
    }
}