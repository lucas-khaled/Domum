using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BauUI : MonoBehaviour
{
    [SerializeField]
    private GameObject fecharBau;

    public bool bauAberto { get; private set; }
    public static BauUI bauUI;
    private void Awake()
    {
        bauUI = this;
    }

    private Bau bauAtual;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private GameObject slot, panelBau, descricaoPanel;
    [SerializeField]
    private Transform content;

    [Header("Texts")]
    [SerializeField]
    private Text moedaText;   
    [SerializeField]
    private Text descricaoText;
    [SerializeField]
    private Text tituloText;
    [SerializeField]
    private Text pesoTexto;
    [SerializeField]
    private Text danoTexto;

    [Header("Buttons")]
    [SerializeField]
    private Button pegaItemButton;

    private List<GameObject> instancias = new List<GameObject>();
    private Holder_ItemBau selectedHolder;

    public void SetBau(Bau bau)
    {
        UIController.uiController.PauseOn(true);
        bauAtual = bau;
        panelBau.gameObject.SetActive(true);
        AtualizaBau();
    }

    public void SelectHolder(Holder_ItemBau holder)
    {
        selectedHolder = holder;
        pegaItemButton.gameObject.SetActive(true);
    }

    public void DeselectHolder(Holder_ItemBau holder)
    {
        StartCoroutine(HolderDeselection(holder));
    }

    private IEnumerator HolderDeselection(Holder_ItemBau holder)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (selectedHolder == holder)
        {
            selectedHolder = null;
            pegaItemButton.gameObject.SetActive(false);
        }
    }

    private void ClearInstancias()
    {
        foreach(GameObject go in instancias)
        {
            Destroy(go);
        }
    }

    private void AtualizaBau()
    {
        ClearInstancias();

        if (bauAtual.itens.Count > 0)
        {
            foreach (Item item in bauAtual.itens)
            {
                GameObject instancia = Instantiate(slot);

                instancia.GetComponent<Holder_ItemBau>().item = item;
                instancia.transform.SetParent(content);
                instancia.transform.localScale = Vector3.one;

                instancias.Add(instancia);
            }
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(fecharBau);

        // verificar se o x está selecionado
        if (Input.GetButtonDown("Interact") && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == fecharBau && bauAberto)
        {
            Debug.Log(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
            bauAberto = false;
            CloseBau();
        }
        else
        {
            bauAberto = true;
        }
    }

    public void AddItemOnSelectedHolder()
    {
        AddItemInventario(selectedHolder.item);
    }

    public void AddItemInventario(Item item)
    {
        if (Inventario.inventario.AddItem(item))
        {
            pegaItemButton.gameObject.SetActive(false);
            bauAtual.itens.Remove(item);
            AtualizaBau();
        }
    }

    public void CloseBau()
    {
        pegaItemButton.gameObject.SetActive(false);
        panelBau.gameObject.SetActive(false);        
        selectedHolder = null;
        bauAtual = null;
        UIController.uiController.PauseOff();
    }

    public void ShowDescricao(Item item)
    {
        descricaoPanel.SetActive(true);
        tituloText.text = item.nome;
        descricaoText.text = item.descricao;
        moedaText.text = item.custoMoeda.ToString();
        pesoTexto.text = item.peso.ToString();

        if (item.GetType() == typeof(Arma)) {
            Arma arma = (Arma)item;
            danoTexto.text = arma.dano.ToString();
        }
        else {
            danoTexto.text = "--";
        }
        InvokeRepeating("SetDescriptionInMousePlace", 0, 0.1f);
    }

    private void SetDescriptionInMousePlace()
    {
        descricaoPanel.transform.position = new Vector3(Input.mousePosition.x + offset.x, Input.mousePosition.y + offset.y, Input.mousePosition.z);
    }

    public void CloseDescricao()
    {
        descricaoPanel.SetActive(false);
        tituloText.text = string.Empty;
        descricaoText.text = string.Empty;
        moedaText.text = string.Empty;
        pesoTexto.text = string.Empty;

        CancelInvoke("SetDescriptionInMousePlace");
    }

    private void Start()
    {
        bauAberto = false;
    }

}
