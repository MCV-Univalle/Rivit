using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private readonly string keyString = "UserData";
    private UserData userData;

    public UserData PersonalData { get => userData; set => userData = value; }

    public int Coins
    {
        get
        {
            LoadData();
            return PersonalData.Coins;
        }
        set
        {
            PersonalData.Coins += value;
            SaveData();
        }
    }

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
            PersonalData = data;
        }
        else
        {
            Debug.LogWarning("Save data not found!");
            CreateNewDataFile();
        }
    }

    private void CreateNewDataFile()
    {
        PersonalData = new UserData();
        PersonalData.PlayerName = "";
        PersonalData.Clothes = "{ \"hat\":\"Hat0\",\"glasses\":\"Glasses0\",\"accessory\":\"Accessory0\",\"shirt\":\"Shirt0\",\"color\":\"#67CA55\"}";
        PersonalData.PurchasedClothes = new List<string>();
        PersonalData.Coins = 0;
        PersonalData.TopScores = new Dictionary<string, string>();
        PersonalData.PlaySessionsData = new List<PlaySessionData>();
    }

    public string GetTopScoresOfGame(string gameName)
    {
        string topScores;
        bool keyExists = PersonalData.TopScores.TryGetValue(gameName, out topScores);
        if(keyExists)
            return PersonalData.TopScores[gameName];
        else
        {
            var temp = RankingManager.InitializeEmptyRanking();
            PersonalData.TopScores.Add(gameName, JsonConversor.ConvertRankingToJson(temp));
            return PersonalData.TopScores[gameName];
        }
    }

    public void UpdateTopScoresOfGame(string gameName, string ranking)
    {
        PersonalData.TopScores[gameName] = ranking;
        SaveData();
    }

    public void UpdatePlaySessionDates(PlaySessionData data)
    {
        if (PersonalData.PlaySessionsData.Count > 10000)
            PersonalData.PlaySessionsData.RemoveAt(0);
        PersonalData.PlaySessionsData.Add(data);    
        SaveData();
    }

    public void SaveClothes(string clothesJson)
    {
        PersonalData.Clothes = clothesJson;
        SaveData();
    }

    public string LoadClothes()
    {
        LoadData();
        return PersonalData.Clothes;
    }

    public void AddPurchasedClothes(string clothesName)
    {
        PersonalData.PurchasedClothes.Add(clothesName);
        SaveData();
    }

    public List<string> LoadPurchasedClothes()
    {
        LoadData();
        return PersonalData.PurchasedClothes;
    }

    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(PersonalData);
        PlayerPrefs.SetString(keyString, json);
        PlayerPrefs.Save();
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        CreateNewDataFile();
    }
}
