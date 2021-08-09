using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementStars : MonoBehaviour
{
    [SerializeField] private GameObject layout;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;

    [SerializeField] private SpeechBubble speechBubble;
    private string[] felicitaciones = {"Intentalo de nuevo", "Bien hecho", "¡Excelente!"};

    public IEnumerator CheckScore(int score, int[] standars)
    {
        var layoutGroup = this.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.enabled = false;
        speechBubble.gameObject.SetActive(false);
        string text = felicitaciones[0];
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.5F);
            if (score > standars[i])
            {
                layout.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("active", true);
                text = felicitaciones[i];
            }
                
        }
        speechBubble.gameObject.SetActive(true);
        speechBubble.Speech.text = text;
        layoutGroup.enabled = true;
    }

    public void DesactiveStars()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.GetComponent<Animator>().SetBool("active", false);
        }
    }
}
