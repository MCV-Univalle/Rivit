using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saltos
{

    public class LeafController : MonoBehaviour
    {
        public List<GameObject> leavesList = new List<GameObject>();
        //public TouchAndGo touchAndGo = new TouchAndGo();

        Vector3 p;
        int indexLeave;

        public Vector3 isTouchLeaves(Vector3 wp)
        {
            // Recorre la lista de Game Object y verifica cua hoja ha sido tocada
            foreach (GameObject objectLeave in leavesList)
            {
                if (objectLeave.GetComponent<Collider2D>().OverlapPoint(wp)) // Verifica si el punto que se toco se solapa con una planta
                {
                    //Debug.Log("Nombre de la planta: " + object.name);
                    p = objectLeave.transform.position;

                    //nombre = objectLeave.name;
                    //indexLeave = leavesList.IndexOf(objectLeave);

                    //Debug.Log("Nombre de la planta: " + nombre + ", Indice: " + indexLeave);

                    //touchAndGo.MetodoE(p);
                    //touchAndGo.moverToPoint(p);
                }
            }

            return p;
        }

        public bool seTocoLeave(Vector3 wp)
        {
            bool estado = false;
            foreach (GameObject objectLeave in leavesList)
                if (objectLeave.GetComponent<Collider2D>().OverlapPoint(wp)) estado = true;

            return estado;
        }

        public int getIndexLeave(Vector3 wp)
        {
            foreach (GameObject objectLeave in leavesList)
            {
                if (objectLeave.GetComponent<Collider2D>().OverlapPoint(wp)) // Verifica si el punto que se toco se solapa con una planta
                {
                    indexLeave = leavesList.IndexOf(objectLeave);
                    AudioManagerSaltos.Instance.PlaySaltoRanaPlayer();
                }
            }

            return indexLeave;
        }

        public Vector3 getPositionLeave(int numLeave)
        {
            // Retorna la posicion de la hoja correspondiente al parametro de entrada
            GameObject leave;
            leave = leavesList[numLeave];

            //Debug.Log(leave.name);

            Vector3 posLeave = leave.transform.position;

            return posLeave;
        }


    }

}