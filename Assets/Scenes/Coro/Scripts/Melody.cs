using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class Melody : MonoBehaviour
    {
        private List<int> _notes;
        private int _index = 0;
        private int _numberFrogs = 6;
        private NoteVerifier _noteVerifier;
        public bool IsFinished { get; set; }
        public List<int> Notes { get => _notes; }

        public delegate void NumberNotesDelegate(int num);
        public static event NumberNotesDelegate updateNotes;

        void Awake()
        {
            _notes = new List<int>();
            _noteVerifier = new NoteVerifier();
        }

        public void EmptyMelody()
        {
            _index = 0;
            _notes = new List<int>();
        }

        public void AddNote()
        {
            int num = GenerateRandomNote();
            _notes.Add(num);
            _index = 0;
            updateNotes(_notes.Count);
        }

        public void AddNotes(int num)
        {
            for (int i = 0; i < num; i++)
            {
                AddNote();
            }
        }
        public int GenerateRandomNote()
        {
            bool repeatA = true;
            bool repeatB = true;
            int num = 0;
            while ((repeatA) || (repeatB))
            {
                repeatA = false;
                repeatB = false;
                num = Random.Range(0, _numberFrogs);
                if (_notes.Count > 1)
                {
                    repeatA = _noteVerifier.VerifyLastTwoNotes(num, _notes);
                }
                if (_notes.Count > 4)
                {
                    repeatB = _noteVerifier.VerifyMonotoneNotes(num, _notes);
                }
            }
            return num;
        }

        public int CompareWithCorrectNote(int note)
        {
            int correct = -1;
            int currentNote = _notes[_index];
            if (note == currentNote)
            {
                _index++;
                if (_index == _notes.Count)
                    IsFinished = true;
            }
            else
                correct = currentNote;
            return correct;
        }
    }
}

