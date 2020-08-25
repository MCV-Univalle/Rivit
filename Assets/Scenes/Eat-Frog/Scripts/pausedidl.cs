using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pausedidl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(5);

       
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ExampleCoroutine());
    }
}
