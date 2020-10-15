using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreen : UIComponent
{
    void Start() 
    {
        UIManager.executePlayButton += FadeOut;
        UIManager.executeCloseModeSelectionButton += FadeIn;
        UIManager.executeHelpButton += FadeOut;
        UIManager.executeCloseInstructions += FadeIn;
        UIManager.executeQuitGame += FadeIn;
    }

    void OnDestroy() 
    {
        UIManager.executePlayButton -= FadeOut;   
        UIManager.executeCloseModeSelectionButton -= FadeIn;
        UIManager.executeHelpButton -= FadeOut;
        UIManager.executeCloseInstructions -= FadeIn;
        UIManager.executeQuitGame += FadeIn;
    }
}
