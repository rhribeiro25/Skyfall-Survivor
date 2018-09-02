using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {


    bool got = false;   //bool que indica se o check já foi pego (para nao tocar o som varias vezes)
    AudioSource audio;  //audioSource do objeto

    [Tooltip("Sistema de particulas do Checkpoint (Child)")]
    [SerializeField]
    ParticleSystem particles;


	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Controla a passagem da bolinha pelo checkpoint
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            other.SendMessage("Checkpoint", SendMessageOptions.DontRequireReceiver); //Manda msg para a bolinha para salvar a posição
            if(!audio.isPlaying && !got) audio.Play(); //Toca o audio de checkpoint

            //Altera a aparencia do checkpoint
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
