using System.Collections;
using System.Collections.Generic;
using Colores;
using UnityEngine;

namespace Colores
{

    public class Control : MonoBehaviour
    {
        public static Control instance;

        void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public float CambiarEtapa(float tiempo, int etapa)
        {
            switch (etapa)
            {
                case 1:
                    tiempo = 6;
                    break;

                case 2:
                    tiempo = 4;
                    break;

                case 3:
                    tiempo = 2;
                    break;

                case 4:
                    tiempo = 1f;
                    break;
            }

            return tiempo;
        }

        public float CambiarEtapaVel(int etapa, float velocidad)
        {
            switch (etapa)
            {
                case 1:
                    velocidad = 70;
                    break;

                case 2:
                    velocidad = 100;
                    break;

                case 3:
                    velocidad = 130;
                    break;

                case 4:
                    velocidad = 160;
                    break;
            }

            return velocidad;
        }
    }

}