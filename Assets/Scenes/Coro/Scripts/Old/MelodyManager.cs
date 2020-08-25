using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CoroMelodia
{
    public class MelodyManager : MonoBehaviour
    {
        //Singleton implementation
        private static MelodyManager _instance;
        public static MelodyManager Instance
        {
            get
            {
                //Logic to create the _instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("MelodyManager");
                    go.AddComponent<MelodyManager>();
                    _instance = go.GetComponent<MelodyManager>();
                }
                return _instance;
            }
        }

        private List<int> _melodyNotes;
        public List<int> MelodyNotes{ get {return _melodyNotes;} }
        private int _index; //Represents the current note to be checked in the _melodyNotes. 
        //private int _newNotes; //The number of notes that the melodyNotesNotes will increase
        public bool IsMelodyCompleted { get; set; }
        public bool IsPlayingMelody { get; set; }

        // Start is called before the first frame update
        void Awake()
        {
            _instance = this;
            _melodyNotes = new List<int>();
            _index = 0;
            IsPlayingMelody = false;

        }

        //---------------- Note Generation ------------------

        public void EmptyMelody()
        {
            _melodyNotes = new List<int>(); //The current melody is remplaced for a empty one;
        }

        public void ExtendMelody(int newNotes, bool restartMelody) 
        {
            IsMelodyCompleted = false;
            _index = 0;
            if(restartMelody)
            {
                EmptyMelody();
            }
            for (var i = 0; i < newNotes; i++)
            {
                int num = 0;
                num = GenerateRandomNote();
                _melodyNotes.Add(num);
            }
        }
        public int GenerateRandomNote()   
        {
            //Generate a random number.
            //If the generated number aplly for any of the monotone patterns, it generates
            //other number until it do not aply any patten

            bool repA = true;
            bool repB = true;
            int num = 0;
            while ((repA) || (repB))
            {
                repA = false;
                repB = false;
                int maxFrogs = 6;
                num = Random.Range(0, maxFrogs);
                if (_melodyNotes.Count > 1)
                {    
                    repA = VerifyLastTwoPattern(num);
                }
                if (_melodyNotes.Count > 4)
                {
                    repB = VerifyMonotonePattern(num);
                }
            }
            return num;       
        }

        public bool VerifyLastTwoPattern(int n) 
        {
            //Verify if the last 2 notes in the _melodyNotes are equal to n.
            //It prevents the same note beign repeated 3 times or more
            int i = _melodyNotes.Count - 1;
            bool b = false;

            if ((n == _melodyNotes[i]) && (n == _melodyNotes[i - 1]))
            {
                b = true;
            }
            return b;
        }

        public bool VerifyMonotonePattern(int n) 
        {
            //Verify if there are 2 especific notes that are repeated
            //frequently in the last 5 notes notes of the _melodyNotes
            
            int k = _melodyNotes.Count - 1;
            bool b = false;
            int noteA, noteB = 0;
            int contA = 0, contB = 0;

            noteA = _melodyNotes[k - 4];
            contA++;

            for (int i = 0; i <= 3; i++) //Search for the first note different to the note A
            {
                if (noteA != _melodyNotes[k - 3 + i])
                {
                    noteB = _melodyNotes[k - 3 + i];
                    break;
                }
            }
            for (int i = 0; i <= 3; i++) //Then counts how many times note A and note B are repeated
            {
                if (noteA == _melodyNotes[k - 3 + i]) contA++;
                else if (noteB == _melodyNotes[k - 3 + i]) contB++;
            }

            if ((contA >= 2) && (contB >= 2))
            {
                if ((n == noteA) || (n == noteB))
                {
                    b = true;
                }   
            }

            return b;
        }

        //----------------------------------------

        public void IndicateFrog(int note, bool value)
        {
            LightingManager.Instance.IlluminateFrog(note, value);
            AnimationController.Instance.OpenFrog(note, value);
        }

        public IEnumerator ShowMelody(float tempo)
        {
            GameManager.Instance.IsRuningMelodyCoroutine = true;
            IsPlayingMelody = true;
            foreach (int note in _melodyNotes)
            {

                yield return new WaitForSeconds(0.1F);
                IndicateFrog(note, true);
                AnimationController.Instance.ChangeDirectorPose();
                yield return new WaitForSeconds(tempo - 0.1F);
                IndicateFrog(note, false);
            }
            GameManager.Instance.IsRuningMelodyCoroutine = false;
            StartCoroutine(StopPlayingMelody());
        } 

        public IEnumerator StopPlayingMelody()
        {
            yield return new WaitForSeconds(0.1F);
            AnimationController.Instance.ToDirectorIdle();
            //Player turn starts
            LightingManager.Instance.TurnLightsOn();
            AnimationController.Instance.CloseEveryFrog();
            FrogsManager.Instance.AreFrogsBlocked = false;
            IsPlayingMelody = false;
        }

        public void CompareCorrectNote(int numFrog)
        {
            int currentNote = _melodyNotes[_index];
            if(currentNote == numFrog)
            {
                _index++;
                GameManager.Instance.Score++;
            }
            else 
            {
                NotifyFailure(currentNote); //Game is Over
            }

            VerifyMelodyFinished();
        }

        public void NotifyFailure(int numFrog)
        {
            AnimationController.Instance.SurpriseFrog(numFrog, true);
            MessagesManager.Instance.ShowCorrectFrog(FrogsManager.Instance.FrogList[numFrog]);
            AnimationController.Instance.ToDirectorFailPose(true);
            GameManager.Instance.Fail = true;
            // //The current melody is remplaced for a empty one;
            AudioManager.Instance.PlayWrong();
        }

        public void VerifyMelodyFinished()
        {
            if(_index == _melodyNotes.Count)
            {
                AnimationController.Instance.ToDirectorCorrectPose(true);
                IsMelodyCompleted = true;
                AudioManager.Instance.PlayCorrect();
                MessagesManager.Instance.ShowCongratulationMessage();
            }
        }
    }  
}
*/