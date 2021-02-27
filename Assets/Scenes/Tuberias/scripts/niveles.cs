using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class niveles : MonoBehaviour
{
    public GameObject tablero;
    public GameObject button;
    public int columnas;
    public int filas;
    public int nivel;
    private bool señal;
    private int cantidadNivel;
    private GridLayoutGroup div;
    private List<int> piezas = new List<int>();
    private List<int> angulos = new List<int>();
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

    public void SeleccionarNivel(int nivel)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        div = tablero.GetComponent<GridLayoutGroup>();
        div.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        div.constraintCount = columnas;

        for (int i = 0; i < (columnas * filas); i++)
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
