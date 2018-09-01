using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class PauseMenu : MonoBehaviour {

    public static bool onPause = false; //Boolean para verificar se o jogo está no modo pausa ou nao

    [SerializeField]
    GameObject menuPausePanel;

    [SerializeField]
    GameObject menuGameOverPanel;

    [SerializeField]
    GameObject buttonPause;

    // Use this for initialization
    void Start(){}

    // Update is called once per frame
    void Update(){}

    /// <summary>
    /// Retorna o jogo para o ultimo checkpoint
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1;
        onPause = false;
        menuPausePanel.SetActive(false);
    }

    /// <summary>
    /// Apos Game Over retorna o jogo para o ultimo ckeckpoint (após ver o anuncio)
    /// </summary>
    public void ResumeAd()
    {
#if UNITY_ADS
        if (UnityAdController.showAds)
        {
            UnityAdController.ShowAd();
        }
        menuPausePanel.SetActive(false);
#endif

        menuGameOverPanel.SetActive(false);
        buttonPause.SetActive(true);

    }

    /// <summary>
    /// Reinicia a fase
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1;
        onPause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Retorna para a sena StartMenu
    /// </summary>
    public void ShowStartMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }

    /// <summary>
    /// Pausa o Jogo
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        onPause = true;
        menuPausePanel.SetActive(true);
    }

    /// <summary>
    /// Despausa o Jogo
    /// </summary>
    public void Despause()
    {
        Time.timeScale = 1;
        onPause = false;
    }

    /// <summary>
    /// Metodo para carregar uma scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
