using System.Diagnostics;
using Tuberias;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TuberiasUIManager : MonoBehaviour
{
    public static TuberiasUIManager instance;

    private TuberiasGameManager gameManager = new TuberiasGameManager();
    public GameObject levelLabel;
    private TextMeshProUGUI levelTmpLabel;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<TuberiasGameManager>();
        levelTmpLabel = levelLabel.GetComponent<TextMeshProUGUI>();

    }

    public void NextLevel()
    {

        if (gameManager.LevelIndexList < gameManager.LevelsList.Count - 1)
        {
            print(levelTmpLabel.text);
            gameManager.LevelIndexList += 1;
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

    // Update is called once per frame

}
