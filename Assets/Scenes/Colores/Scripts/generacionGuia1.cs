using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;
using UnityEngine;
using UnityEngine.EventSystems;

public class generacionGuia1 : MonoBehaviour
{
    public float velocidad;
    public int contador = 0;

    public bool adentro = false;

    public Color colorFijo;
    public Color colorLeaf1;
    public string fallo;

    // Start is called before the first frame update
    void Start()
    {
        colorLeaf1 = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje < 10) 
        {
            velocidad = 2;
            GameObject.Find("generador").GetComponent<generadorEnemigos>().comienzoDeTiempo = 2;
        }
        else if ((GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje >= 10) & (GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje < 25))
        {
            velocidad = 3;
        }
        else if ((GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje >= 25))
        {
            velocidad = 3;
            GameObject.Find("generador").GetComponent<generadorEnemigos>().comienzoDeTiempo = 1;
        }

        transform.position += transform.right * -velocidad * Time.deltaTime;

        if (contador == 2)
        { adentro = true;}
        else
        { adentro = false;}



        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf")
        {
            
            if ((adentro) & (GameObject.Find("leaf").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf").GetComponent<SpriteRenderer>().color;

                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }
        
        //error leaf1
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf1")
        {
            if((adentro) & (GameObject.Find("leaf1").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf1").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }

        }
        
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf2")
        {
            if ((adentro) & (GameObject.Find("leaf2").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf2").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }
        
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf3")
        {
            if ((adentro) & (GameObject.Find("leaf3").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf3").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }

        //error leaf4
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf4")
        {
            if ((adentro) & (GameObject.Find("leaf4").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf4").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf5")
        {
            if ((adentro) & (GameObject.Find("leaf5").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf5").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }
        //error en leaf8     
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf6")
        {
            if ((adentro) & (GameObject.Find("leaf6").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf6").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }
        
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf7")
        {
            if ((adentro) & (GameObject.Find("leaf7").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf7").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }
        
        if (GameObject.Find("frog").GetComponent<trigger>().posActualJugador == "leaf8")
        {
            if ((adentro) & (GameObject.Find("leaf8").GetComponent<SpriteRenderer>().color == colorLeaf1))
            {
                colorFijo = GameObject.Find("leaf8").GetComponent<SpriteRenderer>().color;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje++;
                GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
                    GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();

                Destroy(gameObject);
            }
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            contador++;

        }

        if (collision.gameObject.tag == "contador")
        {
            Destroy(gameObject);
            GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje--;
            GameObject.Find("finalRuta").GetComponent<logicaScore>().texto.text = "Score: " +
            GameObject.Find("finalRuta").GetComponent<logicaScore>().puntaje.ToString();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            contador--;

        }
    }
}

