using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : Interagivel
{
    [SerializeField]
    Quest[] quests;

    [SerializeField]
    Dialogo famaBaixa;
    [SerializeField]
    Dialogo famaMedia;
    [SerializeField]
    Dialogo famaAlta;

    int questsAceitas = 0;

    private void Awake()
    {
        EventsController.onDialogoTerminado += DarQuest;
    }

    protected override void Start()
    {
        base.Start();
        DeAcceptQuests();
    }

    void DeAcceptQuests()
    {
        try
        {
            if (quests.Length > 0)
            {
                foreach (Quest quest in quests)
                {
                    quest.SetQuestNaoAceita();
                }
            }
        }
        catch
        {
            Debug.Log(this.name);
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (questsAceitas != 0)
        {
            Debug.Log("aaaaaa");
            if (questsAceitas > quests.Length - 1 && !quests[questsAceitas - 1].IsRealizada())
            {
                Debug.Log(quests.Length - 1);
                if (!quests[questsAceitas - 1].IsRealizada())
                {
                    if (Player.player.status.Fama < 30)
                    {
                        DialogueSystem.sistemaDialogo.NPCName(famaBaixa);
                    }
                    else if (Player.player.status.Fama < 60)
                    {
                        DialogueSystem.sistemaDialogo.NPCName(famaMedia);
                    }
                    else
                    {
                        DialogueSystem.sistemaDialogo.NPCName(famaBaixa);
                    }
                }
                return;
            }
        }

        if (!quests[questsAceitas].IsAceita())
        {
            quests[questsAceitas].dialogo.whosDialog = this.name;
            DialogueSystem.sistemaDialogo.NPCName(quests[questsAceitas].dialogo);
        }

    }

    void DarQuest(Dialogo dialogo)
    {
        if (dialogo.whosDialog == this.name)
        {
            QuestLog.questLog.AdicionarQuest(quests[questsAceitas]);
            questsAceitas++;
        }
    }
}
