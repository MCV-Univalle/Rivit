using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MorePanel : MonoBehaviour
{
    private UserDataManager _userDataManager;
    private APIHelper apiHelper;
    [Inject]
    public void Init(UserDataManager dataManager)
    {
        _userDataManager = dataManager;
    }

    private void Start()
    {
        apiHelper = FindObjectOfType<APIHelper>();
    }

    public void UpdateData()
    {
        string id = "Pollo";
        string jsonString = PlayerPrefs.GetString("UserData");

        Debug.Log(id);
        Debug.Log(jsonString);

        apiHelper.Sync(id, jsonString);
    }

    public void SignOut()
    {
        _userDataManager.DeleteData();
        SceneManager.LoadScene("Login");
    }
}
