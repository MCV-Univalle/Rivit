using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{
    public class FlowFreeV2UIManager : MonoBehaviour
    {
        public static FlowFreeV2UIManager _instance;

        [SerializeField] private GameObject panelUserInterface;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
        public void ActivePanelGameOver(bool value)
        {
            GameObject panelGameOver = GetChildWithName(panelUserInterface , "GameOverPanel");
            GameObject panelControlsFlow = GetChildWithName(panelUserInterface, "ControlsFlow");
            panelGameOver.SetActive(value);
            panelControlsFlow.SetActive(!value);
        }

        GameObject GetChildWithName(GameObject obj, string name)
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

