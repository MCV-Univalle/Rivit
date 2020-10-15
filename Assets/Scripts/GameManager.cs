using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class GameManager : MonoBehaviour
{
    protected int _score;
    public virtual int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            updateScore(_score);
            IncreaseDifficulty();
        }
    }

    public abstract string Name { get; }

    protected RankingManager _rankingManager;
    protected PauseManager _pauseManager;
    [SerializeField] private GameMode[] gameModes;
    public GameMode[] GameModes { get => gameModes; set => gameModes = value; }

    protected GameMode _gameMode;

    protected int _gameModeNumber;

    public delegate void GameDelegate();
    public delegate void ScoreDelegate(int num);
    public static event GameDelegate startGame;
    public static event GameDelegate endGame;
    public static event GameDelegate showResults;
    public static event ScoreDelegate updateScore;


    [Inject]
    public void Constructor(RankingManager rankingManager, PauseManager pauseManager)
    {
        _rankingManager = rankingManager;
        _pauseManager = pauseManager;
    }

    public virtual void IncreaseDifficulty()
    {
        _gameMode.IncreaseDifficulty(_score);
    }

    public void InitializeGame(GameMode mode)
    {
        _gameMode = mode;
        _gameMode.InitializeSettings();
        Score = 0;
        StartGame();
    }

    public void LoadScoresInMode(GameMode mode)
    {
        mode.HighScores = _rankingManager.LoadData(mode.ModeID);
    }

    public int RecordScore()
    {
        int highScorePos = _rankingManager.RecordScore(_gameMode, Score);
        return highScorePos;
    }

    public bool IsRecordEmpty()
    {
        foreach (var mode in gameModes)
        {
            print(mode);
            if(mode.HighScores[0] == 0)
            return true;
        }
        return false;
    }

    public List<int> GetCurrentRanking()
    {
        return _gameMode.HighScores;
    }

    public void NotifyGameOver()
    {
        showResults();
        EndGame();
    }

    public abstract void StartGame();
    public void CallStartGameEvent()
    {
        startGame();
    }

    public abstract void EndGame();
    public void CallEndGameEvent()
    {
        endGame();
    }

    public virtual void RestartGame()
    {
        EndGame();
        Score = 0;
        InitializeGame(this._gameMode);   
        StartGame();
    }

    public void ChangePauseState(float time)
    {
        if (Time.timeScale != 0)
            LeanTween.delayedCall(gameObject, time, () => _pauseManager.PauseGame());
        else
            _pauseManager.ResumeGame();
    }
}
