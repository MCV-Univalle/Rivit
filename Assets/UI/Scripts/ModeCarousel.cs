using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ModeCarousel : MonoBehaviour
{
    [SerializeField] private bool displayScoreAsTime;
    private int _index = 0;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private float transitionSpeed = 0.2f;
    [SerializeField] private int buttonGap = 210;
    [SerializeField] private ModeSelectionScreen modeSelectionScreen;
    [SerializeField] private GameObject modeButtonPrefab;
    private ArrowsManager _arrowsManager;
    public GameMode CurrentMode { get; set; }
    [Inject] private GameManager _gameManager;

    private void Awake()
    {
        //LoadScores();
    }
    void Start()
    {
        _arrowsManager = GetComponent<ArrowsManager>();
        GenerateButtons();
        _arrowsManager.VerifyLimits(_index, _gameManager.GameModes.Length);
    }
    public void CreateModeButtonInstance(int num)
    {
        GameObject go = Instantiate(modeButtonPrefab);
        go.transform.SetParent(gameObject.transform, false);
        go.transform.localPosition = new Vector3(buttonGap * num, 0, 0);
        go.gameObject.GetComponent<Image>().sprite = _gameManager.GameModes[num].ModeButton.image;
        _gameManager.GameModes[num].ModeID = num;
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
        string highScore = _gameManager.GetCurrentRanking()[_index][0] + "";
        if (highScore == null)
            highScore = 0 + "";
        if (displayScoreAsTime)
            highScore = FormatToTime(int.Parse(highScore));
        CurrentMode = _gameManager.GameModes[_index];
        modeSelectionScreen.UpdateModeDetails(name, description, highScore);
    }

    public string FormatToTime(int score)
    {
        if (score == 0) score = 100000;
        score = 100000 - score;
        float minutes = Mathf.FloorToInt(score / 60);
        float seconds = Mathf.FloorToInt(score % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Translate(float finalPosition)
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), finalPosition, transitionSpeed).setEaseInOutCubic();
    }


}
