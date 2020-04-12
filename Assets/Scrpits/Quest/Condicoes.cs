using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nova Condição", menuName = "Quest/Condição")]
public class Condicoes : ScriptableObject
{
    Transform local;
    public string descricao;
    bool ativa;
    bool realizada;

    public void SetCondicaoAtual()
    {
        ativa = true;
    }

    public void SetCondicaoRealizada()
    {
        realizada = true;
        ativa = false;
    }
}
