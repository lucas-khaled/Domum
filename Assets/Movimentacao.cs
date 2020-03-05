using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimentacao : MonoBehaviour
{
    public float Velocidade = 2;
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
        #region Joystick
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical_J") * Time.deltaTime * Velocidade);
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal_J") * Time.deltaTime * Velocidade);
        #endregion
        #region Teclado
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
        #endregion
    }
}
