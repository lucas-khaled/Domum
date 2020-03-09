using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour
{

    public Image LifeBar;
    public Animator anim;
    public NavMeshAgent NavMesh;
    public Transform posicaoInicial;

    //Criar Array de Itens para ele dropar

    [SerializeField]
    private int maxVida;
    [SerializeField]
    private int danoMedio;
    [SerializeField]
    private float velocidade;
    [SerializeField]
    private float distanciaAtaque;

    public int vida
    {
        get { return vida; }

        set
        {
            vida = Mathf.Clamp(value, 0, maxVida);
        }
    }

    private void Start()
    {
        vida = maxVida;
    }

    void Atacar()
    {

    }

    void ReceberDano(int danoRecebido)
    {

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
        if (collider.gameObject.tag == "Player")
        {
            Movimentar(collider.transform.position);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Movimentar(posicaoInicial.position);
        }
    }
}
