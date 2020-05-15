using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Holder_Item : Button, ISelectHandler
{
    public Item item;
    public Image Spritu_item;

    protected override void Start()
    {
        base.Start();

        if (item.icone != null)
        {
            Spritu_item.sprite = item.icone;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        InventarioUI.inventarioUI.Info.text = item.descricao;
        InventarioUI.inventarioUI.Titulo.text = item.nome;
        InventarioUI.inventarioUI.peso.text = item.peso.ToString();
        InventarioUI.inventarioUI.Valor_venda.text = item.custoMoeda.ToString();
        InventarioUI.inventarioUI.selecionado = item;
        InventarioUI.inventarioUI.ApareceExcluir();
        if(item.GetType() == typeof(Arma))
        {
            InventarioUI.inventarioUI.ApareceEquipar();
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        InventarioUI.inventarioUI.Info.text = string.Empty;
        InventarioUI.inventarioUI.Titulo.text = string.Empty;
        InventarioUI.inventarioUI.peso.text = string.Empty;
        InventarioUI.inventarioUI.Valor_venda.text = string.Empty;

    }
}
