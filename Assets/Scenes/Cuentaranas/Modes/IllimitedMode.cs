using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class IllimitedMode : GameMode
    {
        [SerializeField] LifeDisplayer lifeDisplayer;
 
        private int _difficultyLevel;

 
        public override void InitializeSettings()
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            gameManager.Rounds = 100000;
            gameManager.IterationNumber = 8;
            gameManager.WaitTime = 2.03f;
            gameManager.NormalSpeed = 0.35f;
            gameManager.SpeedVariance = 0.04f;
            gameManager.ActiveFrogsNumber = 1;

            lifeDisplayer.Lifes = 3;

            _difficultyLevel = 0;
            lifeDisplayer.ActiveHearths();
        }
        public override void IncreaseDifficulty(int round)
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            if (_difficultyLevel < 5)
                _difficultyLevel++;
            gameManager.IterationNumber++;
            GameParameters parameters = new GameParameters(0, 0, 0, 0, 0);
            switch (_difficultyLevel)
            {
                case 0:
                    parameters = new GameParameters(8, 1.7F, 0.4F, 0.045F, 1);
                    break;
                case 1:
                    parameters = new GameParameters(10, 1.5F, 0.5F, 0.045F, 1);
                    break;
                case 2:
                    parameters = new GameParameters(14, 1.4F, 0.55F, 0.05F, 2);
                    break;
                case 3:
                    parameters = new GameParameters(16, 1.2F, 0.61F, 0.055F, 2);
                    break;
                case 4:
                    parameters = new GameParameters(20, 1F, 0.67F, 0.06F, 3);
                    break;
                case 5:
                    parameters = new GameParameters(22, 0.8F, 0.72F, 0.064F, 3);
                    break;
            }
            SetParameters(parameters);
        }

        private void SetParameters(GameParameters parameters)
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            gameManager.IterationNumber = parameters.iterationNumber;
            gameManager.WaitTime = parameters.waitTime;
            gameManager.NormalSpeed = parameters.normalSpeed;
            gameManager.SpeedVariance = parameters.speedVariance;
            gameManager.ActiveFrogsNumber = parameters.activeFrogs;
        }

        public void ReduceLife()
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            lifeDisplayer.ReduceLife();
            if (lifeDisplayer.Lifes == 0)
            {
                gameManager.Rounds = 0;
            }
            if (_difficultyLevel > 0)
            {
                _difficultyLevel--;
                IncreaseDifficulty(0);
            }
                
        }

    }

    struct GameParameters
    {
        public int iterationNumber;
        public float waitTime;
        public float normalSpeed;
        public float speedVariance;
        public int activeFrogs;

        public GameParameters(int iterationNumber, float waitTime, float normalSpeed, float speedVariance, int activeFrogs)
        {
            this.iterationNumber = iterationNumber;
            this.waitTime = waitTime;
            this.normalSpeed = normalSpeed;
            this.speedVariance = speedVariance;
            this.activeFrogs = activeFrogs;
        }
    }
}
