using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BauUI : MonoBehaviour
{
    [SerializeField]
    private GameObject fecharBau;
    [SerializeField]
    private GameObject posicaoItens;

    public bool bauAberto = false;
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

    [SerializeField]
    private Text moedaText, descricaoText, tituloText, pesoTexto, danoTexto; 

    private List<GameObject> instancias = new List<GameObject>();

    public void SetBau(Bau bau)
    {
        bauAtual = bau;
        panelBau.gameObject.SetActive(true);
        AtualizaBau();
        Time.timeScale = 0;

        CameraController.cameraInstance.Trava = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
    }

    public void AddItemInventario(Item item)
    {
        if (Inventario.inventario.AddItem(item))
        {
            bauAtual.itens.Remove(item);
            AtualizaBau();
        }
    }

    public void CloseBau()
    {
        //Debug.Log("Mommy shark");
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CameraController.cameraInstance.Trava = false;
        panelBau.gameObject.SetActive(false);
        bauAtual = null;
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

    public void AtualizaItem()
    {
        Debug.Log("ticuliro");

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(instancias[0]);

        // verificar se o x está selecionado
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == fecharBau && bauAberto)
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

    public void CloseDescricao()
    {
        descricaoPanel.SetActive(false);
        tituloText.text = string.Empty;
        descricaoText.text = string.Empty;
        moedaText.text = string.Empty;
        pesoTexto.text = string.Empty;

        CancelInvoke("SetDescriptionInMousePlace");
    }

}
