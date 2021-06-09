using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Tablero : MonoBehaviour
{

    public GameObject hoja;
    public List<Color> colores;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<4; i++)
        {
            GameObject boton = Instantiate(hoja) as GameObject;
            boton.name = "Hoja" + i;
            boton.transform.SetParent(gameObject.transform, false);
            boton.GetComponent<Image>().color = colores[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
