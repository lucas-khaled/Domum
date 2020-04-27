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

    #region HIDDENVARIABLES

    public ItemPickup itemDaCondicao;
    public List<GameObject> objetosDaCondicao;
    public List<GameObject> inimigosDaCondicao;
    public float raioDeSpawn = 2;
    public float distanciaChegada;
    public Interagivel interagivel;

    #endregion

    GameObject inimigoParent;


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
            volta = CheckPegaItem();
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
        Debug.Log(interagido.name + " - " + interagivel.name);
        if(interagido == interagivel)
        {
            Debug.Log("issaaa");
            interagiu = true;
        }
    }
    #endregion

    #region PEGA_ITEM

    bool CheckPegaItem()
    {
        return itemPego;
    }

    bool itemPego;

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

    public void AtivarCondicao()
    {      
        if (inimigosDaCondicao.Count>0) {

            inimigoParent = new GameObject("InimigoHolder " + descricao);
            Transform parentao = GameObject.Find("CondHolder").transform;
            foreach (GameObject inimigo in inimigosDaCondicao)
            {
                Vector3 loc = local + (Random.insideUnitSphere * raioDeSpawn);
                loc.y = local.y;
                Quaternion rot = Random.rotation;

                GameObject inimigoInstanciado = MonoBehaviour.Instantiate(inimigo, loc, rot);
                inimigoInstanciado.transform.SetParent(inimigoParent.transform);
            }
        }

        if(tipoCondicao == TipoCondicao.INTERACAO)
        {
            GameObject interagivelObj = MonoBehaviour.Instantiate(interagivel.gameObject, local, interagivel.gameObject.transform.rotation);
            interagivel = interagivelObj.GetComponent<Interagivel>();
            EventsController.onInteracao += OnInteracao;
        }

        if(tipoCondicao == TipoCondicao.PEGA_ITEM)
        {
            GameObject itemObj = MonoBehaviour.Instantiate(itemDaCondicao.gameObject);
            itemObj.transform.position = local;
            EventsController.onItemPego += OnItemPego;
        }
    }
}
