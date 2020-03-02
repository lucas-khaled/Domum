using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float Velocidade = 2;
    public float Pulo = 2;

    public LayerMask Piso;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movimento();
    }

    void Movimento()
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Velocidade * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-Vector3.forward * Velocidade * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-Vector3.right * Velocidade * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Velocidade * Time.deltaTime);
        }

        bool PodePular = Physics.SphereCast(transform.GetChild(0).position, 0.5f, -transform.up, out RaycastHit outPiso, Piso);

        if (Input.GetKeyDown(KeyCode.Space) && PodePular == false)
        {
            Rigidbody corpo_fisico = transform.GetComponent<Rigidbody>();

            corpo_fisico.AddForce(Vector3.up * Pulo, ForceMode.Impulse);
        }

    }
}
