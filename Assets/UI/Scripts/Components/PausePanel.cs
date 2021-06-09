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
        UIManager.executePauseButton += () => this.gameObject.SetActive(true);
        UIManager.executeResumeFromPause += () => this.gameObject.SetActive(false);
        UIManager.executeQuitGame += () => this.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        UIManager.executePauseButton -= () => this.gameObject.SetActive(true);
        UIManager.executeResumeFromPause -= () => this.gameObject.SetActive(false);
        UIManager.executeQuitGame -= () => this.gameObject.SetActive(false);
    }
}