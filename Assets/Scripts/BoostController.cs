using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostController : MonoBehaviour {

    PlayerController player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Metodo responsável por aumentar a velocidade de rolamento
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            if (player.getVelocidadeRolamento() == 10f)
            { //Está na velocidade padrão, aplique o boost
                other.GetComponent<PlayerController>().setVelocidadeRolamento(20f);
                Invoke("ResetSpeed", 3); //Após 3 segundos reseta
            }
        }
    }

    /// <summary>
    /// Metodo responsável por restaurar a velocidade de rolamento (após 3s)
    /// </summary>
    private void ResetSpeed()
    {
        player.setVelocidadeRolamento(10f);
    }

}
