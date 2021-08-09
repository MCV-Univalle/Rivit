using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public delegate void GameDelegate();
public delegate void ScoreDelegate(int num);

public abstract class GameManager : MonoBehaviour
{
    protected int _score;
    
    public abstract string Name { get; }
    public bool IsPaused { get; set; }

    protected PlaySessionDataHandler _dataHandler;
    protected UserDataManager _userDataManager;
    

    public static event GameDelegate startGame;
    public static event GameDelegate endGame;
    public static event GameDelegate showResults;

    public int Coins
    {
        get => _userDataManager.Coins;
        set => _userDataManager.Coins = value;
    }
    


    [Inject]
    public void Constructor(PlaySessionDataHandler playerSessionDataHandler, UserDataManager userDataManager)
    {
        _dataHandler = playerSessionDataHandler;
        _userDataManager = userDataManager;
    }

    public virtual string RegisterAdditionalData()
    {
        return "";
    }

    public virtual void NotifyGameOver()
    {
        showResults();
        EndGame();
        var data = _dataHandler.RegisterSessionEnd();
        data.Score = _score;
        data.GameName = Name;
        data.AdditionalData = RegisterAdditionalData();
        _userDataManager.UpdatePlaySessionDates(data);
    }

    public void RecordStartTimeAndStartGame(string levelOrMode)
    {
        _dataHandler.RegisterSessionStar(Name, levelOrMode);
        StartGame();
    }

    public abstract void StartGame();
    public void RaiseStartGameEvent()
    {
        startGame?.Invoke();
    }

    public abstract void EndGame();
    public void RaiseEndGameEvent()
    {
        endGame?.Invoke();
    }

    public abstract void RestartGame();

    public void ChangePauseState(float time)
    {
        if (Time.timeScale != 0)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        IsPaused = false;

    }
}
