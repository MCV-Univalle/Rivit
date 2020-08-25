using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : MonoBehaviour
{
    protected float _fadeTime = 0.125f;
    protected float _moveTimeX = 0.4f;
    protected float _moveTimeY = 0.175f;
    protected float _delay = 0.07f;
    [SerializeField] protected float positionX = 0;
    [SerializeField] protected float positionY = 0;
    //public float PositionY{get {return positionY;} set {positionY = value;}}

    public void FadeOut()
    {
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, _fadeTime)
            .setDelay(_delay);
        gameObject.GetComponent<CanvasGroup>().interactable = false;
        LeanTween.delayedCall(gameObject, _delay + _fadeTime, () => gameObject.SetActive(false));
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, _fadeTime)
            .setDelay(_delay);
        gameObject.GetComponent<CanvasGroup>().interactable = true;
    }
    public void FadeInMoveY()
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0, positionY, 0);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        FadeIn();
        LeanTween.moveY(gameObject.GetComponent<RectTransform>(), 0, _moveTimeY)
            .setDelay(_delay)
            .setEaseInOutCubic();
    }

    public void FadeOutMoveY()
    {
        FadeOut();
        LeanTween.moveY(gameObject.GetComponent<RectTransform>(), positionY, _moveTimeY)
            .setDelay(_delay)
            .setEaseInOutCubic();
        LeanTween.delayedCall(gameObject, _delay + _moveTimeY, () => gameObject.SetActive(false));
    }

    public void FadeInMoveX()
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(positionX, 0, 0);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        FadeIn();
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), 0, _moveTimeX)
            .setDelay(_delay)
            .setEaseInOutCubic();
    }

    public void FadeOutMoveX()
    {
        FadeOut();
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), positionX, _moveTimeX)
            .setDelay(_delay)
            .setEaseInOutCubic();
        LeanTween.delayedCall(gameObject, _delay + _moveTimeX, () => gameObject.SetActive(false));
    }
}
