using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerificacionRespuestas : MonoBehaviour
{
    // Start is called before the first frame update
    public List <GameObject> respuestas;
    private string seleccionado;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VerificarRespuesta()
    {
        for(int i =0; i<4; i++){
            if(gameObject.GetComponent<Toggle>().isOn == true)
                seleccionado = gameObject.name;
        }
    }
}
