using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class LevelSystemGameManager : GameManager
{
    [SerializeField] private LevelsHandler levelsHandler;

    public int NumberOfLevels { get => levelsHandler.NumberOfLevels; }
    public void InitializeGame(int level)
    {
        levelsHandler.GenerateLevel(level);
        RecordStartTimeAndStartGame("Nivel" + level);
    }

    public override void RestartGame()
    {
        EndGame();
        InitializeGame(levelsHandler.CurrentLevel);
        StartGame();
    }

    public void ToNextLevel()
    {
        levelsHandler.ToNextLevel();
    }

    public void ToPreviousLevel()
    {
        levelsHandler.ToPreviousLevel();
    }
}
