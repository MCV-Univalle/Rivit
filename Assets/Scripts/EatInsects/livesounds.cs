using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class livesounds : MonoBehaviour
{
    public float tiempovida;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,tiempovida);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
