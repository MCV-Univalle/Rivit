using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colores;
using UnityEngine.UI;

namespace Colores
{
    public class GeneracionGuias : MonoBehaviour
    {
        public static GeneracionGuias instance;

        void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        void Update()
        {

        }

        public void ValoresDiferentes(List<int> lista, int numColor, int numPalabras)
        {
            numColor = Random.Range(lista[0], lista.Count);
            lista.Remove(numColor);
            numPalabras = Random.Range(lista[0], lista.Count);
            lista.Add(numColor);
        }

        // Update is called once per frame
        public void Enemigos(GameObject esfera, GameObject panel, List<Color> colores, int color, int palabra, List<string> palabrasGuias)
        {
            GameObject guia = Instantiate(esfera) as GameObject;
            guia.transform.SetParent(panel.transform, false);
            guia.GetComponentInChildren<TextMesh>().color = colores[color];
            guia.GetComponentInChildren<TextMesh>().text = palabrasGuias[palabra];
        }

        public void EnemigosExtra(GameObject enemigo,GameObject panel, List<Color> colores, int color)
        {
            GameObject extra = Instantiate(enemigo) as GameObject;
            extra.transform.SetParent(panel.transform, false);
            extra.GetComponent<SpriteRenderer>().color = colores[color];
        }
    }
}