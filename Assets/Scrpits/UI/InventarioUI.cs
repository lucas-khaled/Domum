using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventarioUI : MonoBehaviour
{
    public GameObject Slot;

    public GameObject Grid;

    public GameObject ExcluirB;

    public GameObject EquiparB;

    public Text Dinheiro_Atual;

    public Text Peso_Atual;

    public Text Info;

    public Text Valor_venda;

    public Text Titulo;

    public Text peso;

    public Image ArmaEq;


    [HideInInspector]
    public Item selecionado;

    List<GameObject> Bag = new List<GameObject>();


    public static InventarioUI inventarioUI;
    void Awake()
    {
        EventsController.onInventarioChange += AttUI;
        inventarioUI = this;
    }
    void Start()
    {
        PegaValores();
        ArmaEq.sprite = Inventario.inventario.armaEquipada.icone;
        
    }

    public void AttUI(Item item, bool mudanca)
    {

        ArmaEq.sprite = Inventario.inventario.armaEquipada.icone;
        PegaValores();
        if (mudanca)
        {

            GameObject Obj = Slot;
            Obj.GetComponent<Holder_Item>().item = item;
            Obj.GetComponent<Holder_Item>().isLoja = false;
            GameObject mundano = (GameObject)Instantiate(Obj);
            mundano.transform.SetParent(Grid.transform);
            mundano.transform.localScale = Vector3.one;
            Bag.Add(mundano);

        }
        else
        {
            GameObject itemRemovido;
            itemRemovido = Bag.Find(x => x.GetComponent<Holder_Item>().item == item);
            Bag.Remove(itemRemovido);
            Destroy(itemRemovido);
        }

    }

    public void Excluir()
    {
        Inventario.inventario.RemoverItem(selecionado);
        ExcluirB.SetActive(false);
        EquiparB.SetActive(false);

    }

    public void Equipar()
    {
        Arma selection = (Arma)selecionado;

        if (selection.armaPlayer == GameController.gameController.GetPersonagemEscolhido())
        {
            Inventario.inventario.EquipArma((Arma)selecionado);
            ExcluirB.SetActive(false);
            EquiparB.SetActive(false);
        }

    }
    public void PegaValores()
    {
        Dinheiro_Atual.text = Player.player.status.Dinheiro.ToString();
        Peso_Atual.text = Inventario.inventario.pesoInventario.ToString();
    }

    IEnumerator ApareceExcluirWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        ExcluirB.SetActive(true);
    }

    public void ApareceExcluir()
    {
        StartCoroutine(ApareceExcluirWait());
    }

    IEnumerator ApareceEquiparWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        EquiparB.SetActive(true);
    }

    public void ApareceEquipar()
    {
        StartCoroutine(ApareceEquiparWait());
    }

    public void ClearOpcoes()
    {
        StartCoroutine(ClearBotoes());
        Info.text = string.Empty;
        Titulo.text = string.Empty;
        peso.text = string.Empty;
        Valor_venda.text = string.Empty;
    }

    IEnumerator ClearBotoes()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        ExcluirB.SetActive(false);
        EquiparB.SetActive(false);
    }
}
