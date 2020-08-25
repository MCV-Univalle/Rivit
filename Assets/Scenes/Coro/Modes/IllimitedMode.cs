using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class IllimitedMode : MelodyChorusGameMode
    {
        public override void SetVariables()
        {
            Limit = 10000;
            _gameManager.Speed = 0.8F;
        }
        public override void IncreaseDifficulty(int score)
        {
            if (score > 100)
                _gameManager.Speed = 0.5F;
            else if (score > 40)
                _gameManager.Speed = 0.6F;
            else if (score > 15)
                _gameManager.Speed = 0.7F;
            else if (score > 3)
                _gameManager.Speed = 0.75F;
        }
        public override string IncrementMelody()
        {
            _gameManager.AddNote();

            return "Memoriza esta melodía";
        }
    }
}