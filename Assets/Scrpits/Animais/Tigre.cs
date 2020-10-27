using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Tigre : MonoBehaviour
{
    [Header("Referências")]
    public Image lifeBar;
    public Animator anim;
    public GameObject CBTprefab;
    private NavMeshAgent animal;
    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;
    public Item[] itensDropaveis;
    public Bau drop;

    [Header("Audios")]
    [SerializeField]
    private AudioClip garrada;
    [SerializeField]
    private AudioClip mordida;
    [SerializeField]
    private AudioClip ataquePulo;
    [SerializeField]
    private AudioClip rosnado;
    [SerializeField]
    private AudioSource audioSource;

    //Criar Array de Itens para ele dropar
    [Header("Valores")]
    [SerializeField]
    private int maxVida;
    [SerializeField]
    private float velocidade;
    [SerializeField]
    private int vida;
    [SerializeField]
    private int experienciaMorte;
    [SerializeField]
    private int danoMedio;
    [SerializeField]
    private GameObject boca;
    [SerializeField]
    private GameObject pata;
    [SerializeField]
    private int distanciaAtaque;
    [SerializeField]
    private int cooldown;
    private bool morto;
    private bool aux;

    private GameObject[] respawnTigre;
    private Transform hitCanvas;

    bool canAttack = true;

    #region COMBATE
    private IEnumerator jumpAttack()
    {
        anim.SetTrigger("Atacar");
        Collider[] hit = Physics.OverlapSphere(boca.transform.position, distanciaAtaque, LayerMask.GetMask("Player"));
        if (hit.Length > 0)
        {
            hit[0].gameObject.GetComponent<Player>().ReceberDano(danoMedio);
        }

        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
    #endregion

    private void Start()
    {
        animal = this.GetComponent<NavMeshAgent>();
        respawnTigre = GameObject.FindGameObjectsWithTag("RespawnT");
        anim.SetBool("Idle", true);
        StartCoroutine(Escolher());
        Vida = maxVida;
        hitCanvas = transform.Find("Hit_life");
        anim.SetFloat("Vida", maxVida);
    }

    private void Update()
    {
        //Debug.Log(Vector3.Distance(this.gameObject.transform.position, destino));
        if (Vector3.Distance(this.gameObject.transform.position, destino) < 1f )
        {
            Debug.Log("Tico e teco");
            animal.isStopped = true;
            anim.SetBool("Caminhando", false);
            anim.SetBool("Idle", true);
            aux = false;
        }
    }
    protected virtual void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            anim.SetBool("Caminhando", false);

            float distancia = Vector3.Distance(collider.transform.position, this.transform.position);

            anim.SetFloat("Distancia", distancia);
            anim.SetBool("Idle", false);

            if (distancia > 3f)
            {
                GetComponent<NavMeshAgent>().speed = 3f;
                anim.SetBool("Correndo", true);
            }

            if (Vector3.Distance(this.gameObject.transform.position, collider.transform.position) > 2.5f)
                    animal.SetDestination(collider.transform.position);

            if (distancia <= 3f && anim.GetBool("Correndo") && distancia > 2 && canAttack)
            {
                StartCoroutine(jumpAttack());
                audioSource.PlayOneShot(mordida);
                anim.SetBool("Correndo",false);
                return;
            }

            if (canAttack && distancia <= 2f)
            {
                StartCoroutine(Atacar());
            }  
        }
    }
    protected IEnumerator Atacar()
    {
        int escolha = Random.Range(0, 2);
        if (escolha == 1)
        {
            anim.SetTrigger("Atacar");
            audioSource.PlayOneShot(garrada);
        }
        else
        {
            anim.SetTrigger("Atacar 2");
            audioSource.PlayOneShot(mordida);
        }

        Collider[] hit = Physics.OverlapSphere(boca.transform.position, distanciaAtaque, LayerMask.GetMask("Player"));
        if (hit.Length > 0)
        {
            hit[0].gameObject.GetComponent<Player>().ReceberDano(danoMedio);
        }

        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.SetBool("Correndo", false);
        }
    }
    private IEnumerator Escolher()
    {
        while (true)
        {
            int escolha = Random.Range(0, 100);
            if (escolha <= 70)
            {
                anim.SetBool("Idle", false);
                destino = RandomNavMeshGenerator(10f);
                animal.speed = 1.5f;
                yield return StartCoroutine(Movimentar());
            }
            else if (escolha <= 100)
            {
                yield return StartCoroutine(Farejar());
            }
            yield return new WaitForSeconds(6f);
        }
        /*
        int escolha = Random.Range(0, 100);
        if (escolha <= 60)
        {
            if (anim.GetBool("Idle") && !anim.GetBool("Farejar"))
            {
                anim.SetBool("Idle", false);
                destino = RandomNavMeshGenerator(10f);
                animal.speed = 1.5f;
                Movimentar();
                //anim.SetBool("Idle",true);
            }
        }
        else if (escolha <= 75)
        {
            StartCoroutine(Farejar());
            yield return new WaitForSeconds(10f);
        }
        else
        {
            yield return new WaitForSeconds(10f);
            StartCoroutine(Escolher());
        }*/
    }
    
    
    public int Vida
    {
        get { return vida; }

        private set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
            UIController.uiController.LifeBar(lifeBar, ((float)vida / maxVida));
        }
    }
    public IEnumerator Farejar()
    {
        anim.SetTrigger("Farejar");
        anim.SetBool("Idle", false);
        yield return new WaitForSeconds(6f);
        anim.SetBool("Idle", true);
        StartCoroutine(Escolher());
    }
    public Vector3 RandomNavMeshGenerator(float raioCaminhada)
    {
        Vector3 randomDirection = Random.insideUnitSphere * raioCaminhada;
        randomDirection += this.gameObject.transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1);

        if (NavMesh.SamplePosition(randomDirection, out hit, raioCaminhada, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
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

    void Morrer()
    {
        Destroy(this.gameObject, 10);
        anim.SetBool("Morreu", true);

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<SphereCollider>());


        foreach (FaceCamera destruir in GetComponentsInChildren(typeof(FaceCamera), true))
        {
            Destroy(destruir.gameObject);
        }

        morto = true;
        DroparLoot();
        StopAllCoroutines();

        respawnTigre[Random.Range(0, respawnTigre.Length)].GetComponent<Respawn>().numeroAnimais--;
    }
    public void ReceberDano(int danoRecebido)
    {
        Vida -= danoRecebido;

        //chama metodo do UIController para exibir o dano no worldCanvas
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);


        if (vida <= 0)
        {
            anim.SetTrigger("Tomar dano");
            anim.SetTrigger("Morrer");
            Morrer();
        }
        else
        {
            anim.SetBool("Deitado", false);
            anim.SetTrigger("Tomar dano");
            anim.SetFloat("Vida", vida);
        }

    }
    protected IEnumerator Movimentar(bool move = true)
    {
            anim.SetBool("Idle", false);
            anim.SetBool("Caminhando", true);

            if (!move)
            {
                animal.isStopped = true;
                yield return 0;
            }

            this.transform.LookAt(destino);
            animal.isStopped = false;
            animal.SetDestination(destino);
            yield return 0;
    }
    private void Awake()
    {
        // checa se há um canvas como filho, se não houver, cria um filho worldSpace Canvas e o nomea adequadamente.
        if (transform.GetComponentInChildren<Canvas>() == null)
        {
            GameObject hitLife = new GameObject();
            hitLife.transform.SetParent(this.transform);
            hitLife.transform.position = new Vector3(0, 1, 0);
            hitLife.name = "Hit_Life";
            hitLife.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else if (!collision.gameObject.CompareTag("Chao"))
        {
            destino = RandomNavMeshGenerator(20f);
            animal.SetDestination(destino);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(destino, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(destino, 0.5f);
    }
}
