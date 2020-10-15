using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour
{


    public GameObject leaf;
    public Color colorLeaf;
    public string identificadorPos;
    public string posActualJugador;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("leaf"))
        {
            colorLeaf = GameObject.Find("leaf").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf";
        }
        if (collision.CompareTag("leaf1"))
        {
            colorLeaf = GameObject.Find("leaf1").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf1";
        }
        if (collision.CompareTag("leaf2"))
        {
            colorLeaf = GameObject.Find("leaf2").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf2";
            this.gameObject.transform.localPosition = new Vector3(42.2f, 33.6f, 0f);
        }
        if (collision.CompareTag("leaf3"))
        {
            colorLeaf = GameObject.Find("leaf3").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf3";
        }
        if (collision.CompareTag("leaf4"))
        {
            colorLeaf = GameObject.Find("leaf4").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf4";
        }
        if (collision.CompareTag("leaf5"))
        {
            colorLeaf = GameObject.Find("leaf5").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf5";
        }
        if (collision.CompareTag("leaf6"))
        {
            colorLeaf = GameObject.Find("leaf6").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf6";
        }
        if (collision.CompareTag("leaf7"))
        {
            colorLeaf = GameObject.Find("leaf7").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf7";
        }
        if (collision.CompareTag("leaf8"))
        {
            colorLeaf = GameObject.Find("leaf8").GetComponent<SpriteRenderer>().color;
            posActualJugador = "leaf8";
        }

    }
}
