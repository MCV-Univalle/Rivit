using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private AudioManager _UIaudioManager;
    [SerializeField] private WhiteScreen whiteScreen;
    [Inject] private GameManager _gameManager;

    void Start()
    {
        GameManager.showResults += EndGame;
        if(_gameManager.IsRecordEmpty())  LeanTween.delayedCall(gameObject, 0.5F, () => OnHelpButton());
    }

    void OnDestroy()
    {
        GameManager.showResults -= EndGame;
    }

    public IEnumerator LockTemporally(float delayTime)
    {
        _isLocked = true;
        yield return new WaitForSeconds(delayTime);
        _isLocked = false;
    }

    public void PlayAudio(string name)
    {
        _UIaudioManager.PlayAudio(name);
    }

    public void OnPlayButton()
    {
        if (!_isLocked)
        {
            StartCoroutine(LockTemporally(0.3F));
            executePlayButton();
            _UIaudioManager.PlayAudio("Confirmation");
        }
    }

    public void OnHelpButton()
    {
        if (!_isLocked)
        {
            StartCoroutine(LockTemporally(0.3F));
            executeHelpButton();
            _UIaudioManager.PlayAudio("Confirmation");
        }
    }

    public void OnCloseInstructionsScreen()
    {
        if (!_isLocked)
        {
            StartCoroutine(LockTemporally(0.3F));
            executeCloseInstructions();
            _UIaudioManager.PlayAudio("Back");
        }
    }

    public void OnCloseModeSelectionButton()
    {
        if (!_isLocked)
        {
            StartCoroutine(LockTemporally(0.3F));
            executeCloseModeSelectionButton();
            _UIaudioManager.PlayAudio("Back");
        }
    }

    public void OnPlayAgainButton()
    {
        if (!_isLocked)
        {
            StartCoroutine(LockTemporally(0.3F));
            executePlayAgainButton();
            _UIaudioManager.PlayAudio("Confirmation");
        }
    }

    public void OnPauseButton()
    {
        if (!_isLocked)
        {
            _gameManager.ChangePauseState(0.15F);
            StartCoroutine(LockTemporally(0.075F));
            executePauseButton();
            _UIaudioManager.PlayAudio("Pause");
        }
    }

    public void ResumeFromPause()
    {
        if (!_isLocked)
        {
            _gameManager.ChangePauseState(0.15F);
            StartCoroutine(LockTemporally(0.3F));
            executeResumeFromPause();
            _UIaudioManager.PlayAudio("Confirmation");
        }
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
        if (!_isLocked)
        {
            _gameManager.ChangePauseState(0.15F);
            StartCoroutine(whiteScreen.FadeInAndOut(() => _gameManager.EndGame()));
            StartCoroutine(LockTemporally(0.3F));
            LeanTween.delayedCall(gameObject, 0.2F, () => executeQuitGame());
            _UIaudioManager.PlayAudio("Confirmation");
        }
    }

    public void OnReturnToHomeButton()
    {
        if (!_isLocked)
        {
            whiteScreen.gameObject.SetActive(true);
            whiteScreen.FadeIn();
            StartCoroutine(sceneSwitcher.GoToHomeScreen());
            StartCoroutine(LockTemporally(0.3F));
            _UIaudioManager.PlayAudio("Back");
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
            LeanTween.delayedCall(gameObject, 0.2F, () => executeStartGame());
            _UIaudioManager.PlayAudio("Confirmation");
            StartCoroutine(whiteScreen.FadeInAndOut(() => _gameManager.InitializeGame(mode)));
            StartCoroutine(LockTemporally(0.3F));
        }
    }
}
