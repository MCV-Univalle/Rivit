using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteScreen : UIComponent
{
    public void FadeIn(float velocity, float delayTime)
    {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().alpha = 0F;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1F, velocity).setDelay(delayTime);
    }

    public void FadeOut(float velocity, float delayTime)
    {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().alpha = 1F;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0F, velocity).setDelay(delayTime);
        LeanTween.delayedCall(gameObject, velocity + delayTime, () => gameObject.SetActive(false));

    }

    public void FadeInAndOut(float velocity, float waitTime)
    {
        FadeIn(velocity, 0);
        FadeOut(velocity, waitTime);
    }
}
