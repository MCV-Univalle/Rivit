using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generacionGuias : MonoBehaviour
{

    public int color;
    public int circulos = 4;
    private GameObject[] guias;
    private Vector2 posicionGuias = new Vector2(30, 42);
    public GameObject circuloPrefab;

    private float timeSinceLastSpawned;
    public float spawnRate;
    public float spawnX;

    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
    public Color color5;
    public Color color6;
    public Color color7;
    public Color color8;
    public Color color9;

    private int currentGuia;
    public int numRandom;

    public void cambioColor()
    {

        numRandom = Random.Range(1, 9);

        switch (numRandom)
        {

            case 1:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color1;

                break;

            case 2:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color2;

                break;

            case 3:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color3;

                break;

            case 4:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color4;

                break;

            case 5:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color5;

                break;

            case 6:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color6;

                break;

            case 7:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color7;

                break;

            case 8:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color8;

                break;

            case 9:

                circuloPrefab.GetComponent<SpriteRenderer>().color = color9;

                break;

        }

    }

    // Start is called before the first frame update
    void Start()
    {

        guias = new GameObject[circulos];
        for (int i = 0; i<circulos; i++)
        {
            cambioColor();
            guias[i] = Instantiate(circuloPrefab, posicionGuias, Quaternion.identity);

        }


    }

    // Update is called once per frame
    void Update()
    {

        

        timeSinceLastSpawned += Time.deltaTime;


        if (timeSinceLastSpawned >= spawnRate)
        {

            timeSinceLastSpawned = 0;
            guias[currentGuia].transform.position = new Vector2(spawnX, 42);
            currentGuia++;

            if (currentGuia >= circulos)
            {

                currentGuia = 0;

            }

        }

    }
}
