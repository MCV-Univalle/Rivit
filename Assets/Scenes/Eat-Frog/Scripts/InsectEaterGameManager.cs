using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eat_frog_Game
{
    public class InsectEaterGameManager : ModeSystemGameManager
    {

        public bool Active { get; set; }
        public bool limitreached, paused;
        public bool GameOver { get; set; }
        private FrogController frog;

        public override string Name => "InsectEater";

        void Awake()
        {
            frog = FindObjectOfType<FrogController>();
        }


        public override void StartGame()
        {
            Active = true;
            frog.Velocity = 0.1F;
            frog.curhealth = 100;
        }

        public override void EndGame()
        {
        }
    }
}