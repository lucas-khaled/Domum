using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baú : Interagivel
{
    [SerializeField]
    List<Item> itens = new List<Item>();
    [SerializeField]
    private Item[] vetItens;
    [SerializeField]
    Animator anim;
    /*private void GeraItens()
    {
        int escolha = Random.Range(1, 5);
        for (int i = 0; i < escolha; i++)
        {
            int itemEsc = Random.Range(0, vetItens.Length);
            itens.Add(vetItens[itemEsc]);
        }
    }*/
    public override void Interact()
    {
        if (Inventario.inventario.AddItem(itens[0]))
        {
            Debug.Log("IEI");
        }
        anim.SetBool("Aberto", true);
    }
}
