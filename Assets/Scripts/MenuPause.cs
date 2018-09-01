using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class MenuPause : MonoBehaviour {

    public static bool onPause = false;

    [SerializeField]
    GameObject menuPausePanel;

    [SerializeField]
    GameObject menuGameOverPanel;

    public void Resumir()
    {
        Time.timeScale = 1;
        onPause = false;
        menuPausePanel.SetActive(false);
    }

    public void ResumirAd()
    {
#if UNITY_ADS
        if (UnityAdController.showAds)
        {
            UnityAdController.ShowAd();
        }
        menuPausePanel.SetActive(false);
#endif

        menuGameOverPanel.SetActive(false);

    }

    public void Reiniciar()
    {
        Time.timeScale = 1;
        onPause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }

    public void pausar()
    {
        Time.timeScale = 0;
        onPause = true;
        menuPausePanel.SetActive(true);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
