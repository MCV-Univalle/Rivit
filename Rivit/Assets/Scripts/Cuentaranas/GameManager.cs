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

        void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            _uiManager = UIManager.Instance;
            //base.Start();
        }

        public override void StartGame()
        {
            Debug.Log("Pollo");
            StartCoroutine(FrogsManager.Instance.DetermineFrogs());
        }
        public override void FinishGame()
        {
            Debug.Log("xD");
        }
        public override void AdaptGameParameters()
        {
            Debug.Log("xD");
        }
        public override void ShowResults()
        {
            Debug.Log("xD");
        }

        public string CompareUserInput(int num)
        {
            int realNum = FrogsManager.Instance.CountFrogs();
            if(num == realNum)
            return "Correcto. :D";
            else
            return "Sorry bro. :^(";
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
