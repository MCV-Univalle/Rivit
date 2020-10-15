using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager
{
    private int _recordedScores = 5;
    public string GameName {get; set;}

    public RankingManager(string name)
    {
        GameName = name;
    }

    public int RecordScore(GameMode mode, int score)
    {
        int posInRanking = -1;
        for (int i = 0; i < _recordedScores; i++)
        {
            if(score > mode.HighScores[i])
            {
                posInRanking = i;
                mode.HighScores.Insert(i, score);
                mode.HighScores.RemoveAt(_recordedScores);
                SaveData(mode.HighScores, mode.ModeID);
                break;
            }
        }
        return posInRanking;
    }

    public List<int> InitializeEmptyRanking()
    {
        List<int> ranking = new List<int>();
        for (int i = 0; i < _recordedScores; i++)
        {
            ranking.Add(0);
        }
        return ranking;
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteKey(GameName);
    }

    public List<int> LoadData(int gameMode)
    {
        string name = GameName + gameMode;
        string jsonString = PlayerPrefs.GetString(name);
        if(jsonString != "")
        {
            HighScoreData data = new HighScoreData();
            data = JsonUtility.FromJson<HighScoreData>(jsonString);
            return data.highScores;
        }
        else
        {
            Debug.LogWarning("Save data not found!");
            return InitializeEmptyRanking();
        }
    }

    public void SaveData(List<int> ranking, int gameMode)
    {
        HighScoreData data = new HighScoreData();
        data.highScores = ranking;
        string json = JsonUtility.ToJson(data);
        string name = GameName + gameMode;
        PlayerPrefs.SetString(name, json);
        PlayerPrefs.Save();
    }
}