using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kambim : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private NavMeshAgent vendedor;
    private bool playerPerto;

    public int contVenderComprar;

    Vector3 finalPosition;
    Collider maisPerto;
    Vector3 destino;
    NavMeshHit hit;

    private void Update()
    {
        if (anim.GetBool("Andando"))
        {
            if (this.gameObject.transform.position == destino)
            {
                anim.SetBool("Andando", false);
            }
        }
    }
    private void Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            anim.SetBool("Idle2", false);
        }
        else
        {
            anim.SetBool("Idle2", true);
        }
        StartCoroutine(Escolha());
    }
    private void Andar()
    {
        anim.SetBool("Andando",true);
        vendedor.SetDestination(RandomNavMeshGenerator(4f));
        StartCoroutine(Escolha());
    }
    private IEnumerator Escolha()
    {
        int random = Random.Range(0, 100);
        if (random > 80 && !playerPerto)
        {
            Andar();
        }
        else if (random > 50 && !playerPerto)
        {
            anim.SetTrigger("Dormir");
            anim.SetBool("Dormindo",true);
        }
        else if (random > 30)
        {
            anim.SetBool("Idle2", false);
        }
        else
        {
            yield return new WaitForSeconds(10f);
            StartCoroutine(Escolha());
        }
    }
    
    public void FimInteracao()
    {
        contVenderComprar = LojaUI.lojaUi.contCompraVenda;
        Debug.Log(contVenderComprar);

        if (contVenderComprar > 0)
            anim.SetTrigger("Compra");
        else if (contVenderComprar < 0)
            anim.SetTrigger("Venda");
        else if (Random.Range(0, 2) == 0)
            anim.SetTrigger("Compra");
        else
            anim.SetTrigger("Venda");

    }

    public void Conversa()
    {
        if (anim.GetBool("Dormindo"))
        {
            anim.SetBool("Dormindo", false);
            anim.ResetTrigger("Cumprimentar");
        }
        else
            anim.SetTrigger("Interacao");
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            vendedor.isStopped = true;
            anim.SetBool("Andando", false);
            anim.SetTrigger("Cumprimentar");
            playerPerto = true;
            anim.SetBool("PlayerPerto", true);
        }
        StopCoroutine(Escolha());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.ResetTrigger("Dormir");
            playerPerto = false;
            anim.SetBool("PlayerPerto", false);
            StartCoroutine(Escolha());
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
