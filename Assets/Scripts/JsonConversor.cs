using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonConversor
{
    public static string ConvertRankingToJson(List<List<int>> ranking)
    {
        HighScoreData data = new HighScoreData();
        data.highScores = ranking;
        return JsonConvert.SerializeObject(data);
    }

    public static  List<List<int>> ConvertJsonToRanking(string json)
    {
        HighScoreData data = new HighScoreData();
        return JsonConvert.DeserializeObject<HighScoreData>(json).highScores;
    }
}
