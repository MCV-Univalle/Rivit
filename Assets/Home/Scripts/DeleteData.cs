using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteData : MonoBehaviour
{
    private UserDataManager userDataManager;
    // Start is called before the first frame update
    void Start()
    {
        userDataManager = FindObjectOfType<UserDataManager>();
    }

    public void Delete()
    {
        
    }
}
