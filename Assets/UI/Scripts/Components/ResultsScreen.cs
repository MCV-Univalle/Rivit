using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class ResultsScreen : UIComponent
{
    [SerializeField] bool isScoreRepresentedAsTime = false;

    [Inject] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI topScore;
    [SerializeField] private AchievementStars achievementStars;
    [SerializeField] private TextMeshProUGUI currentCoins;
    [SerializeField] private TextMeshProUGUI newCoins;

    private bool _isCountingFinalScore = false;

    void Start()
    {
        var pos = transform.position;
        positionY = pos.y;
        positionX = pos.x;
        UIManager.executeGameOver += ShowResults;
        UIManager.executePlayAgainButton += () => gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isCountingFinalScore)
        {
            _isCountingFinalScore = false;
            finalScore.text = "" + ((_gameManager as ModeSystemGameManager) as ModeSystemGameManager).Score;
        }
    }

    private void ShowResults()
    {
        FadeInMoveY();
        achievementStars.DesactiveStars();
        (_gameManager as ModeSystemGameManager).RecordScore();
        int top = (_gameManager as ModeSystemGameManager).GetCurrentRanking()[0];
        topScore.text = "Puntaje máximo: " + top;
        currentCoins.text = (_gameManager as ModeSystemGameManager).Coins + "";
        int score = (_gameManager as ModeSystemGameManager).Score;
        var standards = (_gameManager as ModeSystemGameManager).GetStandardsOfCurrentMode();
        StartCoroutine(achievementStars.CheckScore(score, standards));
        int obtainedCoins = (_gameManager as ModeSystemGameManager).AddCoins();
        StartCoroutine(CountCoins(obtainedCoins, 0.01F));
        StartCoroutine(CountFinalScore(score, 0.01F));
    }

    public IEnumerator CountCoins(int num, float waitTime)
    {
        newCoins.text = "+" + num + "";
        int coins = int.Parse(currentCoins.text);
        yield return new WaitForSeconds(0.5F);
        newCoins.gameObject.SetActive(true);
        for (int i = coins; i <= coins + num; i++)
        {
            currentCoins.text = i + "";
            yield return new WaitForSeconds(waitTime);
        }
    }

    public IEnumerator CountFinalScore(int score, float waitTime)
    {
        finalScore.text = "0";
        yield return new WaitForSeconds(0.5F);
        _isCountingFinalScore = true;
        for (int i = 0; (i <= score) && _isCountingFinalScore; i++)
        {
            finalScore.text = i + "";
            yield return new WaitForSeconds(waitTime);
        }
    }

    void OnDestroy()
    {
        UIManager.executePlayButton -= ShowResults; ;
        UIManager.executePlayAgainButton -= () => gameObject.SetActive(false);
    }
}
