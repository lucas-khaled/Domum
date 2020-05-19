using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Atriet : MonoBehaviour
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
        StartCoroutine(Escolha());
    }
    private void Andar()
    {
        anim.SetBool("Idle", false);
        vendedor.SetDestination(RandomNavMeshGenerator(4f));
        StartCoroutine(Escolha());
    }
    private IEnumerator Escolha()
    {
        int random = Random.Range(0, 100);
        if (random > 80 && !playerPerto)
        {
            Andar();
            yield return new WaitForSeconds(5f);
        }
        else if (random > 50 && !playerPerto)
        {
            anim.SetTrigger("Soneca");
            yield return new WaitForSeconds(5f);
        }
        else if (random > 30 && !playerPerto)
        {
            anim.SetTrigger("Alongar");
            yield return new WaitForSeconds(5f);
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

        if (contVenderComprar > 0)
            anim.SetTrigger("Compra");
        else if (contVenderComprar < 0)
            anim.SetTrigger("Venda");
        else if (Random.Range(0, 2) == 0)
            anim.SetTrigger("Compra");
        else
            anim.SetTrigger("Venda");

        StartCoroutine(Escolha());
    }

    public void Conversa()
    {
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
            anim.ResetTrigger("Soneca");
            anim.ResetTrigger("Alongar");

            vendedor.isStopped = true;
            anim.SetBool("Idle", true);
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
            anim.SetBool("PlayerPerto", false);
            StartCoroutine(Escolha());
            playerPerto = false;
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
