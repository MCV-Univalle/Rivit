using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField] private APIHelper apiHelper;
    [SerializeField] private UserDataManager userDataManager;
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMP_InputField userTextField;
    [SerializeField] private TMP_InputField passwordTextField;
    [SerializeField] private TMP_InputField emailTextField;
    [SerializeField] private TMP_InputField passwordTextField2;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;


    // Start is called before the first frame update
    void Start()
    {
        APIHelper.UserAccess += ConfirmAccess;
        passwordTextField.contentType = TMP_InputField.ContentType.Password;
    }

    private void OnDestroy()
    {
        APIHelper.UserAccess -= ConfirmAccess;
    }

    public void VerifyUser()
    {
        string user = userTextField.text;
        string password = passwordTextField.text;

        apiHelper.Login(user, password);
    }

    public void RegisterNewUser()
    {
        string user = userTextField.text;
        string password = passwordTextField.text;

        userDataManager.RegisterEmailAndPassword(user, password);
        apiHelper.Login(user, password);
    }

    private void ConfirmAccess(bool access)
    {
        SceneManager.LoadScene("Home");
            
    }

    public void CreateNewAccount()
    {
        loginPanel.gameObject.SetActive(false);
        registerPanel.gameObject.SetActive(true);
    }

    public void LoginWithAccount()
    {
        loginPanel.gameObject.SetActive(true);
        registerPanel.gameObject.SetActive(false);
    }
}
