using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneSwitcher
{
    public static void GoToHomeScreen()
    {
        SceneManager.LoadScene("Home");
    }

    public static void GoToGame(string gameName)
    {
        SceneManager.LoadScene(gameName);
    }
}
