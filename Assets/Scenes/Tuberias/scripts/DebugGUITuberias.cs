using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUITuberias : MonoBehaviour
{
    public static DebugGUITuberias instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public string movimientos = "";
    public string levelCompleted = "";

    void OnGUI()
    {
        //GUI.skin.label.fontSize = 54;
        //GUI.Label(new Rect(30, (Screen.height / 2) / 2 - 150, Screen.width, 100), flowCountGUI);
        //GUI.Label(new Rect(30, (Screen.height / 2) / 2 - 100, Screen.width, 100), leveCompleted);
    }

}