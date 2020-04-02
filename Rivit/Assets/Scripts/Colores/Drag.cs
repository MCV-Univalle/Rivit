using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{

    private control control;
    float deltaX, deltaY;
    Rigidbody2D rb;
    bool sostenido = false;
    public int numLeaf;
    public int leafDispair;
    public int numColor;
    public int colorPatron;

    public Color color;
    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
    public Color color5;
    public Color color6;
    public Color color7;
    public Color color8;
    public Color color9;
    public Color hoja;
    public Color arrow;

    public Color col1;
    public Color col2;
    public Color col3;
    public Color col4;
    public Color col5;
    public Color col6;
    public Color col7;
    public Color col8;
    public Color col9;

    public GameObject frog;
    public GameObject flecha;

    #region de singleton

    public static Drag instance;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
    }

    #endregion

    public static Drag Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(Drag)) as Drag;
            if (!instance)
                Debug.LogError(":v");
        }

        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
      control = control.Instance();
      
    }

    // Update is called once per frame
    void Update()
    {
        
        numLeaf = Random.Range(1, 9);
        numColor = Random.Range(1, 10);

     
        switch (numColor)
        {

            case 1:

                color = color1;
                colorPatron = 1;
                arrow = col1;

                break;

            case 2:

                color = color2;
                colorPatron = 2;
                arrow = col2;

                break;

            case 3:

                color = color3;
                colorPatron = 3;
                arrow = col3;

                break;

            case 4:

                color = color4;
                colorPatron = 4;
                arrow = col4;

                break;

            case 5:

                color = color5;
                colorPatron = 5;
                arrow = col5;

                break;

            case 6:

                color = color6;
                colorPatron = 6;
                arrow = col6;

                break;

            case 7:

                color = color7;
                colorPatron = 7;
                arrow = col7;

                break;

            case 8:

                color = color8;
                colorPatron = 8;
                arrow = col8;

                break;

            case 9:

                color = color9;
                colorPatron = 9;
                arrow = col9;

                break;

        }
        
        if (sostenido == true)
        {

            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.gameObject.transform.localPosition = new Vector3(mousePos.x, mousePos.y, 0);

        }

    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            sostenido = true;
            Debug.Log(":V");
        }
    }

    private void OnMouseUp()
    {
        sostenido = false;
       control.Game(numLeaf, color, color2, color3, color4, color5, color6, color7, color8, color9,hoja, arrow, flecha);
        Debug.Log(">:V");
    }

    /*
     if (other.tag == "leaf")
     {
         Debug.Log("weeeey");
     }
     */
    // si el objeto que traspasó es el jugador

    

}
