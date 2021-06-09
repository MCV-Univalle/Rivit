using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


[System.Serializable]
public struct PlaySessionData
{
    public string GameMode;
    public DateTime StartTime;
    public DateTime EndTime;
    public TimeSpan TotalPlayTime;
}
public class PlaySessionDataHandler
{
    private PlaySessionData currentData;

    public void RegisterStartTime(string levelOrMode)
    {
        currentData = new PlaySessionData();
        currentData.GameMode = levelOrMode;
        currentData.StartTime = DateTime.Now;
    }

    public PlaySessionData RegisterEndTime()
    {
        currentData.EndTime = DateTime.Now;
        currentData.TotalPlayTime = DateTime.Now - currentData.StartTime;
        return currentData;
        //string jsonString = PlayerPrefs.GetString("UserStatics");
        //List<PlaySessionData> dataList;
        //if (jsonString != "")
        //    dataList = JsonConvert.DeserializeObject<List<PlaySessionData>>(jsonString);
        //else
        //    dataList = new List<PlaySessionData>();
        //dataList.Add(currentData);
        //string json = JsonConvert.SerializeObject(dataList);
        //PlayerPrefs.SetString("UserStatics", json);
        //PlayerPrefs.Save();
    }
}
