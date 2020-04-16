using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
  {
    bool active;
    Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = true;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Start(bool gameactive)
    {
        if (!gameactive)
        {
            canvas.enabled = gameactive;
            Time.timeScale = 1f;
        }
    }
}
