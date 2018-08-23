using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    [Tooltip("Sistema de particulas do Checkpoint (Child)")]
    [SerializeField]
    ParticleSystem particles;

	// Use this for initialization
	void Start () {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            other.SendMessage("Checkpoint", SendMessageOptions.DontRequireReceiver);
            ParticleSystem.MainModule p_module = particles.main; //Warning - particles.startColor is deprecated
            p_module.startLifetime = 0.5f;
            Color g = Color.gray;
            g.a = 0.5f;
            p_module.startColor = g;
            gameObject.GetComponent<Renderer>().material.color = g;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
