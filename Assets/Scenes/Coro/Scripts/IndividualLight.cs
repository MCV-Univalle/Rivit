using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class IndividualLight : MonoBehaviour
    {
        [SerializeField] List<Color> colorList;

        public void TurnOn(int num, Vector3 position)
        {
            position.y += 2.4F; 
            gameObject.transform.position = position;
            GetComponent<Renderer>().material.color = colorList[num];
            GetComponent<Animator>().SetBool("on", true);
        }

        public void TurnOff()
        {
            GetComponent<Animator>().SetBool("on", false);
        }        

    }
}