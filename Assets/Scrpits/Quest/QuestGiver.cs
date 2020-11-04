using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : Interagivel
{
    [SerializeField]
    List<Quest> quests = new List<Quest>();

    [SerializeField]
    Dialogo famaBaixa;
    [SerializeField]
    Dialogo famaMedia;
    [SerializeField]
    Dialogo famaAlta;
    [SerializeField]
    Dialogo semQuest;

    private GameObject icone;

    int questsAceitas = 0;

    public int GetQuestAceitas()
    {
        return questsAceitas;
    }

    public void AddQuestToBeNext(Quest quest)
    {
        quests.Insert(questsAceitas, quest);
        icone.SetActive(true);
    }

    private void Awake()
    {
        EventsController.onDialogoTerminado += DarQuest;
        EventsController.onLinhaTerminada += OnLinhaTerminada;
        EventsController.onQuestLogChange += OnQuestLogChanged;
        questsAceitas = 0;
    }


    protected override void Start()
    {
        base.Start();
        if (GameController.gameController.IsLoadedGame())
        {
            FindMeOnLoad();
        }
        DeAcceptQuests();
        icone = FindChildByLayer("Icones");

        if (questsAceitas > 0)
        {
            OnQuestLogChanged(quests[questsAceitas - 1], true);
        }

        if(questsAceitas >= quests.Count)
        {
            icone.SetActive(false);
        }
    }

    GameObject FindChildByLayer(string layerName)
    {
        GameObject retorno = null;
        for (int i = 0; i<transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                retorno = transform.GetChild(i).gameObject;
            }
        }

        return retorno;
    }

    void FindMeOnLoad()
    {
        string[] qgNames = SaveSystem.data.questData.giversNames;

        for(int i = 0; i < qgNames.Length; i++)
        {
            if (qgNames[i] == this.name)
            {
                questsAceitas = SaveSystem.data.questData.giversQntAceitas[i];
                break;
            }
        }
    }

    void DeAcceptQuests()
    {
        if (quests.Count > 0 && questsAceitas < quests.Count)
        {
            for (int i = questsAceitas; i<quests.Count;i++)
            {
                quests[questsAceitas].SetQuestNaoAceita();
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (!isPartOfDialogue)
        {
            if (questsAceitas != 0)
            {
                if (questsAceitas < quests.Count - 1 && !quests[questsAceitas - 1].IsRealizada())
                {
                    if (!quests[questsAceitas - 1].IsRealizada())
                    {
                        if (Player.player.status.Fama < 30)
                        {
                            DialogueSystem.sistemaDialogo.IniciaDialogo(famaBaixa);
                        }
                        else if (Player.player.status.Fama < 70)
                        {
                            DialogueSystem.sistemaDialogo.IniciaDialogo(famaMedia);
                        }
                        else
                        {
                            DialogueSystem.sistemaDialogo.IniciaDialogo(famaAlta);
                        }
                    }
                    return;
                }

                else if(questsAceitas >= quests.Count)
                {
                    DialogueSystem.sistemaDialogo.IniciaDialogo(semQuest);
                }
            }

            if (!quests[questsAceitas].IsAceita())
            {
                quests[questsAceitas].dialogo.whosDialog = this.name;
                DialogueSystem.sistemaDialogo.IniciaDialogo(quests[questsAceitas].dialogo);
            }
        }
    }

    private void OnQuestLogChanged(Quest quest, bool endQuest = false)
    {       
        if (questsAceitas >= quests.Count)
        {
            if (questsAceitas != 0 && quest.nome == quests[questsAceitas - 1].nome && endQuest)
            {
                Debug.Log(quest.nome + " - Puleei - " + this.name);
                icone.SetActive(false);
            }
        }
        else
        {
            if(questsAceitas != 0 && quest.nome == quests[questsAceitas - 1].nome && endQuest)
            {
                Debug.Log(quest.nome + " - Despuleei - " + this.name);
                icone.SetActive(true);
            }
        }
    }

    private void OnLinhaTerminada(Dialogo dialogo)
    {
        if (dialogo.whosDialog == this.name)
        {
            QuestGiverAnimationController questGiverAnimation = GetComponent<QuestGiverAnimationController>();

            if (questGiverAnimation != null)
            {
                questGiverAnimation.Interagir();
            }
        }

    }

    void DarQuest(Dialogo dialogo)
    {
        if (dialogo.whosDialog == this.name)
        {
            QuestLog.questLog.AdicionarQuest(quests[questsAceitas]);
            icone.SetActive(false);
            questsAceitas++;
        }
    }
}
