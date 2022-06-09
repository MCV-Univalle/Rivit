using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorsFrogCounter
{
    public class HardMode : GameMode
    {
        [SerializeField] FrogsSpawner spawner;
        public override void IncreaseDifficulty(int rounds)
        {
            if (rounds < 4)
            {
                spawner.MaxColors = 5;
                spawner.MaxNumberOfFrogs = 8;
            }
            else
            {
                spawner.MaxNumberOfFrogs++;
                spawner.MaxColors = 4;
            }
        }

        public override void InitializeSettings()
        {
            spawner.MaxColors = 4;
            spawner.MaxNumberOfFrogs = 6;
        }

    }
}