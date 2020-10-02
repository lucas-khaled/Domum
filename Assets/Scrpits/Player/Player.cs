using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EstadoPlayer { NORMAL, COMBATE, ATACANDO, DANO, RECARREGAVEL, MORTO, INTERAGINDO }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(StatusPlayer))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IVulnerable
{
    protected Animator animator;
    private EstadoPlayer estado_player;

    [HideInInspector]
    public StatusPlayer status;

    [Header("Audio Players")]
    [SerializeField]
    private AudioClip movimento;
    public AudioClip ataque;
    public AudioSource audioSource;
    public string audioNovo;

    [Header("Referências")]
    [SerializeField]
    private ParticleSystem particulaArma;
    public Transform posicaoHit;
    public GameObject CBTprefab;

    [Header("Valores")]
    [SerializeField]
    private float velocidade = 2;
    [SerializeField]
    private float turnSmooth = 0.3f;
    [SerializeField]
    private float raioPercepcao;
    [SerializeField]
    private float raioAtaque = 2f;
    [SerializeField]
    private float espera;



    protected Rigidbody rb;
    protected bool podeAtacar = true;
    protected int numClick = 0;

    float turnVelocity;
    private Transform hitCanvas;

    #region GETTERS & SETTERS

    public EstadoPlayer estadoPlayer
    {
        get { return estado_player; }

        set
        {
            if (particulaArma != null)
            {
                if (value == EstadoPlayer.ATACANDO)
                {
                    particulaArma.gameObject.SetActive(true);
                    particulaArma.Play();
                }
                else
                {
                    particulaArma.gameObject.SetActive(false);
                    particulaArma.Stop();
                }
            }

            if (status.Vida <= 0)
            {
                estado_player = EstadoPlayer.MORTO;
                //UIController.uiController.PainelMorteOn();
                return;
            }

            estado_player = value;

            if (EventsController.onPlayerStateChanged != null)
            {
                EventsController.onPlayerStateChanged.Invoke(estado_player);
            }
        }
    }
    #endregion

    #region PreSettings
    public static Player player;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        status = GetComponent<StatusPlayer>();

        EventsController.onPausedGame += OnGamePaused;
        EventsController.onMorteInimigoCallback += OnMorteInimigo;

        player = this;
    }

    protected virtual void Start()
    {
        estadoPlayer = EstadoPlayer.NORMAL;
        hitCanvas = transform.Find("Hit_life");
    }
    #endregion

    #region COMBATE
    protected void MoverPlayerAtaque()
    {
        //transform.LookAt((transform.forward * velocidade * Input.GetAxis("Vertical")) + (transform.right * velocidade * Input.GetAxis("Horizontal")) + player.transform.position);
        rb.velocity = (transform.forward * 5);
    }

    protected IEnumerator WaitForAnimation(string animacao)
    {
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animacao))
        {
            yield return null;
        }

    }

    private void Atacar()
    {
        if (podeAtacar && numClick < status.NumAtaque)
        {
            numClick++;
        }

        if (numClick == 1)
        {
            animator.SetInteger("Ataque", 1);
            animator.applyRootMotion = false;
            estadoPlayer = EstadoPlayer.ATACANDO;
        }
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
        estadoPlayer = EstadoPlayer.MORTO;
    }

    public virtual void ReceberDano(int danoRecebido, Inimigo inim = null)
    {
        status.Vida -= danoRecebido;
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);
        string nomeAnim = "Dano";

        if (danoRecebido > status.maxVida * 30 / 100)
        {
            nomeAnim += "Forte";
        }


        if (status.Vida <= 0)
        {
            nomeAnim = "Morte";
        }

        animator.applyRootMotion = true;
        StartCoroutine(Dano(nomeAnim));

    }

    private IEnumerator Dano(string animationName)
    {
        if (!(estadoPlayer == EstadoPlayer.ATACANDO) && !(estadoPlayer == EstadoPlayer.INTERAGINDO) && !(estadoPlayer == EstadoPlayer.RECARREGAVEL))
        {
            estadoPlayer = EstadoPlayer.DANO;
            animator.SetTrigger(animationName);
            yield return WaitForAnimation(animationName);
        }

        if (animationName == "Morte")
        {
            Morrer();
        }
        else
        {
            estadoPlayer = EstadoPlayer.COMBATE;
        }

    }

    protected Vector3 ProcuraInimigo()
    {
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioPercepcao, LayerMask.GetMask("Inimigo"));

        if (hit.Length > 0)
        {
            foreach (Collider inimigo in hit)
            {
                if (inimigo.GetComponent<Inimigo>().hostil && !inimigo.GetComponent<Inimigo>().morto)
                {
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

    public void ResetCanAttack()
    {
        podeAtacar = true;
        numClick = 0;
        animator.applyRootMotion = false;

        if (estadoPlayer == EstadoPlayer.INTERAGINDO)
            return;

        if (ProcuraInimigo() == Vector3.zero)
        {
            estadoPlayer = EstadoPlayer.NORMAL;
        }

        else
        {
            estadoPlayer = EstadoPlayer.COMBATE;
        }
    }


    #endregion

    protected virtual void Update()
    {
        Movimento();

        if (Input.GetButtonDown("Attack") && (estadoPlayer != EstadoPlayer.RECARREGAVEL && estadoPlayer != EstadoPlayer.MORTO) && podeAtacar && Time.timeScale != 0)
        {
            Atacar();
        }

        if (Input.GetButtonDown("Interact"))
        {
            Interagir();
        }

    }

    void Movimento()
    {
        if (estadoPlayer == EstadoPlayer.NORMAL || estadoPlayer == EstadoPlayer.COMBATE)
        {
            animator.applyRootMotion = false;
            float yAxis = Input.GetAxis("Vertical");
            float xAxis = Input.GetAxis("Horizontal");

            float y = Mathf.Lerp(animator.GetFloat("VetY"), yAxis, 0.4f);
            float x = Mathf.Lerp(animator.GetFloat("VetX"), xAxis, 0.4f);

            if (yAxis != 0 || xAxis != 0)
                Passos.passos.caminhando = true;
            else
                Passos.passos.caminhando = false;

            Vector3 dir = (transform.forward * y) + (transform.right * x);
            Vector3 clampedDir = dir;
            if (dir.magnitude > 1)
                clampedDir = dir.normalized;

            if(xAxis != 0 || yAxis != 0)
            {
                if (VerifyCamAlignment())
                {
                    TurnPlayer();
                }
            }

            transform.position += (clampedDir * velocidade) * Time.deltaTime;

            animator.SetFloat("VetX", x);
            animator.SetFloat("VetY", y);
        }
    }

    bool VerifyCamAlignment()
    {
        float camAngle = CameraController.cameraInstance.GetTarget().eulerAngles.y;
        float playerAngle = transform.eulerAngles.y;
        float difference = camAngle - playerAngle;

        if(difference > 10 || difference < -10)
        {
            return true;
        }

        return false;
    }

    void TurnPlayer(bool attack = false)
    {
        float realSmooth = (attack) ? turnSmooth * 100 : turnSmooth;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, CameraController.cameraInstance.GetTarget().eulerAngles.y, ref turnVelocity, realSmooth);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void Curar(int cura)
    {
        status.Vida += cura;
    }

    private void Interagir()
    {
        InteracaoController.instance.Interact();
    }

    void OnMorteInimigo(int xp)
    {
        status.Experiencia += xp;
        StartCoroutine(ProcuraInimigoMorte());
    }

    public void SetPlayerOnIdle()
    {
        animator.SetFloat("VetY", 0);
        animator.SetFloat("VetX", 0);
    }

    void OnGamePaused()
    {
        animator.SetInteger("Ataque", 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, raioPercepcao);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 2);

        /*Gizmos.DrawWireSphere(pe1.transform.position, raiope);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2);

        Gizmos.DrawWireSphere(pe2.transform.position, raiope);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2);*/

        Gizmos.DrawWireSphere(posicaoHit.transform.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2);
    }



}