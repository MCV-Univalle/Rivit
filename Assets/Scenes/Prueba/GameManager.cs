using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prueba
{
    public class GameManager : GameController
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                //Logic to create the instance
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        void Start()
        {
            _uiManager = UIManager.Instance;
            base.Start();
        }

        public override void StartGame()
        {
            Debug.Log("Empezó el juego");
        }

        public override void FinishGame()
        {
            Debug.Log("FinishGame");
        }

        public override void ShowResults()
        {
            Debug.Log("ShowResults");
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
