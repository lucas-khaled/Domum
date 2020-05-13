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
    private float timeFaca;
    [SerializeField]
    private float timeDash;

    private float tempoDash;
    public float TempoDash
    {
        get
        {
            return tempoDash;
        }

        private set
        {
            tempoDash = Mathf.Clamp(value, 0, status.tempoDashTotal);

            if (value >= 0 && value <= status.tempoDashTotal)
            {
                UIController.uiController.SkillCD((float)tempoDash/status.tempoDashTotal);//Controlador UI da recarga
            }
        }
    }

    [Header("Referências Tyva")]
    public Transform posicaoFaca;
    private Transform pointerPosition;
    public GameObject faca;
    private float contadorFaca;

    #region PreSettings

    protected override void Start()
    {
        base.Start();
        //só para teste, deletar depois
        status.QntColetavel = 3;
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
    }

    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(1))
        {
            Faca();
        }       

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (TempoDash >= status.tempoDashTotal)
            {
                Vector3 movimento = new Vector3(moveHorizontal, 0.0f, moveVertical);
                StartCoroutine(Dash());
                TempoDash = 0;
            }
            else
            {            
                rb.velocity = Vector3.zero;
            }
        }

        TempoDash += Time.deltaTime;
        contadorFaca += Time.deltaTime;
    }

    IEnumerator Dash()
    {
        animator.SetTrigger("Dash");
        estadoPlayer = EstadoPlayer.RECARREGAVEL;

        float tempo = 0;

        while (tempo < timeDash)
        {
            rb.AddForce(transform.forward * velocidadeDash * Time.deltaTime, ForceMode.VelocityChange);
            tempo += Time.deltaTime;
            yield return null;
        }

        estadoPlayer = EstadoPlayer.COMBATE;
    }

    private void Faca()
    {
        if (contadorFaca < timeFaca || status.QntColetavel <= 0)
            return;

        Vector3 target = ProcuraInimigo();
        if (target == Vector3.zero)
            target = FacaFrontTarget();

        StartCoroutine(LancarFaca(target));
    }

    private IEnumerator LancarFaca(Vector3 target)
    {
        contadorFaca = 0;
        animator.SetTrigger("JogarAdaga");
        yield return new WaitForSeconds(0.5f);

        GameObject facaInstanciada = Instantiate(faca, posicaoFaca.position, faca.transform.rotation);
        target.y += 1;
        facaInstanciada.transform.LookAt(target);
        facaInstanciada.GetComponent<Bala>().SetCasterCollider(GetComponent<Collider>());

        status.QntColetavel--;
    }


    Vector3 FacaFrontTarget()
    {
        float rotY = transform.localEulerAngles.y * Mathf.Deg2Rad;

        Vector3 front = new Vector3(posicaoFaca.position.x + Mathf.Sin(rotY),
                                       posicaoFaca.position.y,
                                       posicaoFaca.position.z + Mathf.Cos(rotY));

        return front;
    }
}