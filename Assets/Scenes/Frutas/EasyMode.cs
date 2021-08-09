using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class EasyMode : GameMode
    {
        public override void IncreaseDifficulty(int score)
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;
            if (temp.Time < 10)
            {
                temp.MaxActiveSpawners = 4;
                temp.SelfDestructionTime = 2F;
                temp.MinWaitTime = 0.75F;
                temp.MaxWaitTime = 1.25F;
            }
            else if (temp.Time < 30)
            {
                temp.MaxActiveSpawners = 4;
                temp.SelfDestructionTime = 2.5F;
                temp.MinWaitTime = 1F;
                temp.MaxWaitTime = 1.45F;
            }
            else if (temp.Time < 45)
            {
                temp.MaxActiveSpawners = 3;
                temp.SelfDestructionTime = 3F;
                temp.MinWaitTime = 1.25F;
                temp.MaxWaitTime = 1.75F;
            }
        }

        public override void InitializeSettings()
        {
            FruitsPickerGameManager temp = _gameManager as FruitsPickerGameManager;

            temp.MinWaitTime = 1.5F;
            temp.MaxWaitTime = 2F;
            temp.SelfDestructionTime = 3.5F;
            temp.MaxActiveSpawners = 2;
            temp.MaxActiveFruits = 3;
            temp.Time = 60;
        }
    }
}