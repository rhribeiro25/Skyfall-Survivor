using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    SCRIPT ENCONTRADO NA INTERNET !
    Fonte: http://wiki.unity3d.com/index.php?title=MouseOrbitImproved#Code_C.23
*/

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour {

    [Tooltip("Objeto alvo para o foco da câmera")]
    [SerializeField]
    public Transform target;

    [Tooltip("Velocidade da movimentação em X da câmera")]
    [SerializeField]
    float xSpeed = 120.0f;

    [Tooltip("Velocidade da movimentação em Y da câmera")]
    [SerializeField]
    float ySpeed = 120.0f;

    [Tooltip("Altura mínima da movimentação da câmera")]
    [SerializeField]
    float yMinLimit = 0f;

    [Tooltip("Altura máxima da movimentação da câmera")]
    [SerializeField]
    float yMaxLimit = 80f;

    [Tooltip("Distância mínima da câmera para o alvo")]
    [SerializeField]
    public float distanceMin = .5f;

    [Tooltip("Distância máxima da câmera para o alvo")]
    [SerializeField]
    public float distanceMax = 15f;

    private Rigidbody rigidbody;
    float distance = 4.0f; //Distância para o alvo
    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
