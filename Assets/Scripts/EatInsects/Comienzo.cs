using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Eat_frog_Game
{
    public class Comienzo : MonoBehaviour
{
    public GameObject createinsect;
    public GameObject createinsect2;
    public GameObject barra;
    private Objectivo objectivo;
    public GameObject Look;
    private FrogController frog;
    // Start is called before the first frame update
    void Start()
    {
        frog = FindObjectOfType<FrogController>();
        objectivo = FindObjectOfType<Objectivo>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.Active){
            createinsect.SetActive(true);
            createinsect2.SetActive(true);
            Look.SetActive(true);
        }
    }

    public void Abandonar() {
        GameManager.Instance.Active = false;
        frog.healthlive.fillAmount = 100;
        print("df"+ frog.healthlive.fillAmount );
        createinsect.SetActive(false);
        createinsect2.SetActive(false);
        Look.SetActive(false);
    }

    public void reiniciar(){
        GameManager.Instance.Score = 0;
        GameManager.Instance.Active = false;
        frog.vel = 0.1f;
        frog.Tongue = true;
        createinsect.SetActive(false);
        createinsect2.SetActive(false);
        Look.SetActive(false);
        GameManager.Instance.paused = false;
        frog.curhealth = 100f;
    }   

    public void Comezar() {
        
        GameManager.Instance.Score = 0;
        GameManager.Instance.Active = true;
        StartCoroutine(ExampleCoroutine1());
    }

    IEnumerator ExampleCoroutine1()
    {
        yield return new WaitForSeconds(1);

        frog.Tongue = true;
        createinsect.SetActive(true);
        createinsect2.SetActive(true);
        Look.SetActive(true);
        GameManager.Instance.paused = false;
    }

    public void paused(){
        GameManager.Instance.paused = true;
    }
    public void resumen(){
        StartCoroutine(ExampleCoroutine());
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(1);

        GameManager.Instance.paused = false;
    }
    

}

}
