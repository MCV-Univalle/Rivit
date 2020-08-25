using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class NormalMode : GameMode
    {
        [SerializeField] FrogsCounterGameManager _gameManager;
        public override void SetVariables()
        {
            _gameManager.Rounds = 5;
            _gameManager.IterationNumber = 10;
            _gameManager.WaitTime = 1.7F;
            _gameManager.NormalSpeed = 0.4F;
            _gameManager.SpeedVariance = 0.04F;
            _gameManager.ActiveFrogsNumber = 1;
        }
        public override void IncreaseDifficulty(int round)
        {
            _gameManager.IterationNumber++;
            if (round > 2)
            {
                _gameManager.NormalSpeed += 0.02f;
                _gameManager.SpeedVariance += 0.002f;
                _gameManager.WaitTime -= 0.06f;
            }
            else
            {
                _gameManager.ActiveFrogsNumber = 2;
                _gameManager.NormalSpeed += 0.03f;
                _gameManager.SpeedVariance += 0.0045f;
                _gameManager.WaitTime -= 0.08f;
            }
        }

    }
}
