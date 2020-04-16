using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController : MonoBehaviour
{
    protected HighScoreManager _highScoreManager = new HighScoreManager();
    public HighScoreManager HighScoreManager
    {
        get {return _highScoreManager;}
    }
    public UIController _uiManager; // = UIManager.Instance
    [SerializeField]
    protected string _gameName;
    public int GameMode { get; set; }
    public bool IsGameStarted { get; set; }
    public bool Fail { get; set; }
    protected bool _isPaused;
    public bool IsPaused 
    { 
        get {return _isPaused;} 
        set
        {
            _isPaused = value;
            if(_isPaused) StartCoroutine(PauseGame(true));
            else StartCoroutine(PauseGame(false));
        }
    }
    protected int _score;
    public virtual int Score 
    { 
        get{return _score;} 
        set
        {
            _score = value;
            _uiManager.Score.text = "" + _score;
        } 
    }
    public bool RestartGame { get; set; }


    protected virtual void Start()
    {
        //_uiManager = UIManager.Instance;
        _highScoreManager.GameName = _gameName;
        RestartGame = false;
        IsGameStarted = false;
        Fail = false;
        InitializeHighScoreTables();
    }

    public void InitializeHighScoreTables()
    {
        _highScoreManager.InitializeTables(_uiManager.NumberOfModes);
    }

    public abstract void StartGame();

    public abstract void FinishGame();

    public abstract void AdaptGameParameters(); //Adjus the game parametes according to the game mode

    public virtual void ShowResults()
    {
        HighScoreManager.CompareScores(0, GameMode, _score, false);
        StartCoroutine(_uiManager.ShowResultsPanel());
    }

    public virtual IEnumerator PauseGame(bool value)
    {
        if(value)
        {
            yield return new WaitForSeconds(0.3F);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
