using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class HardMode : MelodyChorusGameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            Limit = 12;
            gameManager.Speed = 0.8F;
        }
        public override void IncreaseDifficulty(int score)
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            if (score > 15)
                gameManager.Speed = 0.6F;
            else if (score > 3)
                gameManager.Speed = 0.7F;
        }
        public override string IncrementMelody()
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            int num = gameManager.MelodyLong;
            gameManager.StartNewMelody(num + 1);

            if (gameManager.MelodyLong < Limit - 1)
                return "Memoriza esta melodía";
            else
                return "¡Esta es la última nota!";
        }
    }
}