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
    [SerializeField]
    private float timeFaca;

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
        QntColetavel = 3;
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
            if (tempoDash <= 0)
            {
                Vector3 movimento = new Vector3(moveHorizontal, 0.0f, moveVertical);
                Dash(movimento);
                tempoDash = 3;
            }
            else
            {
                UIController.uiController.SkillCD(player.Skill,1/tempoDash*Time.deltaTime);//Controlador UI da recarga
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

        UIController.uiController.SkillCD(player.Skill,0);//Zerar Ui de recarga

        rb.velocity = Vector3.zero;
    }

    private void Faca()
    {
        if (contadorFaca < timeFaca || QntColetavel <= 0)
            return;

        Vector3 target = ProcuraInimigo();
        if (target == Vector3.zero)
            target = FacaFrontTarget();

        StartCoroutine(LancarFaca(target));
    }

    private IEnumerator LancarFaca(Vector3 target)
    {
        contadorFaca = 0;
        //tocar animação
        yield return new WaitForSeconds(0.5f);

        GameObject facaInstanciada = Instantiate(faca, posicaoFaca.position, faca.transform.rotation);
        facaInstanciada.transform.LookAt(target);
        facaInstanciada.GetComponent<Bala>().SetCasterCollider(GetComponent<Collider>());
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