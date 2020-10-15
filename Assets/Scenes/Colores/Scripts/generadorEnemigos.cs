using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generadorEnemigos : MonoBehaviour
{

    public GameObject[] guias;
    public float tiempoEntreGuias;
    public float comienzoDeTiempo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempoEntreGuias <= 0)
        {
            int random = Random.Range(0, guias.Length);
            Instantiate(guias[random], transform.position, Quaternion.identity);

            tiempoEntreGuias = comienzoDeTiempo;
        }
        else
        {
            tiempoEntreGuias -= Time.deltaTime;
        }
    }
}
