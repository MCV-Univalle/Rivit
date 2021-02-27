using Tuberias;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TuberiasUIManager : MonoBehaviour
{
    public static TuberiasUIManager instance;

    private TuberiasGameManager gameManager = new TuberiasGameManager();

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<TuberiasGameManager>();

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
