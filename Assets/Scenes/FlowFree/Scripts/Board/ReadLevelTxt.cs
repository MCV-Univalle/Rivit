using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class ReadLevelTxt : MonoBehaviour
{
    public static List<string[]> ReadTxt2(string path)
    {
        

        List<string[]> myList = new List<string[]>();
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                char letter = ' ';
                while ((line = sr.ReadLine()) != null)
                {
                    myList.Add(line.Split(letter));
                    letter = '\t';
                }
            }
        }
        catch (System.Exception e)
        {
            print("The file could not be read:");
            print(e.Message);
        }
        return myList;
    }


    public static List<string[]> ReadTxt(TextAsset txt)
    {
        List<string[]> myList = new List<string[]>();
        try
        {
            List<string> lines = new List<string>(txt.text.Split('\n'));
            char letter = ' ';
            for (int i =0; i < lines.Count; i++)
            {
                myList.Add(lines[i].Split(letter));
                letter = '\t';
            }
        }
        catch (System.Exception e)
        {
            print("The file could not be read:");
            print(e.Message);
        }
        return myList;
    }
}
