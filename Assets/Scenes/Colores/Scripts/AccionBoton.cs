using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Colores;
using UnityEngine;

namespace Colores
{
    public class AccionBoton : MonoBehaviour
    {
        public static AccionBoton instance;

        private GameObject[] listaObjetos;
        public int colorFijo, fallas, correctos;
        public Sprite huella;
        public Sprite blanco; 
        public float tiempo;
        private int auxiliar;

        void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            listaObjetos = GameObject.FindGameObjectsWithTag("leaf");
        }

        // Update is called once per frame
        void Update()
        {
            auxiliar = 0;

            for (int i = 0; i < 4; i++)
            {
                if (listaObjetos[i].GetComponent<Image>().sprite == huella)
                {
                    colorFijo = i + 1;
                }
                else
                {
                    auxiliar++;
                }
            }

            if (auxiliar == 4)
                colorFijo = 5;

            if (tiempo > 0)
            {
                tiempo -= Time.deltaTime;
                QuitarHuella();
            }
        }

        public void QuitarHuella()
        {
            if (tiempo <= 0)
            {
                gameObject.GetComponent<Image>().sprite = blanco;
            }
        }

        public void AsignarHuella()
        {
            gameObject.GetComponent<Image>().sprite = huella;
            tiempo = 0.12f;
        }

    }
}
