using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Eat_frog_Game
{
    public class Comienzo : MonoBehaviour
    {
        public GameObject InsectFactory;
        public GameObject InsectFactory2;
        public GameObject barra;
        private Objective Objective;
        public GameObject Look;
        private FrogController frog;

        private InsectEaterGameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            this._gameManager = gameManager as InsectEaterGameManager;
        }
        // Start is called before the first frame update
        void Start()
        {
            frog = FindObjectOfType<FrogController>();
            Objective = FindObjectOfType<Objective>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_gameManager.Active)
            {
                InsectFactory.SetActive(true);
                InsectFactory2.SetActive(true);
                Look.SetActive(true);
            }
        }

        public void Abandonar()
        {
            _gameManager.Active = false;
            frog.healthlive.fillAmount = 100;
            print("df" + frog.healthlive.fillAmount);
            InsectFactory.SetActive(false);
            InsectFactory2.SetActive(false);
            Look.SetActive(false);
        }

        public void reiniciar()
        {
            _gameManager.Score = 0;
            _gameManager.Active = false;
            frog.Velocity = 0.1f;
            frog.Tongue = true;
            InsectFactory.SetActive(false);
            InsectFactory2.SetActive(false);
            Look.SetActive(false);
            _gameManager.paused = false;
            frog.curhealth = 100f;
        }

        public void Comezar()
        {

            _gameManager.Score = 0;
            _gameManager.Active = true;
            StartCoroutine(ExampleCoroutine1());
        }

        IEnumerator ExampleCoroutine1()
        {
            yield return new WaitForSeconds(1);

            frog.Tongue = true;
            InsectFactory.SetActive(true);
            InsectFactory2.SetActive(true);
            Look.SetActive(true);
            _gameManager.paused = false;
        }

        public void paused()
        {
            _gameManager.paused = true;
        }
        public void resumen()
        {
            StartCoroutine(ExampleCoroutine());
        }

        IEnumerator ExampleCoroutine()
        {
            yield return new WaitForSeconds(1);

            _gameManager.paused = false;
        }


    }

}
