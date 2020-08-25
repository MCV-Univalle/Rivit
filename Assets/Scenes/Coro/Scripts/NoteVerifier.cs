using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class NoteVerifier
    {
        public bool VerifyLastTwoNotes(int num, List<int> notes)
        {
            //It prevents the same note beign repeated 3 times or more
            int melodyLength = notes.Count - 1;
            bool repeat = false;

            if ((num == notes[melodyLength]) && (num == notes[melodyLength - 1]))
            {
                repeat = true;
            }
            return repeat;
        }

        public bool VerifyMonotoneNotes(int num, List<int> notes) 
        {
            //Verify if there are 2 especific notes that are repeated frequently
            
            int melodyLength = notes.Count - 1;
            bool repeat = false;
            int noteA, noteB = 0;
            int contA = 0, contB = 0;

            noteA = notes[melodyLength - 4];
            contA++;

            for (int i = 0; i <= 3; i++) //Search for the first note different to the note A
            {
                if (noteA != notes[melodyLength - 3 + i])
                {
                    noteB = notes[melodyLength - 3 + i];
                    break;
                }
            }
            for (int i = 0; i <= 3; i++) //Then counts how many times note A and note B are repeated
            {
                if (noteA == notes[melodyLength - 3 + i]) contA++;
                else if (noteB == notes[melodyLength - 3 + i]) contB++;
            }

            if ((contA >= 2) && (contB >= 2))
            {
                if ((num == noteA) || (num == noteB))
                {
                    repeat = true;
                }   
            }
            return repeat;
        }

    }
}

