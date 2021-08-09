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
            gameManager.WaitTime = 1.5F;
            gameManager.NormalSpeed = 0.5F;
            gameManager.SpeedVariance = 0.04F;
            gameManager.ActiveFrogsNumber = 2;
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
                gameManager.ActiveFrogsNumber = 3;
                gameManager.NormalSpeed += 0.03f;
                gameManager.SpeedVariance += 0.0045f;
                gameManager.WaitTime -= 0.08f;
            }
        }

    }
}
