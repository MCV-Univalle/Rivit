using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : UIComponent
{
    void Start()
    {
        _fadeTime = 0.075f;
        _moveTimeY = 0.075f;
        _delay = 0.01f;
        positionY = 475;
        UIManager.executePauseButton += FadeInMoveY;
        UIManager.executeResumeFromPause += FadeOutMoveY;
        UIManager.executeQuitGame += FadeOutMoveY;
    }

    void OnDestroy()
    {
        UIManager.executePauseButton -= FadeInMoveY;
        UIManager.executeResumeFromPause -= FadeOutMoveY;
        UIManager.executeQuitGame -= FadeOutMoveY;
    }
}