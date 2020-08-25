using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class GameMode : MonoBehaviour
{
    public int ModeID {get; set;}
    public List<int> HighScores {get; set;}
    [SerializeField] GameModeButton _modeButton;
    public GameModeButton ModeButton { get => _modeButton; set => _modeButton = value; }
    public abstract void SetVariables();
    public abstract void IncreaseDifficulty(int score);

}
