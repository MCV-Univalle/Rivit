using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorsFrogCounter
{
    public class NormalMode : GameMode
    {
        [SerializeField] FrogsSpawner spawner;
        public override void IncreaseDifficulty(int rounds)
        {
            if (rounds < 3)
            {
                spawner.MaxColors = 4;
                spawner.MaxNumberOfFrogs = 9;
            }
            else
            {
                spawner.MaxColors = 3;
                spawner.MaxNumberOfFrogs++;
            }

        }

        public override void InitializeSettings()
        {
            spawner.MaxColors = 3;
            spawner.MaxNumberOfFrogs = 5;
        }

    }
}