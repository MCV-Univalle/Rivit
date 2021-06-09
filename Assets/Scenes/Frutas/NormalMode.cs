using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class NormalMode : GameMode
    {
        public override void IncreaseDifficulty(int score)
        {

        }

        public override void InitializeSettings()
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;

            temp.TimeSpawnGap = 0.5F;
            temp.SpawnRate = 4;
            temp.SelfDestructionTime = 3F;

            temp.Timer.CurrentTime = 30;
        }
    }
}