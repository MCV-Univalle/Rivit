using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ModeCarousel : MonoBehaviour
{
    private int _index = 0;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private float transitionSpeed = 0.2f;
    [SerializeField] private int buttonGap = 210;
    [SerializeField] private ModeSelectionScreen modeSelectionScreen;
    [SerializeField] private GameObject modeButtonPrefab;
    private ArrowsManager _arrowsManager;
    public GameMode CurrentMode { get; set; }
    [Inject] private GameManager _gameManager;

    void Start()
    {
        _arrowsManager = GetComponent<ArrowsManager>();
        GenerateButtons();
        LoadScores();
        _arrowsManager.VerifyLimits(_index, _gameManager.GameModes.Length);
    }

    public void LoadScores()
    {
        foreach (var mode in _gameManager.GameModes)
        {
            _gameManager.LoadScoresInMode(mode);
        }
    }

    public void CreateModeButtonInstance(int num)
    {
        GameObject go = Instantiate(modeButtonPrefab);
        go.transform.SetParent(gameObject.transform, false);
        go.transform.localPosition = new Vector3(buttonGap * num, 0, 0);
        go.gameObject.GetComponent<Image>().sprite = _gameManager.GameModes[num].ModeButton.image;
        _gameManager.GameModes[num].ModeID = num;
        //go.GetComponent<Button>().onClick.AddListener(() => SelectGameMode(mode));
    }

    public void GenerateButtons()
    {
        for (int i = 0; i < _gameManager.GameModes.Length; i++)
        {
            CreateModeButtonInstance(i);
        }
        HighligthCurrentButton();
    }

    public void HighligthCurrentButton()
    {
        Transform buttonChilds = gameObject.transform;
        HideEveryButton(buttonChilds);

        buttonChilds.GetChild(_index).gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 1F;
        if (_index > 0)
            buttonChilds.GetChild(_index - 1).gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 0.4F;
        if (_index + 1 < _gameManager.GameModes.Length)
            buttonChilds.GetChild(_index + 1).gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 0.4F;
    }

    public void HideEveryButton(Transform buttonChilds)
    {
        foreach (Transform child in buttonChilds)
        {
            child.gameObject.GetComponent<CanvasGroup>().alpha = 0F;
        }
    }

    public void MoveCarousel(int direction)
    {
        uiManager.PlayAudio("Arrow");
        _index += direction;
        UpdateDetails();
        Translate(buttonGap * _index * -1);
        HighligthCurrentButton();
        _arrowsManager.VerifyLimits(_index, _gameManager.GameModes.Length);
    }

    public void UpdateDetails()
    {
        string name = _gameManager.GameModes[_index].ModeButton.modeName;
        string description = _gameManager.GameModes[_index].ModeButton.description;
        int highScore = _gameManager.GameModes[_index].HighScores[0];
        if (highScore == null)
            highScore = 0;
        CurrentMode = _gameManager.GameModes[_index];
        modeSelectionScreen.UpdateModeDetails(name, description, highScore);
    }

    public void Translate(float finalPosition)
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), finalPosition, transitionSpeed).setEaseInOutCubic();
    }


}
