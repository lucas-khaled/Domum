using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyva : Player
{
    float moveHorizontal;
    float moveVertical;

    [Header("Valores Tyva")]
    [SerializeField]
    private float velocidadeDash;
    [SerializeField]
    private float tempoDash;
    Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
    }
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (tempoDash <= 0)
            {
                Vector3 movimento = new Vector3(moveHorizontal, 0.0f, moveVertical);
                Dash(movimento);
                tempoDash = 3;
            }
            else
            {
                tempoDash -= Time.deltaTime;
                rb.velocity = Vector3.zero;
            }
        }
        tempoDash -= Time.deltaTime;
    }
    void Dash(Vector3 movimento)
    {
        rb.AddForce(transform.forward * velocidadeDash, ForceMode.VelocityChange);

        rb.velocity = Vector3.zero;
    }
    private IEnumerator LancarFaca(Transform target)
    {
        yield return null;
    }

    private void MirarFaca()
    {

    }
}