using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


[System.Serializable]
public struct PlaySessionData
{
    public string GameName;
    public string GameMode;
    public int Score;
    public DateTime StartTime;
    public DateTime EndTime;
    public TimeSpan TotalPlayTime;
    public string AdditionalData;
}
public class PlaySessionDataHandler
{
    private PlaySessionData currentData;

    public void RegisterSessionStar(string gameName, string levelOrMode)
    {
        currentData = new PlaySessionData();
        currentData.GameMode = levelOrMode;
        currentData.StartTime = DateTime.Now;
    }

    public PlaySessionData RegisterSessionEnd()
    {
        currentData.EndTime = DateTime.Now;
        currentData.TotalPlayTime = DateTime.Now - currentData.StartTime;
        return currentData;
    }
}
