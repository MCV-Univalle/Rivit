/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class GameManager : GameController
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    go.AddComponent<GameManager>();
                    _instance = go.GetComponent<GameManager>(); 
                }
                return _instance;
            }
        }

        private int _iterationNumber;
        private int _difficultyLevel = 0; //Only for ilimetd mode
        [SerializeField]
        private GameObject _bush;
        private IEnumerator _currentCoroutine;
        public bool IsCorrect {get; set;}
        private int _lifes;
        public int Lifes 
        {
            get {return _lifes;} 
            set 
            {
                _lifes = value;
                UIManager.Instance.LifesCounter.text = "" + value;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            _uiManager = UIManager.Instance;
            base.Start();
        }
        
        public override void StartGame()
        {
            Score = 0;
            StartNewIteration();
        }
        public override void FinishGame()
        {
            AdaptGameParameters();
            FrogsManager.Instance.SetActiveEveryFrog(true);
            StartCoroutine(FrogsManager.Instance.ReturnEveryFrogToOriginalPosition());
            StopCoroutine(_currentCoroutine);
            Score = 0;
            UIManager.Instance.TimerText.gameObject.SetActive(false);
        }
        public override void AdaptGameParameters()
        {
            //Adjus the game parametes according to the game mode

            if(GameMode == 0)
            {
                UIManager.Instance.LifesCosito.gameObject.SetActive(false);
                Lifes = 3;
                _iterationNumber = 5;
                FrogsManager.Instance.RemainingJumps = 7;
                FrogsManager.Instance.ActiveFrogsNumber = 1;
                FrogsManager.Instance.NormalSpeed = 0.35f;
                FrogsManager.Instance.SpeedVariance = 0.04f;
                FrogsManager.Instance.WaitTime = 2.03f;
                FrogsManager.Instance.JumpingRatio = 10;
            }
            if(GameMode == 1)
            {
                UIManager.Instance.LifesCosito.gameObject.SetActive(false);
                Lifes = 3;
                _iterationNumber = 5;
                FrogsManager.Instance.RemainingJumps = 9;
                FrogsManager.Instance.ActiveFrogsNumber = 1;
                FrogsManager.Instance.NormalSpeed = 0.37f;
                FrogsManager.Instance.SpeedVariance = 0.04f;
                FrogsManager.Instance.WaitTime = 1.7f;
                FrogsManager.Instance.JumpingRatio = 9;
            }
            if(GameMode == 2)
            {
                UIManager.Instance.LifesCosito.gameObject.SetActive(false);
                Lifes = 3;
                _iterationNumber = 5;
                FrogsManager.Instance.RemainingJumps = 12;
                FrogsManager.Instance.ActiveFrogsNumber = 3;
                FrogsManager.Instance.NormalSpeed = 0.55f;
                FrogsManager.Instance.SpeedVariance = 0.05f;
                FrogsManager.Instance.WaitTime = 1.2f;
            }
            if(GameMode == 3)
            {
                _difficultyLevel = 0;
                UIManager.Instance.LifesCosito.gameObject.SetActive(true);
                Lifes = 3;
                _iterationNumber = 1000000000;
                FrogsManager.Instance.RemainingJumps = 7;
                FrogsManager.Instance.ActiveFrogsNumber = 1;
                FrogsManager.Instance.NormalSpeed = 0.35f;
                FrogsManager.Instance.SpeedVariance = 0.04f;
                FrogsManager.Instance.WaitTime = 2.03f;
                FrogsManager.Instance.JumpingRatio = 10;
            }
        }
        public override void ShowResults()
        {
            base.ShowResults();
        }

        public void StartNewIteration()
        {
            FrogsManager.Instance.SetActiveEveryFrog(true);
            StartCoroutine(FrogsManager.Instance.ReturnEveryFrogToOriginalPosition());
            if((_iterationNumber > 0) && (Lifes > 0)) 
            {
                IncreaseDifficulty();
                _iterationNumber--;
                _currentCoroutine = BeginCountdown();
                StartCoroutine(_currentCoroutine);
            }
            else ShowResults();
        }

        public void IncreaseDifficulty()
        {
            if(GameMode == 0)
            {
                FrogsManager.Instance.RemainingJumps++;
                if(_iterationNumber > 2)
                {
                    FrogsManager.Instance.NormalSpeed += 0.01f;
                    FrogsManager.Instance.SpeedVariance += 0.001f;
                    FrogsManager.Instance.WaitTime -= 0.02f;
                }
                else
                {
                    FrogsManager.Instance.ActiveFrogsNumber = 2;
                    FrogsManager.Instance.JumpingRatio = 8;
                    FrogsManager.Instance.NormalSpeed += 0.02f;
                    FrogsManager.Instance.SpeedVariance += 0.003f;
                    FrogsManager.Instance.WaitTime -= 0.5f;
                    
                }
            }

            if(GameMode == 1)
            {
                FrogsManager.Instance.RemainingJumps++;
                if(_iterationNumber > 2)
                {
                    FrogsManager.Instance.RemainingJumps++;
                    FrogsManager.Instance.NormalSpeed += 0.01f;
                    FrogsManager.Instance.SpeedVariance += 0.001f;
                    FrogsManager.Instance.WaitTime -= 0.05f;
                }
                else
                {
                    FrogsManager.Instance.RemainingJumps += 3;
                    FrogsManager.Instance.ActiveFrogsNumber = 3;
                    FrogsManager.Instance.JumpingRatio = 8;
                    FrogsManager.Instance.NormalSpeed += 0.05f;
                    FrogsManager.Instance.SpeedVariance += 0.01f;
                    FrogsManager.Instance.WaitTime -= 0.25f;
                }
            }

            if(GameMode == 2)
            {
                FrogsManager.Instance.RemainingJumps++;
                if(_iterationNumber > 2)
                {
                    FrogsManager.Instance.RemainingJumps++;
                    FrogsManager.Instance.NormalSpeed += 0.05f;
                    FrogsManager.Instance.SpeedVariance += 0.001f;
                    FrogsManager.Instance.WaitTime -= 0.05f;
                }
                else
                {
                    FrogsManager.Instance.RemainingJumps += 5;
                    FrogsManager.Instance.NormalSpeed += 0.1f;
                    FrogsManager.Instance.SpeedVariance += 0.05f;
                    FrogsManager.Instance.WaitTime -= 0.15f;
                }
            }

            if(GameMode == 3)
            {
                if(_difficultyLevel == 0)
                {
                    FrogsManager.Instance.RemainingJumps = 7;
                    FrogsManager.Instance.ActiveFrogsNumber = 1;
                    FrogsManager.Instance.NormalSpeed = 0.35f;
                    FrogsManager.Instance.SpeedVariance = 0.025f;
                    FrogsManager.Instance.WaitTime = 2.0f;
                }
                else if(_difficultyLevel == 1)
                {
                    FrogsManager.Instance.RemainingJumps = 10;
                    FrogsManager.Instance.ActiveFrogsNumber = 1;
                    FrogsManager.Instance.NormalSpeed = 0.4f;
                    FrogsManager.Instance.SpeedVariance = 0.03f;
                    FrogsManager.Instance.WaitTime = 1.5f;
                }
                else if(_difficultyLevel == 2)
                {
                    FrogsManager.Instance.RemainingJumps = 14;
                    FrogsManager.Instance.ActiveFrogsNumber = 2;
                    FrogsManager.Instance.NormalSpeed = 0.47f;
                    FrogsManager.Instance.SpeedVariance = 0.035f;
                    FrogsManager.Instance.WaitTime = 1.2f;
                }
                else if(_difficultyLevel == 3)
                {
                    FrogsManager.Instance.RemainingJumps = 16;
                    FrogsManager.Instance.ActiveFrogsNumber = 3;
                    FrogsManager.Instance.NormalSpeed = 0.5f;
                    FrogsManager.Instance.SpeedVariance = 0.04f;
                    FrogsManager.Instance.WaitTime = 1.1f;

                }
                else if(_difficultyLevel == 4)
                {
                    _iterationNumber = 5;
                    FrogsManager.Instance.RemainingJumps = 20;
                    FrogsManager.Instance.ActiveFrogsNumber = 3;
                    FrogsManager.Instance.NormalSpeed = 0.6f;
                    FrogsManager.Instance.SpeedVariance = 0.045f;
                    FrogsManager.Instance.WaitTime = 0.9f;
                }
            }
        }

        public IEnumerator BeginCountdown()
        {
            yield return new WaitForSeconds(0.25f);
            _bush.SetActive(false);
            UIManager.Instance.TimerText.gameObject.SetActive(true);
            FrogsManager.Instance.SetActiveEveryFrog(true);
            UIAudio.Instance.PlayCountingClip();
            for(int i = 3; i > 0; i--)
            {
                UIManager.Instance.TimerText.text = "" + i;
                yield return new WaitForSeconds(1f);
                UIManager.Instance.TimerText.GetComponent<Animator>().SetTrigger("blink");
                UIAudio.Instance.PlayCountingClip();
            }
            AudioManager.Instance.PlayFalling();
            _bush.SetActive(true);
            UIManager.Instance.TimerText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.65f);
            _currentCoroutine = FrogsManager.Instance.DetermineFrogs();
            StartCoroutine(_currentCoroutine);
        }

        public int CompareUserInput(int num)
        {
            int realNum = FrogsManager.Instance.CountFrogs();
            _bush.SetActive(false);
            FrogsManager.Instance.SetActiveEveryFrog(false);
            if(num == realNum)
            {
                Score++;
                IsCorrect = true;
                if((GameMode == 3) && (_difficultyLevel < 4))
                _difficultyLevel++;
            }
            else
            {
                IsCorrect = false;
                Lifes--;
                if((GameMode == 3) && (_difficultyLevel > 0))
                _difficultyLevel--;
            }
            return realNum;
        }

        public void PlayCorrectSound()
        {
            if(IsCorrect)
            AudioManager.Instance.PlayCorrect();
            if(!IsCorrect)
            AudioManager.Instance.PlayWrong();
        }

        // Update is called once per frame
        void Update()
        {
            if(FrogsManager.Instance.MakeQuestion)
            {
                StartCoroutine(UIManager.Instance.MakeQuestion());
                FrogsManager.Instance.MakeQuestion = false;
            }
        }
    }   
}
*/