using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeScreen : MonoBehaviour
{
    private UserDataManager _dataManager;
    // Start is called before the first frame update
    void Start()
    {
        _dataManager = FindObjectOfType<UserDataManager>();
        Debug.Log(_dataManager.PersonalData.Email);
        if (_dataManager.PersonalData.Coins != 0)
            SceneManager.LoadScene("Home");
        else
            SceneManager.LoadScene("Login");

    }
}
