using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]

[CreateAssetMenu(fileName = "New Game Mode", menuName = "ScriptableObjects/GameRules")]
public class GameRules : ScriptableObject
{
    public Sprite image;
    public string modeName;
    public string description;
}
