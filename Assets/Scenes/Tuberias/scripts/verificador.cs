using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verificador : MonoBehaviour
{

    public int cantidad;
    public GameObject boton;
    public bool revision;
    public List<int> listaVer = new List<int>();
    private GameObject[] listaObjetos;

    // Start is called before the first frame update

    public void LlenarLista(List<int> ver, int i, List<int>angulos, GameObject boton)
    {
        if ((ver.Contains(i)) && (gameObject.GetComponent<lvl_1>().angulos[i] != boton.GetComponent<Giro>().angulo))
        {
            ver.Remove(i);
        }else if (!(ver.Contains(i)) && (gameObject.GetComponent<lvl_1>().angulos[i] == boton.GetComponent<Giro>().angulo))
        {
            ver.Add(i);
        }
    }

    void Start()
    {
        listaObjetos = GameObject.FindGameObjectsWithTag("boton");
    }

    // Update is called once per frame
    void Update()
    {
        cantidad = gameObject.GetComponent<lvl_1>().angulos.Count;
        for (int i = 0; i < cantidad; i++)
        {
            boton = listaObjetos[i];
            LlenarLista(listaVer, i, gameObject.GetComponent<lvl_1>().angulos, boton);
        }

        if (listaVer.Count == 12)
            revision = true;
    }
}
