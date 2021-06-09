using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leerArchivo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public static List<string> Leerlinea(TextAsset txt)
    {
        List<string> lines = new List<string>();
        try
        {
            lines = new List<string>(txt.text.Split('\n'));
        }
        catch (System.Exception e)
        {
            print("The file could not be read:");
            print(e.Message);
        }
        return lines;
    }
}
