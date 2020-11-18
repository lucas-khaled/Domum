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

    public Quest[] GetQuestOnGiver()
    {
        return quests.ToArray();
    }

    public int GetQntQuestAceitas()
    {
        return questsAceitas;
    }

    public void AddQuestToBeNext(Quest quest)
    {
        quests.Insert(questsAceitas, quest);
        if(icone != null)
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

        /*if (questsAceitas > 0)
        {
            OnQuestLogChanged(quests[questsAceitas - 1], true);
        }*/

        if(questsAceitas >= quests.Count || quests[questsAceitas] == null)
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
        QuestData.QuestGiverSave[] qg = SaveSystem.data.questData.questGiversSave;

        for(int i = 0; i < qg.Length; i++)
        {
            if (qg[i].giverName == this.name)
            {
                quests.Clear();
                questsAceitas = qg[i].giverQntAceitas;
                quests.AddRange(qg[i].questsOnGiver);
                Debug.Log("Carreguei: " + name + " - " + questsAceitas + " - " + quests.Count);
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
            if (quests.Count == 0)
            {
                if (Player.player.status.Fama < 500)
                    DialogueSystem.sistemaDialogo.IniciaDialogo(famaBaixa);
                else if (Player.player.status.Fama < 1000)
                    DialogueSystem.sistemaDialogo.IniciaDialogo(famaMedia);
                else
                    DialogueSystem.sistemaDialogo.IniciaDialogo(famaAlta);
            }
            else if (questsAceitas >= quests.Count)
                DialogueSystem.sistemaDialogo.IniciaDialogo(semQuest);
            else
            {
                if (!quests[questsAceitas].IsAceita())
                {
                    quests[questsAceitas].dialogo.whosDialog = this.name;
                    DialogueSystem.sistemaDialogo.IniciaDialogo(quests[questsAceitas].dialogo);
                }
                else
                    DialogueSystem.sistemaDialogo.IniciaDialogo(semQuest);
            }
        }
        
    }

    private void OnQuestLogChanged(Quest quest, bool endQuest = false, bool isLoaded = false)
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
    private void OnDestroy()
    {
        EventsController.onDialogoTerminado -= DarQuest;
        EventsController.onLinhaTerminada -= OnLinhaTerminada;
        EventsController.onQuestLogChange -= OnQuestLogChanged;
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
