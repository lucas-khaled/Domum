﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EstadoPlayer { NORMAL, COMBATE, ATACANDO, DANO }

public class Player : MonoBehaviour
{
    private EstadoPlayer estado_player;

    public EstadoPlayer estadoPlayer
    {
        get { return estado_player; }

        private set
        {
            estado_player = value;
            if (value == EstadoPlayer.NORMAL)
            {
                InvokeRepeating("ProcuraInimigo", 0f, 0.2f);
            }
        }
    }

    [Header("Referências")]
    public Transform posicaoHit;


    //Adicionar Interagível

    public static Player player;

    [Header("Valores")]
    public int danoMedio;
    public const int MAXLEVEL = 100;
    public int maxVida;
    [SerializeField]
    private float velocidade = 2;
    [SerializeField]
    private int maxColetavel;
    [SerializeField]
    private float raioPercepcao;

    private int level;
    private int vida, qtnCristais, dinheiro, experiencia;

    public int QtnCristais
    {
        get { return qtnCristais; }

        set
        {
            qtnCristais = Mathf.Clamp(value, 0, maxColetavel);
        }
    }

    public int Dinheiro
    {
        get { return dinheiro; }

        set
        {
            dinheiro = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public int Experiencia
    {
        get { return experiencia; }
        set
        {
            if (level < MAXLEVEL)
            {
                experiencia = value;
            }
        }
    }

    void Start()
    {
        estadoPlayer = EstadoPlayer.NORMAL;
        player = this;
    }

    protected virtual void Update()
    {
        Movimento();

        if (Input.GetMouseButtonDown(0) && estadoPlayer == EstadoPlayer.COMBATE)
        {
            StartCoroutine(Atacar());
        }
    }

    void Movimento()
    {
        if (estadoPlayer != EstadoPlayer.ATACANDO)
        {
            transform.Translate(Vector3.right * velocidade * Input.GetAxis("Horizontal") * Time.deltaTime);
            transform.Translate(Vector3.forward * velocidade * Input.GetAxis("Vertical") * Time.deltaTime);

            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-Vector3.forward * velocidade * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-Vector3.right * velocidade * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * velocidade * Time.deltaTime);
            }
        }
    }

    #region COMBATE
    private IEnumerator Atacar()
    {
        estadoPlayer = EstadoPlayer.ATACANDO;

        // tocar animação de ataque

        Debug.Log("Ataque");

        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, 10, LayerMask.GetMask("Inimigo"));
        hit[0].gameObject.GetComponent<Inimigo>().ReceberDano(danoMedio);

        yield return new WaitForSeconds(0.5f);

        estadoPlayer = EstadoPlayer.COMBATE;
    }

    private float CalculaDano()
    {
        return danoMedio + Random.Range(-5, 5);
    }

    private void Morrer()
    {

    }

    public void ReceberDano(int danoRecebido)
    {
        vida = Mathf.Clamp(vida - danoRecebido, 0, maxVida);

        string nomeAnim = "Dano";

        if (vida <= 0)
        {
            nomeAnim = "Morte";
            Morrer();
        }

        StartCoroutine(Dano(nomeAnim));
        
    }

    private IEnumerator Dano(string animationName)
    {
        estadoPlayer = EstadoPlayer.DANO;
        //tocar animação
        yield return new WaitForSeconds(0.2f);
        estadoPlayer = EstadoPlayer.COMBATE;
    }

    private void ProcuraInimigo()
    {
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, raioPercepcao, LayerMask.GetMask("Inimigo"));
        if (hit != null)
        {
            foreach (Collider inimigo in hit)
            {
                if (inimigo.GetComponent<Inimigo>().hostil)
                {
                    estadoPlayer = EstadoPlayer.COMBATE;
                    CancelInvoke("ProcuraInimigo");
                    return;
                }
            }
        }

    }

    #endregion

    public void Curar(int cura)
    {

    }

    private void Interagir()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, raioPercepcao);
    }

}