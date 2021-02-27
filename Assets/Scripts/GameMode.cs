using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class GameMode : MonoBehaviour
{
    protected GameManager _gameManager;
    public int ModeID {get; set;}
    [SerializeField] GameModeButton _modeButton;
    public GameModeButton ModeButton { get => _modeButton; set => _modeButton = value; }
    public abstract void InitializeSettings();
    public abstract void IncreaseDifficulty(int score);

    [Inject]
    public void Constructor(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    
}
