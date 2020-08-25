using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class Light : MonoBehaviour
    {
        [SerializeField] private GameObject environmentDark;
        [SerializeField] private IndividualLight individualLight;
        [SerializeField] private Color darkColor;
        [SerializeField] private Color brightColor;
        [SerializeField] FrogsManager frogsManager;

        private void Start()
        {
            GameManager.endGame += ToDefault;
        }
        private void OnDestroy()
        {
            GameManager.endGame -= ToDefault;
        }

        public void ToDefault()
        {
            SwitchEnvironmentLight(true);
            individualLight.TurnOff();
        }

        public void SwitchEnvironmentLight(bool value)
        {
            environmentDark.gameObject.SetActive(!value);
            if (value)
                IlluminateFrogs();
            else
                BecomeDarkFrogs();
        }

        public void BecomeDarkFrogs()
        {
            frogsManager.ChangeColorFrogs(darkColor);
        }
        public void IlluminateFrogs()
        {
            frogsManager.ChangeColorFrogs(brightColor);
        }
        public void IlluminateIndividualFrog(int num)
        {
            Vector3 position = frogsManager.FrogsList[num].transform.position;
            individualLight.TurnOn(num, position);
            frogsManager.ChangeColorFrog(num, brightColor);
        }
        public void TurnOffIndividualLight(int num)
        {
            individualLight.TurnOff();
            frogsManager.ChangeColorFrog(num, darkColor);
        }
    }
}