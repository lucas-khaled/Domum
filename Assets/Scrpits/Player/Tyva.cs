using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyva : Player
{
    float moveHorizontal;
    float moveVertical;

    [Header("Valores Tyva")]
    [SerializeField]
    private float forcaDash;
    [SerializeField]
    private float timeFaca;
    [SerializeField]
    private float tempoRecarga = 1;

    [Header("Audio Tyva")]
    [SerializeField]
    private AudioClip dash;
    [SerializeField]
    private AudioClip lancarFaca;

    private bool dashEspecial = false;

    public bool facaLetal { get; set; }

    public bool DashEspecial
    {
        get
        {
            return dashEspecial;
        }
        set
        {
            dashEspecial = value;
            animator.SetBool("DashEspecial", value);
        }
    }

    public float TempoRecarga
    {
        get
        {
            return tempoRecarga;
        }
        set
        {
            tempoRecarga = value;
        }
    }

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
        facaLetal = false;
    }

    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    protected override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("AtkColetavel") && !UIController.uiController.isPaused)
        {
            Faca();
        }       

        if (Input.GetButtonDown("Recarregavel") && !UIController.uiController.isPaused)
        {
            if (TempoDash >= status.tempoDashTotal)
            {
                StartCoroutine(Dash());
                TempoDash = -10;
            }
        }

        TempoDash += Time.deltaTime*tempoRecarga;
        contadorFaca += Time.deltaTime;
    }

    IEnumerator Dash()
    {
        animator.SetTrigger("Dash");

        audioSource.PlayOneShot(dash);

        if (dashEspecial)
            estadoPlayer = EstadoPlayer.ATACANDO;
        else
            estadoPlayer = EstadoPlayer.RECARREGAVEL;


        while (estadoPlayer == EstadoPlayer.RECARREGAVEL || estadoPlayer == EstadoPlayer.ATACANDO)
        {
            //rb.AddForce(transform.forward * forcaDash * Time.deltaTime, ForceMode.VelocityChange);
            transform.position += transform.forward * forcaDash * Time.deltaTime;
            yield return null;
        }
    }

    void EndDash()
    {
        estadoPlayer = EstadoPlayer.NORMAL;
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
        audioSource.PlayOneShot(lancarFaca);
        yield return new WaitForSeconds(0.5f);

        GameObject facaInstanciada = Instantiate(faca, posicaoFaca.position, faca.transform.rotation);
        target.y += 1;
        facaInstanciada.transform.LookAt(target);
        facaInstanciada.GetComponent<Bala>().SetCasterCollider(GetComponent<Collider>());

        if (facaLetal)
            facaInstanciada.GetComponent<Bala>().canBeLetal = true;

        status.QntColetavel--;
    }

    Vector3 front = Vector3.zero;
    Vector3 FacaFrontTarget()
    {
        float rotY = transform.localEulerAngles.y * Mathf.Deg2Rad;

        front = new Vector3(posicaoFaca.position.x + Mathf.Sin(rotY)*3,
                                       posicaoFaca.position.y-1f,
                                       posicaoFaca.position.z + Mathf.Cos(rotY)*3);

        return front;
    }

    public void CheckCombo()
    {
        podeAtacar = false;
        audioSource.PlayOneShot(ataque);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && numClick == 1)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && numClick >= 2)
        {
            animator.SetInteger("Ataque", 2);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && numClick == 2)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && numClick >= 3)
        {
            animator.SetInteger("Ataque", 3);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && numClick == 3)
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && numClick >= 4)
        {
            animator.SetInteger("Ataque", 4);
            podeAtacar = true;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            animator.SetInteger("Ataque", 0);
            numClick = 0;
            estadoPlayer = EstadoPlayer.COMBATE;
        }
    }

    private void OnDrawGizmos()
    {
        if(front != Vector3.zero)
        {
            Gizmos.DrawWireSphere(front, 0.5f);
            Debug.Log(front);
        }
    }
}