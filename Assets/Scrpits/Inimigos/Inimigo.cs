using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour, IVulnerable
{
    [Header("Partículas")]
    [SerializeField]
    private ParticleSystem hitInimigo;

    [Header("Referências")]
    public Image lifeBar;
    public Animator anim;
    public NavMeshAgent NavMesh;
    
    public GameObject CBTprefab;
    public Item[] itensDropaveis;
    public Bau drop;

    [Header("Audios")]
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    private AudioClip passos;

    private double audioAux;
    private Vector3 posicaoInicial;

    //Criar Array de Itens para ele dropar
    [Header("Valores")]
    public bool hostil;
    [SerializeField]
    private int maxVida;
    [SerializeField]
    private int danoMedio;
    [SerializeField]
    private float velocidade;
    [SerializeField]
    protected float distanciaAtaque;
    [SerializeField]
    private int vida;
    [SerializeField]
    protected float velocidadeAtaque;
    [SerializeField]
    protected float velocidadeAnim;
    [SerializeField]
    private int experienciaMorte;
    private Transform hitCanvas;

    [SerializeField]
    protected Transform hitPoint;

    protected bool canAttack = true;

    protected float ataqueCooldown;

    protected bool goHome = false;

    protected bool move;

    [HideInInspector]
    public bool morto = false;

    public int Vida
    {
        get { return vida; }

        private set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
            UIController.uiController.LifeBar(lifeBar,((float)vida / maxVida));
        }
    }

    private void Awake()
    {
        // checa se há um canvas como filho, se não houver, cria um filho worldSpace Canvas e o nomea adequadamente.
        if(transform.GetComponentInChildren<Canvas>() == null)
        {
            GameObject hitLife = new GameObject();
            hitLife.transform.SetParent(this.transform);
            hitLife.transform.position = new Vector3(0, 1, 0);
            hitLife.name = "Hit_Life";
            hitLife.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        }
    }

    private void Start()
    {
        //travaMovimento = false;
        Vida = maxVida;
        hitCanvas = transform.Find("Hit_life");
        posicaoInicial = this.transform.position;
        InvokeRepeating("CheckIfHitLifeMustBeActive", 0.5f, 1f);
    }

    void CheckIfHitLifeMustBeActive()
    {
        if (gameObject.activeInHierarchy)
        {
            if (hostil)
            {
                if (Vector3.Distance(Player.player.transform.position, transform.position) > 7)
                {
                    hitCanvas.gameObject.SetActive(false);
                }
                else
                {
                    hitCanvas.gameObject.SetActive(true);
                }
            }
            else
            {
                hitCanvas.gameObject.SetActive(false);
            }
        }
    }

    //realiza o ataque do inimigo
    protected virtual IEnumerator Atacar()
    {

        if (Player.player.estadoPlayer == EstadoPlayer.MORTO)
        {
            yield break;
        }

        canAttack = false;
       // travaMovimento = true;

        int escolha = Random.Range(1, 3);
        if (escolha == 1)
            anim.SetTrigger("Ataque 1");
        else
            anim.SetTrigger("Ataque 2");

        //yield return new WaitForSeconds(0.5f);
       // travaMovimento = false;

        yield return new WaitForSeconds(velocidadeAtaque);
        canAttack = true;

    }

    public void DoDamage()
    {
        Collider[] hit = Physics.OverlapSphere(hitPoint.position, 0.7f, LayerMask.GetMask("Player"));
        if (hit.Length > 0)
        {
            if(hitInimigo != null)
            {
                ParticleSystem particula = Instantiate(hitInimigo.gameObject, hit[0].transform.position, hitInimigo.transform.rotation).GetComponent<ParticleSystem>();
                particula.Play();
                Destroy(particula.gameObject, 5);
            }
                
            hit[0].gameObject.GetComponent<Player>().ReceberDano(danoMedio, this);
        }
    }


    public virtual void ReceberDano(int danoRecebido, Inimigo inim = null)
    {
        if (morto)
        {
            return;
        }

        Vida -= danoRecebido;
        
        //chama metodo do UIController para exibir o dano no worldCanvas
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);

        StopAllCoroutines();
        
        if (vida <= 0)
        {
            anim.SetTrigger("Tomar Dano");
            anim.SetBool("Morreu", true);
            Morrer();
        }
        else
        {
            anim.SetTrigger("Tomar Dano");
        }
        
    }
   
    public virtual void Morrer()
    {
        Destroy(this.gameObject, 10);
        anim.SetBool("Morreu", true);
        CancelInvoke("CheckIfHitLifeMustBeActive");

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<SphereCollider>());

        foreach(FaceCamera destruir in GetComponentsInChildren(typeof(FaceCamera), true))
        {
            Destroy(destruir.gameObject);
        }

        morto = true;
        DroparLoot();

        //AQUI O DELEGATE DE MORTE DO INIGO É CHAMADO E TODOS OS MÉTODOS INSCRITOS NESTE DELEGATE SERÃO CHAMADOS
        if (EventsController.onMorteInimigoCallback != null)
        {
            EventsController.onMorteInimigoCallback.Invoke(experienciaMorte);
        }
    }

    void DroparLoot()
    {
        int numeroItens = Random.Range(0, 4);

        GameObject dropzera = Instantiate(drop.gameObject, transform.position, transform.rotation);
        Destroy(dropzera, 30);

        for (int i = 0; i <= numeroItens; i++)
        {
            int itemEsc = Random.Range(0, itensDropaveis.Length);
            dropzera.GetComponent<Bau>().itens.Add(itensDropaveis[itemEsc]);
        }        
    }

    protected void Movimentar(Vector3 destino, bool move = true)
    {
        //if (!travaMovimento)
        //{
            if (!move && NavMesh != null)
            {
                NavMesh.isStopped = true;
                return;
            }

            NavMesh.isStopped = false;
            NavMesh.SetDestination(destino);
       // }
        //else
            //anim.SetBool("Idle", false);
    }

    protected void FaceTarget()
    {
        Vector3 direcao = (Player.player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*5);
    }

    protected virtual void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            anim.SetBool("Idle", false);

            move = true;
            goHome = false;

            if (audioAux <= 0 && distancia > distanciaAtaque)
            {
                audioSource.volume = Random.Range(0.8f, 1);
                audioSource.pitch = Random.Range(0.8f, 1.1f);
                audioSource.PlayOneShot(passos);
                audioAux = 0.5f;
            }
            else audioAux -= Time.deltaTime;

            if (distancia <= distanciaAtaque)
            {
                anim.SetBool("PertoPlayer", true);
                move = false;
                FaceTarget();
                if (canAttack)
                  StartCoroutine(Atacar());        
            }
            else
            {
                anim.SetBool("PertoPlayer", false);
            }

            Movimentar(collider.transform.position, move);
        }

    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Movimentar(posicaoInicial);
            //travaMovimento = false;
            ataqueCooldown = 0;
            StartCoroutine(SetIdle());

        }
    }

    IEnumerator SetIdle()
    {
        goHome = true;
        while (Vector3.Distance(transform.position, posicaoInicial) > 0.5f)
        {
            if (!goHome)
                yield break;

            yield return new WaitForSeconds(0.3f);
        }
        anim.SetBool("Idle", true);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, distanciaAtaque);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(hitPoint.position, 0.7f);
    }
}
