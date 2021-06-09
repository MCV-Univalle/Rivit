using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class GameMode : MonoBehaviour
{
    protected GameManager _gameManager;
    public int ModeID {get; set;}
    public string ModeName { get => modeName; set => modeName = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public GameMode PreviousMode { get => previousMode; set => previousMode = value; }
    public int[] ScoreStandards { get => scoreStandards; set => scoreStandards = value; }

    [SerializeField] private Sprite icon;
    [SerializeField] private string modeName;
    [SerializeField] private GameMode previousMode;
    [SerializeField] private int[] scoreStandards = new int[3];
    public abstract void InitializeSettings();
    public abstract void IncreaseDifficulty(int score);

    [Inject]
    public void Constructor(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    
}
