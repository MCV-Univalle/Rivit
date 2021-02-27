using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhiteScreen : UIComponent
{
    public void FadeAlpha(float initial, float final, float velocity, float waitTime)
    {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().alpha = initial;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), final, velocity);
        if(final == 0)
            LeanTween.delayedCall(gameObject, waitTime + (velocity * 2), () => gameObject.SetActive(false));
    }

    public void FadeInAndOut(float initial, float final, float velocity, float waitTime)
    {
        FadeAlpha(initial, final, velocity, waitTime);
        LeanTween.delayedCall(gameObject, waitTime, () => FadeAlpha(final, initial, velocity, waitTime));
    }
}
