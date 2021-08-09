using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewScoreText : MonoBehaviour
{
    private Vector3 initialPosition;
    [SerializeField] private float speed = 0.025F;
    private void Awake()
    {
        initialPosition = this.gameObject.transform.localPosition;
    }
    private void OnEnable()
    {
        var tempPos = initialPosition + new Vector3(-5F, 0, 0);
        this.transform.localPosition = tempPos;
        LeanTween.moveLocalX(this.gameObject, initialPosition.x, speed).setEase(LeanTweenType.easeInBounce);
        this.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(this.GetComponent<CanvasGroup>(), 1, speed);
        LeanTween.delayedCall(this.gameObject, 2.5F, () => Disable());
    }

    private void Disable()
    {
        LeanTween.alphaCanvas(this.GetComponent<CanvasGroup>(), 0, speed);
        LeanTween.delayedCall(this.gameObject, speed, () => gameObject.SetActive(false));
    }
}
