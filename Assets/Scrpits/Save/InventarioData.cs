using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventarioData
{
    public Item[] itens;
    public Arma armaEquipada;

    public InventarioData(Inventario inventory)
    {
        itens = inventory.Getitens().ToArray();
        armaEquipada = inventory.armaEquipada;
    }
}
