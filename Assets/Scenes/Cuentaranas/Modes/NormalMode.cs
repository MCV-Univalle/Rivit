using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class NormalMode : GameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            gameManager.Rounds = 5;
            gameManager.IterationNumber = 10;
            gameManager.WaitTime = 1.7F;
            gameManager.NormalSpeed = 0.4F;
            gameManager.SpeedVariance = 0.04F;
            gameManager.ActiveFrogsNumber = 1;
        }
        public override void IncreaseDifficulty(int round)
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            gameManager.IterationNumber++;
            if (round > 2)
            {
                gameManager.NormalSpeed += 0.02f;
                gameManager.SpeedVariance += 0.002f;
                gameManager.WaitTime -= 0.06f;
            }
            else
            {
                gameManager.ActiveFrogsNumber = 2;
                gameManager.NormalSpeed += 0.03f;
                gameManager.SpeedVariance += 0.0045f;
                gameManager.WaitTime -= 0.08f;
            }
        }

    }
}