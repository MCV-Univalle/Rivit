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

        //This returns the angle in radians
        public float AngleInRad(Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        //This returns the angle in degrees
        public float AngleInDeg(Vector3 vec1, Vector3 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }

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
                //player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));


                Vector3 targetDir = targetPos - objectPos;
                //int angle2 = (int) Vector2.Angle(targetDir, new Vector2(player.transform.position.x,0) );
                int angle2 = (int)AngleInDeg(objectPos, targetDir);

                /*
                if (player.name == "Player")
                {
                    int numSalto = 0;

                    if (angle2 < 20 && angle > -20) numSalto = 5; // derecha
                    if (angle2 > 160 && angle2 < 200) numSalto = 3; // izquierda

                    
                    if (angle2 > -120 && angle2 < -60)
                    {
                        Debug.Log("entro");
                        numSalto = 7; //abajo
                    }

                    if (angle2 > 20 && angle2 < 90) numSalto = 1; // arriba


                    AnimationManager.Instance.SetDireccionSaltoPlayer(numSalto);
                    //Debug.Log("Angulo de salto de " + player.name + " = " + angle2);
                }
                */

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
