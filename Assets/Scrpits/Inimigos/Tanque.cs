using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanque : Inimigo
{
    [SerializeField]
    private float cooldownGeral = 1;
    public GameObject escudo;

    private bool choose = true;

    protected override void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && hostil)
        {
            bool mover = true;

            float distancia = Vector3.Distance(collider.gameObject.transform.position, gameObject.transform.position);

            if (distancia <= distanciaAtaque)
            {
                mover = false;
                if(choose)
                   EscolheAcao();
            }


            Movimentar(collider.transform.position, mover);
        }
    }
    public override void ReceberDano(int danoRecebido)
    {
        if(!escudo.activeSelf)
            base.ReceberDano(danoRecebido);
    }

    protected override IEnumerator Atacar()
    {
        choose = false;
        Coroutine c = StartCoroutine(base.Atacar());

        yield return c;
        Debug.Log("Ataque Tanque");
        yield return new WaitForSeconds(cooldownGeral);

        choose = true;
    }

    IEnumerator Defender()
    {
        escudo.SetActive(true);
        choose = false;

        yield return new WaitForSeconds(2);

        escudo.SetActive(false);

        yield return new WaitForSeconds(cooldownGeral);

        choose = true;
    }

    void EscolheAcao()
    {
        int chance = Mathf.CeilToInt(Random.Range(1, 100));

        if (chance >= 50)      
            StartCoroutine(Atacar());     
        else
            StartCoroutine(Defender());
    }

}
