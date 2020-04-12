using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nova Quest", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public bool principal;
    public List<Condicoes> condicoes;

    Condicoes condicaoAtual;
    bool realizada, ativa;
    int numCondicoes, condicaoAtualIndex;

    private void Awake()
    {
        
    }

    public void AceitarQuest()
    {
        QuestLog.questLog.AdicionarQuest(this);
        condicaoAtual = condicoes[0];
        condicaoAtual.SetCondicaoAtual();
        realizada = false;
        numCondicoes = condicoes.Count;
        condicaoAtualIndex = 0;

    }

    private void ProximaCondicao()
    {
        condicaoAtual.SetCondicaoRealizada();
        if (condicaoAtualIndex < numCondicoes)
        {
            condicaoAtual = condicoes[++condicaoAtualIndex];
            condicaoAtual.SetCondicaoAtual();
        }
        else
        {
            TerminaMissao();
        }
    }

    private void TerminaMissao()
    {
        // adicionar recompensa
        realizada = true;
    }
}
