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
    public NavMeshAgent animal;
    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;

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
    private float ataqueCooldown;
    private Rigidbody rb;

    private Transform hitCanvas;

    #region COMBATE
    private void jumpAttack()
    {
        anim.SetTrigger("Atacar");
        Collider[] hit = Physics.OverlapSphere(boca.transform.position, distanciaAtaque, LayerMask.GetMask("Player"));
        if (hit.Length > 0)
        {
            hit[0].gameObject.GetComponent<Player>().ReceberDano(danoMedio);
        }
    }
    #endregion
    private void Update()
    {
        if (anim.GetBool("Caminhando"))
        {
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 1f)
            {
                anim.SetBool("Caminhando", false);
                StartCoroutine(Escolher());
            }
        }
    }
    protected virtual void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            anim.SetBool("Caminhando", false);
            anim.SetFloat("Distancia", Vector3.Distance(collider.transform.position, this.transform.position));
            anim.SetBool("Idle", false);

            if (anim.GetFloat("Distancia") > 3f)
            {
                GetComponent<NavMeshAgent>().speed = 3f;
                anim.SetBool("Correndo", true);
            }

            if (Vector3.Distance(this.gameObject.transform.position, collider.transform.position) > 2.5f)
                    animal.SetDestination(collider.transform.position);

            if (anim.GetFloat("Distancia") <= 3f && anim.GetBool("Correndo"))
            {
                jumpAttack();
                anim.SetBool("Correndo",false);
                ataqueCooldown = cooldown;
            }
            else if (ataqueCooldown <= 0 && anim.GetFloat("Distancia") <= 2f)
            {
                Atacar();
                ataqueCooldown = cooldown;
            }
            ataqueCooldown -= Time.deltaTime * 1;
        }
    }
    protected void Atacar()
    {
        int escolha = Random.Range(0, 2);
        if (escolha == 1)
        {
            anim.SetTrigger("Atacar");
            ataqueCooldown = cooldown;
        }
        else
        {
            anim.SetTrigger("Atacar 2");
            ataqueCooldown = cooldown;
        }
           
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ataqueCooldown = 0;
            anim.SetBool("Correndo", false);
        }
    }
    private IEnumerator Escolher()
    {
        int escolha = Random.Range(0, 100);
        Debug.Log(escolha);
        if (escolha <= 70)
        {
            if (anim.GetBool("Idle"))
            {
                anim.SetBool("Idle", false);
                Debug.Log("ANDAAA");
                destino = RandomNavMeshGenerator(10f);
                animal.speed = 1.5f;
                Movimentar(destino);
                anim.SetBool("Idle",true);
            }
        }
        else if(escolha <= 80)
        {
            Debug.Log("Farejar");
            StartCoroutine(Farejar());
        }
        else
        {
            Debug.Log("IA");
            yield return new WaitForSeconds(4f);
            //StopCoroutine(Escolher());
            StartCoroutine(Escolher());
        }
    }
    private void Start()
    {
        anim.SetBool("Idle", true);
        StartCoroutine(Escolher());
        Vida = maxVida;
        hitCanvas = transform.Find("Hit_life");
        anim.SetFloat("Vida", maxVida);
        rb = this.gameObject.GetComponent<Rigidbody>();
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
        yield return new WaitForSeconds(5f);
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
    void Morrer()
    {
        StopAllCoroutines();
        animal.Stop();
        Debug.Log("Morri");
    }
    public virtual void ReceberDano(int danoRecebido)
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
    protected void Movimentar(Vector3 destino, bool move = true)
    {
        GetComponent<NavMeshAgent>().speed = 0.8f;
        anim.SetBool("Caminhando", true);
        if (!move)
        {
            animal.isStopped = true;
            return;
        }

        animal.isStopped = false;
        animal.SetDestination(destino);
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
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

}
