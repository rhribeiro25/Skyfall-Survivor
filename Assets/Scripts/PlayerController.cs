using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController: MonoBehaviour
{

    public enum TipoMovimentoHorizontal { acelerometro, touch }

    public TipoMovimentoHorizontal movimentoHorizontal = TipoMovimentoHorizontal.touch;


    /// <summary>
    /// Referencia para a variavel rigidbody
    /// </summary>
    Rigidbody rb;

    Vector3 touchInicio;

    [Tooltip("Indica o contato com o chão (Automático)")]
    [SerializeField]
    bool onGround = false;

    [Tooltip("A velocidade de deslocamento frontal")]
    [Range(1, 50)]
    [SerializeField]
    float velocidadeRolamento = 10.0f;

    [Header("Membros responsáveis pelo swipe")]
    [Tooltip("Variável para determinar a distância mínima de swipe")]
    [SerializeField]
    float minDisSwipe = 200.0f;

    [Tooltip("Variável para determinar amplitude do swipe")]
    [SerializeField]
    float swipeMove = 2.0f;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (MenuPause.onPause)
        //{
        //    return;
        //}

        var direcaoHorizontal = 0.0f;
        var direcaoVertical = 0.0f;

        //#if UNITY_STANDALONE
        /* Teclado */
        direcaoHorizontal = Input.GetAxis("Horizontal");
        direcaoVertical = Input.GetAxis("Vertical");

        /* Mouse */
        if (Input.GetMouseButton(0))
        {
            //var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            //if(pos.x < 0.5)
            //{
            //direcaoHorizontal = -1;
            //} else
            //{
            //direcaoHorizontal = 1;
            //}

            touchObject(Input.mousePosition);
        }

        //#elif UNITY_IOS || UNITY_ANDROID

        
        if (movimentoHorizontal == TipoMovimentoHorizontal.acelerometro) /* Acelerômetro */
        {
            direcaoHorizontal = Input.acceleration.x;
        }
        else /* Touch */
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.touches[0];

                var pos = Camera.main.ScreenToViewportPoint(t.position);

                if (pos.x < 0.5)
                {
                    direcaoHorizontal = -1;
                }
                else
                {
                    direcaoHorizontal = 1;
                }

                //Verify if a swipe occurs
                //SwipeTeleport(t);
            }
        }
        //#endif

        float constante = 20;
        Vector3 velocidade = new Vector3(velocidadeRolamento * direcaoHorizontal, 0, velocidadeRolamento * direcaoVertical);
        rb.AddForce(velocidade * Time.deltaTime * constante);

        if (Input.GetKeyDown("space") && onGround == true){
            rb.AddForce(0, 300, 0);
        }

    }

    void OnCollisionEnter(Collision other)
    {

        //checks to make sure the collision we collided with is the ground.
        //You will need to tag the terrain as ground
        if (other.gameObject.tag == "ground")
        {
            print("we are on the ground");
            onGround = true;
        }
    }

    void OnCollisionExit(Collision otherExit)
    {

        //checks to make sure the collision we are no longer colliding with is the ground
        if (otherExit.gameObject.tag == "ground")
        {
            print("we are off the ground");
            onGround = false;
        }

    }

    /// <summary>
    /// Método para tratar o Swipe
    /// </summary>
    /// <param name="t"></param>
    void SwipeTeleport(Touch t)
    {
        //Verify if this is the swipe start point
        if (t.phase == TouchPhase.Began)
        {
            touchInicio = t.position;
        }
        else if (t.phase == TouchPhase.Ended)
        {
            Vector3 touchFim = t.position;
            Vector3 dir;

            float dif = touchFim.x - touchInicio.x;

            if (Mathf.Abs(dif) >= minDisSwipe)
            {

                if (dif < 0)
                {
                    dir = Vector3.left;
                }
                else
                {
                    dir = Vector3.right;
                }

                RaycastHit hit;
                if (!rb.SweepTest(dir, out hit, swipeMove))
                {
                    rb.MovePosition(rb.position + (dir * swipeMove));
                }

            }
            else
            {
                return;
            }
        }

    }

    static void touchObject(Vector3 posicaoClick)
    {
        Ray cliqueRay = Camera.main.ScreenPointToRay(posicaoClick);

        RaycastHit hit;

        if (Physics.Raycast(cliqueRay, out hit))
        {
            //if (hit.transform.GetComponent<Obsctacle>())
            //{
            //    hit.transform.SendMessage("ObjetoTocado", SendMessageOptions.DontRequireReceiver);
            //    Destroy(hit.transform.gameObject);
            //}
        }
    }
}
