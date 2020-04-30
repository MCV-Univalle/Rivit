using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eat_frog_Game
{
    public class GameManager : GameController
    {

        public bool Active,limitreached,paused;
        private FrogController frog;

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
            frog = FindObjectOfType<FrogController>();
            _instance = this;
            
        }

        void Start()
        {
            _uiManager = UIManager.Instance;
            base.Start();
        }

        public override void StartGame()
        {
            Active = true;
            frog.curhealth = 100;
            Debug.Log("Empezó el juego");
        }

        public override void FinishGame()
        {
            Debug.Log("FinishGame");
        }

        public override void AdaptGameParameters() //Adjus the game parametes according to the game mode
        {
            Debug.Log("AdaptGameParameters");
        }
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {

        }
    }
}