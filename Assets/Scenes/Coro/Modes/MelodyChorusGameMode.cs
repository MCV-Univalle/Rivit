using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public abstract class MelodyChorusGameMode : GameMode
    {
        [SerializeField] protected NumberNotesDisplayer notesDisplayer;

        public abstract string IncrementMelody();
        public int Limit {get; set;}
    }
}