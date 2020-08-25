using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModeSelectionScreen : UIComponent
{
    [SerializeField]  private UIManager uiManager;
    [SerializeField]  private ModeCarousel modeCarousel; 
    [SerializeField] private TextMeshProUGUI modeNameText;
    [SerializeField] private TextMeshProUGUI gameModeTopScoreText;
    [SerializeField] private TextMeshProUGUI descriptionText;


    void Start()
    {
        positionY = -675;
        positionX = -500;
        UIManager.executePlayButton += OpenModeSelectionScreen;
        UIManager.executeCloseModeSelectionButton += FadeOutMoveY;
        UIManager.executeStartGame += FadeOutMoveY;
        UIManager.executePlayAgainButton += FadeInMoveX;
        UIManager.executeGameOver += modeCarousel.UpdateDetails;
    }

    public void OpenModeSelectionScreen()
    {
        modeCarousel.UpdateDetails();
        FadeInMoveY();
    }

    public void UpdateModeDetails(string name, string description, int highScore)
    {
        modeNameText.text = name;
        gameModeTopScoreText.text = highScore + "";
        descriptionText.text = description;
    }

    public void SelectGameMode()
    {
        GameMode mode = modeCarousel.CurrentMode;
        uiManager.SelectGameMode(mode);
    }

    void OnDestroy()
    {
        UIManager.executePlayButton -= OpenModeSelectionScreen;
        UIManager.executeCloseModeSelectionButton -= FadeOutMoveY;
        UIManager.executeStartGame -= FadeOutMoveY;
        UIManager.executePlayAgainButton -= FadeInMoveX;
        UIManager.executeGameOver -= modeCarousel.UpdateDetails;
    }
}
