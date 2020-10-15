using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 0.75F;
    [SerializeField] private float positionX;
    private void Start()
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), positionX, speed).setEaseInOutCubic().setLoopPingPong();
    }
}