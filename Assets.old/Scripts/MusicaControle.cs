using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaControle : MonoBehaviour {

    public static MusicaControle 
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
