using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condicoes  
{
    public enum TipoCondicao { IDA, FALA, COMBATE, INTERACAO, PEGA_ITEM, DEVOLVE_ITEM,};

    public TipoCondicao tipoCondicao;

    public Vector3 local;
    public string descricao;

    [SerializeField]
    private List<ConditionInstance> objsToInstantiate;
    [SerializeField]
    private List<string> objsToDestroy;

    #region HIDDENVARIABLES

    [SerializeField]
    private Dialogo dialogoDaCondicao;
    [SerializeField]
    private bool isOnScene;
    [SerializeField]
    private string nameOnScene;

    public ItemPickup itemDaCondicao;
    public List<GameObject> inimigosDaCondicao;
    public float raioDeSpawn = 2;
    public float distanciaChegada = 5;
    public Interagivel interagivelPrefab;

    #endregion

    private Interagivel interagivel;
    public GameObject inimigoParent { get; private set; }

    public bool IsOnScene()
    {
        return isOnScene;
    }

    public void FinalizaCondicao()
    {
        if(objsToDestroy.Count > 0)
        {
            Transform pai = GameObject.Find("Objs de Condicao").transform;
            if (pai == null)
                return;

            if(pai.childCount > 0)
            {
                foreach(string nome in objsToDestroy)
                {
                    Transform delete = pai.Find(nome);
                    if(delete != null)
                    {
                        MonoBehaviour.Destroy(delete.gameObject);
                    }
                }
            }
        }
    }

    public void CleanUnsusedConditions()
    {
        if(tipoCondicao != TipoCondicao.COMBATE)
        {
            inimigosDaCondicao.Clear();
        }

        if (tipoCondicao != TipoCondicao.PEGA_ITEM && tipoCondicao != TipoCondicao.DEVOLVE_ITEM)
        {
            itemDaCondicao = null;
        }

        if(tipoCondicao != TipoCondicao.INTERACAO && tipoCondicao != TipoCondicao.DEVOLVE_ITEM)
        {
            interagivel = null;
        }
    }

    public bool ChecaCondicao()
    {
        bool volta = false;

        if(tipoCondicao == TipoCondicao.IDA)
        {
            volta = CheckIda();
        }

        if(tipoCondicao == TipoCondicao.COMBATE)
        {
            volta = CheckCombate();
        }

        if(tipoCondicao == TipoCondicao.INTERACAO)
        {
            volta = CheckInteracao();
        }

        if(tipoCondicao == TipoCondicao.PEGA_ITEM)
        {
            volta = itemPego;
        }

        if(tipoCondicao == TipoCondicao.DEVOLVE_ITEM)
        {
            volta = CheckDevolveItem();
        }

        if(tipoCondicao == TipoCondicao.FALA)
        {
            volta = CheckFala();
        }

        return volta;
    }

    #region IDA
    private bool CheckIda()
    {
        if (Vector3.Distance(Player.player.transform.position, local) < distanciaChegada)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region COMBATE
    private bool CheckCombate()
    {
        bool retorno = false;
        if (inimigoParent.transform.childCount <= 0)
        {
            retorno = true;
        }
        return retorno;
    }
    #endregion

    #region INTERACAO
    bool interagiu = false;
    private bool CheckInteracao()
    {
        return interagiu;
    }

    private void OnInteracao(Interagivel interagido)
    {
        if(interagido == interagivel)
        {
            interagiu = true;
            if(tipoCondicao == TipoCondicao.DEVOLVE_ITEM)
            {
                Inventario.inventario.RemoverItem(itemDaCondicao.item);
            }
        }
    }
    #endregion

    #region PEGA_ITEM

    bool itemPego = false;

    private void OnItemPego(Item item)
    {
        if (item == itemDaCondicao.item)
        {
            itemPego = true;
            return;
        }
        itemPego = false;
    }

    #endregion

    #region DEVOLVE_ITEM
    // a parte mais importante desse código está na região de interação, pois aqui foi reutilizada sua estrutula lógica.

    private bool CheckDevolveItem()
    {
        return interagiu;
    }

    #endregion

    #region FALA
    bool falou = false;

    bool CheckFala()
    {
        return falou;
    }

    void OnFalaTerminada(Dialogo dialogo)
    {
        if(dialogo.whosDialog == this.descricao)
        {
            falou = true;
        }
    }

    #endregion

    public Vector3 SpawnRandomico()
    {
        Vector3 loc = local + (Random.insideUnitSphere * raioDeSpawn);
        loc.y = local.y;
        return loc;
    }

    public void AtivarCondicao()
    {      
        if(objsToInstantiate.Count > 0)
        {
            GameObject game = GameObject.Find("Objs de Condicao");
            if(game == null)
                game = new GameObject("Objs de Condicao");

            foreach(ConditionInstance c in objsToInstantiate)
            {
                c.Instanciar(game.transform);
            }
        }

        if (inimigosDaCondicao.Count>0) {

            inimigoParent = new GameObject("InimigoHolder " + descricao);
            Transform parentao = GameObject.Find("CondHolder").transform;
            foreach (GameObject inimigo in inimigosDaCondicao)
            {
                GameObject inimigoInstanciado = MonoBehaviour.Instantiate(inimigo, SpawnRandomico(), Quaternion.identity);
                inimigoInstanciado.transform.SetParent(inimigoParent.transform);
            }
        }

        if(tipoCondicao == TipoCondicao.INTERACAO || tipoCondicao == TipoCondicao.DEVOLVE_ITEM || tipoCondicao == TipoCondicao.FALA)
        {
            interagiu = false;
            if (!isOnScene)
            {
                Vector3 localization = new Vector3(local.x, local.y, local.z);
                GameObject interagivelObj = MonoBehaviour.Instantiate(interagivelPrefab.gameObject, localization, interagivelPrefab.gameObject.transform.rotation);
                interagivel = interagivelObj.GetComponent<Interagivel>();

                GameObject game = GameObject.Find("Objs de Condicao");
                if (game == null)
                    game = new GameObject("Objs de Condicao");

                interagivelObj.transform.SetParent(game.transform);
            }
            else
            {
                GameObject game = GameObject.Find("Objs de Condicao");
                if (game == null)
                    game = new GameObject("Objs de Condicao");

                interagivel = GameObject.Find(nameOnScene).GetComponent<Interagivel>();
                local = interagivel.transform.position;
            }
            EventsController.onInteracao += OnInteracao;
        }

        if(tipoCondicao == TipoCondicao.FALA || tipoCondicao == TipoCondicao.DEVOLVE_ITEM)
        {
            falou = false;
            if (dialogoDaCondicao.dialogueLines.Length > 0)
            {
                dialogoDaCondicao.whosDialog = this.descricao;
                interagivel.SetDialogoCondicao(dialogoDaCondicao);
                EventsController.onDialogoTerminado += OnFalaTerminada;
            }
        }

        if(tipoCondicao == TipoCondicao.PEGA_ITEM)
        {
            itemPego = false;
            EventsController.onItemPego += OnItemPego;
            GameObject itemObj = MonoBehaviour.Instantiate(itemDaCondicao.gameObject);
            itemObj.transform.position = local;          
        }
    }
}

[System.Serializable]
public class ConditionInstance
{
    [SerializeField]
    private GameObject objetoPrefab;
    [SerializeField]
    private Vector3 local;
    [SerializeField]
    private Vector3 rotacao;
    [SerializeField]
    private string nome;

    GameObject objetoInstanciado;

    public void Instanciar(Transform pai)
    {
        objetoInstanciado = MonoBehaviour.Instantiate(objetoPrefab, local, Quaternion.Euler(rotacao));
        objetoInstanciado.name = nome;
        objetoInstanciado.transform.parent = pai;
    }

    public void CopiarValores(Transform copia)
    {
        //objetoPrefab = copia.gameObject;
        local = copia.position;
        rotacao = copia.rotation.eulerAngles;
    }
}