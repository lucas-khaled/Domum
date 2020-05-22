using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour, IVulnerable
{

    [Header("Referências")]
    public Image lifeBar;
    public Animator anim;
    public NavMeshAgent NavMesh;
    public Vector3 posicaoInicial;
    public GameObject CBTprefab;
    public Item[] itensDropaveis;
    public Bau drop;

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
        Vida = maxVida;
        hitCanvas = transform.Find("Hit_life");
        posicaoInicial = this.transform.position;
    }

    //realiza o ataque do inimigo
    protected virtual IEnumerator Atacar()
    {
        if (Player.player.estadoPlayer == EstadoPlayer.MORTO)
        {
            yield break;
        }

        canAttack = false;

        int escolha = Random.Range(1, 3);
        if (escolha == 1)
            anim.SetTrigger("Ataque 1");
        else
            anim.SetTrigger("Ataque 2");
        
        yield return new WaitForSeconds(velocidadeAtaque);
        canAttack = true;
    }

    public void DoDamage()
    {
        Collider[] hit = Physics.OverlapSphere(hitPoint.position, 0.7f, LayerMask.GetMask("Player"));
        if (hit.Length > 0)
        {
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
   
    protected virtual void Morrer()
    {
        Destroy(this.gameObject, 10);
        anim.SetBool("Morreu", true);

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

        Debug.Log(numeroItens);

        Bau dropzera = Instantiate(drop.gameObject, transform.position, transform.rotation).GetComponent<Bau>();

        for (int i = 0; i <= numeroItens; i++)
        {
            int itemEsc = Random.Range(0, itensDropaveis.Length);
            dropzera.itens.Add(itensDropaveis[itemEsc]);
        }        
    }

    protected void Movimentar(Vector3 destino, bool move = true)
    {
        if (!move && NavMesh != null)
        {
            NavMesh.isStopped = true;
            return;
        }

        NavMesh.isStopped = false;
        NavMesh.SetDestination(destino);
    }

    protected virtual void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            anim.SetBool("Idle", false);
            bool mover = true;

            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);
            if (distancia <= distanciaAtaque)
            {
                anim.SetBool("PertoPlayer", true);
                mover = false;
                if(canAttack)
                  StartCoroutine(Atacar());        
            }
            else
            {
                anim.SetBool("PertoPlayer", false);
            }

            Movimentar(collider.transform.position, mover);
        }

    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Movimentar(posicaoInicial);
            ataqueCooldown = 0;
            anim.SetBool("Idle", true);
        }
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
