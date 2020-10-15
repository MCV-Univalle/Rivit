using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreComponent : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI currentScore;

    void Start()
    {
        GameManager.updateScore += UpdateScore;
    }

    public void UpdateScore(int score)
    {
        currentScore.text = score + "";
    }

    private void OnDestroy()
    {
        GameManager.updateScore -= UpdateScore;
    }
}
