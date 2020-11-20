using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Respawn : MonoBehaviour
{
    [SerializeField]
    private int numeroMaximo;
    [SerializeField]
    private float raioSpawn;
    [SerializeField]
    private GameObject animal;
    [SerializeField]
    private Transform parentescoHierarquia;
    
    public int numeroAnimais;
    Vector3 finalPosition;
    NavMeshHit hit;


    void Start()
    {
        numeroAnimais = 0;
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            numeroAnimais = this.transform.childCount;

            while (numeroAnimais < numeroMaximo)
            {
                Vector3 novoSpawn = RandomNavMeshGenerator();
                Collider[] hit = Physics.OverlapSphere(novoSpawn, 2f, LayerMask.GetMask("Ground"));
                if (hit.Length > 0)
                {
                    Instantiate(animal, novoSpawn, Quaternion.identity, parentescoHierarquia);
                    numeroAnimais++;
                }
            }
            yield return new WaitForSeconds(118f);
        }
    }

    public Vector3 RandomNavMeshGenerator()
    {
        Vector3 randomDirection = Random.insideUnitSphere * raioSpawn;
        randomDirection += this.gameObject.transform.position;
        NavMesh.SamplePosition(randomDirection, out hit, raioSpawn, 1);

        if (NavMesh.SamplePosition(randomDirection, out hit, raioSpawn, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, raioSpawn);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

}
