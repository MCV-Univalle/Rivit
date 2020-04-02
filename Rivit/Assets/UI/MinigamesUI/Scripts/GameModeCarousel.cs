using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameModeCarousel : MonoBehaviour
{
    public float transitionSpeed = 0.2F;

    [SerializeField]
    private GameRules[] _gameModes;
    public int NumberOfModes {get {return _gameModes.Length;}}
    private int _index = 0;
    public int Index { get {return _index;}}

    [SerializeField]
    private int _buttonsGap = 210;

    [SerializeField]
    private GameObject _buttonContainer;
    [SerializeField]
    private GameObject _modeButtonPrefab;
    [SerializeField]
    public GameObject _leftArrow;
    [SerializeField]
    public GameObject _rightArrow;
    [SerializeField]
    private GameObject _leftButton;
    [SerializeField]
    private GameObject _rightButton;
    
    void Start()
    {
        //DisplayGameModes();
    }

    public GameObject CreateGameModeButton(int i)
    {
        GameObject go = Instantiate(_modeButtonPrefab);
        go.transform.SetParent(_buttonContainer.transform, false);
        go.transform.localPosition = new Vector3(_buttonsGap * i, 0, 0);
        go.gameObject.GetComponent<Image>().sprite = _gameModes[i].image;
        //go.transform.localScale = new Vector3(1, 1, 1);   
        
        return go;
    }

    public void MoveCarousel(int direction, UIController uiController)
    {
        ChangeButtonTransparency(_index, true);
        _index += direction;
        ChangeButtonTransparency(_index, false);
        VerifyLimits();
        StartCoroutine(TranslateButtons(_buttonsGap * _index * -1, uiController));
    }

    public GameRules GetCurrentGameMode()
    {
        GameRules currentMode = _gameModes[_index];
        return currentMode;
    }

    public void VerifyLimits()
    {
        if(_index == _gameModes.Length - 1)
        {
        _rightArrow.gameObject.SetActive(false);
        _rightButton.gameObject.GetComponent<Button>().interactable = false;
        }
        else
        { 
        _rightArrow.gameObject.SetActive(true);
        _rightButton.gameObject.GetComponent<Button>().interactable = true;
        }
        if(_index == 0)
        {
        _leftArrow.gameObject.SetActive(false);
        _leftButton.gameObject.GetComponent<Button>().interactable = false;
        }
        else
        { 
        _leftArrow.gameObject.SetActive(true);
        _leftButton.gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public IEnumerator TranslateButtons(float finalPosition, UIController uiController)
    {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / transitionSpeed;

        float initialPosition = _buttonContainer.transform.localPosition.x;
        
        while(true)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / transitionSpeed;

            float currentValue = Mathf.Lerp(initialPosition, finalPosition, percentageComplete);

            _buttonContainer.transform.localPosition = new Vector3(currentValue, 210, 0);

            if(percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
        uiController.UpdateGameModeInformation();
    }

    public void ChangeDefaultAlpha()
    {
        Transform buttonChilds = _buttonContainer.gameObject.transform;
        foreach (Transform child in buttonChilds)
        {
            child.gameObject.GetComponent<UIFader>().DefaultAlpha = 0.4F;      
        }
        _buttonContainer.gameObject.transform.GetChild(_index).gameObject.GetComponent<UIFader>().DefaultAlpha = 1F;   
    }

    public void ChangeButtonTransparency(int num, bool toTransparent)
    {
        if(toTransparent)
        _buttonContainer.gameObject.transform.GetChild(num).gameObject.GetComponent<UIFader>().FadeOut(0.4F, true);
        else
        _buttonContainer.gameObject.transform.GetChild(num).gameObject.GetComponent<UIFader>().FadeIn(1F, true);
    }
}