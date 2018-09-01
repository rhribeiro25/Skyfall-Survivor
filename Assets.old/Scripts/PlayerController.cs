using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

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
    Vector3 v = new Vector3(0,3,0); //checkpoint
    Canvas canvas;

    [Header("Sons do Player")]
    [Tooltip("Som ao pular")]
    [SerializeField]
    AudioClip jumpSound;

    [Tooltip("Som ao cair")]
    [SerializeField]
    AudioClip fallSound;

    [Tooltip("Som ao ganhar")]
    [SerializeField]
    AudioClip winSound;

    [Tooltip("Som ao perder")]
    [SerializeField]
    AudioClip loseSound;

    AudioSource audio; //AudioSource to reproduce Audio clips


    [Header("Movimentação do Player")]
    [Tooltip("Indica o contato com o chão (Automático)")]
    [SerializeField]
    bool onGround = false;

    public Image lifeBar;
    float widthLifeBar;
    float heightLifeBar;
    public int maximumLife = 10;
    public int currentLife;

    [Tooltip("A velocidade de deslocamento frontal")]
    [Range(1, 50)]
    [SerializeField]
    float velocidadeRolamento = 10.0f;

    public void setVelocidadeRolamento(float velocidade)
    {
        this.velocidadeRolamento = velocidade;
    }

    public float getVelocidadeRolamento()
    {
        return this.velocidadeRolamento;
    }

    // Use this for initialization
    void Start()
    {
        currentLife = maximumLife;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody>();
        heightLifeBar = lifeBar.rectTransform.rect.height;
        widthLifeBar = lifeBar.rectTransform.rect.width;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Se o jogo estiver pausado nao faça nada
        if (PauseMenu.pause)
        {
            Time.timeScale = 0;
            return;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (currentLife == 0)
        {
            GameOver();            
        }
        var direcaoHorizontal = 0.0f;
        var direcaoVertical = 0.0f;

        //#if UNITY_STANDALONE

        /* Teclado */
        direcaoHorizontal = Input.GetAxis("Horizontal");
        direcaoVertical = Input.GetAxis("Vertical");
        Vector3 camera_frontDir = Camera.main.transform.forward;
        Vector3 camera_sideDir = Camera.main.transform.right;

        /* Mouse */
        if (Input.GetMouseButton(0))
        {           
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
            }
        }
        //#endif

        camera_frontDir.Normalize();
        camera_sideDir.Normalize();

        camera_frontDir = new Vector3(camera_frontDir.x * velocidadeRolamento * direcaoVertical, 0, velocidadeRolamento * direcaoVertical * camera_frontDir.z);
        camera_sideDir = new Vector3(camera_sideDir.x * velocidadeRolamento * direcaoHorizontal, 0, velocidadeRolamento * camera_sideDir.z * direcaoHorizontal);

        if (onGround == true){ //No chão, o movimento ocorre normalmente
            float constante = 20f;

            rb.AddForce(camera_frontDir * Time.deltaTime * constante);
            rb.AddForce(camera_sideDir * Time.deltaTime * constante);

            if (Input.GetKeyDown("space")) {
                if(!audio.isPlaying) audio.PlayOneShot(jumpSound);
                rb.AddForce(0, 300, 0);
            }
        }
        else{ //No ar, o movimento é complicado, portanto torna-se reduzido
            float constante = 10f;

            rb.AddForce(camera_frontDir * Time.deltaTime * constante);
            rb.AddForce(camera_sideDir * Time.deltaTime * constante);
        }

    }

    void OnCollisionStay(Collision other)
    {
        //checks to make sure the collision we collided with is the ground.
        //You will need to tag the terrain as ground
        if (other.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TheEnd")
        {
            YouWin();
        }
    }

    void OnCollisionExit(Collision otherExit)
    {

        //checks to make sure the collision we are no longer colliding with is the ground
        if (otherExit.gameObject.tag == "Ground")
        {
            onGround = false;
        }
        if(otherExit.gameObject.tag == "GameOver")
        {
            AudioSource.PlayClipAtPoint(fallSound, Camera.main.transform.position);
            gameObject.SetActive(false);
            Invoke("SetLastLocation", 3f);

            currentLife--;
            //Calculo para redimencionar a barra de vida
            lifeBar.rectTransform.sizeDelta = new Vector2(currentLife * widthLifeBar / maximumLife, heightLifeBar);
            //Alterando cor critica (30%) da barra de vida
            if ( lifeBar.rectTransform.rect.width * 100 / widthLifeBar <= 30)
            {
                lifeBar.color = Color.red;
            }
        }
    }

    public void Checkpoint()
    {
        v = transform.position;
        v = new Vector3(v.x, v.y + 1, v.z);
    }

    private void SetLastLocation()
    {
        rb.isKinematic = true;
        Invoke("Restart", 0.1f);
    }

    private void Restart()
    {
        gameObject.SetActive(true);
        rb.transform.position = v;
        rb.isKinematic = false;
    }

    static void touchObject(Vector3 posicaoClick)
    {
        Ray cliqueRay = Camera.main.ScreenPointToRay(posicaoClick);

        RaycastHit hit;

        if (Physics.Raycast(cliqueRay, out hit))
        {
            if (hit.transform.gameObject.tag == "Bridge")
            {
                hit.rigidbody.isKinematic = false;
               //hit.transform.SendMessage("ObjetoTocado", SendMessageOptions.DontRequireReceiver);
               //Destroy(hit.transform.gameObject); //Destroi outro objeto
            }
        }
    }

    /// <summary>
    /// Metodo para carregar uma scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowAds()
    {
    #if UNITY_ADS
                if (UnityAdControle.showAds)
                {
                    UnityAdControle.ShowAd();
                }
    #endif
    }

    void GameOver()
    {
        Camera.main.GetComponent<AudioSource>().Stop();
        if (!audio.isPlaying)
            audio.PlayOneShot(loseSound);
        PauseMenu.pause = true;
        canvas.transform.Find("GameOverPanel").gameObject.SetActive(true);
    }

    void YouWin()
    {
        audio.PlayOneShot(winSound);
        PauseMenu.pause = true;
        rb.isKinematic = true;
        canvas.transform.Find("YouWinPanel").gameObject.SetActive(true);
    }

    public void RevivePlayer()
    {
        currentLife = maximumLife;
    }
}
