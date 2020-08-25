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
       //control.Game(numLeaf, color, color2, color3, color4, color5, color6, color7, color8, color9,hoja, arrow, flecha);
    }


    

}
