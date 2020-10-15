using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class logicaNivel : MonoBehaviour
{

    public int cambioNivel = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision) 
    {

        if (collision.CompareTag("guia"))
        {

            cambioNivel += 1; 

        }

    }
}
