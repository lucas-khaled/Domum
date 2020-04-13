using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iovelik : Player
{
    [Header("Valores Iovelik")]
    public GameObject escudo;
    public float tempoEscudo = 3;
    public float tempoRecargaEscudo = 1.5f;
    [SerializeField]
    private float valorDanoArea;
    [SerializeField]
    private float raioDanoArea;
    [SerializeField]
    private int coolDownDanoArea;
    private float esperaDanoArea = 0;
    private float distancia;

    private float recarregaEscudo;
    bool usandoDanoArea = false;
    public float RecarregaEscudo
    {
        get
        {
            return recarregaEscudo;
        }

        private set
        {
            recarregaEscudo = Mathf.Clamp(value, 0, tempoEscudo);

            if (value >= tempoEscudo)
                escudoCarregado = true;

            if (value <= 0)
            {
                escudoCarregado = false;
                AtivarEscudo(false);
            }
        }
    }

    private bool escudoCarregado = true;

    protected override void Start()
    {
        base.Start();

        QntColetavel = 3;
        RecarregaEscudo = tempoEscudo;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(danoArea());
        }
        esperaDanoArea -= Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) && escudoCarregado && recarregaEscudo > 0)
        {
            AtivarEscudo(true);
            return;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            AtivarEscudo(false);
            escudoCarregado = false;
        }
        RecarregaEscudo += tempoRecargaEscudo * Time.deltaTime;
    }

    public override void ReceberDano(int danoRecebido)
    {
        if(!escudo.activeSelf)
            base.ReceberDano(danoRecebido);
    }

    private IEnumerator AtaqueEmArea()
    {
        yield return null;
    }

    private void AtivarEscudo(bool ativo)
    {
        escudo.SetActive(ativo);

        RecarregaEscudo = ativo ? RecarregaEscudo - Time.deltaTime : RecarregaEscudo + tempoRecargaEscudo * Time.deltaTime;
        estadoPlayer = ativo ? EstadoPlayer.RECARREGAVEL : EstadoPlayer.COMBATE;
    }

    private IEnumerator danoArea()
    {
        estadoPlayer = EstadoPlayer.ATACANDO;
        if (esperaDanoArea < 0 && QntColetavel > 0)
        {
            Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioDanoArea, LayerMask.GetMask("Inimigo"));

            foreach (Collider inimigoArea in hit)
            {
                distancia = Vector3.Distance(inimigoArea.gameObject.transform.position, this.gameObject.transform.position);
                inimigoArea.gameObject.GetComponent<Inimigo>().ReceberDano((int)(valorDanoArea / distancia));
                esperaDanoArea = coolDownDanoArea;
            }

            QntColetavel--;
            yield return new WaitForSeconds(0.5f);
        }
        estadoPlayer = EstadoPlayer.COMBATE;

    }
}
