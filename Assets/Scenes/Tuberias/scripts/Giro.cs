using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Tuberias
{

public class Giro : MonoBehaviour
{
    AudioSource MyAudioSource;
    bool play;
    public float z;
    public float angulo;
    public int clicks;

    // Start is called before the first frame update

    void Start()
    {
        MyAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GirarBoton()
    {
        transform.Rotate(new Vector3(0, 0, z + 90));
        angulo = transform.eulerAngles.z;
        MyAudioSource.Play();
        clicks +=1;
    }

}

}

