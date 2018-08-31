using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

    [Tooltip("Velocidade do deslocamento")]
    [Range(0.001f,0.5f)]
    [SerializeField]
    public float speed = 0.1f;

    [Tooltip("Delay entre as inversões de movimento")]
    [Range(0, 5)]
    [SerializeField]
    public float delay = 0.3f;

    [Tooltip("Eixo de deslocamento 1=X, 2=Y e 3=Z")]
    [Range(1, 3)]
    [SerializeField]
    public int axis = 1;

    private float speed_backup; //Guarda a velocidade para que seja restaurada

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ground")
        {
            speed_backup = speed;
            speed = 0;
            Invoke("revert", delay);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        print(collision.gameObject.name);
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        Vector3 v = transform.position;

        if (axis == 1) v.x += speed;
        else if (axis == 2) v.y += speed;
        else if (axis == 3) v.z += speed;

        transform.position = v;
    }

    void revert()
    {
        speed = speed_backup;
        speed *= -1;
    }

}
