using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelSelectionScreen : UIComponent
{
    [SerializeField] private UIManager uiManager;

    [SerializeField] private GameObject levelButtonsContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject confirmButton;

    public GameObject ActiveButton { get; set; }

    [SerializeField] private LevelsHandler levelsHandler;

    [Inject] private GameManager _gameManager;

    public int NumberOfLevels { get; set; }

    void Start()
    {
        var pos = transform.position;
        positionX = pos.x;
        positionY = pos.y;

        UIManager.executeLevelCompleted += FadeOutMoveX;
        UIManager.executeStartGame += FadeOutMoveX;
        UIManager.executePlayAgainButton += Open;
        UIManager.executeHelpButton += FadeInMoveX;
        UIManager.executeCloseInstructions += Open;

        NumberOfLevels = (_gameManager as LevelSystemGameManager).NumberOfLevels;
        GenerateLevelButtons();
    }

    private void Open()
    {
        FadeInMoveX();
        //confirmButton.GetComponent<Button>().interactable = false;
        //DeselectEveryButton();
    }

    public void ActivateConfirmButton()
    {
        confirmButton.GetComponent<Button>().interactable = true;
    }

    private void GenerateLevelButtons()
    {
        for (int i = 0; i < NumberOfLevels; i++)
        {
            var button = Instantiate(levelButtonPrefab, levelButtonsContainer.transform);
            button.gameObject.GetComponent<LevelButton>().Id = i + 1;   
        }
        //DeselectEveryButton();  
    }

    public void DeselectEveryButton()
    {
        foreach (Transform child in levelButtonsContainer.transform)
        {
            child.gameObject.GetComponent<Image>().color = new Color(1F, 1F, 1F, 0.3F);
            child.gameObject.GetComponent<LevelButton>().IsActive = false;
        }
    }

    public void SelectLevel()
    {
        int levelId = ActiveButton.GetComponent<LevelButton>().Id;
        uiManager.SelectLevel(levelId);
    }

}
