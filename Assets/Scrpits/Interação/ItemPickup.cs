using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interagivel
{
    public Item item;

    public override void Interact()
    {
        Debug.Log("Peguei: " + item.nome);
    }

}
