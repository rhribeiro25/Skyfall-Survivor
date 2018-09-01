using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController: MonoBehaviour
{

    public enum TipoMovimentoHorizontal { acelerometro, touch }

    public TipoMovimentoHorizontal movimentoHorizontal = TipoMovimentoHorizontal.touch;
   
    Rigidbody rb;                               //Referencia para a variavel rigidbody
    float widthLifeBar;                         //Tamanho inicial da barra de vida
    float heightLifeBar;                        //Altura inicial da barra de vida
    public int maximumLife = 10;                //Vida maxima do jogador
    public int currentLife;                     //Vida atual do jogador
    Vector3 touchInicio;                        //
    Vector3 v = new Vector3(0.08f, 3f,-6.37f);  //checkpoint
    Canvas canvas;                              //Canvas da sena atual
    AudioSource audio;                          //AudioSource to reproduce Audio clips

    [Tooltip("Imagem para a barra de vida")]
    [SerializeField]
    Image lifeBar;

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

    [Header("Movimentação do Player")]
    [Tooltip("Indica o contato com o chão (Automático)")]
    [SerializeField]
    bool onGround = false;

    [Tooltip("A velocidade de deslocamento frontal")]
    [Range(1, 50)]
    [SerializeField]
    float velocidadeRolamento = 10.0f;

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
        if (PauseMenu.onPause)
        {
            return;
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

    /// <summary>
    /// Metodo chamado na colisao da bolinha
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionStay(Collision other)
    {
        //checks to make sure the collision we collided with is the ground.
        //You will need to tag the terrain as ground
        if (other.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }

    /// <summary>
    /// Metodo chamado quando a bolinha ultrapassa o trigger da platafomrma (queda)
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TheEnd")
        {
            YouWin();
        }
        if (other.gameObject.tag == "GameOver")
        {
            AudioSource.PlayClipAtPoint(fallSound, Camera.main.transform.position);
            Invoke("SetLastLocation", 2f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="otherExit"></param>
    void OnCollisionExit(Collision otherExit)
    {

        //checks to make sure the collision we are no longer colliding with is the ground
        if (otherExit.gameObject.tag == "Ground")
        {
            onGround = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Checkpoint()
    {
        v = transform.position;
        v = new Vector3(v.x, v.y + 1, v.z);
    }

    private void SetLastLocation()
    {
        LifeController();
        
        rb.isKinematic = true;
        Invoke("Restart", 0.1f);
    }

    /// <summary>
    /// Controla o ciclo de vida do jogador
    /// </summary>
    private void LifeController()
    {
        currentLife--;
        BarControl();
    }

    /// <summary>
    /// Consede metade da vida total ao jogador ao reviver (Após ver o anuncio)
    /// </summary>
    private void Restart()
    {
        if (currentLife == 0) currentLife = maximumLife/2;
        BarControl();
        gameObject.SetActive(true);
        rb.transform.position = v;
        rb.isKinematic = false;
    }

    /// <summary>
    /// Controla as cores da barra de vida, sendo vermelho <= 30% e verde > 70%
    /// </summary>
    private void BarControl()
    {
        //Calculo para redimencionar a barra de vida
        lifeBar.rectTransform.sizeDelta = new Vector2(currentLife * widthLifeBar / maximumLife, heightLifeBar);
        //Alterando cor critica (30%) da barra de vida
        if (lifeBar.rectTransform.rect.width * 100 / widthLifeBar <= 30)
        {
            lifeBar.color = Color.red;
        }
        else
        {
            lifeBar.color = Color.green;
        }
        if (currentLife == 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="posicaoClick"></param>
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

    /// <summary>
    /// Metodo chamado ao atingir 0 de vida
    /// </summary>
    void GameOver()
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<AudioSource>().Stop();
        if (!audio.isPlaying)
            audio.PlayOneShot(loseSound);
        PauseMenu.onPause = true;
        canvas.transform.Find("GameOverPanel").gameObject.SetActive(true);
        canvas.transform.Find("ButtonPause").gameObject.SetActive(false);
    }

    /// <summary>
    /// Metodo chamado ao chegar na etapa final da fase
    /// </summary>
    void YouWin()
    {
        audio.PlayOneShot(winSound);
        PauseMenu.onPause = true;
        rb.isKinematic = true;
        canvas.transform.Find("YouWinPanel").gameObject.SetActive(true);
        canvas.transform.Find("ButtonPause").gameObject.SetActive(false);
    }

    //Getters e Setters
    public void setVelocidadeRolamento(float velocidade)
    {
        this.velocidadeRolamento = velocidade;
    }

    public float getVelocidadeRolamento()
    {
        return this.velocidadeRolamento;
    }
}
