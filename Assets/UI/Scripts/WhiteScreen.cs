using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteScreen : UIComponent
{
    void Start() 
    {
        StartCoroutine(StartFadingOut());
    }

    public IEnumerator FadeInAndOut(Action function)
    {
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, 0.075F);
        yield return new WaitForSeconds(1F);
        function();
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, 0.075F);
        yield return new WaitForSeconds(1F);
        gameObject.SetActive(false);
    }

    public IEnumerator StartFadingOut()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, 0.325F).setDelay(0.5F);
        yield return new WaitForSeconds(1F);
        gameObject.SetActive(false);
    }
}
