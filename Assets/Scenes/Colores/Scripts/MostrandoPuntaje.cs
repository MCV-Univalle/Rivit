using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Colores
{
    public class MostrandoPuntaje : MonoBehaviour
    {

    // Start is called before the first frame update
        public int puntajeObtenido;
        public float tiempo;
        void Start()
        {
        
        }

    // Update is called once per frame
        void Update()
        {
            if(gameObject.GetComponentInChildren<Text>().text == " ")
            {
                mostrarPuntaje(puntajeObtenido);
                tiempo = 0.15f;
                
            }else if(tiempo <= 0){
                gameObject.GetComponentInChildren<Text>().text = " ";
                puntajeObtenido = 0;
            }

            if (tiempo > 0)
                tiempo -= Time.deltaTime;
        }

        public void mostrarPuntaje(int puntaje)
        {
            if(puntaje == 2)
            {
                gameObject.GetComponentInChildren<Text>().text = "+2";

            }else if(puntaje == 1){
                gameObject.GetComponentInChildren<Text>().text = "+1";
            }

        }
    }

}