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
    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private GameObject finalScorePanel;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] protected GameObject _separator;
    [SerializeField] protected GameObject _rankingContainer;
    [SerializeField] protected GameObject _scorePrefab;


    private bool _isCountingFinalScore = false;

    void Start()
    {
        positionY = -675;
        positionX = 500;
        UIManager.executeGameOver += PrepareResults;
        UIManager.executePlayAgainButton += FadeOutMoveX;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isCountingFinalScore)
        {
            _isCountingFinalScore = false;
            finalScore.text = "" + _gameManager.Score;
        }
    }

    public void PrepareResults()
    {
        newRecordText.gameObject.SetActive(false);
        finalScore.text = 0 + "";
        FadeInMoveY();
        int score = _gameManager.Score;
        finalScorePanel.GetComponent<UIComponent>().FadeIn();
        rankingPanel.GetComponent<CanvasGroup>().alpha = 0;
        rankingPanel.GetComponent<CanvasGroup>().interactable = false;
        finalScorePanel.GetComponent<CanvasGroup>().alpha = 0;
        if (isScoreRepresentedAsTime)
        {
            score = -score + 100000;
            float minutes = Mathf.FloorToInt(score / 60);
            float seconds = Mathf.FloorToInt(score % 60);

            finalScore.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            StartCoroutine(ShowHighScore());
        }

        else
            StartCoroutine(CountFinalScore(score, 0.025F));
    }

    public IEnumerator CountFinalScore(int score, float waitTime)
    {
        yield return new WaitForSeconds(1F);
        _isCountingFinalScore = true;
        for (int i = 0; (i <= score) && _isCountingFinalScore; i++)
        {
            finalScore.text = i + "";
            yield return new WaitForSeconds(waitTime);
        }
        StartCoroutine(ShowHighScore());
    }

    public IEnumerator ShowHighScore()
    {
        yield return new WaitForSeconds(0.08F);
        int newRecordPos = _gameManager.RecordScore();
        DisplayNewRecordText(newRecordPos);
        yield return new WaitForSeconds(1.5F);
        finalScorePanel.GetComponent<UIComponent>().FadeOut();
        LeanTween.delayedCall(gameObject, 0.5F, () => ShowRanking(newRecordPos));
    }

    public void ShowRanking(int newRecordPos)
    {
        List<int> ranking = _gameManager.GetCurrentRanking()[_gameManager.CurrentGameMode];
        DestroyRanking();
        GenerateRankingTable(ranking, newRecordPos);
        rankingPanel.GetComponent<UIComponent>().FadeIn();
    }

    public void DisplayNewRecordText(int num)
    {
        if(num != -1)
        {
            newRecordText.gameObject.SetActive(true);
            LeanTween.alphaCanvas(newRecordText.gameObject.GetComponent<CanvasGroup>(), 1, 0.3F);
        }
    }

    public void GenerateRankingTable(List<int> ranking, int newRecordPos)
    {
        for (int i = 0; i < 5; i++)
        {
            int currentScore = ranking[i];
            GameObject go = CreateScoreInstance(i, currentScore);
            if(i == newRecordPos)
            HighligthScore(go, new Color32(255, 255, 0, 255));
            CreateSeparator(i);   
        }
    }

    public GameObject CreateScoreInstance(int num, int score)
    {
        GameObject go = Instantiate(_scorePrefab);
        go.transform.SetParent(_rankingContainer.transform, false);
        go.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = num + 1 + ".";
        if (isScoreRepresentedAsTime)
            ShowScoreAsTime(go, score);
        else 
            go.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "" + score;
        return go;
    }

    public void ShowScoreAsTime(GameObject go, int score)
    {
        if (score == 0) score = 100000;
        score = 100000 - score;
        float minutes = Mathf.FloorToInt(score / 60);
        float seconds = Mathf.FloorToInt(score % 60);

        go.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void HighligthScore(GameObject go, Color32 color)
    {
        go.transform.Find("Number").GetComponent<TextMeshProUGUI>().color = color ; //Yellow
        go.transform.Find("Score").GetComponent<TextMeshProUGUI>().color = color;
    }

    public void CreateSeparator(int num)
    {
        GameObject go = Instantiate(_separator);
        go.transform.SetParent(_rankingContainer.transform, false);
        go.transform.localPosition = new Vector3(0, (80 * num) + 20, 0);
    }

    public void DestroyRanking()
    {
        Transform children = _rankingContainer.transform;
        foreach(Transform child in children)
            {
                GameObject choice = child.gameObject;
                Destroy(choice);
            }
    }

    void OnDestroy()
    {
        UIManager.executePlayButton -= FadeInMoveY;
        UIManager.executePlayAgainButton -= FadeOutMoveX;
    }
}
