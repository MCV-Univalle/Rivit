using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using Tuberias;

public class manejadorTablero : MonoBehaviour
{
    public static manejadorTablero instance;
    private int rows, cols, cantidadPiezas, primeraPieza, ultimaPieza;
    public GameObject generador;
    private List<int> piezas = new List<int>();
    public List<int> angulos = new List<int>();
    private List<int> correctos = new List<int>();
    private GridLayoutGroup div;
    public GameObject button;
    public Sprite tuboL, tuboCruz, tuboC, tuboT;

    public int Cols { get => cols; set => cols = value; }
    public int Rows { get => rows; set => rows = value; }
    public int CantidadPiezas { get => cantidadPiezas; set => cantidadPiezas = value; }
    //public List<int> Angulos { get => angulos; set => angulos = value; }


    void Awake()
    {
        instance = this;
    }

    public void AcomodarTablero(List<string[]> nivel)
    {
        Cols = int.Parse(nivel[0][0]);
        Rows = int.Parse(nivel[1][0]);
        primeraPieza = int.Parse(nivel[3][0]);
        ultimaPieza = int.Parse(nivel[3][nivel[3].Length-1]);

        for (int i = 0; i < nivel[2].Length; i++)
        {   
            piezas.Add(int.Parse(nivel[2][i]));
            angulos.Add(int.Parse(nivel[3][i]));
        }

        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentRect.rect.width / Cols, parentRect.rect.height / Rows);

        CrearNivel(piezas, Cols);
        cantidadPiezas = piezas.Count;
        piezas.Clear();
    }

    public void CrearNivel(List<int> piezas, int col)
    {
        Transform panelTransform = gameObject.transform;
        foreach (Transform child in panelTransform)
        {
            Destroy(child.gameObject);
        }

        div = gameObject.GetComponent<GridLayoutGroup>();
        div.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        div.constraintCount = col;

        for (int i = 0; i < piezas.Count; i++)
        {
            GameObject boton = Instantiate(button) as GameObject;
            DesactivarBotones(i, boton);
            boton.name = "Boton" + i;
            boton.transform.SetParent(gameObject.transform, false);
            asignarImagen(piezas[i], boton);
        }
    }

    public void DesactivarBotones(int i, GameObject boton)
    {
        if (i == 0)
        {
            boton.GetComponent<Button>().interactable = false;
            boton.transform.Rotate(new Vector3(0, 0, primeraPieza));
        }
        else if (i == piezas.Count - 1)
        {
            boton.GetComponent<Button>().interactable = false;
            boton.transform.Rotate(new Vector3(0, 0, ultimaPieza));
        }
    }

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

    public void LlenarLista(List<string[]> nivel, List<int> ver, int i, GameObject boton)
    {
        if ((i != 0) && (angulos.Any()))
        {
            if ((ver.Contains(i)))
            {
                if ((boton.GetComponent<Image>().sprite == tuboL) || (boton.GetComponent<Image>().sprite == tuboCruz))
                {
                    FichasEspecialesQuitar(boton, angulos[i], i, ver);
                }else if (angulos[i] != boton.GetComponent<Giro>().angulo)
                    ver.Remove(i);
            }
            else if (!(ver.Contains(i)))
            {
                if((boton.GetComponent<Image>().sprite == tuboL) || (boton.GetComponent<Image>().sprite == tuboCruz))
                {
                    FichasEspecialesAñadir(boton, angulos[i], i, ver);
                }
                else if (angulos[i] == boton.GetComponent<Giro>().angulo)
                    ver.Add(i);
            }
        }
    }

    public void FichasEspecialesAñadir(GameObject boton, int angulo, int i, List<int> ver)
    {
        if (boton.GetComponent<Image>().sprite == tuboL)
        {
            switch (angulo)
            {
                case 0:
                    if((boton.GetComponent<Giro>().angulo==0)||(boton.GetComponent<Giro>().angulo==180))
                        ver.Add(i);
                    break;
                case 180:
                    if ((boton.GetComponent<Giro>().angulo == 0) || (boton.GetComponent<Giro>().angulo == 180))
                        ver.Add(i);
                    break;
                case 90:
                    if ((boton.GetComponent<Giro>().angulo == 90) || (boton.GetComponent<Giro>().angulo == 270))
                        ver.Add(i);
                    break;
                case 270:
                    if ((boton.GetComponent<Giro>().angulo == 90) || (boton.GetComponent<Giro>().angulo == 270))
                        ver.Add(i);
                    break;
            }
        }
        else if (boton.GetComponent<Image>().sprite == tuboCruz)
        {
            ver.Add(i);
        }
    }

    public void FichasEspecialesQuitar(GameObject boton, int angulo, int i,List<int> ver)
    {
        if (boton.GetComponent<Image>().sprite == tuboL)
        {
            switch (angulo)
            {
                case 0:
                    if ((boton.GetComponent<Giro>().angulo == 90) || (boton.GetComponent<Giro>().angulo == 270))
                        ver.Remove(i);
                    break;
                case 180:
                    if ((boton.GetComponent<Giro>().angulo == 90) || (boton.GetComponent<Giro>().angulo == 270))
                        ver.Remove(i);
                    break;
                case 90:
                    if ((boton.GetComponent<Giro>().angulo == 0) || (boton.GetComponent<Giro>().angulo == 180))
                        ver.Remove(i);
                    break;
                case 270:
                    if ((boton.GetComponent<Giro>().angulo == 0) || (boton.GetComponent<Giro>().angulo == 180))
                        ver.Remove(i);
                    break;
            }
        }

    }

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
