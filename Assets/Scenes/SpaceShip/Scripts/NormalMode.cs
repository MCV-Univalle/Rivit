using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class NormalMode : GameMode
    {
        private int nextTargetScore;
        public override void IncreaseDifficulty(int score)
        {
            var manager = _gameManager as SpaceShipGameManager;
            if (score > nextTargetScore)
            {
                if (nextTargetScore < 30)
                    nextTargetScore += 10;
                else if (nextTargetScore < 70)
                    nextTargetScore += 20;
                else if (nextTargetScore < 130)
                    nextTargetScore += 30;
                else if (nextTargetScore < 230)
                    nextTargetScore += 50;
                else if (nextTargetScore < 1000)
                    nextTargetScore += 1000;

                manager.AsteroidSpeed -= 0.25F;
                manager.MinSpawnSpeed -= 0.05F;
                manager.MaxSpawnSpeed -= 0.075F;
                manager.EnergyGaugeDecreaseRate += 0.01F;
            }
        }

        public override void InitializeSettings()
        {
            nextTargetScore = 10;
            var manager = _gameManager as SpaceShipGameManager;
            manager.AsteroidSpeed = 4.5F;
            manager.MinSpawnSpeed = 1.2F;
            manager.MaxSpawnSpeed = 1.9F;
            manager.EnergyGaugeDecreaseRate = 0.04F;
        }
    }
}