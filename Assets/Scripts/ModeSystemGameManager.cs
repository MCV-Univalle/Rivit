using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModeSystemGameManager : GameManager
{
    public static event ScoreDelegate updateScore;

    [SerializeField] private GameMode[] gameModes;
    public GameMode[] GameModes { get => gameModes; set => gameModes = value; }

    protected GameMode _gameMode;
    public int CurrentGameMode { get; set; }

    protected int _gameModeNumber;

    public virtual int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            IncreaseDifficulty();
        }
    }

    public void InitializeGame(GameMode mode)
    {
        _gameMode = mode;
        CurrentGameMode = mode.ModeID;
        _gameMode.InitializeSettings();
        RecordStartTimeAndStartGame(mode.ModeName);
        Score = 0;
    }

    public virtual void IncreaseDifficulty()
    {
        _gameMode.IncreaseDifficulty(_score);
    }

    public override void RestartGame()
    {
        EndGame();
        Score = 0;
        InitializeGame(this._gameMode);
        //StartGame();
    }

    public string LoadRankingData()
    {
        return _userDataManager.GetTopScoresOfGame(Name);
    }

    public List<List<int>> GetRankingOfEveryMode()
    {
        return JsonConversor.ConvertJsonToRanking(LoadRankingData());
    }

    public List<int> GetCurrentRanking()
    {
        return JsonConversor.ConvertJsonToRanking(LoadRankingData())[_gameMode.ModeID];
    }

    public int[] GetStandardsOfCurrentMode()
    {
        return _gameMode.ScoreStandards;
    }

    public int AddCoins()
    {
        var standards = GetStandardsOfCurrentMode();
        int coins = 0;
        if (_score > standards[2])
            coins = 25;
        else if (_score > standards[1])
            coins = 10;
        else if (_score > standards[0])
            coins = 3;
        else
            coins = 1;

        Coins = coins;
        return coins;
    }

    public void RecordScore()
    {
        string highScores = RankingManager.RecordScore(LoadRankingData(), _gameMode.ModeID, Score);
        _userDataManager.UpdateTopScoresOfGame(Name, highScores);
    }
}
