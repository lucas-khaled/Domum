using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawQuesGizmo : MonoBehaviour
{
    Condicoes condHolder;

    List<GameObject> instanciados = new List<GameObject>();

    int times = 0;

    public void SetCondicaoOnHolder(Condicoes c)
    {
        if (times < 1 || condHolder.descricao != c.descricao)
        {
            condHolder = c;
            CondicaoInstatiantes();
            times = 1;
        }
    }
    private void OnDrawGizmos()
    {
        if (condHolder != null)
        {
            Gizmos.color = GizmoColor();

            float sphereSize = 10;
            if(condHolder.tipoCondicao == Condicoes.TipoCondicao.COMBATE)
            {
                sphereSize = condHolder.raioDeSpawn;
            }
            else if(condHolder.tipoCondicao == Condicoes.TipoCondicao.IDA)
            {
                sphereSize = condHolder.distanciaChegada;
            }

            Gizmos.DrawWireSphere(condHolder.local, condHolder.distanciaChegada);      
        }
    }

    Color GizmoColor()
    {
        Color retorno = Color.black;
        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.COMBATE)
        {
            retorno = Color.red;
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.IDA)
        {
            retorno = Color.yellow;
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.INTERACAO)
        {
            retorno = Color.white;
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.PEGA_ITEM)
        {
            retorno = Color.magenta;
        }
        
        return retorno;
    }


    void CondicaoInstatiantes()
    {
        if(instanciados.Count>0)
            Clean();

        if ((condHolder.tipoCondicao == Condicoes.TipoCondicao.INTERACAO || condHolder.tipoCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM) && condHolder.interagivelPrefab != null)
        {
            if (!condHolder.IsOnScene())
            {
                instanciados.Add((GameObject)Instantiate(condHolder.itemDaCondicao.gameObject, condHolder.local, Quaternion.identity));
            }           
        }

        if (condHolder.tipoCondicao == Condicoes.TipoCondicao.COMBATE && condHolder.inimigosDaCondicao.Count>0)
        {
            foreach(GameObject inimigo in condHolder.inimigosDaCondicao)
            {
                if(inimigo != null)
                    instanciados.Add(Instantiate(inimigo, condHolder.SpawnRandomico(), Quaternion.identity));
            }
        }           

        if ((condHolder.tipoCondicao == Condicoes.TipoCondicao.PEGA_ITEM || condHolder.tipoCondicao == Condicoes.TipoCondicao.DEVOLVE_ITEM) && condHolder.itemDaCondicao != null)
        {
            instanciados.Add((GameObject)Instantiate(condHolder.itemDaCondicao.gameObject, condHolder.local, Quaternion.identity));
        }

        AparentarInstanciados();
    }

    void AparentarInstanciados()
    {
        foreach(GameObject go in instanciados)
        {
            go.transform.SetParent(transform);
        }
    }

    public void Clean()
    {
        foreach (GameObject destruir in instanciados)
        {
            DestroyImmediate(destruir);
        }
        instanciados.Clear();
    }

    private void Awake()
    {
        Clean();
        Destroy(this.gameObject);
    }
}
