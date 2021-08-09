using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PurchaseHandler : MonoBehaviour
{
    private UserDataManager _userDataManager;
    [SerializeField] private GameObject notEnoughPanel;
    [SerializeField] private GameObject confirmattionPanel;

    [SerializeField] private Image miniature;
    [SerializeField] private TextMeshProUGUI priceText;

    private ClotheButton button;

    [Inject]
    public void Init(UserDataManager userDataManager)
    {
        _userDataManager = userDataManager;
    }

    public void ValidatePurchase()
    {
        var clothes = button.Clothes;
        _userDataManager.Coins = clothes.price * -1;
        _userDataManager.AddPurchasedClothes(clothes.clotheName);
        CloseConfirmattionPanel();
        button.AlreadyPurchased = true;
        button.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void OpenPanel(GameObject button)
    {
        this.button = button.GetComponent<ClotheButton>();
        var clothes = this.button.Clothes;
        if (_userDataManager.Coins < clothes.price)
            notEnoughPanel.gameObject.SetActive(true);
        else
        {
            confirmattionPanel.gameObject.SetActive(true);
            miniature.sprite = this.button.Clothes.sprite;
            priceText.text = this.button.Clothes.price + "";
        }
            
    }

    public void CloseNotEnoughPanel()
    {
        notEnoughPanel.gameObject.SetActive(false);
    }

    public void CloseConfirmattionPanel()
    {
        confirmattionPanel.gameObject.SetActive(false);
    }
}
