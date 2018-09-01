using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    [Tooltip("Sistema de particulas do Checkpoint (Child)")]
    [SerializeField]
    ParticleSystem particles;

    bool got = false;

    AudioSource audio;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            other.SendMessage("Checkpoint", SendMessageOptions.DontRequireReceiver);
            if(!audio.isPlaying && !got) audio.Play();
            ParticleSystem.MainModule p_module = particles.main; //Warning - particles.startColor is deprecated
            p_module.startLifetime = 0.5f;
            Color g = Color.gray;
            g.a = 0.5f;
            p_module.startColor = g;
            gameObject.GetComponent<Renderer>().material.color = g;
            got = true;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
