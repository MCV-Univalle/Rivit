using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giro : MonoBehaviour
{
    public float z;
    public float angulo;

    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GirarBoton()
    {
        transform.Rotate(new Vector3(0, 0, z + 90));
        angulo = transform.eulerAngles.z;
    }

   /* public void Verificar() 
    {
        for (int i = 0; i < cantidad; i++)
        {
            boton = gameObject.name;
            angulo = transform.eulerAngles.z;

            if ((boton == "Boton"+i) && (nivel1.angulos[i] == angulo))
            {
                correcto = true;
            }
            else
            {
                correcto = false;
            }
        }
    }*/

}
