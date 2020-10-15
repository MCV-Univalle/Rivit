using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;


public class control : MonoBehaviour
{
    private int randColor;
    public string posActualJugador;
    public string pos;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        posActualJugador = "";
        //timer = gameobject.GetComponent<Timer>();

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

        
    }

}
