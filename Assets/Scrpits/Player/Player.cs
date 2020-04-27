using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EstadoPlayer { NORMAL, COMBATE, ATACANDO, DANO, RECARREGAVEL }

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(Collider))] [RequireComponent(typeof(StatusPlayer))]
public class Player : MonoBehaviour, IVulnerable
{

    private EstadoPlayer estado_player;

    [HideInInspector]
    public StatusPlayer status;

    [Header("Referências")]
    public Transform posicaoHit;
    public GameObject CBTprefab;

    [Header("Valores")]
    [SerializeField]
    private float velocidade = 2;
    [SerializeField]
    private float raioPercepcao;
    [SerializeField]
    private float raioAtaque = 2f;

    protected Rigidbody rb;

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
    #endregion

    #region PreSettings
    public static Player player;
    protected virtual void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        status = GetComponent<StatusPlayer>();

        EventsController.onMorteInimigoCallback += OnMorteInimigo;

        player = this;     
    }

    protected virtual void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        estadoPlayer = EstadoPlayer.NORMAL;
        hitCanvas = transform.Find("Hit_life");
    }
    #endregion

    #region COMBATE
    private void MoverPlayerAtaque()
    {
        //transform.LookAt((transform.forward * velocidade * Input.GetAxis("Vertical")) + (transform.right * velocidade * Input.GetAxis("Horizontal")) + player.transform.position);
        rb.velocity = (transform.forward * 3);
    }

    private IEnumerator Atacar()
    {
        estadoPlayer = EstadoPlayer.ATACANDO;

        // tocar animação de ataque

        MoverPlayerAtaque();

        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioAtaque, LayerMask.GetMask("Inimigo"));

        if (hit.Length > 0)
        {
            int danin = CalculaDano();
            hit[0].gameObject.GetComponent<Inimigo>().ReceberDano(danin);
        }

        yield return new WaitForSeconds(0.5f);

        estadoPlayer = EstadoPlayer.COMBATE;
    }

    public int CalculaDano()
    {
        return status.DanoMedio + Random.Range(-5, 5);
    } 
    // Passei essa parte do calcula dano para o script de ArmaPlayer, não faz sentido o player calcular o dano que
    // vai ser passado no inimigo pelo script ArmaPlayer.
    // mantive comentado só pra você saber o que aconteceu.

    private void Morrer()
    {

    }

    public virtual void ReceberDano(int danoRecebido)
    {
        status.Vida -= danoRecebido;
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);
        string nomeAnim = "Dano";

        if (status.Vida <= 0)
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

    /*void VerificarColetaveis()
    {
        
    }*/

    protected virtual void Update()
    {
        Movimento();
        //VerificarColetaveis();       

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
            rb.velocity = (transform.forward * velocidade * Input.GetAxis("Vertical")) + (transform.right * velocidade * Input.GetAxis("Horizontal"));
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
        status.Experiencia += xp;
        StartCoroutine(ProcuraInimigoMorte());
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, raioPercepcao);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

}