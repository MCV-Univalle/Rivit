using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{

    float deltaX, deltaY;
    Rigidbody2D rb;
    bool sostenido = false;
    public int numLeaf;
    public int leafDispair;
    public string lugar;
    public int colorPatron;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (sostenido == true)
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.gameObject.transform.localPosition = new Vector3(mousePos.x, mousePos.y, 0);
        }

        lugar = GameObject.Find("frog").GetComponent<trigger>().posActualJugador;

    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            sostenido = true;
        }
    }

    private void OnMouseUp()
    {
        sostenido = false;

        if (lugar=="leaf")
        {
            this.gameObject.transform.localPosition = new Vector3(37.5f, 33.6f, 0f);
        }
        if (lugar == "leaf1")
        {
            this.gameObject.transform.localPosition = new Vector3(40.05f, 33.6f, 0f);
        }
        if (lugar == "leaf2")
        {
            this.gameObject.transform.localPosition = new Vector3(42.5f, 33.6f, 0f);
        }
        if (lugar == "leaf3")
        {
            this.gameObject.transform.localPosition = new Vector3(37.5f, 36f, 0f);
        }
        if (lugar == "leaf4")
        {
            this.gameObject.transform.localPosition = new Vector3(40.05f, 36f, 0f);
        }
        if (lugar == "leaf5")
        {
            this.gameObject.transform.localPosition = new Vector3(42.5f, 36f, 0f);
        }
        if (lugar == "leaf6")
        {
            this.gameObject.transform.localPosition = new Vector3(37.5f, 38.5f, 0f);
        }
        if (lugar == "leaf7")
        {
            this.gameObject.transform.localPosition = new Vector3(40.05f, 38.5f, 0f);
        }
        if (lugar == "leaf8")
        {
            this.gameObject.transform.localPosition = new Vector3(42.5f, 38.5f, 0f);
        }
    }


    

}
