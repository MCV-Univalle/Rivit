using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsHandler : MonoBehaviour
{
    [SerializeField] private LevelSelectionScreen levelSelectionScreen;
    [SerializeField] private GameObject levelGenerator;
    [SerializeField] private List<TextAsset> levelsList;
    private int levelIndex = 0;


    public int NumberOfLevels { get => levelsList.Count; }
    public int CurrentLevel { get => levelIndex; set => levelIndex = value; }
    public string NameLevelCurrent = "";

    public void GenerateLevel(int level)
    {
        CurrentLevel = level;
        SetLevelCurrentName(CurrentLevel);
        var levelData = ReadLevelTxt.ReadTxt(levelsList[CurrentLevel-1]);
        levelGenerator.GetComponent<ILevelGenerator>().GenerateLevel(levelData);
    }

    public void ToNextLevel()
    {
        if (CurrentLevel < NumberOfLevels)
        {
            CurrentLevel++;
            GenerateLevel(CurrentLevel);
        }
    }

    public void ToPreviousLevel()
    {
        if (CurrentLevel - 1 > 0)
        {
            CurrentLevel--;
            GenerateLevel(CurrentLevel);
        }
    }

    public void ReloadLevel()
    {
        GenerateLevel(CurrentLevel);
    }

    public void SetLevelCurrentName(int index)
    {
        NameLevelCurrent = levelsList[index - 1].name;
    }

}
