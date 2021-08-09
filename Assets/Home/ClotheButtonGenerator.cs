using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ClotheButtonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Clothes[] clothes;
    [SerializeField] private ClothesDataHandler dataHandler;
    [SerializeField] private PurchaseHandler purchaseHandler;
    private UserDataManager _userDataManager;
    private List<string> purchasedClothes = new List<string>();

    [Inject]
    public void Init(UserDataManager userDataManager)
    {
        _userDataManager = userDataManager;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Clothes item in clothes)
        {
            purchasedClothes = _userDataManager.LoadPurchasedClothes();
            var temp = Instantiate(buttonPrefab);
            temp.GetComponent<ClotheButton>().Clothes = item;
            temp.GetComponent<ClotheButton>().Handler = purchaseHandler;
            temp.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.sprite;
            if (dataHandler.Hat == item.clotheName || dataHandler.Glasses == item.clotheName || dataHandler.Accessory == item.clotheName || dataHandler.Shirt == item.clotheName)
                temp.GetComponent<Outline>().enabled = true;
            if (purchasedClothes.Contains(item.clotheName) || item.price == 0)
                temp.GetComponent<ClotheButton>().AlreadyPurchased = true;
            else if (item.price > 0)
            {
                temp.GetComponent<ClotheButton>().AlreadyPurchased = false;
                temp.transform.GetChild(1).gameObject.SetActive(true);
                temp.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.price + "";
            }       
            temp.transform.SetParent(this.transform, false);
        }
    }
}
