using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPanel : UIComponent
{
    void Start()
    {
        UIManager.executeHelpButton += () => gameObject.SetActive(true);
        UIManager.executeStartGame += () => gameObject.SetActive(false);
        UIManager.executeGameOver += () => gameObject.SetActive(true);
        UIManager.executeQuitGame += () => gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        UIManager.executeHelpButton -= () => gameObject.SetActive(true);
        UIManager.executeStartGame -= () => gameObject.SetActive(false);
        UIManager.executeGameOver -= () => gameObject.SetActive(true);
        UIManager.executeQuitGame -= () => gameObject.SetActive(true);
    }
}
