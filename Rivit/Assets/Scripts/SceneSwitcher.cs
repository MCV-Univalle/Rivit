using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void PaLaCasitaUwU() //Luego elimino esta función
    {
        SceneManager.LoadScene("Home");
    }
    public IEnumerator GoToHomeScreen()
    {
        yield return new WaitForSeconds(0.35f);
        SceneManager.LoadScene("Home");
    }

    public IEnumerator GoToGame(string gameName)
    {
        yield return new WaitForSeconds(0.35f);
        SceneManager.LoadScene(gameName);
    }
}
