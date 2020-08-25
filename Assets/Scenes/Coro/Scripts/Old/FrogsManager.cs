using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CoroMelodia
{
    public class FrogsManager : MonoBehaviour
    {
        //Singleton implementation
        private static FrogsManager _instance;
        public static FrogsManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("FrogsManager");
                    go.AddComponent<FrogsManager>();
                    _instance = go.GetComponent<FrogsManager>(); 
                }
                return _instance;
            }
        }
        //private bool frogBlock; //If true, every frog can not be pressed (or released).
        //bool paused; // It's true when the game is paused

        //private float _manaAmount = 0.3F;
        //private bool _isBarActive = false;

        [SerializeField]
        private GameObject _xd;

        [SerializeField]
        private GameObject _redFrog;
        [SerializeField]
        private GameObject _blueFrog;
        [SerializeField]
        private GameObject _yellowFrog;
        [SerializeField]
        private GameObject _purpleFrog;
        [SerializeField]
        private GameObject _brownFrog;
        [SerializeField]
        private GameObject _blackFrog;
        [SerializeField]
        private GameObject _directorFrog;
        public GameObject DirectorFrog { get { return _directorFrog; } }

        private GameObject[] _frogList;
        public GameObject[] FrogList { get { return _frogList; } } 
        private BreathAndBlinkScript _breathAndBlink;
        public bool AreFrogsBlocked { get; set; }

        void Awake()
        {
            _instance = this;
            _frogList = new GameObject[6];
            _frogList[0] = _redFrog;
            _frogList[1] = _blueFrog;
            _frogList[2] = _yellowFrog;
            _frogList[3] = _purpleFrog;
            _frogList[4] = _brownFrog;
            _frogList[5] = _blackFrog;

            _breathAndBlink = new BreathAndBlinkScript();
            AreFrogsBlocked = false;
        }

        void Start()
        {
            StartBlinkAndBreath();
        }

        public void PressFrog(int numFrog)
        {
            if(!GameManager.Instance.IsPaused)
            {
                UIManager.Instance.ShowTempoGauge(true);
                UIManager.Instance.DetermineGaugeColor(numFrog);
                AnimationController.Instance.OpenFrog(numFrog, true);
                if (GameManager.Instance.IsGameStarted)
                {
                    MelodyManager.Instance.CompareCorrectNote(numFrog);
                }
            }
        }

        public void ReleaseFrog(int numFrog)
        {
            if(!GameManager.Instance.IsPaused)
            {
                UIManager.Instance.ShowTempoGauge(false);
                AnimationController.Instance.OpenFrog(numFrog, false);
                if(GameManager.Instance.IsGameStarted)
                {
                    if(GameManager.Instance.Fail == true)
                    {
                        GameManager.Instance.FinishGame();
                        GameManager.Instance.ShowResults();
                    }
                    else if(MelodyManager.Instance.IsMelodyCompleted)
                    {
                        AreFrogsBlocked = true;
                        GameManager.Instance.ContinueMelody();
                    }
                }
            }
        }

        public void StartBlinkAndBreath()
        {
            StartCoroutine(_breathAndBlink.BlinkFrog());
            StartCoroutine(_breathAndBlink.BreathFrog());
        }

        // Update is called once per frame
        void Update()
        {
            if(_isBarActive)
            {
                //Debug.Log(_xd.gameObject.GetComponent<Image>());
                _xd.gameObject.GetComponent<Image>().fillAmount += _manaAmount * Time.deltaTime; 
            }
        }
    }
}
*/