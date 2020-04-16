using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public abstract class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _sceneSwitcher;
    [SerializeField]
    protected GameObject _scorePrefab;
    [SerializeField]
    protected GameModeCarousel _gameModeCarousel;
    protected GameController _gameManager;
    [SerializeField]
    protected GameObject _whiteScreen;
    [SerializeField]
    protected GameObject _blackPanel;
    [SerializeField]
    protected GameObject _pausePanel;
    [SerializeField]
    protected TextMeshProUGUI _scoreText;
    public TextMeshProUGUI Score { get {return _scoreText;} set {_scoreText = value;} }
    [SerializeField]
    protected TextMeshProUGUI _finalScore;
    [SerializeField]
    protected GameObject _topElements;
    [SerializeField]
    protected GameObject _mainScreen;
    [SerializeField]
    protected GameObject _blurSuperfice;
    [SerializeField]
    protected GameObject _gameModeScreen;
    [SerializeField]
    protected GameObject _instructionsScreen;
    [SerializeField]
    protected GameObject _resultsScreen;
    [SerializeField]
    protected GameObject _scorePanel;
    [SerializeField]
    protected GameObject _rankingPanel;
    [SerializeField]
    protected GameObject _rankingContainer;
    [SerializeField]
    protected GameObject _separator;
    [SerializeField]
    protected TextMeshProUGUI _modeNameText;
    public TextMeshProUGUI ModeName {get {return _modeNameText;}}
    [SerializeField]
    protected TextMeshProUGUI _gameModeTopScore;
    [SerializeField]
    protected TextMeshProUGUI _newRecordText;
    [SerializeField]
    protected TextMeshProUGUI _descriptionText;
    public TextMeshProUGUI Description {get {return _descriptionText;}}
    public int NumberOfModes {get { return _gameModeCarousel.NumberOfModes;}}
    [SerializeField]
    private float timeToStartFading = 0.07F;


    //void Awake()

    public void Start()
    {
        _whiteScreen.gameObject.SetActive(true);
        StartCoroutine(FadeElement(_whiteScreen, false));
        DisplayGameModes();
    }

    public IEnumerator FadeElement(GameObject go, bool value)
    {
        yield return new WaitForSeconds(timeToStartFading);
        if(value) 
        go.SetActive(true);
        else if((value == false) && (go.active == true))
        {
            go.gameObject.GetComponent<UIFader>().FadeOut(0, false);
        }
    }
    public IEnumerator DisplayDesactiveAnimation(GameObject go)
    {
        yield return new WaitForSeconds(timeToStartFading);
        go.gameObject.GetComponent<Animator>().SetTrigger("desactive");
    }

    public IEnumerator ShowBlurSuperfice(bool value)
    {
        yield return new WaitForSeconds(timeToStartFading);
        _blurSuperfice.SetActive(value);
    }

    public void DisplayGameModes()
    {
        for(int i = 0; i < _gameModeCarousel.NumberOfModes; i++)
        {
            GameObject go = _gameModeCarousel.CreateGameModeButton(i);
            go.GetComponent<Button>().onClick.AddListener(() => SelectGameMode());
        }
        _gameModeCarousel.VerifyLimits();
    }

    public void MoveCarousel(int direction)
    {
        _gameModeCarousel.MoveCarousel(direction, this);
        UIAudio.Instance.PlayCarouselTransitionClip();   
    }

    public void UpdateGameModeInformation()
    {
        GameRules currentMode = _gameModeCarousel.GetCurrentGameMode();
        _modeNameText.text = currentMode.modeName;
        _descriptionText.text = currentMode.description;
        UpdateTopScore(_gameModeCarousel.Index);
    }

    public void UpdateTopScore(int index)
    {
        int topScore = _gameManager.HighScoreManager.ScoresTable[index * 5];
        _gameModeTopScore.text = "" + topScore;
    }

    public void SelectGameMode()
    {
        bool value = false;
        StartCoroutine(FadeElement(_blackPanel, value));
        StartCoroutine(DisplayDesactiveAnimation(_blackPanel));
        StartCoroutine(FadeElement(_gameModeScreen, value));
        StartCoroutine(ShowBlurSuperfice(value));
        StartCoroutine(FadeElement(_topElements, !value));

        int modeNum = _gameModeCarousel.Index;
        _gameManager.GameMode = modeNum;
        _gameManager.AdaptGameParameters();
        _gameManager.StartGame();
        UIAudio.Instance.PlayConfirmationClip();
    }

    public virtual IEnumerator ShowResultsPanel()
    {
        yield return new WaitForSeconds(0.9F);
        int gameMode = _gameManager.GameMode;
        List<int> scores = _gameManager.HighScoreManager.ScoresTable;
        bool value = true;
        StartCoroutine(FadeElement(_blackPanel, value));
        StartCoroutine(FadeElement(_resultsScreen, value));
        StartCoroutine(ShowBlurSuperfice(value));
        _gameModeScreen.gameObject.SetActive(!value);
        _rankingPanel.gameObject.SetActive(!value);
        StartCoroutine(FadeElement(_topElements, !value));
        _newRecordText.gameObject.SetActive(false);
        StartCoroutine(IncrementFinalScore(_gameManager));
        DestroyRanking();
        GenerateRanking(scores, gameMode);
    }

    public void DestroyRanking()
    {
        Transform children = _rankingContainer.transform;
        foreach(Transform child in children)
            {
                GameObject choice = child.gameObject;
                Destroy(choice);
            }
    }

    public void GenerateRanking(List<int> scores, int gameMode)
    {
        int newHighScore = VerifyNewHighScore();
        for(int i = (gameMode * 5); i < (gameMode + 1) * 5; i++)
        {
            GameObject go = Instantiate(_scorePrefab);
            go.transform.SetParent(_rankingContainer.transform, false);
            go.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = "" + ((i + 1) - (gameMode * 5)) + ".";
            go.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "" + scores[i];
            if(i == newHighScore)
            {
                go.transform.Find("Number").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 0, 255); //Yellow
                go.transform.Find("Score").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 0, 255);
            }
            go.transform.localPosition = new Vector3(0, 80 * i, 0);
            if(i != 4)
            {
                go = Instantiate(_separator);
                go.transform.SetParent(_rankingContainer.transform, false);
                go.transform.localPosition = new Vector3(0, (80 * i) + 20, 0);
            }
        }
    }

    public virtual int VerifyNewHighScore()
    {
        return _gameManager.HighScoreManager.NewHighScore;
    }

    public virtual IEnumerator IncrementFinalScore(GameController gc) //The score displayed in the results panel
    {
        int newHighScore = VerifyNewHighScore();
        StartCoroutine(FadeElement(_scorePanel, true));
        yield return new WaitForSeconds(0.35F);
        for(int i = 0; i <= gc.Score; i++)
        {
            _finalScore.text = "" + i;
            UIAudio.Instance.PlayCountingClip();
            yield return new WaitForSeconds(0.025F);
        }
        yield return new WaitForSeconds(0.25F);
        if(newHighScore != -1) _newRecordText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2F);
        StartCoroutine(FadeElement(_scorePanel, false));
        StartCoroutine(DisplayDesactiveAnimation(_scorePanel));
        yield return new WaitForSeconds(0.1F);
        StartCoroutine(FadeElement(_rankingPanel, true));
    }

    public virtual void RestartGame()
    {
        _gameManager.FinishGame();
        _gameManager.RestartGame = true;
        _gameManager.StartGame();
    }

    public virtual void ExecutePauseButton()
    {
        bool value = true;
        StartCoroutine(FadeElement(_pausePanel, value));
        StartCoroutine(FadeElement(_topElements, !value));
        _gameManager.IsPaused = true;
        UIAudio.Instance.PlayPauseClip();  
    }

    public virtual void ExecuteHelpButton()
    {
        bool value = true;
        //_instructionsScreen.gameObject.SetActive(value);
        _gameModeScreen.gameObject.SetActive(!value);
        StartCoroutine(ShowBlurSuperfice(value));
        StartCoroutine(FadeElement(_blackPanel, value));
        StartCoroutine(FadeElement(_instructionsScreen, value));
        StartCoroutine(FadeElement(_mainScreen, !value));
        UIAudio.Instance.PlayConfirmationClip();
    }


    public virtual void ExecuteCloseButton()
    {
        bool value = false;
        StartCoroutine(ShowBlurSuperfice(value));
        if(_gameModeScreen.gameObject.active) StartCoroutine(FadeElement(_gameModeScreen, value));
        if(_instructionsScreen.gameObject.active) StartCoroutine(FadeElement(_instructionsScreen, value));
        StartCoroutine(FadeElement(_blackPanel, value));
        StartCoroutine(FadeElement(_mainScreen, !value));
        StartCoroutine(DisplayDesactiveAnimation(_blackPanel));
        UIAudio.Instance.PlayCancelClip();
    }

    public virtual void ExecutePlayButton()
    {
        bool value = true;
        _instructionsScreen.gameObject.SetActive(!value);
        UpdateGameModeInformation();
        _gameModeCarousel.ChangeDefaultAlpha();
        StartCoroutine(ShowBlurSuperfice(value));
        StartCoroutine(FadeElement(_blackPanel, value));
        StartCoroutine(FadeElement(_gameModeScreen, value));
        StartCoroutine(FadeElement(_mainScreen, !value));
        UIAudio.Instance.PlayConfirmationClip();
    }

    public virtual void ExecuteResumeButton()
    {
        bool value = false;
        StartCoroutine(FadeElement(_pausePanel, value));
        StartCoroutine(DisplayDesactiveAnimation(_pausePanel));
        StartCoroutine(FadeElement(_topElements, !value));
        _gameManager.IsPaused = false;
        UIAudio.Instance.PlayConfirmationClip();
    }

    public virtual void ExecuteQuitButton()
    {
        bool value = false;     
        _gameManager.IsPaused = value; 
        _gameManager.FinishGame();
        StartCoroutine(FadeElement(_pausePanel, value));
        StartCoroutine(DisplayDesactiveAnimation(_pausePanel));
        StartCoroutine(FadeElement(_mainScreen, !value));
        UIAudio.Instance.PlayConfirmationClip();
    }

    public virtual void ExecuteRestartButton()
    {
        bool value = false;
        /*_gameManager.IsPaused = value;
        _gameManager.PauseGame(value);*/
        StartCoroutine(FadeElement(_pausePanel, value));
        StartCoroutine(FadeElement(_topElements, !value));
        StartCoroutine(DisplayDesactiveAnimation(_pausePanel));
        RestartGame();
        _gameManager.IsPaused = false;
        UIAudio.Instance.PlayConfirmationClip();
    }

    public virtual void ExecutePlayAgainButton()
    {
        _gameModeCarousel.ChangeDefaultAlpha();
        UpdateGameModeInformation();
        StartCoroutine(FadeElement(_resultsScreen, false));
        StartCoroutine(DisplayDesactiveAnimation(_resultsScreen));
        _gameModeScreen.gameObject.SetActive(true);
        _gameModeScreen.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        _gameModeScreen.gameObject.GetComponent<UIFader>().FadeIn(1, true);
        _gameModeScreen.GetComponent<Animator>().SetTrigger("slideRight");
        UIAudio.Instance.PlayConfirmationClip();
    }

    public virtual void ReturnToHomeScreen()
    {
        UIAudio.Instance.PlayCancelClip();
        _whiteScreen.gameObject.SetActive(true);
        _whiteScreen.gameObject.GetComponent<UIFader>().FadeOut(1, true);
        StartCoroutine(_sceneSwitcher.GetComponent<SceneSwitcher>().GoToHomeScreen());
    }
    
} 
