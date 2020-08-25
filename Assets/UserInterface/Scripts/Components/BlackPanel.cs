using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPanel : UIComponent
{
    void Start()
    {
        positionY = -675;
        UIManager.executeHelpButton += FadeInMoveY;
        UIManager.executeCloseInstructions += FadeOutMoveY;
        UIManager.executePlayButton += FadeInMoveY;
        UIManager.executeCloseModeSelectionButton += FadeOutMoveY;
        UIManager.executeStartGame += FadeOutMoveY;
        UIManager.executeGameOver += FadeInMoveY;
    }

    void OnDestroy()
    {
        UIManager.executeHelpButton -= FadeInMoveY;
        UIManager.executeCloseInstructions -= FadeOutMoveY;
        UIManager.executePlayButton -= FadeInMoveY;
        UIManager.executeCloseModeSelectionButton -= FadeOutMoveY;
        UIManager.executeStartGame -= FadeOutMoveY;
        UIManager.executeGameOver -= FadeInMoveY;
    }
}
