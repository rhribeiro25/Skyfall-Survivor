using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public static MusicController 
        musicaControle = null;

    private void Awake() {
        if(musicaControle != null) {
            Destroy(gameObject);
        } else {
            musicaControle = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
