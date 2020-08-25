using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour
{

    #region de singleton
    public static trigger instance;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
    }

    #endregion

    public static trigger Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(trigger)) as trigger;
            if (!instance)
                Debug.LogError("error trigger");
        }

        return instance;
    }

    public GameObject leaf;
    public Color colorLeaf;
    private control manejador;

    // Start is called before the first frame update
    void Start()
    {
        manejador = control.Instance();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            colorLeaf = leaf.GetComponent<SpriteRenderer>().color;
            manejador.posActualJugador = leaf.gameObject.tag;
    

        }
    }
}
