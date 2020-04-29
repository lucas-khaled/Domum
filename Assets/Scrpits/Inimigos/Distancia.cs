using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distancia : Inimigo
{
    public GameObject imagemAlerta;
    public GameObject bala;
    public Transform pontoTiro;
    protected override IEnumerator Atacar()
    {

        ataqueCooldown = velocidadeAtaque;
        Coroutine mira = StartCoroutine(Mirar());

        Vector3 transformBala = this.gameObject.transform.position;
        transformBala = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 2, this.gameObject.transform.position.z);

        GameObject alerta = Instantiate(imagemAlerta, transform.position, imagemAlerta.transform.rotation);
        alerta.AddComponent<FaceCamera>();


        yield return new WaitForSeconds(3f);

        Destroy(alerta);
        anim.SetTrigger("Atirar");
        StopCoroutine(mira);

        GameObject balaInstanciada = Instantiate(bala, pontoTiro.position, pontoTiro.transform.rotation);
        balaInstanciada.GetComponent<Bala>().SetCasterCollider(this.GetComponent<Collider>());
        

        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator Mirar()
    {
        anim.SetBool("Atacar", true);

        while (true)
        {
            this.transform.LookAt(Player.player.transform);
            yield return null;
        }
    }
}
