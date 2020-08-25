using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class HardMode : MelodyChorusGameMode
    {
        public override void SetVariables()
        {
            Limit = 12;
            _gameManager.Speed = 0.8F;
        }
        public override void IncreaseDifficulty(int score)
        {
            if (score > 15)
                _gameManager.Speed = 0.6F;
            else if (score > 3)
                _gameManager.Speed = 0.7F;
        }
        public override string IncrementMelody()
        {
            int num = _gameManager.MelodyLong;
            _gameManager.StartNewMelody(num + 1);

            if (_gameManager.MelodyLong < Limit - 1)
                return "Memoriza esta melodía";
            else
                return "¡Esta es la última nota!";
        }
    }
}