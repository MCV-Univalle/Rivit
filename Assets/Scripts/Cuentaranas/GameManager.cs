using System.Collections;
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
        [SerializeField]
        private GameObject _bush;

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
            StartNewIteration();
        }
        public override void FinishGame()
        {
            Debug.Log("xD");
        }
        public override void AdaptGameParameters()
        {
            //Adjus the game parametes according to the game mode

            if(GameMode == 0)
            {
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
                _iterationNumber = 10;
                FrogsManager.Instance.RemainingJumps = 9;
                FrogsManager.Instance.ActiveFrogsNumber = 1;
                FrogsManager.Instance.NormalSpeed = 0.37f;
                FrogsManager.Instance.SpeedVariance = 0.04f;
                FrogsManager.Instance.WaitTime = 1.7f;
                FrogsManager.Instance.JumpingRatio = 9;
            }
            if(GameMode == 2)
            {
                Debug.Log("xD");
            }
        }
        public override void ShowResults()
        {
            base.ShowResults();
            Score = 0;
        }

        public void StartNewIteration()
        {
            FrogsManager.Instance.SetActiveEveryFrog(true);
            UIManager.Instance._userAnswer.gameObject.SetActive(false);
            FrogsManager.Instance.ReturnEveryFrogToOriginalPosition();
            if(_iterationNumber > 0)
            {
                IncreaseDifficulty();
                _bush.SetActive(false);
                _iterationNumber--;
                StartCoroutine(BeginCountdown());
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
                if(_iterationNumber > 6)
                {
                    FrogsManager.Instance.RemainingJumps++;
                    FrogsManager.Instance.NormalSpeed += 0.01f;
                    FrogsManager.Instance.SpeedVariance += 0.001f;
                    FrogsManager.Instance.WaitTime -= 0.05f;
                }
                else if (_iterationNumber > 2)
                {
                    FrogsManager.Instance.RemainingJumps += 2;
                    FrogsManager.Instance.ActiveFrogsNumber = 2;
                    FrogsManager.Instance.JumpingRatio = 9;
                    FrogsManager.Instance.NormalSpeed += 0.025f;
                    FrogsManager.Instance.SpeedVariance += 0.005f;
                    FrogsManager.Instance.WaitTime -= 0.1f;
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
        }

        public IEnumerator BeginCountdown()
        {

            UIManager.Instance.TimerText.gameObject.SetActive(true);
            FrogsManager.Instance.SetActiveEveryFrog(true);
            for(int i = 3; i > 0; i--)
            {
                UIManager.Instance.TimerText.text = "" + i;
                yield return new WaitForSeconds(1f);
                UIManager.Instance.TimerText.GetComponent<Animator>().SetTrigger("blink");
            }
            _bush.SetActive(true);
            UIManager.Instance.TimerText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(FrogsManager.Instance.DetermineFrogs());
        }

        public string CompareUserInput(int num)
        {
            int realNum = FrogsManager.Instance.CountFrogs();
            _bush.SetActive(false);
            FrogsManager.Instance.SetActiveEveryFrog(false);
            if(num == realNum)
            {
                Score++;
                AudioManager.Instance.PlayCorrect();
                return "Correcto. :D";
            }
            else
            {
                AudioManager.Instance.PlayWrong();
                return "Sorry bro. :^(";
            }
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
