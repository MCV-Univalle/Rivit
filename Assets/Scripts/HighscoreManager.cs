using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager
{
    private static HighScoreManager _instance;
    public static HighScoreManager Instance
    {
        get
        {
            //Logic to create the instance
            if(_instance == null)
            {
                _instance = new HighScoreManager();
            }
            return _instance;
        }
    }
    private int _modesNum;
    public List<int> ScoresTable {get; set;}  
    public int NewHighScore {get; set;} 
    private string _gameName;
    public string GameName {get {return _gameName;} set {_gameName = value;}}

    void Awake()
    {
        _instance = this;
    }

    public void CompareScores(int num, int gameMode, int score, bool val)
    {
        bool isNewHighScore = val;
        int newScore = score;
        if((num == 5) && (!isNewHighScore))
        {
            NewHighScore = -1; //There is no new high score
        }
        if(num < 5)
        {
            if(score > ScoresTable[(gameMode * 5) +  num])
            {
                newScore = ScoresTable[(gameMode * 5) +  num];
                ScoresTable[(gameMode * 5) +  num] = score;
                SaveData();
                if(!isNewHighScore)
                {
                    NewHighScore = (gameMode * 5) + num;
                    isNewHighScore = true;
                }
            }
            CompareScores(num + 1, gameMode, newScore, isNewHighScore);
        }
    }

    public void InitializeTables(int modesNum)
    {
        _modesNum = modesNum;
        ScoresTable = new List<int>();
        for(int i = 0; i < _modesNum * 5; i++)
        {
            ScoresTable.Add(0);
        }

        LoadData();
    }

    public void SaveData()
    {
        HighScoresTable data = new HighScoresTable();
        data.highScores = ScoresTable;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(_gameName, json);
        PlayerPrefs.Save();
    }
    
    public void LoadData()
    {
        string jsonString = PlayerPrefs.GetString(_gameName);
        if(jsonString != "")
        {
            HighScoresTable data = new HighScoresTable();
            data = JsonUtility.FromJson<HighScoresTable>(jsonString);
            Debug.Log(jsonString);
            List<int> dataCopy = CopySaveData(data.highScores);
            ScoresTable = dataCopy;
        }
        else Debug.Log("Save data not found!");
    }

    public List<int> CopySaveData(List<int> list)
    {
        List<int> copy = new List<int>();
        for(int i = 0; i < _modesNum * 5; i++)
        {
            copy.Add(list[i]);
        }
        return copy;
    }

}   