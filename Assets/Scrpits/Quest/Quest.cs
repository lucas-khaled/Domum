using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nova Quest", menuName = "Quest/Nova Quest")]
public class Quest : ScriptableObject
{
    public bool principal;
    public string nome;

    public Dialogo dialogo;

    public List<Condicoes> condicoes;

    Condicoes condicaoAtual;
    bool realizada, aceita;
    int numCondicoes, condicaoAtualIndex;

    [SerializeField]
    private Recompensa reward;

    public Recompensa getRecompensaAtual()
    {
        return reward;
    }

    public Condicoes getCondicaoAtual()
    {
        return condicaoAtual;
    }

    public void AceitarQuest()
    {
        condicaoAtual = condicoes[0];
        realizada = false;
        condicaoAtualIndex = 0;
        aceita = true;
    }

    public Condicoes getProximaCondicao()
    {
        return condicoes[condicaoAtualIndex + 1];
    }

    public Condicoes ProximaCondicao()
    {
        condicaoAtualIndex++;
        if (condicaoAtualIndex < condicoes.Count)
        {
            condicaoAtual = condicoes[condicaoAtualIndex];
        }
        else
        {
            condicaoAtual = null;
            //TerminaMissao();
        }

        return condicaoAtual;
    }

    public void TerminaMissao()
    {
        reward.DarRecompensa();
        realizada = true;
    }

    public bool IsRealizada()
    {
        return realizada;
    }
}
