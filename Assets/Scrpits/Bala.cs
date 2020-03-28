using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Bala : MonoBehaviour
{

    public float velocidadeBala;
    public int danoBala;
    public string targetTag;

    Rigidbody rb;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        Destroy(this.gameObject, 8);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = this.transform.forward * velocidadeBala;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Acertei:" + other.name);
        if (other.isTrigger)
            return;

        if (other.gameObject.CompareTag(targetTag))
        {
            Debug.Log("Acertei o certo");
            other.GetComponent<IVulnerable>().ReceberDano(danoBala);
        }

        Destroy(this.gameObject);
        
    }
}
