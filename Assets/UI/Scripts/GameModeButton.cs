using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]

[CreateAssetMenu(fileName = "Game Mode Button", menuName = "Rivit/GameModeButton")]
public class GameModeButton : ScriptableObject
{
    public Sprite image;
    public string modeName;
    public string description;
}