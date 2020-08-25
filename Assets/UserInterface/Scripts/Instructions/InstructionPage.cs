using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPage : MonoBehaviour
{
    [SerializeField] private float positionX = 30;

    public void MovePageOut(int direction)
    {
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0, 0.2F).setDelay(0.05F);
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), positionX * direction, 0.15F).setDelay(0.05F).setEaseInOutCubic();
        gameObject.SetActive(false);
    }

    public void MovePageIn(int direction)
    {
        gameObject.transform.localPosition = new Vector3(positionX * direction, 0, 0);
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, 0.2F).setDelay(0.05F);
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), 0, 0.15F).setDelay(0.05F).setEaseInOutCubic();
    }
}