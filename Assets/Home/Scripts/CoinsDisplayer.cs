using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class CoinsDisplayer : MonoBehaviour
{
    [Inject] private UserDataManager _dataManager;
    [SerializeField] private TextMeshProUGUI coinsText;

    void Update()
    {
        coinsText.text = _dataManager.Coins + "";
    }
}
