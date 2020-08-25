using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eat_frog_Game
{
    public class UIManager : UIController
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {

                 if(_instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    go.AddComponent<UIManager>();
                    _instance = go.GetComponent<UIManager>(); 
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
            
            _gameManager = GameManager.Instance;
            Debug.Log(_gameManager);
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
        }

        public void ExecutedStart()
        {
            StartGame canvas = GetComponent<StartGame>();
            canvas.Start(false);
        }
    }
}

