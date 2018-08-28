using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

    [Tooltip("Velocidade do deslocamento")]
    [Range(0.01f,2f)]
    [SerializeField]
    public float speed = 0.1f;

    [Tooltip("Eixo de deslocamento 1=X, 2=Y e 3=Z")]
    [Range(1, 3)]
    [SerializeField]
    public int axis = 1;

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ground")
        {
            speed *= -1;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        print(collision.gameObject.name);
    }

    void Update()
    {
        Vector3 v = transform.position;

        if (axis == 1) v.x += speed;
        else if (axis == 2) v.y += speed;
        else if (axis == 3) v.z += speed;

        transform.position = v;
    }

}
