using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyva : Player
{
    float moveHorizontal;
    float moveVertical;

    public Transform posicaoFaca;
    public GameObject faca;

    [Header("Valores Tyva")]
    [SerializeField]
    private float velocidadeDash;
    [SerializeField]
    private float tempoDash;
    [SerializeField]
    private float timeFaca;




    Rigidbody rb;

    private float contadorFaca;

    private void Start()
    {
        //só para teste, deletar depois
        QntColetavel = 3;
        rb = this.GetComponent<Rigidbody>();
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
    }
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(1))
        {
            Faca();
        }

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

        contadorFaca += Time.deltaTime;
        tempoDash -= Time.deltaTime;
    }
    void Dash(Vector3 movimento)
    {
        rb.AddForce(transform.forward * velocidadeDash, ForceMode.VelocityChange);

        rb.velocity = Vector3.zero;
    }

    private void Faca()
    {
        if (contadorFaca < timeFaca || QntColetavel <= 0)
            return;

        Vector3 target = ProcuraInimigo();
        if (target == Vector3.zero)
        {
            target = posicaoFaca.forward; 
        }

        StartCoroutine(LancarFaca(target));
    }

    private IEnumerator LancarFaca(Vector3 target)
    {
        contadorFaca = 0;
        //tocar animação
        yield return new WaitForSeconds(0.5f);

        GameObject facaInstanciada = Instantiate(faca, posicaoFaca.position, faca.transform.rotation);
        facaInstanciada.transform.LookAt(target);
    }

    private void MirarFaca()
    {

    }
}