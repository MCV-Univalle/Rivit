using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Colores
{
    public class Timer : MonoBehaviour
    {
        public int tiempoInicial;

        [Tooltip("Escala del tiempo")]
        [Range(-10.0f, 10.0f)]

        public float velocidadTiempo = 1;
        private Text myText;
        private float tiempoFrame = 0f;
        private float mostrarSeg = 0f;
        private float alPausar, escalaDeTiempoInicial;
        private bool pausado = false;

        public float masTiempo = 0f;
        // Start is called before the first frame update
        void Start()
        {
            escalaDeTiempoInicial = velocidadTiempo;
            myText = GetComponent<Text>();
            mostrarSeg = tiempoInicial;

            actualizarTiempo(tiempoInicial, myText);
        }

        void Update()
        {

            tiempoFrame = Time.deltaTime * velocidadTiempo;
            mostrarSeg += tiempoFrame;
            actualizarTiempo(mostrarSeg, myText);

        }

        public void actualizarTiempo(float tiempo, Text myText)
        {

            int segundos = 0;
            string textoReloj;

            if (tiempo < 0) tiempo = 0;

            segundos = (int)tiempo % 60;

            if (segundos <= 10)
            {
                myText.color = Color.red;
            }
            else
            {
                myText.color = Color.green;
            }

            textoReloj = segundos.ToString("00") + "s";

            myText.text = textoReloj;
        }

    }
}