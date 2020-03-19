using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;



public class Inimigo : MonoBehaviour
{

    [Header("Referências")]
    public Image LifeBar;
    public Animator anim;
    public NavMeshAgent NavMesh;
    public Transform posicaoInicial;

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
    private float distanciaAtaque;
    [SerializeField]
    private int vida;
    [SerializeField]
    private float velocidadeAtaque;
    [SerializeField]
    private float ataqueCooldown;
    [SerializeField]
    private int experienciaMorte;

    bool podeAtacar = false;

    public int Vida
    {
        get { return vida; }

        private set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
        }
    }

    private void Start()
    {
        Vida = maxVida;
    }

    IEnumerator Atacar()
    {
        if (ataqueCooldown <= 0)
        {          
            Collider[] hit = Physics.OverlapSphere(transform.position, distanciaAtaque, LayerMask.GetMask("Player"));

            if (hit.Length > 0)
            {
                Debug.Log("Ataque do inimigo");
                hit[0].gameObject.GetComponent<Player>().ReceberDano(danoMedio);
            }
            ataqueCooldown = velocidadeAtaque;
            yield return new WaitForSeconds(0.5f);
        }
        
        ataqueCooldown -= Time.deltaTime * 1;
    }

    public void ReceberDano(int danoRecebido)
    {
        Vida -= danoRecebido;
        
        if (vida <= 0)
        {
            //Animação de morte
            Morrer();
        }
        else
        {
            //Animação de dano  
        }
        
    }
    
    void Morrer()
    {
        Destroy(this.gameObject);
        if (EventsController.onMorteInimigoCallback != null)
        {
            EventsController.onMorteInimigoCallback.Invoke(experienciaMorte);
        }
        Debug.Log("Morri");
    }

    void DroparLoot()
    {

    }

    void Movimentar(Vector3 destino, bool move = true)
    {
        if (!move)
        {
            NavMesh.isStopped = true;
            return;
        }

        NavMesh.isStopped = false;
        NavMesh.SetDestination(destino);
    }

    private void OnTriggerEnter(Collider other)
    {
        podeAtacar = true;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil)
        {
            bool mover = true;

            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            if (distancia <= distanciaAtaque)
            {
                mover = false;
                StartCoroutine(Atacar());        
            }
            

            Movimentar(collider.transform.position, mover);
        }

    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Movimentar(posicaoInicial.position);
            ataqueCooldown = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, distanciaAtaque);
    }
}
