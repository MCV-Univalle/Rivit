using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class IllimitedMode : MelodyChorusGameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            gameManager.FreeMode = false;

            Limit = 100000;
            notesDisplayer.Limit = 0;
            gameManager.Speed = 0.8F;
        }
        public override void IncreaseDifficulty(int score)
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            if (score > 100)
                gameManager.Speed = 0.5F;
            else if (score > 40)
                gameManager.Speed = 0.6F;
            else if (score > 15)
                gameManager.Speed = 0.7F;
            else if (score > 3)
                gameManager.Speed = 0.75F;
        }
        public override string IncrementMelody()
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            gameManager.AddNote();

            return "Memoriza esta melodía";
        }
    }
}