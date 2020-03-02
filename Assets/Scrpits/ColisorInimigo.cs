using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ColisorInimigo : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent NavMesh;
    float distance = 0;
    public Transform posicaoInicial;

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            NavMesh.SetDestination(target.position);
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            NavMesh.SetDestination(posicaoInicial.position);
        }
    }
}
