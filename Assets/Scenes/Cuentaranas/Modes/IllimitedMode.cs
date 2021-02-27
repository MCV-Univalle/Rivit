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
            switch (_difficultyLevel)
            {
                case 0:
                    gameManager.IterationNumber = 8;
                    gameManager.WaitTime = 1.7f;
                    gameManager.NormalSpeed = 0.4f;
                    gameManager.SpeedVariance = 0.045f;
                    gameManager.ActiveFrogsNumber = 1;
                    break;
                case 1:
                    gameManager.IterationNumber = 10;
                    gameManager.WaitTime = 1.5F;
                    gameManager.NormalSpeed = 0.5f;
                    gameManager.SpeedVariance = 0.045f;
                    gameManager.ActiveFrogsNumber = 1;
                    break;
                case 2:
                    gameManager.IterationNumber = 14;
                    gameManager.WaitTime = 1.4f;
                    gameManager.NormalSpeed = 0.55f;
                    gameManager.SpeedVariance = 0.05f;
                    gameManager.ActiveFrogsNumber = 2;
                    break;
                case 3:
                    gameManager.IterationNumber = 16;
                    gameManager.WaitTime = 1.2f;
                    gameManager.NormalSpeed = 0.61f;
                    gameManager.SpeedVariance = 0.055f;
                    gameManager.ActiveFrogsNumber = 2;
                    break;
                case 4:
                    gameManager.IterationNumber = 20;
                    gameManager.WaitTime = 1f;
                    gameManager.NormalSpeed = 0.67f;
                    gameManager.SpeedVariance = 0.06f;
                    gameManager.ActiveFrogsNumber = 3;
                    break;
                case 5:
                    gameManager.IterationNumber = 22;
                    gameManager.WaitTime = 0.8f;
                    gameManager.NormalSpeed = 0.72f;
                    gameManager.SpeedVariance = 0.065f;
                    gameManager.ActiveFrogsNumber = 3;
                    break;
            }
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
}
