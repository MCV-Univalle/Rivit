using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public abstract class MelodyChorusGameMode : GameMode
    {
        public abstract string IncrementMelody();
        public int Limit {get; set;}
    }
}