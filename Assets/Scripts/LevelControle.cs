using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControle : MonoBehaviour {

	public void CarregaLevel(string sceneNome) {
        
    }

    public void CarregaProxLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene()
            .buildIndex + 1);
    }

    public void BlocoDestruido() {
       
    }

}
