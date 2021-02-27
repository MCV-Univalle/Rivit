using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlidingPuzzle
{
    public class SlidingPuzzleGameMode : GameMode
    {
        [SerializeField] int size;

        public int Size { get => size; set => size = value; }

        public override void IncreaseDifficulty(int score)
        {
            //Empty
        }

        public override void InitializeSettings()
        {
            //Empty
        }
    }
}