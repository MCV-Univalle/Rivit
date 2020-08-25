using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager
{
    public bool IsPaused {get; set;}
    public void ChangePauseState()
    {
        if(!IsPaused) PauseGame();
        else ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        IsPaused = false;

    }
}
