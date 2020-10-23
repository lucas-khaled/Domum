using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscolhaPersonagemController : MonoBehaviour
{
    [Header("Starter")]
    [SerializeField]
    private GameObject tyva;
    [SerializeField]
    private GameObject iovelik;
    [SerializeField]
    private Transform posicaoInicio;

    [Header("Quest Starter")]
    [SerializeField]
    private QuestGiver firstQuestGiver;
    [SerializeField]
    private Quest firstQuestTyva;
    [SerializeField]
    private Quest firstQuestIovelik;


    void Awake()
    {
        bool savedGame = GameController.gameController.IsLoadedGame();

        if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.TYVA)
        {
            Instantiate(tyva, posicaoInicio.position, posicaoInicio.rotation);
            if (!savedGame)
                firstQuestGiver.AddQuestToBeNext(firstQuestTyva);
        }
        else if (GameController.gameController.GetPersonagemEscolhido() == TipoPlayer.IOVELIK)
        {
            Instantiate(iovelik, posicaoInicio.position, posicaoInicio.rotation);
            if (!savedGame)
                firstQuestGiver.AddQuestToBeNext(firstQuestIovelik);
        }
    }
}
