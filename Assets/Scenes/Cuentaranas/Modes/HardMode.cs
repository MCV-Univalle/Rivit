using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class HardMode : GameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            gameManager.Rounds = 5;
            gameManager.IterationNumber = 15;
            gameManager.WaitTime = 1.3F;
            gameManager.NormalSpeed = 0.5f;
            gameManager.SpeedVariance = 0.05f;
            gameManager.ActiveFrogsNumber = 2;
        }
        public override void IncreaseDifficulty(int round)
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            if (round > 2)
            {
                gameManager.IterationNumber++;
                gameManager.NormalSpeed += 0.025f;
                gameManager.SpeedVariance += 0.0035f;
                gameManager.WaitTime -= 0.08f;
            }
            else
            {  
                gameManager.IterationNumber += 2;
                gameManager.ActiveFrogsNumber = 3;
                gameManager.NormalSpeed += 0.035f;
                gameManager.SpeedVariance += 0.0075f;
                gameManager.WaitTime -= 0.125f;
            }
        }

    }
}
