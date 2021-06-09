using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<AudioSource>().Play(); 
        LeanTween.delayedCall(gameObject, 1F, () => Destroy(this.gameObject));
    }
}
