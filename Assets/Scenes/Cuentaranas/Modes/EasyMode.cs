using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class EasyMode : GameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            gameManager.Rounds = 5;
            gameManager.IterationNumber = 7;
            gameManager.WaitTime = 2.03f;
            gameManager.NormalSpeed = 0.35f;
            gameManager.SpeedVariance = 0.04f;
            gameManager.ActiveFrogsNumber = 1;
        }
        public override void IncreaseDifficulty(int round)
        {
            var gameManager = (FrogsCounterGameManager)_gameManager;
            if (round > 2)
            {
                gameManager.NormalSpeed += 0.01f;
                gameManager.SpeedVariance += 0.001f;
                gameManager.WaitTime -= 0.02f;
            }
            else
            {
                gameManager.IterationNumber++;
                gameManager.ActiveFrogsNumber = 2;
                gameManager.NormalSpeed += 0.02f;
                gameManager.SpeedVariance += 0.003f;
                gameManager.WaitTime -= 0.05f;
            }
        }

    }
}
