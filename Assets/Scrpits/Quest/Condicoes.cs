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

    public List<GameObject> objetosDaCondicao;
    public List<GameObject> inimigosDaCondicao;
    public float raioDeSpawn = 2;
    public float distanciaChegada;

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

        return volta;
    }


    private bool CheckIda()
    {
        if (Vector3.Distance(Player.player.transform.position, local) < distanciaChegada)
        {
            return true;
        }

        return false;
    }

    private bool CheckCombate()
    {
        bool retorno = false;
        if (inimigoParent.transform.childCount <= 0)
        {
            retorno = true;
        }
        Debug.Log(retorno);
        return retorno;
    }

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
    }
}
