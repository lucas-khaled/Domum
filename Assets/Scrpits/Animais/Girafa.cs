using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Girafa : MonoBehaviour
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
    public Item[] itensDropaveis;
    public Bau drop;
    [SerializeField]
    private GameObject[] respawnGirafa;

    [Header("Audios")]
    [SerializeField]
    private AudioClip deitar;
    [SerializeField]
    private AudioClip comer;
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

    private Transform hitCanvas;

    //auxiliares
    private float aux = 0;
    private bool vaicomer;
    private int corrida;
    private bool correndo;

    private void Update()
    {
        if (anim.GetBool("Caminhando") && !anim.GetBool("Correndo"))
         {
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 1.2f)
            {
                anim.SetBool("Caminhando", false);
            }
        }
        if (anim.GetBool("Correndo") && corrida > 0)
        {
            this.transform.LookAt(destino);
            if (Vector3.Distance(this.gameObject.transform.position, destino) < 1.2f)
            {
                destino = RandomNavMeshGenerator(20f);
                animal.SetDestination(destino);
                corrida--;
            }
        }
        else if (anim.GetBool("Correndo") && corrida <= 0)
        {
            GetComponent<NavMeshAgent>().speed = 0.5f;
            anim.SetBool("Correndo", false);
            correndo = false;
            destino = RandomNavMeshGenerator(10f);
            Movimentar(destino);
        }
    }

    void DroparLoot()
    {
        int numeroItens = UnityEngine.Random.Range(0, 4);
        Bau dropzera = Instantiate(drop.gameObject, transform.position, transform.rotation).GetComponent<Bau>();

        for (int i = 0; i <= numeroItens; i++)
        {
            int itemEsc = UnityEngine.Random.Range(0, itensDropaveis.Length);
            dropzera.itens.Add(itensDropaveis[itemEsc]);
        }
    }

    private IEnumerator Escolher()
    {
        while (true)
        {
            if (!anim.GetBool("Deitado") && !anim.GetBool("Caminhando") && !anim.GetBool("Correndo"))
            {
                int escolha = UnityEngine.Random.Range(0, 4);
                if (escolha == 0)
                {
                    destino = RandomNavMeshGenerator(10f);
                    Movimentar(destino);
                }
                else if (escolha == 2)
                {
                    yield return StartCoroutine(Deitado());
                }
            }
            yield return new WaitForSeconds(13f);   
        }
    }
    private void Start()
    {
        respawnGirafa = GameObject.FindGameObjectsWithTag("RespawnGirafa");
        StartCoroutine(Escolher());
        Vida = maxVida;
        hitCanvas = transform.Find("Hit_life");
        anim.SetFloat("Vida", maxVida);
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
    public IEnumerator Deitado()
    {
        if (!anim.GetBool("Caminhando") && !anim.GetBool("Caminhando"))
        {
            animal.isStopped = true;
        anim.SetBool("Caminhando", false);

        anim.SetBool("Deitado", true);
        audioSource.PlayOneShot(deitar);
        yield return new WaitForSeconds(20f);
        anim.SetTrigger("Levantar");
        anim.SetBool("Deitado", false);
        yield return new WaitForSeconds(3f);
            destino = RandomNavMeshGenerator(10f);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Caminhando", true);
        Movimentar(destino);
        }
        
    }
    private Vector3 RandomNavMeshGenerator(float raioCaminhada)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * raioCaminhada;
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
        Destroy(this.gameObject, 10);
        anim.SetBool("Morreu", true);

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<SphereCollider>());


        foreach (FaceCamera destruir in GetComponentsInChildren(typeof(FaceCamera), true))
        {
            Destroy(destruir.gameObject);
        }

        animal.isStopped = true;
        DroparLoot();
        respawnGirafa[UnityEngine.Random.Range(0, respawnGirafa.Length)].GetComponent<Respawn>().numeroAnimais--;
        StopAllCoroutines();
    }
    public void ReceberDano(int danoRecebido)
    {
        StopCoroutine(Escolher());
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
            
            if (vida > 30)
            Correr();
            else
            {
                GetComponent<NavMeshAgent>().speed = 0.5f;
                anim.SetBool("Caminhando", true);
                correndo = true;
                corrida = UnityEngine.Random.Range(2, 6);
                destino = RandomNavMeshGenerator(20f);
                animal.SetDestination(destino);
            }

        }

    }
    protected void Movimentar(Vector3 destino, bool move = true)
    {
        if (!anim.GetBool("Deitado"))
        {
            anim.SetBool("Caminhando", true);
            if (!move)
            {
                animal.isStopped = true;
                return;
            }

            this.transform.LookAt(destino);
            animal.isStopped = false;
            animal.SetDestination(destino);
        }
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
    private void Correr()
    {
        anim.SetBool("Correndo", true);
        correndo = true;
        corrida = UnityEngine.Random.Range(2, 6);
        GetComponent<NavMeshAgent>().speed = 4;
        destino = RandomNavMeshGenerator(20f);
        animal.SetDestination(destino);
    }
    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    private IEnumerator Comer()
    {
        anim.SetBool("Caminhando", false);
        anim.SetBool("Comendo", true);
        audioSource.PlayOneShot(comer);
        yield return new WaitForSeconds(9f);
        audioSource.Stop();
        anim.SetBool("Comendo",false);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(destino, 2f);
        Gizmos.color = Color.black; 
        Gizmos.DrawWireSphere(destino, 2);
    }
}
