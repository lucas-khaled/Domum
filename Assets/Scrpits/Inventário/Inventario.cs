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

    [HideInInspector]
    public SkinnedMeshRenderer armaMesh = null;

    public float pesoMaximo;   
    public Arma armaDefaultIovelik;
    public Arma armaDefautTyva;

    [HideInInspector]
    public Arma armaEquipada = null;

    List<Item> itens = new List<Item>();
    public float pesoInventario { get; private set; }


    void LoadInventario()
    {
        armaEquipada = SaveSystem.data.inventarioData.armaEquipada;
        armaMesh.sharedMesh = armaEquipada.armaMesh;
        armaMesh.sharedMaterial = armaEquipada.armaMaterial;

        foreach(Item item in SaveSystem.data.inventarioData.itens)
        {
            AddItem(item);
        }
        
    }

    private void Start()
    {
        if (GameController.gameController.IsLoadedGame())
        {
            LoadInventario();
        }
        else
        {
            pesoInventario = 0;
            if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
            {
                armaEquipada = armaDefaultIovelik;
            }
            else
            {
                armaEquipada = armaDefautTyva;
            }
        }
    }

    public List<Item> Getitens()
    {
        return itens;
    }

    public bool AddItem(Item item)
    {
        bool inseriu = false;
        if (pesoInventario + item.peso <= pesoMaximo)
        {
            itens.Add(item);
            pesoInventario += item.peso;
            inseriu = true;

            if (EventsController.onItemPego != null)
            {
                EventsController.onItemPego.Invoke(item);
                
            }
            if(EventsController.onInventarioChange != null)
            {
                EventsController.onInventarioChange.Invoke(item, true);
            }
        }
        else
        {
            UIController.uiController.BlockMessage("Inventário Cheio");
        }

        return inseriu;
    }

    public void RemoverItem(Item item)
    {
        itens.Remove(item);
        pesoInventario -= item.peso;
        if(EventsController.onInventarioChange != null)
        {
            EventsController.onInventarioChange.Invoke(item, false);
        }
    }

    #region ARMA
    public void EquipArma(Arma arma)
    {
        if (Player.player.status.Level >= arma.nivelMinimo)
        {
            if(Player.player.status.Fama >= arma.famaMinima)
            {
                UnequipArma();
                armaEquipada = arma;
                RemoverItem(arma);
                armaMesh.sharedMesh = arma.armaMesh;
                armaMesh.sharedMaterial = arma.armaMaterial;
            }
            else
                UIController.uiController.BlockMessage("Fama Insuficiente");
        }
        else
            UIController.uiController.BlockMessage("Level Insuficiente");
    }
    
    public void UnequipArma()
    {
        if (armaEquipada != null)
        {
            armaMesh.sharedMesh = null;
            AddItem(armaEquipada);
            armaEquipada = null;
        }
    }
    #endregion
}
