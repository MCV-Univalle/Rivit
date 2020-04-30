using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    [SerializeField]
    private float _lerpTime;
    [SerializeField]
    private float _defaultAlpha = 1;
    [SerializeField]
    private bool _executeOnEnable = true;
    private IEnumerator _fadeCanvas;
    private bool _isFading = false;
    public float DefaultAlpha { set {_defaultAlpha = value;}}

    void OnEnable()
    {
        if(_executeOnEnable)
        {
            CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            FadeIn(_defaultAlpha, true);
        }
    }

    public void InterruptFading()
    {
        if(_isFading) StopCoroutine(_fadeCanvas);
    }

    public void FadeIn(float finalAlpha, bool isActive)
    {
        InterruptFading();
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        _fadeCanvas = FadeCanvasGroup(cg, cg.alpha, finalAlpha, isActive);
        _isFading = true;
        StartCoroutine(_fadeCanvas);
    }
    public void FadeOut(float finalAlpha, bool isActive)
    {
        InterruptFading();
        CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
        _fadeCanvas = FadeCanvasGroup(cg, cg.alpha, finalAlpha, isActive);
        _isFading = true;
        StartCoroutine(_fadeCanvas);
    }


    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float initialAlpha, float finalAlpha, bool isActive)
    {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / _lerpTime;
        
        while(true)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / _lerpTime;

            float currentValue = Mathf.Lerp(initialAlpha, finalAlpha, percentageComplete);

            cg.alpha = currentValue;

            if(percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
        
        gameObject.SetActive(isActive);
        cg.blocksRaycasts = isActive;
        _isFading = false;
    }
}
