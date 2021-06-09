using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string PlayerName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Clothes { get; set; }
    public Dictionary<string, string> TopScores { get; set; }
    public List<PlaySessionData> PlaySessionsData { get; set; }
}
