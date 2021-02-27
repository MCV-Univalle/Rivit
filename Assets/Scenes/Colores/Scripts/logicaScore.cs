using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class logicaScore : MonoBehaviour
{

    public int puntaje = 0;
    public TextMeshProUGUI texto;
    public int contador = 0;
    public bool adentro = false;

    /// <summary>
    /// /////////////////
    /// </summary>
    public int color;
    public int circulos = 4;
    private GameObject[] guias;
    private Vector2 posicionGuias = new Vector2(30, 42);
    public GameObject circuloPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //texto = GameObject.Find("score").GetComponent<TextMeshProUGUI>();


    }

    // Update is called once per frame
    void Update()
    {


    }

}
