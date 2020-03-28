using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;



public class Inimigo : MonoBehaviour, IVulnerable
{

    [Header("Referências")]
    public Image LifeBar;
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

    public int Vida
    {
        get { return vida; }

        private set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
            LifeBar.fillAmount = ((float)vida / maxVida) ;
        }
    }

    private void Start()
    {
        Vida = maxVida;
    }

    protected virtual IEnumerator Atacar()
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

    public virtual void ReceberDano(int danoRecebido)
    {
        Vida -= danoRecebido;
       
        InitCBT(danoRecebido.ToString());
        
        
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
    
    void InitCBT(string text)
    {
        
        GameObject temp = Instantiate(CBTprefab) as GameObject;
        
        RectTransform tempRect = temp.GetComponent<RectTransform>();
        temp.transform.SetParent(transform.Find("Hit_life"));
        tempRect.transform.localPosition = CBTprefab.transform.localPosition;
        tempRect.transform.localScale = CBTprefab.transform.localScale;
        tempRect.transform.localRotation = CBTprefab.transform.localRotation;
        temp.GetComponent<Animator>().SetTrigger("Hit");
        temp.GetComponent<Text>().text = text;
        
        Destroy(temp.gameObject,2);
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
