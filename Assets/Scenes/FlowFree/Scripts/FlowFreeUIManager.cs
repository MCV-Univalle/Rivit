using FlowFree;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowFreeUIManager : MonoBehaviour
{
    public static FlowFreeUIManager instance;

    private FlowFreeGameManager gameManager = new FlowFreeGameManager();
    public GameObject levelLabel;
    private TextMeshProUGUI levelTmpLabel;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<FlowFreeGameManager>();
        levelTmpLabel = levelLabel.GetComponent<TextMeshProUGUI>();
    }

    public void NextLevel()
    {

        if (gameManager.LevelIndexList < gameManager.LevelsList.Count - 1)
        {
            print(levelTmpLabel.text);
            gameManager.LevelIndexList += 1;
            gameManager.LoadLevel();
        }
    }

    public void PreviusLevel()
    {
        if (gameManager.LevelIndexList > 0)
        {
            print(levelTmpLabel.text);
            gameManager.LevelIndexList -= 1;
            gameManager.LoadLevel();
        }
    }

    public void SetLevelTmpLabel()
    {
        levelTmpLabel.text = gameManager.levelsList2[gameManager.LevelIndexList].name;
    }
}
