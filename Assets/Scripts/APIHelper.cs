using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class APIHelper : MonoBehaviour
{
    [SerializeField] private string url;
    [SerializeField] private string stadisticsURL;
    public static event Action<bool> UserAccess = delegate { };

    [Serializable]
    private class Access
    {
        public string Message { get; set; }
    }

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("APIHelper");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }


    public void Login(string id, string password)
    {
        StartCoroutine(VerifyUser(id, password));
    }

    public void Sync(string id, string json)
    {
        StartCoroutine(SyncronizeData(id, json));
    }

    private void ConfirmAccess(string text)
    {
        Debug.Log(text);
        if (text == "{\"user_access\":\"success\"}")
        {
            UserAccess.Invoke(true);
            Debug.Log("No entró");
        }
            
        else
            UserAccess.Invoke(false);
    }

    private IEnumerator VerifyUser(string id, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("user", id);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log(www.error);
            else
            {
                var data = JsonConvert.DeserializeObject<Access>(www.downloadHandler.text);

                ConfirmAccess(data.Message);
            }
        }
    }

    private IEnumerator SyncronizeData(string id, string json)
    {
        WWWForm form = new WWWForm();
        form.AddField("user", id);
        form.AddField("stadistics", json);

        using (UnityWebRequest www = UnityWebRequest.Post(stadisticsURL, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log(www.error);
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public static implicit operator APIHelper(GameObject v)
    {
        throw new NotImplementedException();
    }
}
