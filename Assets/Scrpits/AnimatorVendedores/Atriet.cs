﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Atriet : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private NavMeshAgent vendedor;

    public static int contVendasCompras;

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
        if (random > 80)
        {
            Andar();
        }
        else if (random > 50)
        {
            anim.SetTrigger("Soneca");
        }
        else if (random > 30)
        {
            anim.SetTrigger("Alongar");
        }
        else
        {
            yield return new WaitForSeconds(10f);
            StartCoroutine(Escolha());
        }
    }

    public void FimInteração()
    {
        contVendasCompras = LojaUI.contVendasCompras;

        if (contVendasCompras > 0)
            anim.SetTrigger("Compra");
        else if (contVendasCompras < 0)
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
            vendedor.isStopped = true;
            anim.SetBool("Idle", true);
            anim.SetTrigger("Cumprimentar");
        }
        StopCoroutine(Escolha());
    }
    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(Escolha());
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
