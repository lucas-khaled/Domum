using System.Collections;
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
        vida = maxVida;
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

       
        Collider[] hit = Physics.OverlapSphere(posicaoHit.position, 2, LayerMask.GetMask("Inimigo"));

        if (hit.Length > 0)
        {
            int danin = CalculaDano();
            Debug.Log("Ataque: " + danin);
            hit[0].gameObject.GetComponent<Inimigo>().ReceberDano(danin);
        }

        yield return new WaitForSeconds(0.5f);

        estadoPlayer = EstadoPlayer.COMBATE;
    }

     private int CalculaDano()
    {
        return danoMedio + Random.Range(-5, 5);
    } 
    // Passei essa parte do calcula dano para o script de ArmaPlayer, não faz sentido o player calcular o dano que
    // vai ser passado no inimigo pelo script ArmaPlayer.
    // mantive comentado só pra você saber o que aconteceu.

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
                try
                {
                    if (inimigo.GetComponent<Inimigo>().hostil)
                    {
                        estadoPlayer = EstadoPlayer.COMBATE;
                        CancelInvoke("ProcuraInimigo");
                        return;
                    }
                }
                catch { }
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

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 2);
    }

}