using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Saltos
{
    public class UIManager : UIController
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                //Logic to create the instance
                if (_instance == null)
                {
                    _instance = new UIManager();
                }
                return _instance;
            }
        }
        private bool inicio = false;

        [SerializeField]
        private int cont = 0;

        [SerializeField]
        GameObject checkT;
        [SerializeField]
        GameObject checkF;
        [SerializeField]
        GameObject buttonInicio;
        [SerializeField]
        GameObject panel;
        [SerializeField]
        GameObject panelBlanco;
        [SerializeField]
        GameObject buttonRecargar;
        [SerializeField]
        GameObject timerText;
        [SerializeField]
        GameObject panelTimerText;
        [SerializeField]
        TextMeshProUGUI canSaltosText;
        public float targetTime;

        void Awake()
        {
            _instance = this;
        }

        void Start()
        {
            _gameManager = GameManager.Instance;
            base.Start();
        }

        public void eventButton()
        {
            setButtonInicio(true);

            //checkT.SetActive(true);
        }

        public bool isClikButtonInicio()
        {
            //cont += 1;
            //Debug.Log("Se toco: " + cont);
            return inicio;
        }

        public void setButtonInicio(bool estado)
        {
            inicio = estado;
        }

        public bool getVarInicio()
        {
            return inicio;
        }

        public void activeCheckTrueOrFalse(bool activar, bool est)
        {
            if (activar)
            {
                if (est)
                {
                    checkT.SetActive(true);
                }
                else
                {
                    checkF.SetActive(true);
                }
            }
            else
            {
                checkT.SetActive(false);
                checkF.SetActive(false);
            }

        }

        public void buttonVisible(bool est)
        {
            //buttonInicio.SetActive(est);
            panel.SetActive(est);
            panelBlanco.SetActive(est);
        }

        public void buttonRecargarVisible(bool est)
        {
            buttonRecargar.SetActive(est);
        }

        public void ExecutePlayButton()
        {
            base.ExecutePlayButton();
        }

        public void ExecuteQuitButton()
        {
            base.ExecuteQuitButton();
        }

        public void SetValTimerText(float tiempo)
        {
            if(tiempo >= 0.5)
            {
                panelTimerText.SetActive(true);
                SetVisibilidadTextTimer(true);
                AudioManagerSaltos.Instance.PlayTimerPop();

                //timerText.text = "" + tiempo.ToString("f0");

                timerText.GetComponent<Text>().text = "" + tiempo.ToString("f0");

                //Debug.Log("var tiempo = " + tiempo.ToString("f0"));
            }
            else
            {
                SetVisibilidadTextTimer(false);
                panelTimerText.SetActive(false);
            }      
        }

        public void SetCanSaltosText(int cantidad)
        {
            canSaltosText.text = "" + cantidad.ToString("f0") + "/8";
            //Debug.Log("var saltos = " + cantidad.ToString("f0"));   
        }

        public void SetVisibilidadTextTimer(bool valor)
        {
            timerText.SetActive(valor);
        }


    }
}

