using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class manejadorTablero : MonoBehaviour
{
    public int rows;
    public int cols;
    public GameObject generador;

    void Start()
    {
        rows = generador.GetComponent<lvl_1>().filas;
        cols = generador.GetComponent<lvl_1>().columnas;

        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentRect.rect.width / cols, parentRect.rect.height / rows);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
