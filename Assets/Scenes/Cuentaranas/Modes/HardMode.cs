using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class HardMode : GameMode
    {
        [SerializeField] FrogsCounterGameManager _gameManager;
        public override void SetVariables()
        {
            _gameManager.Rounds = 5;
            _gameManager.IterationNumber = 15;
            _gameManager.WaitTime = 1.3F;
            _gameManager.NormalSpeed = 0.5f;
            _gameManager.SpeedVariance = 0.05f;
            _gameManager.ActiveFrogsNumber = 2;
        }
        public override void IncreaseDifficulty(int round)
        {
            if (round > 2)
            {
                _gameManager.IterationNumber++;
                _gameManager.NormalSpeed += 0.025f;
                _gameManager.SpeedVariance += 0.0035f;
                _gameManager.WaitTime -= 0.08f;
            }
            else
            {  
                _gameManager.IterationNumber += 2;
                _gameManager.ActiveFrogsNumber = 3;
                _gameManager.NormalSpeed += 0.035f;
                _gameManager.SpeedVariance += 0.0075f;
                _gameManager.WaitTime -= 0.125f;
            }
        }

    }
}
