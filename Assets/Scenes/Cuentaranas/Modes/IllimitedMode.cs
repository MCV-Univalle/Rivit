using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class IllimitedMode : GameMode
    {
        [SerializeField] FrogsCounterGameManager _gameManager;
        [SerializeField] LifeDisplayer lifeDisplayer;
 
        private int _difficultyLevel;

 
        public override void SetVariables()
        {
            _gameManager.Rounds = 100000;
            _gameManager.IterationNumber = 8;
            _gameManager.WaitTime = 2.03f;
            _gameManager.NormalSpeed = 0.35f;
            _gameManager.SpeedVariance = 0.04f;
            _gameManager.ActiveFrogsNumber = 1;

            lifeDisplayer.Lifes = 3;

            _difficultyLevel = 0;
            lifeDisplayer.ActiveHearths();
        }
        public override void IncreaseDifficulty(int round)
        {
            if (_difficultyLevel < 5)
                _difficultyLevel++;
            _gameManager.IterationNumber++;
            switch (_difficultyLevel)
            {
                case 0:
                    _gameManager.IterationNumber = 8;
                    _gameManager.WaitTime = 1.9f;
                    _gameManager.NormalSpeed = 0.4f;
                    _gameManager.SpeedVariance = 0.04f;
                    _gameManager.ActiveFrogsNumber = 1;
                    break;
                case 1:
                    _gameManager.IterationNumber = 10;
                    _gameManager.WaitTime = 1.7F;
                    _gameManager.NormalSpeed = 0.45f;
                    _gameManager.SpeedVariance = 0.045f;
                    _gameManager.ActiveFrogsNumber = 1;
                    break;
                case 2:
                    _gameManager.IterationNumber = 12;
                    _gameManager.WaitTime = 1.5f;
                    _gameManager.NormalSpeed = 0.5f;
                    _gameManager.SpeedVariance = 0.05f;
                    _gameManager.ActiveFrogsNumber = 2;
                    break;
                case 3:
                    _gameManager.IterationNumber = 14;
                    _gameManager.WaitTime = 1.3f;
                    _gameManager.NormalSpeed = 0.6f;
                    _gameManager.SpeedVariance = 0.055f;
                    _gameManager.ActiveFrogsNumber = 2;
                    break;
                case 4:
                    _gameManager.IterationNumber = 18;
                    _gameManager.WaitTime = 1.1f;
                    _gameManager.NormalSpeed = 0.65f;
                    _gameManager.SpeedVariance = 0.06f;
                    _gameManager.ActiveFrogsNumber = 3;
                    break;
                case 5:
                    _gameManager.IterationNumber = 20;
                    _gameManager.WaitTime = 0.8f;
                    _gameManager.NormalSpeed = 0.7f;
                    _gameManager.SpeedVariance = 0.065f;
                    _gameManager.ActiveFrogsNumber = 3;
                    break;
            }
        }

        public void ReduceLife()
        {
            lifeDisplayer.ReduceLife();
            if (lifeDisplayer.Lifes == 0)
            {
                _gameManager.Rounds = 0;
            }
            if (_difficultyLevel > 0)
            {
                _difficultyLevel--;
                IncreaseDifficulty(0);
            }
                
        }

    }
}
