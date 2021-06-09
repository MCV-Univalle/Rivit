using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    public static DebugGUI _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public string flowCountGUI = "";
    public string leveCompleted = "";

    void OnGUI()
    {
        GUI.skin.label.fontSize = 54;
        GUI.Label(new Rect(30, (Screen.height / 2) / 2 - 150, Screen.width, 100), flowCountGUI);
        GUI.Label(new Rect(30, (Screen.height / 2) / 2 - 100, Screen.width, 100), leveCompleted);
    }

}
