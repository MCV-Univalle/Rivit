using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class IllimitedMode : GameMode
    {
        public override void IncreaseDifficulty(int score)
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;
            if (temp.Time < 10)
            {
                temp.MaxActiveSpawners = 4;
                temp.SelfDestructionTime = 1.5F;
                temp.MinWaitTime = 0.5F;
                temp.MaxWaitTime = 1F;
            }
            else if (temp.Time < 30)
            {
                temp.MaxActiveSpawners = 4;
                temp.SelfDestructionTime = 2F;
                temp.MinWaitTime = 0.7F;
                temp.MaxWaitTime = 1.15F;
            }
            else if (temp.Time < 45)
            {
                temp.MaxActiveSpawners = 3;
                temp.SelfDestructionTime = 2.5F;
                temp.MinWaitTime = 1F;
                temp.MaxWaitTime = 1.5F;
            }
        }

        public override void InitializeSettings()
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;

            temp.MinWaitTime = 1.2F;
            temp.MaxWaitTime = 1.7F;
            temp.SelfDestructionTime = 3F;
            temp.MaxActiveSpawners = 2;
            temp.MaxActiveFruits = 5;
            temp.Time = 60;
        }
    }
}