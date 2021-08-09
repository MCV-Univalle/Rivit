using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private UserDataManager userDataManager;
    // Start is called before the first frame update
    void Start()
    {
        userDataManager = FindObjectOfType<UserDataManager>();
        if (userDataManager.PersonalData.PlayerName != "")
            SceneSwitcher.GoToHomeScreen();
    }

    public void SaveData()
    {
        string name = inputField.text;
        userDataManager.PersonalData.PlayerName = name;
        userDataManager.SaveData();
        SceneSwitcher.GoToHomeScreen();
    }
}
