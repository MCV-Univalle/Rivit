using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class RankingManager
{
    private int _rankedScoresNumber = 5;
    private int _modesNumber;

    public int RecordScore(int mode, int score)
    {
        int posInRanking = -1;
        for (int i = 0; i < _rankedScoresNumber; i++)
        {
            if(score > Ranking[mode][i])
            {
                posInRanking = i;
                Ranking[mode].Insert(i, score);
                Ranking[mode].RemoveAt(_rankedScoresNumber);
                break;
            }
        }
        SaveData();
        return posInRanking;
    }
    public string GameName { get; set; }
    public List<List<int>> Ranking { get; set; }

    public RankingManager(string name, int number)
    {
        GameName = name;
        this._modesNumber = number;
        LoadData();
    }

    public void LoadData()
    {
        string jsonString = PlayerPrefs.GetString(GameName);
        if (jsonString != "")
        {
            HighScoreData data = new HighScoreData();
            data = JsonConvert.DeserializeObject<HighScoreData>(jsonString);
            Ranking =  data.highScores;
        }
        else
        {
            Debug.LogWarning("Save data not found!");
            Ranking = InitializeEmptyRanking();
        }
            
    }

    public List<List<int>> InitializeEmptyRanking()
    {
        var newRanking = new List<List<int>>();
        for (int i = 0; i < _modesNumber; i++)
        {
            var temp = new List<int>();
            newRanking.Add(temp);
            for (int j = 0; j < _rankedScoresNumber; j++)
            {
                newRanking[i].Add(0);
            }
        }
        return newRanking;
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteKey(GameName);
    }

    public void SaveData()
    {
        HighScoreData data = new HighScoreData();
        data.highScores = Ranking;
        string json = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString(GameName, json);
        PlayerPrefs.Save();
    }
}