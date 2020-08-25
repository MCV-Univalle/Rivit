using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class LifeDisplayer : MonoBehaviour
    {
        public int Lifes {get; set;}
        [SerializeField] GameObject lifeDisplayer;
        private void Start()
        {
            GameManager.endGame += Desactive;
        }
        private void OnDestroy()
        {
            GameManager.endGame -= Desactive;
        }

        public void ActiveHearths()
        {
            lifeDisplayer.gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                lifeDisplayer.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void ReduceLife()
        {
            lifeDisplayer.transform.GetChild(Lifes - 1).gameObject.SetActive(false);
            Lifes--;
        }

        public void Desactive()
        {
            lifeDisplayer.gameObject.SetActive(false);
        }
    }
}
