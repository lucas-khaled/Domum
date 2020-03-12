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

    float distancia;

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
            Debug.Log("Ataque do inimigo");
            ataqueCooldown = velocidadeAtaque;
        }
        yield return new WaitForSeconds(0.5f);
        ataqueCooldown -= Time.deltaTime * 1;
    }

    public void ReceberDano(int danoRecebido)
    {
        vida = Mathf.Clamp(vida - danoRecebido, 0, maxVida);
        
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

    }

    void DroparLoot()
    {

    }

    void Movimentar(Vector3 destino)
    {
        NavMesh.SetDestination(destino);
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil)
        {
            Movimentar(collider.transform.position);

            distancia = Vector3.Distance(collider.transform.position, NavMesh.transform.position);

            if (distancia <= distanciaAtaque)
            {
                StartCoroutine(Atacar());
            }
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
}
