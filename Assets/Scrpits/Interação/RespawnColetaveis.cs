using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RespawnColetaveis : MonoBehaviour
{
    private NavMeshHit hit;
    Vector3 finalPosition;
    [SerializeField]
    private float raioN;
    [SerializeField]
    private GameObject coletavel;

    public void RespawnColetavel()
    {
        Instantiate(coletavel , RandomNavMeshGenerator(raioN), Quaternion.identity);
    }

    private Vector3 RandomNavMeshGenerator(float raio)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * raio;
        randomDirection += this.gameObject.transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, raio, 1);

        if (NavMesh.SamplePosition(randomDirection, out hit, raio, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, raioN);
        Gizmos.color = Color.blue;
    }
}
