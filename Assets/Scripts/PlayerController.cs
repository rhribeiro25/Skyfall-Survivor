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
    Vector3 v = new Vector3(0.08f, 3f,-6.37f);  //Posição da bola atualizada a cada checkpoint
    Canvas canvas;                              //Canvas da sena atual
    AudioSource audio;                          //AudioSource to reproduce Audio clips

    [Tooltip("Imagem para a barra de vida")]
    [SerializeField]
    Image lifeBar;

    [Header("Sons do Player")]
    [Tooltip("Som ao pular")]
    [SerializeField]
    AudioClip jumpSound;

    [Tooltip("Som ao derrubar ponte")]
    [SerializeField]
    AudioClip bridgeSound;

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

        var direcaoHorizontal = 0.0f; //Inicialização
        var direcaoVertical = 0.0f;

        //#if UNITY_STANDALONE

        /* Teclado */
        direcaoHorizontal = Input.GetAxis("Horizontal");
        direcaoVertical = Input.GetAxis("Vertical");
        // Pega a referencia da direção da camera (para que WASD funcione independente da posição da mesma)
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
        else /* Touch */ //Não utilizado, mas deixado para futuras implementações
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

        //Normalização dos vetores de camera
        camera_frontDir.Normalize();
        camera_sideDir.Normalize();

        //Calculo das forças horizontal e vertical
        camera_frontDir = new Vector3(camera_frontDir.x * velocidadeRolamento * direcaoVertical, 0, velocidadeRolamento * direcaoVertical * camera_frontDir.z);
        camera_sideDir = new Vector3(camera_sideDir.x * velocidadeRolamento * direcaoHorizontal, 0, velocidadeRolamento * camera_sideDir.z * direcaoHorizontal);

        if (onGround == true){ //No chão, o movimento ocorre normalmente
            float constante = 20f;

            //Aplicação das forças
            rb.AddForce(camera_frontDir * Time.deltaTime * constante);
            rb.AddForce(camera_sideDir * Time.deltaTime * constante);

            //APlicação da força para cima (pulo)
            if (Input.GetKeyDown("space")) {
                if(!audio.isPlaying) audio.PlayOneShot(jumpSound);
                rb.AddForce(0, 300, 0);
            }
        }
        else{ //No ar, o movimento é complicado, portanto torna-se reduzido
            float constante = 10f;

            //Aplicação de força enquanto no ar
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
        //Caso em colisão com o chão
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
            YouWin(); //Caso vença
        }
        if (other.gameObject.tag == "GameOver") //Caso passe pelo limite inferior da fase = caiu da plataforma
        {
            AudioSource.PlayClipAtPoint(fallSound, Camera.main.transform.position);
            Invoke("SetLastLocation", fallSound.length); //Delay coerente com o tempo do audio
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="otherExit"></param>
    void OnCollisionExit(Collision otherExit)
    {

        //Se estiver pulando, não estará em contato com o chão
        if (otherExit.gameObject.tag == "Ground")
        {
            onGround = false;
        }
    }

    /// <summary>
    /// Função chamada para salvar a posição da bola ao passar por um checkpoint
    /// </summary>
    public void Checkpoint()
    {
        v = transform.position;
        v = new Vector3(v.x, v.y + 1, v.z);
    }

    /// <summary>
    /// Seta o local da bola após queda
    /// </summary>
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
        else //Garante a cor verde caso a barra seja maior de 30%
        {
            lifeBar.color = Color.green;
        }
        if (currentLife == 0) //Se não tiver mais vidas = Game Over
        {
            GameOver();
        }
    }

    /// <summary>
    ///  Controla o toque sobre os objetos de ponte
    /// </summary>
    /// <param name="posicaoClick"></param>
    private void touchObject(Vector3 posicaoClick)
    {
        Ray cliqueRay = Camera.main.ScreenPointToRay(posicaoClick);

        RaycastHit hit;

        if (Physics.Raycast(cliqueRay, out hit))
        {
            if (hit.transform.gameObject.tag == "Bridge")
            {
                audio.PlayOneShot(bridgeSound);
                hit.rigidbody.isKinematic = false;
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
