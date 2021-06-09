using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colores;
using UnityEngine.UI;
using System;
using Zenject;

namespace Colores
{
    public class Señales : MonoBehaviour
    {
    
        public static Señales instance;
       [Inject(Id = "SFXManager")] AudioManager SFXManager;
        public Sprite bien, error, vacio, racha;
        public ColoresGameManager coloresGameManager;
        public int senal;
        private float tiempo;

        void Awake()
        {
            instance = this;
        }

        public void CambiarImagen()
        {
            if(senal == 1)
            {
                tiempo = 0.2f;
                gameObject.GetComponent<Image>().sprite = bien;
                SFXManager.PlayAudio("Boing");
                gameObject.GetComponentInChildren<Text>().text = " ";
                senal = 0;
            }
            else if(senal == 2)
            {
                tiempo = 0.2f;
                gameObject.GetComponent<Image>().sprite = error;
                gameObject.GetComponentInChildren<Text>().text = " ";
                SFXManager.PlayAudio("Wrong");
                senal = 0;
            }               
        }

        public void CambiarImagenRacha(int puntajeAcumulado)
        {
            if(senal == 1)
            {
                gameObject.GetComponent<Image>().sprite = racha;
                SFXManager.PlayAudio("Boing");
                gameObject.GetComponentInChildren<Text>().text = Convert.ToString(puntajeAcumulado);
                senal = 0;
            }
            else if(senal == 2)
            {
                tiempo = 0.2f;
                gameObject.GetComponent<Image>().sprite = error;
                SFXManager.PlayAudio("Wrong");
                gameObject.GetComponentInChildren<Text>().text = " ";
                senal = 0;
            } 
        }

        public void QuitarHuella()
        {
            if (tiempo <= 0)
            {
                gameObject.GetComponent<Image>().sprite = vacio;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            coloresGameManager = FindObjectOfType<ColoresGameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if(coloresGameManager.correctosSeguidos >= 5)
            {
                CambiarImagenRacha(coloresGameManager.correctosSeguidos);
            }
            else
            {
                CambiarImagen();

                if (tiempo > 0)
                {
                    tiempo -= Time.deltaTime;
                    QuitarHuella();
                }
            }
        }
    }
}
