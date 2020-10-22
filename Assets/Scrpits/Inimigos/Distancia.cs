using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Distancia : Inimigo
{
    [Header("Audios")]
    [SerializeField]
    private AudioClip tiro;

    public GameObject imagemAlerta;
    public GameObject bala;
    public Transform pontoTiro;
    public Transform pontoAlerta;

    protected override IEnumerator Atacar()
    {

        canAttack = false;

        Coroutine mira = StartCoroutine(Mirar());
        travaMovimento = true;
        imagemAlerta.SetActive(true);
        
        yield return new WaitForSeconds(3f);

        imagemAlerta.SetActive(false);
        anim.SetTrigger("Atirar");
        audioSource.PlayOneShot(tiro);
        anim.SetBool("Atacar", false);
        StopCoroutine(mira);

        anim.ResetTrigger("Atirar");
        travaMovimento = false;

        GameObject balaInstanciada = Instantiate(bala, pontoTiro.position, pontoTiro.transform.rotation);
        balaInstanciada.GetComponent<Bala>().SetCasterCollider(this.GetComponent<Collider>());
        

        yield return new WaitForSeconds(velocidadeAtaque);
        canAttack = true;

    }

    IEnumerator Mirar()
    {
        anim.SetBool("Atacar", true);
        this.gameObject.GetComponent<NavMeshAgent>().SetDestination(this.transform.position);

        while (true)
        {
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
