using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;


public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject inGamePanel;
    //[Inject (Id="SFXManager")] private AudioManager _SFXManager;

    public IEnumerator StartCountdown(int num, float time, Action function)
    {
        inGamePanel.SetActive(false);
        gameObject.SetActive(true);
        countdownText.text = num + "";
        yield return new WaitForSeconds(time * 4);
        for (int i = num - 1; i > 0; i--)
        {
            countdownText.text = i + "";
            LeanTween.scale(countdownText.gameObject.GetComponent<RectTransform>(), new Vector3(1.25F, 1.25F, 1), 0.15F);
            yield return new WaitForSeconds(time);
            LeanTween.scale(countdownText.gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.15F);
            yield return new WaitForSeconds(time * 3);
        }
        function();
        gameObject.SetActive(false);
        inGamePanel.SetActive(true);
    }
}
