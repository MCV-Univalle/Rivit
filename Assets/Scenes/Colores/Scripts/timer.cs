using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    [SerializeField]
    private int seconds;

    private int s;

    [SerializeField]
    private Text tiempoTexto;

    // Start is called before the first frame update
    void Start()
    {

    }
/*
    public void startTimer() 
    {
        s = seconds;
        writeTimer(s);
        //invoke("updateTimer", 1f);
    }
    public void stopTimer() { }
    public void updateTimer() 
    {
        s--;
        writeTimer(s);
       //invoke("updateTimer", 1f);
    }
    
    public void writeTimer(int s) 
    {
        if (s < 10)
        {
            tiempoTexto.text = "0" + s.ToString();
        }
        else {
            tiempoTexto = s.ToString();
        }
    }*/

}
