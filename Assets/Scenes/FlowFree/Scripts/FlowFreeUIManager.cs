using FlowFree;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowFreeUIManager : MonoBehaviour
{
    public static FlowFreeUIManager instance;

    //private FlowFreeGameManager gameManager = new FlowFreeGameManager();
    private GenerateLevel generateLevel = new GenerateLevel();

    public GameObject levelLabel;
    private TextMeshProUGUI levelTmpLabel;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        generateLevel = GameObject.Find("GenerateLevel").GetComponent<GenerateLevel>();
        levelTmpLabel = levelLabel.GetComponent<TextMeshProUGUI>();
    }

    public void NextLevel()
    {

        if (generateLevel.LevelIndexList < generateLevel.LevelsList.Count - 1)
        {
            print(levelTmpLabel.text);
            generateLevel.LevelIndexList += 1;
            //generateLevel.LoadLevel();
        }
    }

    public void PreviusLevel()
    {
        if (generateLevel.LevelIndexList > 0)
        {
            print(levelTmpLabel.text);
            generateLevel.LevelIndexList -= 1;
            //generateLevel.LoadLevel();
        }
    }

    public void SetLevelTmpLabel()
    {
        levelTmpLabel.text = generateLevel.levelsList2[generateLevel.LevelIndexList].name;
    }
}
