using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saltos
{
    public class AnimationManager : MonoBehaviour
    {
        private static AnimationManager _instance;
        public static AnimationManager Instance
        {
            get
            {
                //Logic to create the instance
                if (_instance == null)
                {
                    _instance = new AnimationManager();
                }
                return _instance;
            }
        }

        private Animator _playerObjectTouchAndGo;
        private Animator _comLibelulaAnim;
        private int direcionSaltoPlayer = 0;

        void Awake()
        {
            _instance = this;
        }


        public void ComVueloTriger(bool isMovingLibelula, GameObject libelula)
        {

            _comLibelulaAnim = libelula.GetComponent<Animator>();
            _comLibelulaAnim.SetBool("estVuelo", libelula.GetComponent<TouchAndGo>().isMoving);

            //Debug.Log("entro en la enimacion: libelula " + TouchAndGo.Instance.player.name);
        }

        public void PalyerSaltroTriger(bool isMovingFrog, GameObject player)
        {
            _playerObjectTouchAndGo = player.GetComponent<Animator>();

            //Debug.Log("direccion player = " + direcionSaltoPlayer);
            _playerObjectTouchAndGo.SetInteger("NumeroSalto", direcionSaltoPlayer);

            _playerObjectTouchAndGo.SetBool("isMovingFrog", player.GetComponent<TouchAndGo>().isMoving);
            //Debug.Log("entro en la enimacion: rana" + TouchAndGo.Instance.player.name);
        }

        public void SetDireccionSaltoPlayer(int valor)
        {
            direcionSaltoPlayer = valor;
        }

        

    }

}