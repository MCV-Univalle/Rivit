using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class EasyMode : GameMode
    {
        [SerializeField] FrogsCounterGameManager _gameManager;
        public override void SetVariables()
        {
            _gameManager.Rounds = 5;
            _gameManager.IterationNumber = 7;
            _gameManager.WaitTime = 2.03f;
            _gameManager.NormalSpeed = 0.35f;
            _gameManager.SpeedVariance = 0.04f;
            _gameManager.ActiveFrogsNumber = 1;
        }
        public override void IncreaseDifficulty(int round)
        {
            if (round > 2)
            {
                _gameManager.NormalSpeed += 0.01f;
                _gameManager.SpeedVariance += 0.001f;
                _gameManager.WaitTime -= 0.02f;
            }
            else
            {
                _gameManager.IterationNumber++;
                _gameManager.ActiveFrogsNumber = 2;
                _gameManager.NormalSpeed += 0.02f;
                _gameManager.SpeedVariance += 0.003f;
                _gameManager.WaitTime -= 0.05f;
            }
        }

    }
}
