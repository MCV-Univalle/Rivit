using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prueba
{
    public class UIManager : UIController
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    _instance = new UIManager();
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
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
