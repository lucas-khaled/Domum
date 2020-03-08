using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
         
    public int danoMedio;
    public const int MAXLEVEL = 100;
    
    //Adicionar Interagível

    public Player player;

    [SerializeField]
    private float velocidade = 2;
    [SerializeField]
    private int maxColetavel;

    private int level;
    private int vida;
    private bool emCombate = false;

    public int qtnCristais
    {
        get { return qtnCristais; }

        set
        {
            qtnCristais = Mathf.Clamp(value, 0, maxColetavel);
        }
    }

    public int dinheiro
    {
        get { return dinheiro; }

        set
        {
            dinheiro = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public int experiencia
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
        player = this;
    }

    protected virtual void Update()
    {
        Movimento();
    }

    void Movimento()
    {
        if (!emCombate)
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

    public void ReceberDano(int danoRecebido)
    {

    }

    public void Curar(int cura)
    {

    }

    private void Morrer()
    {

    }

    private IEnumerator Atacar()
    {
        yield return null;
    }

    private void Interagir()
    {

    }


}
