using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lvl_1 : MonoBehaviour
{
    public GameObject tablero;
    public GameObject button;
    public int columnas = 4;
    public int filas = 3;
    private GridLayoutGroup div;

    private List<int> piezas = new List<int>()
    {
        2,2,3,2,3,4,2,1,2,3,1,3
    };

    public List<int> angulos = new List<int>()
    {
        270,0,0,270,90,0,180,0,90,180,90,180
    };

    public Sprite tuboL, tuboCruz, tuboC, tuboT;

    public void asignarImagen(int tipoTubo, GameObject boton)
    {
        switch (tipoTubo)
        {
            case 1:
                boton.GetComponent<Image>().sprite = tuboL;
                break;
            case 2:
                boton.GetComponent<Image>().sprite = tuboC;
                break;
            case 3:
                boton.GetComponent<Image>().sprite = tuboT;
                break;
            case 4:
                boton.GetComponent<Image>().sprite = tuboCruz;
                break;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        div = tablero.GetComponent<GridLayoutGroup>();
        div.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        div.constraintCount = columnas;

        for (int i = 0; i < (columnas*filas); i++) 
        {
            GameObject boton = Instantiate(button) as GameObject;
            boton.name = "Boton" + i;
            boton.transform.SetParent(tablero.transform, false);
            asignarImagen(piezas[i], boton);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
