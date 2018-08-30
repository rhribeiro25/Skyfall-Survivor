using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public static bool pause;

    [Tooltip("Referencia para o GO, Usado para Ligar/Desligar")]
    public GameObject pauseMenu;

    /// <summary>
    /// Metodo para reiniciar a Scene, reiniciando o jogo
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Metodo para pausar o jogo
    /// </summary>
    public void SetPauseMenu(bool isPause)
    {
        pause = isPause;
    }

    /// <summary>
    /// Metodo para carregar uma scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
	// Use this for initialization
	void Start () {
        pause = false;
        SetPauseMenu(false);
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
