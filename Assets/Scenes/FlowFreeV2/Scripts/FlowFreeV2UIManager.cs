using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{
    public class FlowFreeV2UIManager : MonoBehaviour
    {
        public static FlowFreeV2UIManager _instance;

        [SerializeField] private GameObject panelUserInterface;
        private GameObject panelGameOver;
        private GameObject panelControlsFlow;
        [SerializeField] private LevelsHandler levelsHandler;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
                panelGameOver = GetChildWithName(panelUserInterface, "GameOverPanelFlow");
                panelControlsFlow = GetChildWithName(panelUserInterface, "ControlsFlow");
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            
        }

        public void ActivePanelGameOver(bool value)
        {
            panelGameOver.SetActive(value);
            panelControlsFlow.SetActive(!value);
        }

        public void ActivePanelControlsFlow(bool value)
        {
            panelControlsFlow.SetActive(value);
        }

        public void ChangeLevelNameCurrent()
        {
            GameObject lnCurrent = GetChildWithName(panelControlsFlow, "LevelNameCurrent");
            GameObject tLevelName = GetChildWithName(lnCurrent, "TextLevelName");

            string valueName = levelsHandler.GetComponent<LevelsHandler>().NameLevelCurrent;

            tLevelName.GetComponent<TMPro.TextMeshProUGUI>().text = valueName;
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

