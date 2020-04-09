using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{

    #region SINGLETON

    public static Inventario inventario;
    private void Awake()
    {
        inventario = this;
    }

    #endregion

    public float pesoMaximo;

    List<Item> itens = new List<Item>();
    private float pesoInventario = 0;

    public bool AddItem(Item item)
    {
        bool inseriu = false;
        if (pesoInventario <= pesoMaximo)
        {
            itens.Add(item);
            pesoInventario += item.peso;
            inseriu = true;
        }

        return inseriu;
    }

    public void RemoverItem(Item item)
    {
        itens.Remove(item);
        pesoInventario -= item.peso;
    }

    //so de teste. Apagar depois
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoverItem(itens[0]);
        }
    }
}
