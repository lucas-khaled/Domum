using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iovelik : Player
{
    [Header("Valores Iovelik")]
    public GameObject escudo;
    public float tempoEscudo = 3;
    public float tempoRecargaEscudo = 1.5f;

    private float recarregaEscudo;   
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

    private void Start()
    {
        RecarregaEscudo = tempoEscudo;
    }

    protected override void Update()
    {
        base.Update();

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
}
