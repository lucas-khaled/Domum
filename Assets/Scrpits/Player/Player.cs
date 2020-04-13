using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EstadoPlayer { NORMAL, COMBATE, ATACANDO, DANO, RECARREGAVEL }

public class Player : MonoBehaviour, IVulnerable
{

    private EstadoPlayer estado_player;

    [Header("Referências")]
    public Transform posicaoHit;
    public GameObject CBTprefab;

    //Adicionar Interagível

    [Header("Valores")]
    public int danoMedio;
    public const int MAXLEVEL = 100;
    public int maxVida;
    [SerializeField]
    private float velocidade = 2;
    [SerializeField]
    private int maxColetavel;
    [SerializeField]
    private float raioPercepcao;
    [SerializeField]
    private float raioAtaque = 2f;

    private int level;
    private int vida, qtnColetavel, dinheiro, experiencia;

    private Transform hitCanvas;

    #region GETTERS & SETTERS
    public EstadoPlayer estadoPlayer
    {
        get { return estado_player; }

        protected set
        {
            estado_player = value;
            if (value == EstadoPlayer.NORMAL)
            {
                InvokeRepeating("ProcuraInimigo", 0f, 0.2f); 
            }
            EventsController.onPlayerStateChanged.Invoke(estado_player);
        }
    }

    public int QntColetavel
    {
        get { return qtnColetavel; }

        set
        {
            qtnColetavel = Mathf.Clamp(value, 0, maxColetavel);
        }
    }

    public int Dinheiro
    {
        get { return dinheiro; }

        set
        {
            dinheiro = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public int Experiencia
    {
        get { return experiencia; }
        set
        {
            if (level < MAXLEVEL)
            {
                experiencia = value;
            }
        }
    }
    #endregion

    #region PreSettings
    public static Player player;
    protected virtual void Awake()
    {
        EventsController.onMorteInimigoCallback += OnMorteInimigo;
        player = this;
        vida = maxVida;
    }

    protected virtual void Start()
    {
        estadoPlayer = EstadoPlayer.NORMAL;
        hitCanvas = transform.Find("Hit_life");
    }
    #endregion

    #region COMBATE
    private IEnumerator Atacar()
    {
        estadoPlayer = EstadoPlayer.ATACANDO;

        // tocar animação de ataque

       
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioAtaque, LayerMask.GetMask("Inimigo"));

        if (hit.Length > 0)
        {
            int danin = CalculaDano();
            Debug.Log("Ataque: " + danin);
            hit[0].gameObject.GetComponent<Inimigo>().ReceberDano(danin);
        }

        yield return new WaitForSeconds(0.5f);

        estadoPlayer = EstadoPlayer.COMBATE;
    }

    public int CalculaDano()
    {
        return danoMedio + Random.Range(-5, 5);
    } 
    // Passei essa parte do calcula dano para o script de ArmaPlayer, não faz sentido o player calcular o dano que
    // vai ser passado no inimigo pelo script ArmaPlayer.
    // mantive comentado só pra você saber o que aconteceu.

    private void Morrer()
    {

    }

    public virtual void ReceberDano(int danoRecebido)
    {
        vida = Mathf.Clamp(vida - danoRecebido, 0, maxVida);
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);
        string nomeAnim = "Dano";

        if (vida <= 0)
        {
            nomeAnim = "Morte";
            Morrer();
        }

        StartCoroutine(Dano(nomeAnim));
        
    }

    private IEnumerator Dano(string animationName)
    {
        estadoPlayer = EstadoPlayer.DANO;
        //tocar animação
        yield return new WaitForSeconds(0.2f);
        estadoPlayer = EstadoPlayer.COMBATE;
    }

    protected Vector3 ProcuraInimigo()
    {
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioPercepcao, LayerMask.GetMask("Inimigo"));
        if (hit.Length > 0)
        {
            foreach (Collider inimigo in hit)
            {             
                    if (inimigo.GetComponent<Inimigo>().hostil && inimigo.gameObject.activeSelf)
                    {
                        estadoPlayer = EstadoPlayer.COMBATE;
                        CancelInvoke("ProcuraInimigo");
                        return inimigo.transform.position;
                    }
            }         
        }

        return Vector3.zero;

    }

    private IEnumerator ProcuraInimigoMorte()
    {
        yield return new WaitForSeconds(1);
        if (!Physics.CheckSphere(posicaoHit.position, raioPercepcao, LayerMask.GetMask("Inimigo")))
        {
            estadoPlayer = EstadoPlayer.NORMAL;
        }

    }
    #endregion

    protected virtual void Update()
    {
        Movimento();

        if (Input.GetMouseButtonDown(0) && estadoPlayer == EstadoPlayer.COMBATE)
        {
            StartCoroutine(Atacar());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interagir();
        }
    }

    void Movimento()
    {
        if (estadoPlayer != EstadoPlayer.ATACANDO)
        {


            transform.Translate(Vector3.right * velocidade * Input.GetAxis("Horizontal") * Time.deltaTime);
            transform.Translate(Vector3.forward * velocidade * Input.GetAxis("Vertical") * Time.deltaTime);

            /*if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-Vector3.forward * velocidade * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-Vector3.right * velocidade * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * velocidade * Time.deltaTime);
            }*/
        }
    }


    public void Curar(int cura)
    {

    }

    private void Interagir()
    {
        if (InteracaoController.instance.interagivelAtual != null)
            InteracaoController.instance.interagivelAtual.Interact();     
    }

    void OnMorteInimigo(int xp)
    {
        Experiencia += xp;
        StartCoroutine(ProcuraInimigoMorte());
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, raioPercepcao);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

}