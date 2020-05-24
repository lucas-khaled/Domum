using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : Interagivel
{
    [SerializeField]
    Quest[] quests;

    int questsAceitas = 0;

    public override void Interact()
    {
        base.Interact();

        if (questsAceitas != 0)
        {
            if (questsAceitas > quests.Length - 1 && !quests[questsAceitas - 1].IsRealizada())
            {
                if (!quests[questsAceitas - 1].IsRealizada())
                {
                    if (Player.player.status.Fama < 30)
                    {
                        DialogueSystem.sistemaDialogo.NPCName(quests[questsAceitas -1].dialogo[1]);
                    }
                    else if (Player.player.status.Fama < 60)
                    {
                        DialogueSystem.sistemaDialogo.NPCName(quests[questsAceitas - 1].dialogo[2]);
                    }
                    else
                    {
                        DialogueSystem.sistemaDialogo.NPCName(quests[questsAceitas - 1].dialogo[3]);
                    }
                }
                return;
            }
        }

        if (!quests[questsAceitas].IsAceita())
        {
            DialogueSystem.sistemaDialogo.NPCName(quests[questsAceitas].dialogo[0]);
            DarQuest();
        }

    }

    public void DarQuest()
    {
        QuestLog.questLog.AdicionarQuest(quests[questsAceitas]);
        questsAceitas++;
    }
}
