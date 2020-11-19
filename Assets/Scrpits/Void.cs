using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField]
    private Transform posicaoInicio;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = posicaoInicio.position;
        }
        else if (other.gameObject.CompareTag("Inimigo"))
        {
            other.gameObject.GetComponent<Inimigo>().Morrer();
        }
        else if (other.gameObject.CompareTag("Tigre"))
        {
            other.gameObject.GetComponent<Tigre>().ReceberDano(1000);
        }
        else if (other.gameObject.CompareTag("Girafa"))
        {
            other.gameObject.GetComponent<Girafa>().ReceberDano(1000);
        }
    }
}
