using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventarioUI : MonoBehaviour
{
    [SerializeField]
    private GameObject slot;
    [SerializeField]
    private GameObject grid;

    [Header("Buttons")]
    [SerializeField]
    private GameObject excluirButton;
    [SerializeField]
    private GameObject equiparButton;
    [SerializeField]
    private GameObject usarButton;

    [Header("Texts")]
    [SerializeField]
    private Text dinheiro_atualText;
    [SerializeField]
    private Text peso_atualText;
    [SerializeField]
    private Text fama_atualText;
    [SerializeField]
    private Text lvl_atualText;
    [SerializeField]
    private Text infoText;
    [SerializeField]
    private Text valor_vendaText;
    [SerializeField]
    private Text tituloText;
    [SerializeField]
    private Text pesoText;
    [SerializeField]
    private Text danoText;
    [SerializeField]
    private Text famaText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text danoEquipadaText;
    [SerializeField]
    private Text pesoEquipadaText;
    [SerializeField]
    private Text nomeEquipadaText;
    


    [Header("Images")]
    [SerializeField]
    private Image armaEquipadaImage;


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
        armaEquipadaImage.preserveAspect = true;
    }

    public void AttUI(Item item, bool mudanca)
    {
        armaEquipadaImage.sprite = Inventario.inventario.armaEquipada.icone;
        armaEquipadaImage.preserveAspect = true;
        PegaValores();
        if (mudanca)
        {

            GameObject Obj = slot;
            Obj.GetComponent<Holder_Item>().item = item;
            Obj.GetComponent<Holder_Item>().isLoja = false;
            GameObject mundano = (GameObject)Instantiate(Obj);
            mundano.transform.SetParent(grid.transform);
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
        excluirButton.SetActive(false);
        equiparButton.SetActive(false);

    }

    

    public void Equipar()
    {
        Arma selection = (Arma)selecionado;

        if (selection.armaPlayer == GameController.gameController.GetPersonagemEscolhido())
        {
            Inventario.inventario.EquipArma((Arma)selecionado);
            excluirButton.SetActive(false);
            equiparButton.SetActive(false);
            ClearOpcoes();
        }
        else
            UIController.uiController.BlockMessage("Esse Personagem não usa esta arma!");
    }

    public void Usar()
    {
        Cura itemCura = (Cura)selecionado;
        Player.player.Curar(itemCura.quantidadeCura);

        Inventario.inventario.RemoverItem(selecionado);
        selecionado = null;
        ClearOpcoes();
    }

    public void PegaValores()
    {
        StartCoroutine(PegaValoresWait());
    }

    IEnumerator PegaValoresWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        dinheiro_atualText.text = Player.player.status.Dinheiro.ToString();
        peso_atualText.text = Inventario.inventario.pesoInventario.ToString();
        fama_atualText.text = Player.player.status.Fama.ToString();
        lvl_atualText.text = Player.player.status.Level.ToString();
        danoEquipadaText.text = Inventario.inventario.armaEquipada.dano.ToString();
        nomeEquipadaText.text = Inventario.inventario.armaEquipada.nome.ToString();
        pesoEquipadaText.text = Inventario.inventario.armaEquipada.peso.ToString();
        armaEquipadaImage.sprite = Inventario.inventario.armaEquipada.icone;
    }

    IEnumerator ApareceExcluirWait()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        excluirButton.SetActive(true);
    }

    public void ApareceExcluir()
    {
        StartCoroutine(ApareceExcluirWait());
    }

    IEnumerator ApareceEquiparWait()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        equiparButton.SetActive(true);
    }

    public void ApareceEquipar()
    {
        StartCoroutine(ApareceEquiparWait());
    }

    public void ApareceUsar()
    {
        StartCoroutine(ApareceUsarWait());
    }

    IEnumerator ApareceUsarWait()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        usarButton.SetActive(true);
    }

    public void ClearOpcoes()
    {
        StartCoroutine(ClearBotoes());
        infoText.text = string.Empty;
        tituloText.text = string.Empty;
        pesoText.text = string.Empty;
        valor_vendaText.text = string.Empty;
        famaText.text = string.Empty;
    }

    IEnumerator ClearBotoes()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        excluirButton.SetActive(false);
        equiparButton.SetActive(false);
        usarButton.SetActive(false);
    }

    public void ShowInfo(Item item)
    {
        if(item == null)
        {
            Debug.Log("é o item");
        }

        if(infoText.text == null)
        {
            Debug.Log("cu");
        }

        infoText.text = (item.descricao != null) ? item.descricao: string.Empty;
        tituloText.text = item.nome;
        pesoText.text = item.peso.ToString();
        valor_vendaText.text = item.custoMoeda.ToString();

        if (item.GetType() == typeof(Arma)) {
            Arma arma = (Arma)item;
            famaText.text = arma.famaMinima.ToString();
            danoText.text = arma.dano.ToString();
            levelText.text = arma.nivelMinimo.ToString();
        }
        else {
            danoText.text = "---";
            famaText.text = "---";
            levelText.text = "---";
        }

        selecionado = item;
    }
}
