using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private readonly string keyString = "UserData";
    private UserData userData;

    public UserData UserData { get => userData; set => userData = value; }

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("UserDataManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        LoadData();
    }

    private void LoadData()
    {
        string jsonString = PlayerPrefs.GetString(keyString);
        if (jsonString != "")
        {
            UserData data = new UserData();
            data = JsonConvert.DeserializeObject<UserData>(jsonString);
            UserData = data;
        }
        else
        {
            Debug.LogWarning("Save data not found!");
            CreateNewDataFile();
        }
    }

    private void CreateNewDataFile()
    {
        UserData = new UserData();
        UserData.PlayerName = "";
        UserData.TopScores = new Dictionary<string, string>();
        UserData.PlaySessionsData = new List<PlaySessionData>();
    }

    public string GetTopScoresOfGame(string gameName)
    {
        string topScores;
        bool keyExists = UserData.TopScores.TryGetValue(gameName, out topScores);
        if(keyExists)
            return UserData.TopScores[gameName];
        else
        {
            var temp = RankingManager.InitializeEmptyRanking();
            UserData.TopScores.Add(gameName, JsonConversor.ConvertRankingToJson(temp));
            return UserData.TopScores[gameName];
        }
    }

    public void UpdateTopScoresOfGame(string gameName, string ranking)
    {
        UserData.TopScores[gameName] = ranking;
        SaveData();
    }

    public void UpdatePlaySessionDates(PlaySessionData data)
    {
        UserData.PlaySessionsData.Add(data);
        SaveData();
    }

    public void SaveClothes(string clothesJson)
    {
        UserData.Clothes = clothesJson;
        SaveData();
    }

    public string LoadClothes()
    {
        return UserData.Clothes;
    }

    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(UserData);
        PlayerPrefs.SetString(keyString, json);
        PlayerPrefs.Save();
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        CreateNewDataFile();
    }
}
