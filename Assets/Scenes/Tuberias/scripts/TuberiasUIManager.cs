using Tuberias;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Tuberias
{

public class TuberiasUIManager : MonoBehaviour
{
    public static TuberiasUIManager instance;

    [SerializeField] private GameObject panelUserInterface;
    [SerializeField] private LevelsHandler levelsHandler;
    [SerializeField] private GameObject panelBoard;
    private GameObject panelGameOver;
    private GameObject panelControlsTuberias;

    private TuberiasGameManager gameManager = new TuberiasGameManager();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                panelGameOver = GetChildWithName(panelUserInterface, "GameOverPanelTuberias");
                panelControlsTuberias = GetChildWithName(panelUserInterface, "ControlsTuberias");
            }
            else
            {
                Destroy(this);
            }
        }

    // Start is called before the first frame update
    void Start()
    {
       // gameManager = GameObject.Find("GameManager").GetComponent<TuberiasGameManager>();
        //levelTmpLabel = levelLabel.GetComponent<TextMeshProUGUI>();

    }

    private void Update() {
        
    }

    public void EraseBoard(bool value)
    {
        panelBoard.SetActive(value);
    }

    public void ActivePanelGameOver(bool value)
        {
            panelGameOver.SetActive(value);
            panelControlsTuberias.SetActive(!value);
        }

    public void ActivePanelControlsTuberias(bool value)
    {
        panelControlsTuberias.SetActive(value);
    }
/*    public void NextLevel()
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
*/
    public void ChangeLevelNameCurrent()
    {
        GameObject lnCurrent = GetChildWithName(panelControlsTuberias, "LevelNameCurrent");
        GameObject tLevelName = GetChildWithName(lnCurrent, "TextLevelName");

        string valueName = levelsHandler.GetComponent<LevelsHandler>().NameLevelCurrent;

        tLevelName.GetComponent<TMPro.TextMeshProUGUI>().text = valueName;
    }

    /*public void SetLevelTmpLabel()
    {
        levelTmpLabel.text = gameManager.levelsList2[gameManager.LevelIndexList].name;
    }*/

    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }


    // Update is called once per frame

}


}