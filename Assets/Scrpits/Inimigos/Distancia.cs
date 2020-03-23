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
        //tocar animação de mirar
        Transform transform = this.gameObject.transform;
        transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        GameObject gameObject = Instantiate(imagemAlerta, transform);

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);

        Instantiate(bala, pontoTiro);
        //tocar animação de tiro

        yield return new WaitForSeconds(0.5f);

        ataqueCooldown = velocidadeAtaque;

    }
}
