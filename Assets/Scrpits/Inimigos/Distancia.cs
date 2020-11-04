using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Distancia : Inimigo
{
    [Header("Audios")]
    [SerializeField]
    private AudioClip tiro;

    [Header("Distância")]
    public GameObject imagemAlerta;
    public GameObject bala;
    public Transform pontoTiro;
    public Transform pontoAlerta;
    [SerializeField]
    private float miraTime = 2;

    bool inAttack = false;
    protected override IEnumerator Atacar()
    {
        inAttack = true;
        canAttack = false;

        Coroutine mira = StartCoroutine(Mirar());
        imagemAlerta.SetActive(true);
        
        yield return new WaitForSeconds(miraTime);

        if (!inAttack)
        {
            canAttack = true;
            yield break;
        }
            

        imagemAlerta.SetActive(false);
        anim.SetTrigger("Atirar");
        audioSource.PlayOneShot(tiro);
        anim.SetBool("Atacar", false);
        StopCoroutine(mira);

        anim.ResetTrigger("Atirar");

        GameObject balaInstanciada = Instantiate(bala, pontoTiro.position, pontoTiro.transform.rotation);
        balaInstanciada.GetComponent<Bala>().SetCasterCollider(this.GetComponent<Collider>());
        

        yield return new WaitForSeconds(velocidadeAtaque);
        canAttack = true;

    }

    protected override void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil && Player.player.estadoPlayer != EstadoPlayer.MORTO)
        {
            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            anim.SetBool("Idle", false);

            move = !inAttack;
            goHome = false;

            if (distancia <= distanciaAtaque)
            {
                move = false;
                if (canAttack)
                    StartCoroutine(Atacar());
                else
                {
                    anim.SetBool("Idle", true);
                    FaceTarget();
                }
            }
            else
                inAttack = false;

            Movimentar(collider.transform.position, move);
        }

    }

    IEnumerator Mirar()
    {
        anim.SetBool("Atacar", true);
        this.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.transform.position);

        while (true)
        {
            if (!inAttack)
            {
                anim.SetBool("Atacar", false);
                yield break;
            }
            move = false;
            this.transform.LookAt(Player.player.transform);
            yield return null;
        }
    }

    protected override void Morrer()
    {
        base.Morrer();

        Destroy(GetComponentInChildren(typeof(FaceCamera), true));
    }
}
