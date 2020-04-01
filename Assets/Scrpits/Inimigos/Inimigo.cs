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
    public Transform posicaoInicial;
    public GameObject CBTprefab;

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
    protected float ataqueCooldown;
    [SerializeField]
    private int experienciaMorte;

    private Transform hitCanvas;

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
    }

    //realiza o ataque do inimigo
    protected virtual IEnumerator Atacar()
    {    

        Collider[] hit = Physics.OverlapSphere(transform.position, distanciaAtaque, LayerMask.GetMask("Player"));

        // é checado se o ataque atingiu o player e lhe dá o dano
        if (hit.Length > 0)
        {
            Debug.Log("Ataque do inimigo");
            hit[0].gameObject.GetComponent<Player>().ReceberDano(danoMedio);
        }

        //reseta o cooldown(espera) do ataque e espera para tocar a animação
        ataqueCooldown = velocidadeAtaque;
        yield return new WaitForSeconds(0.5f);              
    }

    public virtual void ReceberDano(int danoRecebido)
    {
        Vida -= danoRecebido;
        
        //chama metodo do UIController para exibir o dano no worldCanvas
        UIController.uiController.InitCBT(danoRecebido.ToString(), CBTprefab, hitCanvas);
        
        
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

        //AQUI O DELEGATE DE MORTE DO INIGO É CHAMADO E TODOS OS MÉTODOS INSCRITOS NESTE DELEGATE SERÃO CHAMADOS
        if (EventsController.onMorteInimigoCallback != null)
        {
            EventsController.onMorteInimigoCallback.Invoke(experienciaMorte);
        }
        Debug.Log("Morri");
    }

    void DroparLoot()
    {

    }

    protected void Movimentar(Vector3 destino, bool move = true)
    {
        if (!move)
        {
            NavMesh.isStopped = true;
            return;
        }

        NavMesh.isStopped = false;
        NavMesh.SetDestination(destino);
    }

    protected virtual void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil)
        {
            bool mover = true;

            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            if (distancia <= distanciaAtaque)
            {
                mover = false;
                if(ataqueCooldown<=0)
                  StartCoroutine(Atacar());        
            }

            ataqueCooldown -= Time.deltaTime * 1;
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
