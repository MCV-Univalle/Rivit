using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


public class UIManager : MonoBehaviour
{
    private bool _isLocked = false;
    public delegate void OnButtonClickDelegate();
    public static event OnButtonClickDelegate executePlayButton;
    public static event OnButtonClickDelegate executeHelpButton;
    public static event OnButtonClickDelegate executeCloseInstructions;
    public static event OnButtonClickDelegate executeCloseModeSelectionButton;
    public static event OnButtonClickDelegate executePlayAgainButton;
    public static event OnButtonClickDelegate executePauseButton;
    public static event OnButtonClickDelegate executeResumeFromPause;
    public static event OnButtonClickDelegate executeGameOver;
    public static event OnButtonClickDelegate executeQuitGame;
    public static event OnButtonClickDelegate executeStartGame;

    [SerializeField] private WhiteScreen whiteScreen;
    [Inject] private GameManager _gameManager;

    void Start()
    {
        whiteScreen.FadeAlpha(1, 0, 0.35F, 0.75F);
        GameManager.showResults += EndGame;
        if(_gameManager.IsRecordEmpty())  LeanTween.delayedCall(gameObject, 0.5F, () => OnHelpButton());
    }

    void OnDestroy()
    {
        GameManager.showResults -= EndGame;
        executePlayButton = null;
        executeHelpButton = null;
        executeCloseInstructions = null ;
        executeCloseModeSelectionButton = null;
        executePlayAgainButton = null;
        executePauseButton = null;
        executeResumeFromPause = null;
        executeGameOver = null;
        executeQuitGame = null;
        executeStartGame = null;
}

    public IEnumerator LockTemporally(float delayTime)
    {
        _isLocked = true;
        yield return new WaitForSeconds(delayTime);
        _isLocked = false;
    }

    public void RiseEvent(Action evnt, float delayTime)
    {
        if (!_isLocked)
        {
            evnt?.Invoke();
            StartCoroutine(LockTemporally(delayTime));
        }
    }

    public void OnPlayButton()
    {
        RiseEvent(() => executePlayButton(), 0.15F);
    }

    public void OnHelpButton()
    {
        RiseEvent(() => executeHelpButton(), 0.15F);
    }

    public void OnCloseInstructionsScreen()
    {
        RiseEvent(() => executeCloseInstructions(), 0.15F);
    }

    public void OnCloseModeSelectionButton()
    {
        RiseEvent(() => executeCloseModeSelectionButton(), 0.15F);
    }

    public void OnPlayAgainButton()
    {
        RiseEvent(() => executePlayAgainButton(), 0.15F);
    }

    public void OnPauseButton()
    {
        RiseEvent(() => executePauseButton(), 0.15F);
        _gameManager.ChangePauseState(0.25F);
    }

    public void ResumeFromPause()
    {
        _gameManager.ChangePauseState(0.15F);
        RiseEvent(() => executeResumeFromPause(), 0.3F);
        
    }

    public void OnResumeButton()
    {
        ResumeFromPause();
    }

    public void OnRestartButton()
    {
        ResumeFromPause();
        _gameManager.RestartGame();
    }

    public void OnQuitButton()
    {
        _gameManager.ChangePauseState(0.15F);
        _gameManager.EndGame();
        RiseEvent(() => executeQuitGame(), 0.3F); 
        whiteScreen.FadeInAndOut(0, 1, 0.25F, 0.5F);
    }

    public void OnReturnToHomeButton()
    {
        if (!_isLocked)
        {
            whiteScreen.gameObject.SetActive(true);
            whiteScreen.FadeIn();
            LeanTween.delayedCall(gameObject, 0.3F, () => SceneManager.LoadScene("Home"));
            StartCoroutine(LockTemporally(0.3F));
        }
    }

    public void EndGame()
    {
        executeGameOver();
    }

    public void SelectGameMode(GameMode mode)
    {
        if (!_isLocked)
        {
            LeanTween.delayedCall(gameObject, 0.4F, () => executeStartGame());
            whiteScreen.FadeInAndOut(0, 1, 0.25F, 0.5F);
            LeanTween.delayedCall(gameObject, 0.41F, () => _gameManager.InitializeGame(mode));
            StartCoroutine(LockTemporally(0.3F));
        }
    }
}
