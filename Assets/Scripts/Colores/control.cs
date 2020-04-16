using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class control : MonoBehaviour
{
    #region de singleton
    public static control instance;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
    }

    #endregion

    public static control Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(control)) as control;
            if (!instance)
                Debug.LogError("error nenufar");
        }

        return instance;
    }

    private trigger disparador;
    private int randColor;
    public string posActualJugador;

    public void Game(int numLeaf, Color color, Color color2, Color color3, Color color4, Color color5, Color color6, Color color7, Color color8, Color color9, Color hoja, Color arrow,  GameObject flecha)
    {


            switch (numLeaf)
            {

                case 1:
                /*
                    leaf1.GetComponent<SpriteRenderer>().color = color;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                    */
                flecha.GetComponent<SpriteRenderer>().color = arrow;

                    break;

                case 2:
                /*
                    leaf2.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                    */
                    flecha.GetComponent<SpriteRenderer>().color = arrow;

                break;

                case 3:
                /*
                    leaf3.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                    */
                    flecha.GetComponent<SpriteRenderer>().color = arrow;

                break;


                case 4:
                /*
                    leaf5.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                    */
                    flecha.GetComponent<SpriteRenderer>().color = arrow;

                break;

                case 5:
                /*
                    leaf6.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                    */
                flecha.GetComponent<SpriteRenderer>().color = arrow;

                break;

                case 6:
                /*
                    leaf7.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                */
                flecha.GetComponent<SpriteRenderer>().color = arrow;

                break;

                case 7:
                /*
                    leaf8.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf9.GetComponent<SpriteRenderer>().color = hoja;
                    */
                flecha.GetComponent<SpriteRenderer>().color = arrow;

                break;

                case 8:
                /*
                    leaf9.GetComponent<SpriteRenderer>().color = color;
                    leaf1.GetComponent<SpriteRenderer>().color = hoja;
                    leaf2.GetComponent<SpriteRenderer>().color = hoja;
                    leaf3.GetComponent<SpriteRenderer>().color = hoja;
                    leaf4.GetComponent<SpriteRenderer>().color = hoja;
                    leaf5.GetComponent<SpriteRenderer>().color = hoja;
                    leaf6.GetComponent<SpriteRenderer>().color = hoja;
                    leaf7.GetComponent<SpriteRenderer>().color = hoja;
                    leaf8.GetComponent<SpriteRenderer>().color = hoja;
                    */
                flecha.GetComponent<SpriteRenderer>().color = arrow;

                    break;
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        disparador = trigger.Instance();

        /*
        int[] colores = new int[9];

        randColor = Random.Range(1,9);

        for (int i = 0; i < colores.Length; i++)
        {
            colores[i] = i+1;
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        posActualJugador = disparador.ubicacion;
        
    }

}
