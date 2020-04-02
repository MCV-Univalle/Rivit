using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Saltos
{

    public class TouchAndGo : MonoBehaviour
    {
        private static TouchAndGo _instance;
        public static TouchAndGo Instance
        {
            get
            {
                //Logic to create the instance
                if (_instance == null)
                {
                    _instance = new TouchAndGo();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        [SerializeField]
        float moveSpeed = 5f;

        public GameObject player;
        Rigidbody2D rb;

        public Touch touch;
        Vector3 touchPosition, whereToMove;
        public bool isMoving;

        float previousDistanceToTouchPos, currentDistanceToTouchPos;

        public void moveToPoint(Vector3 targetPos)
        {
            // Rota la rana en la direcion hacia donde se va mover
            Vector3 targ = targetPos;
            //targ.z = 0f;

            Vector3 objectPos = player.transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;



            if (targetPos != objectPos)
            {
                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            }


            // Mueve la rana de posicion
            if (player.transform.position != targetPos)
            {
                player.transform.position = Vector2.MoveTowards(player.transform.position, targetPos, moveSpeed * Time.deltaTime);
                isMoving = true;

                //Debug.Log(player.name);
                if (player.name == "Libelula") AnimationManager.Instance.ComVueloTriger(isMoving, player);
                if (player.name == "Player") AnimationManager.Instance.PalyerSaltroTriger(isMoving, player);
                
            }
            else
            {
                isMoving = false;
                if (player.name == "Libelula") AnimationManager.Instance.ComVueloTriger(isMoving, player);
                if (player.name == "Player") AnimationManager.Instance.PalyerSaltroTriger(isMoving, player);
                
            }
        }

        public void setPosPLayer(Vector3 newPos)
        {
            player.transform.position = newPos;
        }


    }

}
