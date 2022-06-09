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

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                panelGameOver = GetChildWithName(panelUserInterface, "LevelCompletedPanelTuberias");
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

    }

    private void Update() 
    {
            TuberiasCompleted();
    }

    public void DesactiveBoard(bool value)
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

    public void ChangeLevelNameCurrent()
    {
        GameObject lnCurrent = GetChildWithName(panelControlsTuberias, "LevelNameCurrent");
        GameObject tLevelName = GetChildWithName(lnCurrent, "TextLevelName");

        string valueName = levelsHandler.GetComponent<LevelsHandler>().NameLevelCurrent;

        tLevelName.GetComponent<TMPro.TextMeshProUGUI>().text = valueName;
    }

    public void TuberiasCompleted()
    {
            GameObject lvInformation =  GetChildWithName(panelControlsTuberias, "LevelInformation");
    }

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

}


}