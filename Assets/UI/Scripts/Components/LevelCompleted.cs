using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LevelCompleted : UIComponent
{
    [Inject] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI levelCompleted;

    void Start()
    {
        var pos = transform.position;
        positionY = pos.y;
        positionX = pos.x;

        UIManager.executeLevelCompleted += Open;
    }

    private void Open()
    {
        FadeInMoveX();
    }

    private void Close()
    {
        FadeOutMoveY();
    }
}
