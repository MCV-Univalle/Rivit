using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 finalPosition;
    [SerializeField] private TextMeshProUGUI speech;

    public TextMeshProUGUI Speech { get => speech; set => speech = value; }

    private void Start()
    {
        finalPosition = transform.localPosition;
        initialPosition = finalPosition;
        initialPosition.y -= 1F;
    }

    private void OnEnable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 1F, 0.2F);
    }
}
