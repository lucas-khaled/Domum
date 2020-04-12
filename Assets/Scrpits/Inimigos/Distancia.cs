﻿using System.Collections;
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

        Transform transform = this.gameObject.transform;
        transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 2, this.gameObject.transform.position.z);
        GameObject gameObject = Instantiate(imagemAlerta, transform.position, imagemAlerta.transform.rotation);
        gameObject.AddComponent<FaceCamera>();


        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
        StopCoroutine(mira);

        GameObject balaInstanciada = Instantiate(bala, pontoTiro.position, bala.transform.rotation);
        balaInstanciada.GetComponent<Bala>().SetCasterCollider(this.GetComponent<Collider>());
        
        //tocar animação de tiro
        

        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator Mirar()
    {
        //tocar animação de mirar

        while (true)
        {
            this.transform.LookAt(Player.player.transform);
            yield return null;
        }
    }
}
