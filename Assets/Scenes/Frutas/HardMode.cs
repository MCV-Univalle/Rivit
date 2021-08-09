using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class HardMode : GameMode
    {
        public override void IncreaseDifficulty(int score)
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;
            if (temp.Time < 20)
            {
                temp.MaxActiveSpawners = 4;
                temp.SelfDestructionTime = 1.2F;
                temp.MinWaitTime = 0.25F;
                temp.MaxWaitTime = 0.65F;
            }
            else if (temp.Time < 50)
            {
                temp.MaxActiveSpawners = 4;
                temp.SelfDestructionTime = 1.6F;
                temp.MinWaitTime = 0.5F;
                temp.MaxWaitTime = 1.1F;
            }
            else if (temp.Time < 75)
            {
                temp.MaxActiveSpawners = 3;
                temp.SelfDestructionTime = 2F;
                temp.MinWaitTime = 0.75F;
                temp.MaxWaitTime = 1.25F;
            }
        }

        public override void InitializeSettings()
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;

            temp.MinWaitTime = 1F;
            temp.MaxWaitTime = 1.5F;
            temp.SelfDestructionTime = 2.5F;
            temp.MaxActiveSpawners = 2;
            temp.MaxActiveFruits = 7;
            temp.Time = 90;
        }
    }
}