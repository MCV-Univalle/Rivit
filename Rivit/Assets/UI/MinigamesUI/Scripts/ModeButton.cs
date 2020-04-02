using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class ModeButton : MonoBehaviour
    {
        public void SelectGameMode()
        {
            UIManager.Instance.SelectGameMode();
        }
    }   
}
