using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;

public class ModeSelectionScreen : UIComponent
{
    [SerializeField]  private UIManager uiManager;
    //[SerializeField]  private ModeCarousel modeCarousel; 
    [SerializeField] private TextMeshProUGUI modeNameText;
    [SerializeField] private TextMeshProUGUI gameModeTopScoreText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Inject] private GameManager _gameManager;

    [SerializeField] private GameObject layout;
    [SerializeField] private GameObject modeButtonPrefab;


    void Start()
    {
        var pos = this.transform.position;
        positionY = pos.y;
        positionX = pos.x;
        //UIManager.executePlayButton += OpenModeSelectionScreen;
        UIManager.executePlayAgainButton += ReloadGameModesList;
        UIManager.executeHelpButton += FadeOutMoveX;
        UIManager.executeStartGame += () => this.gameObject.SetActive(false);
        UIManager.executeCloseInstructions += FadeInMoveX;
        UIManager.executeQuitGame += FadeInMoveX;

        GenerateButtons();
    }

    private void GenerateButtons()
    {
        int index = 0;
        int previousBestScore = 0;
        foreach (var item in (_gameManager as ModeSystemGameManager).GameModes)
        {
            GameObject go = Instantiate(modeButtonPrefab);
            go.transform.SetParent(layout.transform, false);
            go.transform.GetChild(0).GetComponent<Image>().sprite = item.Icon;
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.ModeName;
            if (item.PreviousMode != null)
            {
                if (previousBestScore < item.PreviousMode.ScoreStandards[1])
                {
                    go.GetComponent<Button>().interactable = false;
                    go.transform.GetChild(4).gameObject.SetActive(true);
                }
            }
            previousBestScore = (_gameManager as ModeSystemGameManager).GetRankingOfEveryMode()[index][0];
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = previousBestScore + "";
            go.GetComponent<ModeButton>().Mode = item;
            index++;
        }
    }

    private void DestroyButtons()
    {
        foreach (Transform item in layout.transform)
        {
            Destroy(item.gameObject);
        }
    }

    private void ReloadGameModesList()
    {
        DestroyButtons();
        GenerateButtons();
        FadeInMoveX();
    }

    void OnDestroy()
    {
        //UIManager.executePlayButton -= OpenModeSelectionScreen;
        UIManager.executePlayAgainButton -= ReloadGameModesList;
        UIManager.executeHelpButton -= FadeOutMoveX;
        UIManager.executeStartGame -= () => this.gameObject.SetActive(false);
        UIManager.executeCloseInstructions -= FadeInMoveX;
        UIManager.executeQuitGame -= FadeInMoveX;
    }
}
